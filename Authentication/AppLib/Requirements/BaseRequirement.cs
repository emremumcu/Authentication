namespace Authentication.AppLib.Requirements
{
    using Authenticate.AppLib.Concrete;
    using Authentication.AppLib.StartupExt;
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
        /// Requirements: NameIdentifier claim & Session login key (as true)
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
                // User MUST have a NameIdentifier
                if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                {
                    context.Fail();
                    RemoveUserLoginInfo();
                    return Task.CompletedTask;
                }

                // Session MUST have a LOGIN key set to true
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
            // _httpContextAccessor.HttpContext.Session.Clear(); // Do NOT clear session
            _httpContextAccessor.HttpContext.User = null;

            //var session_cookie = _httpContextAccessor.HttpContext.Request.Cookies[Constants.Session_Cookie_Name];
            //if (session_cookie != null)
            //{
            //    var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
            //    _httpContextAccessor.HttpContext.Response.Cookies.Append(Constants.Session_Cookie_Name, session_cookie, options);
            //}

            //var auth_cookie = _httpContextAccessor.HttpContext.Request.Cookies[Constants.Auth_Cookie_Name];
            //if (auth_cookie != null)
            //{
            //    var options = new CookieOptions { Expires = DateTime.Now.AddDays(-1) };
            //    _httpContextAccessor.HttpContext.Response.Cookies.Append(Constants.Session_Cookie_Name, auth_cookie, options);
            //}
        }
    }
}