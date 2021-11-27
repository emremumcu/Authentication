namespace Authentication.AppLib.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class BaseRequirement : IAuthorizationRequirement
    {
        public BaseRequirement() { }
    }

    public class BaseHandler : AuthorizationHandler<BaseRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IServiceProvider _serviceProvider;

        public BaseHandler(IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// If BaseRequirement is not met, all user & login related information is cleared.
        /// </summary>        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BaseRequirement requirement)
        {
            //if (context.Resource is HttpContext httpContext) {
            //    var endpoint = httpContext.GetEndpoint();
            //    var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>(); }
            //else if (context.Resource is Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext mvcContext) { }

            try
            {
                if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    context.Fail();
                    RemoveUserLoginInfo();
                    return Task.CompletedTask;
                }

                if (_httpContextAccessor.HttpContext.Session.GetKey<bool>(Constants.SessionKeyLogin))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                    RemoveUserLoginInfo();
                }

                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }

        private void RemoveUserLoginInfo()
        {
            _httpContextAccessor.HttpContext.Session.Remove(Constants.SessionKeyLogin);

            _httpContextAccessor.HttpContext.Session.Clear();

            _httpContextAccessor.HttpContext.User = null;

            var cookie = _httpContextAccessor.HttpContext.Request.Cookies[Constants.Cookie_Name];

            if (cookie != null)
            {
                var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
                _httpContextAccessor.HttpContext.Response.Cookies.Append(Constants.Cookie_Name, cookie, options);
            }
        }
    }
}
