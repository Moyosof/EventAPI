using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Event.Application.Contracts;
using Event.Application.DataContext;
using Event.DataAccess.UnitOfWork.Interface;
using Event.Domain.Entities.Auth;
using Event.DTO;
using Event.DTO.Mapper;
using Event.DTO.OAuth;
using Utility = Event.Application.Helpers.Util;

namespace Event.Infrastructure.JwtToken
{
    /// <summary>
    /// This class is implements the ITokenGenerator interface
    /// </summary>
    public sealed class TokenGenerator : ITokenGenerator
    {
        private readonly IUnitOfWork<RefreshToken> _refreshTokenUoW;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JwtSettings _jwtSettings;

        public TokenGenerator(IUnitOfWork<RefreshToken> refreshTokenUoW, IOptions<JwtSettings> jwtsettings, UserManager<ApplicationUser> userManager)
        {
            _refreshTokenUoW = refreshTokenUoW;
            this.userManager = userManager;
            _jwtSettings = jwtsettings.Value;
        }

        public async Task<AuthResponse> GenerateJwtToken(string user_id, string phoneNumber, string username, string email, IList<string> roles = null)
        {
            var expirationDate = DateTime.UtcNow.AddDays(1); //Convert.ToDouble(_jwtsettings.ExpirationTime);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
           {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, expirationDate.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user_id),
                    new Claim(ClaimTypes.MobilePhone, phoneNumber),
                    new Claim(ClaimTypes.Email, email ?? string.Empty),
                    //new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? ""),
                    //new Claim(ClaimTypes.Sid, customer_id ?? string.Empty),
                }),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Site,
                Audience = _jwtSettings.Audience,
                Expires = expirationDate
            };

            if (roles != null)
            {
                roles.ToList().ForEach(role =>
                {
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
                });
            }

            SecurityToken Token = tokenHandler.CreateToken(tokenDescriptor);

            string user_token = tokenHandler.WriteToken(Token);

            string RefreshToken = await GenerateRefreshToken(); //generate a refresh token

            await SaveRefreshToken(user_id, Token.Id, RefreshToken); //save the refresh token

            return new AuthResponse()
            {
                RefreshToken = RefreshToken,
                AccessToken = user_token,
                UserId = user_id,
                Username = username,
                PhoneNumber = phoneNumber,
                Email = email,
                FullName = "",
                //Roles = roles.ToArray(),
                ExpiryTime = expirationDate
            };
        }

        public async Task<Result<AuthResponse>> VerifyRefreshToken(string RefreshToken)
        {
            try
            {
                var RefreshTokens = await _refreshTokenUoW.Repository.ReadAll();

                var StoredRefreshToken = RefreshTokens.FirstOrDefault(x => x.Token.Equals(RefreshToken, StringComparison.OrdinalIgnoreCase));

                if (StoredRefreshToken is null) return new() { Succeeded = false, Message = "refresh token doesn't exist", Data = null };


                //get the user that owns the refreshtoken
                var dbUser = await userManager.FindByIdAsync(StoredRefreshToken.UserId);

                if (dbUser is null) return new() { Succeeded = false, Message = "User doesn't exist", Data = null };

                var dbUserRoles = await userManager.GetRolesAsync(dbUser);

                //Check the date of the saved token if it has expired
                if (StoredRefreshToken.JwtExpiryDate > DateTime.UtcNow) return new() { Succeeded = false, Message = "We cannot refresh this since the token has not expired", Data = null };

                //Check the date of the refresh token if it has expired
                if (StoredRefreshToken.ExpiryDate < DateTime.UtcNow) return new() { Succeeded = false, Message = "refresh token has expired, user needs to login again", Data = null };

                // Check if the token is revoked
                if (StoredRefreshToken.IsRevoked) return new() { Succeeded = false, Data = null, Message = "token has been revoked" };

                //// check if the refresh token has been used
                if (StoredRefreshToken.IsUsed) return new() { Succeeded = false, Message = "token has been used", Data = null };

                StoredRefreshToken.IsUsed = true;
                _refreshTokenUoW.Repository.Update(StoredRefreshToken);
                await _refreshTokenUoW.SaveAsync();



                var authResponse = await GenerateJwtToken(dbUser.Id, dbUser.PhoneNumber, dbUser.UserName, dbUser.Email, dbUserRoles);

                return Map.GetModelResult(new List<AuthResponse>() { authResponse }, true, "Token Generated");
                //return new() { Succeeded = true, Message = "Token Generated", ModelObject = authResponse };

            }
            catch (Exception)
            {

                throw;
            }
        }


        private async Task<string> GenerateRefreshToken()
        {
            var RefreshTokens = await _refreshTokenUoW.Repository.ReadAll();
            bool isUsed = true;
            string refresh_token = string.Empty;
            do
            {
                refresh_token = Utility.GenerateRandomString(10) + "-" + Guid.NewGuid();
                isUsed = RefreshTokens.Any(x => x.Token.Equals(refresh_token, StringComparison.OrdinalIgnoreCase));

            } while (isUsed);
            return refresh_token;
        }

        private async Task SaveRefreshToken(string userId, string jwtId, string refresh_token)
        {
            //save refresh token
            RefreshToken NewRefreshToken = new()
            {
                JwtId = jwtId,
                IsUsed = false,
                UserId = userId,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = refresh_token,
                JwtExpiryDate = DateTime.UtcNow.Add(_jwtSettings.TokenLifeTime)
            };

            await _refreshTokenUoW.Repository.Add(NewRefreshToken);
            await _refreshTokenUoW.SaveAsync();
        }
    }
}
