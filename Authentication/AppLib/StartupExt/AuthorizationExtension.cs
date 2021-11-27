﻿namespace Authentication.AppLib.StartupExt
{
    using Authentication.AppLib.Base;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class AuthorizationExtension
    {
        public static IServiceCollection _AddAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                /*
                 * Gets or sets the default authorization policy with no policy name specified.
                 * Defaults to require authenticated users.
                 * 
                 * The DefaultPolicy is the policy that is applied when:
                 *      (*) You specify that authorization is required, either using RequireAuthorization(), by applying an AuthorizeFilter, 
                 *          or by using the[Authorize] attribute on your actions / Razor Pages.
                 *      (*) You don't specify which policy to use.
                 *      
                 */

                //options.DefaultPolicy = new AuthorizationPolicyBuilder()
                //     .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                //     .RequireAuthenticatedUser()
                //     .Build();

                options.DefaultPolicy = AuthorizationPolicyLibrary.defaultPolicy;

                /*
                 * Gets or sets the fallback authorization policy when no IAuthorizeData have been provided.
                 * By default the fallback policy is null.
                 * 
                 * The FallbackPolicy is applied when the following is true:
                 *      (*) The endpoint does not have any authorisation applied. No[Authorize] attribute, no RequireAuthorization, nothing.
                 *      (*) The endpoint does not have an[AllowAnonymous] applied, either explicitly or using conventions.
                 *      
                 * So the FallbackPolicy only applies if you don't apply any other sort of authorization policy, 
                 * including the DefaultPolicy, When that's true, the FallbackPolicy is used.
                 * By default, the FallbackPolicy is a no - op; it allows all requests without authorization.
                 * 
                 */

                //options.FallbackPolicy = new AuthorizationPolicyBuilder()
                //     .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                //     .RequireAuthenticatedUser()
                //     .Build();

                options.FallbackPolicy = AuthorizationPolicyLibrary.fallbackPolicy;
            });            

            return services;
        }

        /// <summary>
        /// UseRouting, UseAuthentication, UseAuthorization, and UseEndpoints must be called in the order of typing
        /// </summary>
        public static IApplicationBuilder _UseAuthorization(this IApplicationBuilder app)
        {
            app.UseAuthorization();

            return app;
        }
    }
}

// services.AddAuthorization(options =>

// options.AddPolicy("ClaimPolicy", policy => policy.RequireClaim("claim1"));
// options.AddPolicy("ClaimWithValuePolicy", policy => policy.RequireClaim("claim2", "val1", "val2"));
// options.AddPolicy("SingleRolePolicy", policy => policy.RequireRole("role1"));
// options.AddPolicy("MultiRolePolicy", policy => policy.RequireRole("role1", "role2"));
// options.AddPolicy("ClaimRoleCombinedPolicy", policy => { policy.RequireClaim("claim1"); policy.RequireRole("role1"); });
// options.AddPolicy("InlineDefinedPolicy", policy =>
// {
//    policy.AddAuthenticationSchemes("Cookie, Bearer");
//    policy.RequireAuthenticatedUser();
//    policy.RequireRole("Admin");
//    policy.RequireClaim("editor", "contents");
// });
// options.AddPolicy("DefinedPolicy", AuthorizationPolicyLibrary.assertionPolicy);
// options.AddPolicy("PolictWithRequirement", policy => policy.Requirements.Add(new AppLib.Requirements.MinimumAgeRequirement(18)));

// services.AddSingleton<IAuthorizationHandler, AppLib.AuthorizationHandlers.MinimumAgeHandler>();