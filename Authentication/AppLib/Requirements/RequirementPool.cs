namespace Authentication.AppLib.Requirements
{
    using Microsoft.AspNetCore.Authorization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RequirementPool
    {
    }

    public class PermissionHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                // validate each requirement
            }

            return Task.CompletedTask;
        }
    }
}
