using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Event.Application.Contracts;

namespace Event.Infrastructure.Contracts
{
    public sealed class BusinessRule : IBusinessRule
    {
        private readonly IHttpContextAccessor _httpAccessor;

        public BusinessRule(IHttpContextAccessor httpContextAccessor)
        {
            _httpAccessor = httpContextAccessor;
        }
        public string GetCurrentLoggedinUserEmail()
        {
            try
            {
                ClaimsIdentity identity = _httpAccessor.HttpContext.User.Identity as ClaimsIdentity;

                IEnumerable<Claim> claims = identity.Claims;

                string email = claims.First(x => x.Type == ClaimTypes.Email).Value;

                return email;
            }
            catch (Exception)
            {

                return String.Empty;
            }
        }

        public List<string> GetCurrentLoggedinUserRole()
        {
            try
            {
                ClaimsIdentity identity = _httpAccessor.HttpContext.User.Identity as ClaimsIdentity;

                IEnumerable<Claim> claims = identity.Claims;

                List<string> role = claims.Where(x => x.Type == ClaimTypes.Role).Select(i => i.Value).ToList();

                return role;
            }
            catch (Exception)
            {

                return new();
            }
        }

        public string GetLoggedInUserId()
        {
            try
            {
                ClaimsIdentity identity = _httpAccessor.HttpContext.User.Identity as ClaimsIdentity;

                IEnumerable<Claim> Claims = identity.Claims;

                string userId = Claims.Last(x => x.Type == ClaimTypes.NameIdentifier).Value;

                return userId;
            }
            catch (Exception)
            {

                return String.Empty;
            }
        }
    }
}
