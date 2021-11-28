namespace Authentication.AppLib.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AdminRequirement : IAuthorizationRequirement
    {   
        public string PolicyName { get; private set; } = "AdminPolicy";

        public string RoleName { get; private set; } = "ADMIN";

        public AdminRequirement(string adminRoleName)
        {
            RoleName = adminRoleName;
        }
    }

    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            try
            {
                if (context.User.Identity.IsAuthenticated)
                {

                    ClaimsPrincipal principal = context.User;

                    String role = principal.FindFirst(ClaimTypes.Role).Value;
                    
                    return Task.CompletedTask;

                    //if (role.Split(Constants.Claims_Role_Seperator).Any(i => i == requirement.RoleName))
                    //{
                    //    context.Succeed(requirement);
                    //    return Task.CompletedTask;
                    //}
                    //else
                    //{
                    //    context.Fail();
                    //    return Task.CompletedTask;
                    //}
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
