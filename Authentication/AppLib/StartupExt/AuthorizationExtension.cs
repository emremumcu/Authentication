namespace Authentication.AppLib.StartupExt
{
    using Authenticate.AppLib.Abstract;
    using Authenticate.AppLib.Concrete;
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
                //options.DefaultPolicy = AuthorizationPolicyLibrary.defaultPolicy;
                //options.FallbackPolicy = AuthorizationPolicyLibrary.fallbackPolicy;               
                //options.AddPolicy(AdminHandler.PolicyName, AuthorizationPolicyLibrary.adminPolicy);
                //options.AddPolicy(DeveloperRequirement.PolicyName, AuthorizationPolicyLibrary.developerPolicy);
            });

            //services.AddSingleton<IAuthorizationHandler, BaseHandler>();
            //services.AddSingleton<IAuthorizationHandler, UserHandler>();
            //services.AddSingleton<IAuthorizationHandler, DeveloperHandler>();
            //services.AddSingleton<IAuthorizationHandler, AdminHandler>();
            //services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            // TODO Set Authorizer
            services.AddSingleton<IAuthorize, TestAuthorize>();

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

    public static class AuthorizationPolicyLibrary
    {
        public static AuthorizationPolicy defaultPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           //.AddRequirements(new BaseRequirement())           
           .Build();

        public static AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           //.AddRequirements(new BaseRequirement())
           .Build();

        public static AuthorizationPolicy developerPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("dev")
            .Build();

        public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            .Build();

        public static AuthorizationPolicy age18Policy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            //.Requirements.Add(new AgeRequirement(18))
            .Build();

        public static AuthorizationPolicy assertionPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            // The Require Assertion method takes a lambda that receives the Http Context object and returns a Boolean value. 
            // Therefore, the assertion is simply a conditional statement.
            .RequireAssertion(ctx => { return ctx.User.HasClaim("editor", "contents") || ctx.User.HasClaim("level", "senior"); })
            .Build();
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