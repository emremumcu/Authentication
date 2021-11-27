using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication.AppLib.StartupExt
{
    public static class Template
    {
        public static IServiceCollection _ConfigureServicesBase(this IServiceCollection services)
        {
            // ---
            // add custom logic here (if required)
            // ---

            return services;
        }

        public static IApplicationBuilder _ConfigureBase(this IApplicationBuilder app)
        {
            // ---
            // add custom logic here (if required)
            // ---

            return app;
        }
    }
}
