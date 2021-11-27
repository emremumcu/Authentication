namespace Authentication.AppLib.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AdminRequirement : IAuthorizationRequirement
    {
        public string RoleName { get; private set; }

        public AdminRequirement(string adminRoleName)
        {
            RoleName = adminRoleName;
        }
    }

    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {
        public const string PolicyName = "AdminPolicy";

        public const string RoleName = "VS_ADMIN";

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            try
            {
                if (context.User.Identity.IsAuthenticated)
                {

                    ClaimsPrincipal principal = context.User;

                    String role = principal.FindFirst(ClaimTypes.Role).Value;

                    if (role.Split(Constants.Claims_Role_Seperator).Any(i => i == requirement.RoleName))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                    else
                    {
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }
                else
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
