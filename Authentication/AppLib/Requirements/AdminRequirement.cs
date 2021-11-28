namespace Authentication.AppLib.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class AdminRequirement : IAuthorizationRequirement
    {   
        public const string PolicyName = "AdminPolicy";

        public string[] Roles { get; private set; }

        public AdminRequirement(params string[] roles)
        {
            Roles = roles;
        }
    }

    public class AdminHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            try
            {
                if (context.User != null && context.User.Identity.IsAuthenticated)
                {
                    string[] roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

                    // Using Any() assures that the intersection algorithm stops when the first equal object is found.
                    if (requirement.Roles.Intersect(roles).Any())
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
