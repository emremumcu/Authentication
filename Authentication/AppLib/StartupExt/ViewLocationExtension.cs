using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Authentication.AppLib.StartupExt
{
    public static class ViewLocationExtension
    {
        public static IServiceCollection _AddViewLocationExpander(this IServiceCollection services)
        {
            services
                .Configure<RazorViewEngineOptions>(options =>
                {
                    options.ViewLocationExpanders.Add(new ViewLocationExpander());
                });

            return services;
        }

        public static IApplicationBuilder _InitApp(this IApplicationBuilder app)
        {
            return app;
        }
    }

    public class ViewLocationExpander : IViewLocationExpander
    {
        // Adds "Partials" folder in default search locations for views etc.

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //{2} is for Area, {1} is for Controller,{0} is for Action
            string[] locations = new string[] {                 
                "/ViewPartials/{0}" + RazorViewEngine.ViewExtension 
            };

            return locations.Union(viewLocations);
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            context.Values["customviewlocation"] = nameof(ViewLocationExpander);
        }
    }
}
