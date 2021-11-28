namespace Authentication
{
    using Authenticate.AppLib.Abstract;
    using Authenticate.AppLib.Concrete;
    using Authentication.AppLib.Evaluators;
    using Authentication.AppLib.StartupExt;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization.Policy;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using System;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services._InitMVC();

            services._AddViewLocationExpander();

            services._AddSession();

            services._AddAuthentication();

            services._AddAuthorization();





            // TODO: commen tout in PROD
            //services.AddSingleton<IPolicyEvaluator, TestPolicyEvaluator>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app._InitApp(env);

            app.UseStaticFiles();

            app.UseRouting();

            app._UseSession();

            app._UseAuthentication();

            app._UseAuthorization();            

            app.UseEndpoints(endpoints =>
            {
                // specific route should be created before the generic route
                endpoints.MapGet("/license", async context => { await context.Response.WriteAsync(System.IO.File.ReadAllText("license.md")); });
                endpoints.MapAreaControllerRoute(name: "admin", areaName: "admin", pattern: "admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(name: "areaRoute", pattern: "{area:exists}/{controller}/{action}");
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            /*
             * The big problem with the AuthorizeFilter approach is that it's an MVC-only feature. 
             * ASP.NET Core 3.0+ provides a different mechanism for setting authorization on endpoints—the 
             * RequireAuthorization() extension method on IEndpointConventionBuilder.
             * 
             * Instead of configuring a global AuthorizeFilter, call RequireAuthorization() when configuring 
             * the endpoints of your application, in Configure():
             */
            ////app.UseEndpoints(endpoints =>
            ////{
            ////    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").RequireAuthorization();
            ////    endpoints.MapHealthChecks("/health").RequireAuthorization();
            ////    endpoints.MapRazorPages().RequireAuthorization("MyCustomPolicy");
            ////    endpoints.MapHealthChecks("/healthz").RequireAuthorization("OtherPolicy", "MainPolicy");
            ////});

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Service was unable to handle this request.");
            });
        }
    }
}

// ------------------------------------------------------------------------------------------------------------
// The following Startup.Configure method adds security-related middleware components in the recommended order:
// ------------------------------------------------------------------------------------------------------------

//public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//{
//    if (env.IsDevelopment()) {
//        app.UseDeveloperExceptionPage();
//        app.UseDatabaseErrorPage();
//    }
//    else {
//        app.UseExceptionHandler("/Error");
//        app.UseHsts();
//    }
//    app.UseHttpsRedirection();
//    app.UseStaticFiles();
//    app.UseCookiePolicy();
//    app.UseRouting();
//    app.UseRequestLocalization();
//    app.UseCors();
//    app.UseAuthentication();
//    app.UseAuthorization();
//    app.UseSession();
//    app.UseResponseCaching();
//    app.UseEndpoints(endpoints =>
//    {
//        endpoints.MapRazorPages();
//        endpoints.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}");
//    });
//}