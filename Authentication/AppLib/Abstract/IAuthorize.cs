namespace Authenticate.AppLib.Abstract
{
    using Microsoft.AspNetCore.Authentication;
    using System.Collections.Generic;
    using System.Security.Claims;

    public interface IAuthorize
    {
        /// <summary>
        /// Exclude NameIdentifier Claim in the list. Its added by default using userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userClaims"></param>
        /// <returns></returns>
        public AuthenticationTicket GetTicket(List<Claim> userClaims);
    }
}
