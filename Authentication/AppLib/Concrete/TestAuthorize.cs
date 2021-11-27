namespace Authenticate.AppLib.Concrete
{
    using Authenticate.AppLib.Abstract;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    public class TestAuthorize : IAuthorize
    {
        private ClaimsPrincipal GetPrincipal(List<Claim> userClaims)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme));
        }

        private AuthenticationProperties GetProperties() => new AuthenticationProperties
        {
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20),
            IsPersistent = true,
            IssuedUtc = DateTimeOffset.UtcNow,
            RedirectUri = "RedirectUri"
        };

        public AuthenticationTicket GetTicket(List<Claim> userClaims) => 
            new AuthenticationTicket(
                GetPrincipal(userClaims), 
                GetProperties(), 
                CookieAuthenticationDefaults.AuthenticationScheme
            );
    }
}