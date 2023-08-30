using Event.API.Filters;
using Event.Application.Contracts;
using Event.Application.EmailService;
using Event.Application.Filters;
using Event.Application.SmsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ModelStateCheck]
    [ValidateApiKey]
    public class BaseController : ControllerBase
    {
        private IBusinessRule _businessRule;
        private IMailService _mailService;
        private ISmsService _smsService;
        private IFluentEmailClient _fluentEmail;

        private ISqlDBObjects _sql;
        private string _userId;
        private string _username;
        private List<string> _roles;
        // private string _token;

        protected IBusinessRule BusinessRule => _businessRule ??= HttpContext.RequestServices.GetRequiredService<IBusinessRule>();
        protected ISqlDBObjects dBObjects => _sql ??= HttpContext.RequestServices.GetRequiredService<ISqlDBObjects>();
        protected IMailService mailService => _mailService ??= HttpContext.RequestServices.GetRequiredService<IMailService>();
        protected ISmsService smsService => _smsService ??= HttpContext.RequestServices.GetRequiredService<ISmsService>();
        protected IFluentEmailClient fluentEmail => _fluentEmail ??= HttpContext.RequestServices.GetRequiredService<IFluentEmailClient>();

        protected string UserId => _userId ??= BusinessRule.GetLoggedInUserId();
        protected string UserEmail => _username ??= BusinessRule.GetCurrentLoggedinUserEmail();
        protected List<string> UserRoles => _roles ??= BusinessRule.GetCurrentLoggedinUserRole();
    }
}
