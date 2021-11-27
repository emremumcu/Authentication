namespace Authentication.AppLib.StartupExt
{
    using Authentication.AppLib.Base;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class AuthenticationExtension
    {
        public static IServiceCollection _AddAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AuthCookieName";
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ClaimsIssuer = "ClaimsIssuer";
                    options.ReturnUrlParameter = "_ReturnUrl";
                    options.SlidingExpiration = true;
                    options.Cookie.HttpOnly = true; // false makes xss vulnerability
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Validate();
                    options.EventsType = typeof(CustomCookieAuthenticationEvents);
                })
            ;

            services.AddScoped<CustomCookieAuthenticationEvents>();

            return services;
        }

        /// <summary>
        /// Authentication and Authorization must be placed in between Routing and Endpoints.
        /// UseRouting, UseAuthentication, UseAuthorization, and UseEndpoints must be called in the order of typing.
        /// </summary>
        public static IApplicationBuilder _UseAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication();

            return app;
        }
    }
}

// AddCookie Options => 

// options.Events.OnRedirectToLogin = (context) =>
// {
//    // Ajax Request:
//    // context.Response.Headers["Location"] = context.RedirectUri;
//    // context.Response.StatusCode = 401;
//    // context.Response.Redirect(context.RedirectUri);
//    return Task.CompletedTask;
// };
// options.Events.OnRedirectToLogout = (context) =>
// {
//    return Task.CompletedTask;
// };
// options.Events.OnRedirectToAccessDenied = (context) =>
// {
//    return Task.CompletedTask;
// };
// options.Events.OnSignedIn = (context) =>
// {
//    return Task.CompletedTask;
// };