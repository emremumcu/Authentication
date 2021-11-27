﻿namespace Authentication.AppLib.StartupExt
{
    using Authentication.AppLib.Filters;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;

    public static class InitMvcExtension
    {
        public static IServiceCollection _InitMVC(this IServiceCollection services)
        {
            IMvcBuilder mvcBuilder = services.AddControllersWithViews(
                config => {
                    config.Filters.Add(new AuthorizeFilter());
                    config.Filters.Add(new AddHeaderFilter("X-Frame-Options", "SAMEORIGIN")); // prevent click-jacking
                    // config.Conventions.Add(new ControllerBasedAuthorizeFilterConvention());
                })
                /// Use session based TempData instead of cookie based TempData
                .AddSessionStateTempDataProvider();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                /// Install-Package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation   
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            services.AddRazorPages();

            services.AddHttpContextAccessor();

            return services;
        }

        public static IApplicationBuilder _InitApp(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            return app;
        }
    }
}