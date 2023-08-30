using Event.Application.Contracts.Auth;
using Event.Application.DataContext;
using Event.DataAccess.UnitOfWork.Interface;
using Event.Domain.Entities.Auth.AuthUser;
using Event.Domain.Enum;
using Event.Domain.ReadOnly;
using Event.DTO;
using Event.DTO.WriteOnly.AuthDTO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Infrastructure.Contracts.Auth
{
    public class UserAuth : IUserAuth, ITokenAuth
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IWebHostEnvironment env;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork<OneTimeCode> _unitOfWorkOTP;

        public event Func<object, OneTimeCodeDTO, CancellationToken, Task> OneTimePasswordEmailEventhandler;

        public UserAuth(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            IWebHostEnvironment _env,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork<OneTimeCode> unitOfWorkOTP
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            env = _env;
            _roleManager = roleManager;
            _unitOfWorkOTP = unitOfWorkOTP;
        }
        public async Task AddOneTimeCodeToSender(OneTimeCodeDTO oneTimeCodeDTO, CancellationToken cancellationToken)
        {
            OneTimeCode oneTimeCode = new(oneTimeCodeDTO);
            try
            {
                await _unitOfWorkOTP.Repository.Add(oneTimeCode);
                await _unitOfWorkOTP.SaveAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            await Task.Delay(1000);
            //Invoke events
            await OnOtpAuth(oneTimeCodeDTO, cancellationToken);
        }

        public async Task<string> ChangePassword(string userEmail, ChangePasswordDTO passwordDTO)
        {
            var currentUser = await _userManager.FindByEmailAsync(userEmail);
            var changePassword = await _userManager.ChangePasswordAsync(currentUser, passwordDTO.CurrentPassword, passwordDTO.NewPassword);
            if (changePassword.Succeeded)
            {
                return string.Empty;
            }

            else
            {
                string error = changePassword.Errors.First().Description;
                if (error == "passwordMismatch")
                {
                    return "Current password os incorrect";
                }
                else
                {
                    return "We are not able to change your password right now. Please contact admin";
                }
            }
        }

        public async Task<string> CreateRoleAndAddUserToRole(string name, ApplicationUser user)
        {
            bool checkRole = await _roleManager.RoleExistsAsync(name);
            if(!checkRole)
            {
                IdentityResult role = await _roleManager.CreateAsync(new IdentityRole(name));
                if(!role.Succeeded)
                {
                    return role.Errors.First().Description;
                }
            }
            IdentityResult AddUserToRole = await _userManager.AddToRoleAsync(user, name);
            if (AddUserToRole.Succeeded)
            {
                return string.Empty;
            }

            return AddUserToRole.Errors.First().Description;
        }

        public async Task<string> DeleteUser(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is not null)
            {
                await _userManager.DeleteAsync(user);
                return string.Empty;
            }
            return "User not found";
        }

        /// <summary>
        /// Find User by Username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> GetByUserName(string userName)=> await _userManager.FindByNameAsync(userName);
        

        public async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
        }

        public async Task<string> LoginUser(LoginDTO loginDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, lockoutOnFailure: false);
            if(result.Succeeded)
            {
                return string.Empty;
            }
            return "Invalid login credentials";
        }

        /// <summary>
        /// Generate password reset token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> PasswordResetToken(ApplicationUser user)=> await _userManager.GeneratePasswordResetTokenAsync(user);

        public async Task<string> RegisterAdmin(RegisterUserDTO registerUser)
        {
            return await Register(registerUser, Roles.Admin);
        }

        public async Task<string> RegisterUser(RegisterUserDTO registerUser)
        {
            return await Register(registerUser, Roles.User);
        }

        /// <summary>
        /// Reset the user password and making sure the token is valid for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<string> ResetUserPassword(ApplicationUser user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if(result.Succeeded)
            {
                return string.Empty;
            }
            return result.Errors.First().Description;
        }

        public async Task<string> VerifyOneTimeCode(VerifyOneTimeCodeDTO verifyOneTimeCodeDTO)
        {
            var tokenDb = await _unitOfWorkOTP.Repository.ReadAllQuery().Where(x => x.IsUsed == false && x.Token == verifyOneTimeCodeDTO.Token && x.Sender == verifyOneTimeCodeDTO.Sender && x.ExpiringDate > DateTime.UtcNow).FirstOrDefaultAsync();
            if (tokenDb is not null)
            {
                tokenDb.IsUsed = true;
                await _unitOfWorkOTP.SaveAsync();

                // get username
                var user = await GetByUserName(verifyOneTimeCodeDTO.Sender);
                if (user is not null)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                    return string.Empty;
                }
                return "User not found";

            }
            return "Invalid Token";
        }

        private async Task<string> Register(RegisterUserDTO registerUserDTO, Roles role)
        {
            ApplicationUser user = new();

            user = new ApplicationUser { Email = registerUserDTO.EmailAddress, PhoneNumber = registerUserDTO.PhoneNumber, PhoneNumberConfirmed = false, EmailConfirmed = true, UserName = registerUserDTO.EmailAddress, Firstname = registerUserDTO.FirstName.ToUpper(), Lastname = registerUserDTO.LastName.ToUpper() };

            // Creates a new user and password harh
            var result = await _userManager.CreateAsync(user, registerUserDTO.Password);
            if (result.Succeeded)
            {
                string roleName = role.ToString();
                return await CreateRoleAndAddUserToRole(roleName, user);
            }
            return result.Errors.First().Description;
        }

        protected async virtual Task OnOtpAuth(OneTimeCodeDTO oneTimeCodeDTO, CancellationToken cancellationToken)
        {
            Func<object, OneTimeCodeDTO, CancellationToken, Task> handler = OneTimePasswordEmailEventhandler;
            if (handler is not null)
            {
                Delegate[] invocationList = handler.GetInvocationList();
                Task[] handlerTasks = new Task[invocationList.Length];
                for (int i = 0; i < invocationList.Length; i++)
                {
                    handlerTasks[i] = ((Func<object, OneTimeCodeDTO, CancellationToken, Task>)invocationList[i])(this, oneTimeCodeDTO, cancellationToken);
                }

                await Task.WhenAll(handlerTasks);
            }
        }
    }
}
