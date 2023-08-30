using Event.Application.Contracts.Auth;
using Event.Application.Contracts;
using Event.Application.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Event.API.Helpers;
using Event.Application.Helpers;
using Event.Domain.ReadOnly;
using Event.DTO.WriteOnly.AuthDTO;
using System.Net;
using Event.DTO.OAuth;
using System.Security.Claims;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Event.API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly IUserAuth _userAuth;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenAuth _tokenAuth;


        public AuthController(IUserAuth userAuth, ITokenGenerator tokenGenerator, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ITokenAuth tokenAuth)
        {
            _userAuth = userAuth;
            _tokenGenerator = tokenGenerator;
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenAuth = tokenAuth;
        }

        /// <summary>
        /// This is to register a new user
        /// </summary>
        /// <param name="registerUserDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("register_new_user")]
        [ProducesResponseType(typeof(JsonMessage<string>), 200)]
        public async Task<IActionResult> RegisterNewUser([FromBody] RegisterUserDTO registerUserDTO, CancellationToken cancellationToken)
        {
            #region Register User
            _tokenAuth.OneTimePasswordEmailEventhandler += fluentEmail.SendOneTimeCodeEmail;

            var result = await _userAuth.RegisterUser(registerUserDTO);

            if (string.IsNullOrWhiteSpace(result))
            {
                // send otp email to verify user
                string code = Util.GenerateRandomDigits(6);
                OneTimeCodeDTO oneTimeCodeDTO = new OneTimeCodeDTO()
                {
                    Token = code,
                    Sender = registerUserDTO.EmailAddress
                };
                // Save token to the sender
                await _tokenAuth.AddOneTimeCodeToSender(oneTimeCodeDTO, cancellationToken);
                //unsubcribe event
                _tokenAuth.OneTimePasswordEmailEventhandler -= fluentEmail.SendOneTimeCodeEmail;
                return Ok(new JsonMessage<string>()
                {
                    status = true,
                    success_message = "Confirm Email to complete authentication"
                });
            }

            return Ok(new JsonMessage<string>()
            {
                error_message = result,
                status = false,
                status_code = (int)HttpStatusCode.BadRequest
            });
            #endregion
        }

        [HttpPost]
        [Route("register_admin")]
        [ProducesResponseType(typeof(JsonMessage<string>), 200)]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserDTO registerUserDTO)
        {
            #region register admin
            var result = await _userAuth.RegisterAdmin(registerUserDTO);
            if (string.IsNullOrWhiteSpace(result))
            {
                // send email to verify email

                return Ok(new JsonMessage<string>()
                {
                    status = true,
                    success_message = "Confirm Email to complete authentication"
                });
            }
            return Ok(new JsonMessage<string>()
            {
                error_message = result,
                status = false,
                status_code = (int)HttpStatusCode.BadRequest
            });
            #endregion
        }

        /// <summary>
        /// Login the User, provide the required credentials. On login the user should receive an email message about its login information.
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(JsonMessage<string>), 200)]

        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            #region LOGIN
            try
            {
                var result = await _userAuth.LoginUser(loginDTO);
                if (result != null)
                {
                    ApplicationUser user = await _userAuth.GetByUserName(loginDTO.Email);
                    List<string> roles = await _userAuth.GetUserRoles(user);
                    var jwtToken = await _tokenGenerator.GenerateJwtToken(user.Id,user.PhoneNumber, user.Email, user.UserName, roles);

                    return Ok(new JsonMessage<AuthResponse>()
                    {
                        success_message = "Login Successful",
                        status = true,
                        result = new List<AuthResponse>() { jwtToken }
                    });
                }
                else
                {
                    return BadRequest(new JsonMessage<string>()
                    {
                        error_message = "",
                        status = false,
                        status_code = (int)HttpStatusCode.BadRequest
                    });
                }
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new JsonMessage<string>()
                {
                    error_message = ex.Message,
                    status = false,
                    status_code = (int)HttpStatusCode.BadRequest
                });
            }
            #endregion
        }

        /// <summary>
        /// Enter the user's email, current password and enter new password to chnage password
        /// </summary>
        /// <param name="changePassword"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("change_password")]
        [ProducesResponseType(typeof(JsonMessage<string>), 200)]
        [Authorize(Roles = "Admin, User")]

        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            #region CHANGE PASSWORD
            var currentUserEmail = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var result = await _userAuth.ChangePassword(currentUserEmail, changePasswordDTO);
            if (string.IsNullOrEmpty(result))
            {
                // Send Email password change successful
                var body = "You have successfully changed your password";
                string subject = "FX Password change successful";
                await mailService.SendGridClient.SendMail(currentUserEmail, subject, body);
                // Password change successfully
                return Ok(new JsonMessage<string>()
                {
                    status = true,
                    success_message = "Successfully Changed Password"
                });

            }
            return Ok(new JsonMessage<string>()
            {
                error_message = result,
                status = false,
                status_code = (int)HttpStatusCode.BadRequest
            });
            #endregion
        }


        [HttpPost]
        [Route("forget_password")]
        [ProducesResponseType(typeof(JsonMessage<string>), 200)]

        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO forgetPasswordDTO)
        {
            var user = await _userAuth.GetByUserName(forgetPasswordDTO.Email);
            if (user == null)
            {
                return Ok(new JsonMessage<string>()
                {
                    error_message = "User not Found",
                    status = false
                });
            }
            string code = await _userAuth.PasswordResetToken(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            string link = $"{forgetPasswordDTO.ResetPasswordPageLink}?token={code}";

            //Send EMail Successful
            var body = $"Click <a href=\"{link}\">here</a> to reset your account";
            string subject = "Easy Life On Chart Account Reset";
            await mailService.SendGridClient.SendMail(forgetPasswordDTO.Email, subject, body);

            return Ok(new JsonMessage<string>()
            {
                status = true,
                success_message = "reset password email sent"
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("delete_user/{Id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonMessage<string>), 200)]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            string result = await _userAuth.DeleteUser(Id);
            if (string.IsNullOrWhiteSpace(result))
            {

                return Ok(new JsonMessage<string>()
                {
                    status = true,
                    success_message = "successful",
                    status_code = (int)HttpStatusCode.OK
                });
            }
            return Ok(new JsonMessage<string>()
            {
                error_message = result,
                status = false,
                status_code = (int)HttpStatusCode.NotFound
            });
        }
    }
}
