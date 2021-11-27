namespace Authentication.AppLib.Requirements
{
    using AspNetTemplate2.AppLib.Ext;
    using Microsoft.AspNetCore.Authorization;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DeveloperRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "DeveloperPolicy";

        public List<string> RoleNames { get; private set; }

        public DeveloperRequirement()
        {
            RoleNames = new List<string>() { "VS_ADMIN", "VS_DEVELOPER" };
        }
    }

    public class DeveloperHandler : AuthorizationHandler<DeveloperRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeveloperRequirement requirement)
        {
            try
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    ClaimsIdentity ci = ((System.Security.Claims.ClaimsIdentity)context.User.Identity);

                    if (requirement.RoleNames.Intersect(ci.GetRoles()).Count() > 0)
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                }

                context.Fail();
                return Task.CompletedTask;
            }
            catch
            {
                throw;
            }
        }
    }
}
