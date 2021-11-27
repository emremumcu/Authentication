namespace Authentication.AppLib.AuthorizationHandlers
{
    using Authentication.AppLib.Requirements;
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Threading.Tasks;

    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var user = context.User;

            if (!user.HasClaim(c => c.Type == "Age")) return Task.CompletedTask;

            var since = Convert.ToInt32(user.FindFirst("Age").Value);

            if (since >= requirement.Age) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
