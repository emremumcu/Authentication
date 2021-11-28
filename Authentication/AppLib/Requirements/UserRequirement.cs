using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.AppLib.Requirements
{
    public class UserRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "UserPolicy";

        public string[] Roles { get; private set; }

        public UserRequirement(params string[] roles)
        {
            Roles = roles;
        }
    }

    public class UserHandler : AuthorizationHandler<UserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
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