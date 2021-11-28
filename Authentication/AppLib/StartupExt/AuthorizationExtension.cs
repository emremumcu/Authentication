namespace Authentication.AppLib.StartupExt
{
    using Authenticate.AppLib.Abstract;
    using Authenticate.AppLib.Concrete;
    using Authentication.AppLib.Requirements;
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
                options.DefaultPolicy = AuthorizationPolicyLibrary.defaultPolicy;
                options.FallbackPolicy = AuthorizationPolicyLibrary.fallbackPolicy;               
                options.AddPolicy(AdminRequirement.PolicyName, AuthorizationPolicyLibrary.adminPolicy);
                //options.AddPolicy(DeveloperRequirement.PolicyName, AuthorizationPolicyLibrary.developerPolicy);
            });

            services.AddSingleton<IAuthorizationHandler, BaseHandler>();
            services.AddSingleton<IAuthorizationHandler, UserHandler>();
            services.AddSingleton<IAuthorizationHandler, AdminHandler>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

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
           .AddRequirements(new BaseRequirement())           
           .Build();

        public static AuthorizationPolicy fallbackPolicy = new AuthorizationPolicyBuilder()
           .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
           .RequireAuthenticatedUser()
           .AddRequirements(new BaseRequirement())
           .Build();

        public static AuthorizationPolicy userPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .AddRequirements(new BaseRequirement())
            .AddRequirements(new UserRequirement("USER"))
            //.RequireRole("USER")
            .Build();

        public static AuthorizationPolicy adminPolicy = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .AddRequirements(new BaseRequirement())
            .AddRequirements(new AdminRequirement("ADMIN","ADMINISTRATOR"))
            //.RequireRole("ADMIN")
            .Build();

        //public static AuthorizationPolicy assertionPolicy = new AuthorizationPolicyBuilder()
        //    .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
        //    .RequireAuthenticatedUser()
        //    .RequireRole("admin")
        //    // The Require Assertion method takes a lambda that receives the Http Context object and returns a Boolean value. 
        //    // Therefore, the assertion is simply a conditional statement.
        //    .RequireAssertion(ctx => { return ctx.User.HasClaim("editor", "contents") || ctx.User.HasClaim("level", "senior"); })
        //    .Build();
    }
}

// Snippets
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


//public static IServiceCollection _AddAuthorization(this IServiceCollection services)
//{
//    services.AddAuthorization(options =>
//    {
//        /*
//         * Gets or sets the default authorization policy with no policy name specified.
//         * Defaults to require authenticated users.
//         * 
//         * The DefaultPolicy is the policy that is applied when:
//         *      (*) You specify that authorization is required, either using RequireAuthorization(), by applying an AuthorizeFilter, 
//         *          or by using the[Authorize] attribute on your actions / Razor Pages.
//         *      (*) You don't specify which policy to use.
//         *      
//         */
//        options.DefaultPolicy = new AuthorizationPolicyBuilder()
//             .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
//             .RequireAuthenticatedUser()
//             .Build();

//        /*
//         * Gets or sets the fallback authorization policy when no IAuthorizeData have been provided.
//         * By default the fallback policy is null.
//         * 
//         * The FallbackPolicy is applied when the following is true:
//         *      (*) The endpoint does not have any authorisation applied. No[Authorize] attribute, no RequireAuthorization, nothing.
//         *      (*) The endpoint does not have an[AllowAnonymous] applied, either explicitly or using conventions.
//         *      
//         * So the FallbackPolicy only applies if you don't apply any other sort of authorization policy, 
//         * including the DefaultPolicy, When that's true, the FallbackPolicy is used.
//         * By default, the FallbackPolicy is a no - op; it allows all requests without authorization.
//         * 
//         */
//        options.FallbackPolicy = new AuthorizationPolicyBuilder()
//             .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
//             .RequireAuthenticatedUser()
//             .Build();


//        options.AddPolicy("ClaimPolicy", policy => policy.RequireClaim("claim1"));
//        options.AddPolicy("ClaimWithValuePolicy", policy => policy.RequireClaim("claim2", "val1", "val2"));
//        options.AddPolicy("SingleRolePolicy", policy => policy.RequireRole("role1"));
//        options.AddPolicy("MultiRolePolicy", policy => policy.RequireRole("role1", "role2"));
//        options.AddPolicy("ClaimRoleCombinedPolicy", policy => { policy.RequireClaim("claim1"); policy.RequireRole("role1"); });
//        options.AddPolicy("InlineDefinedPolicy", policy =>
//        {
//            policy.AddAuthenticationSchemes("Cookie, Bearer");
//            policy.RequireAuthenticatedUser();
//            policy.RequireRole("Admin");
//            policy.RequireClaim("editor", "contents");
//        });
//        options.AddPolicy("DefinedPolicy", AuthorizationPolicyLibrary.assertionPolicy);
//        options.AddPolicy("PolictWithRequirement", policy => policy.Requirements.Add(new AppLib.Requirements.MinimumAgeRequirement(18)));
//    });

//    services.AddSingleton<IAuthorizationHandler, AppLib.AuthorizationHandlers.MinimumAgeHandler>();

//    return services;
//}