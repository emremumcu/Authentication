using Authentication.AppLib.StartupExt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Authentication.AppLib.Base
{
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        /*
         * Once an authentication cookie is created, it becomes the single source of identity. 
         * If a user account is invalidated and not valid anymore in backend, the app's cookie 
         * authentication system continues to process requests based on the authentication cookie.
         * 
         * The user remains signed into the app as long as the authentication cookie is valid.
         * The ValidatePrincipal event can be used to intercept and override validation of the 
         * cookie identity. Validating the cookie on every request mitigates the risk of revoked 
         * users accessing the app.
         * 
         */

        /// <summary>
        /// CustomCookieAuthenticationEvents ValidatePrincipal method runs in every request!!!
        /// </summary>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            ClaimsPrincipal user = context.Principal;

            Boolean login = context.HttpContext.Session.GetKey<bool>("login");

            if (!(user.Identity.IsAuthenticated && login))
            {
                context.RejectPrincipal();

                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }
    }
}
