namespace Authentication.AppLib.StartupExt
{
    using Authenticate.AppLib.Concrete;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class SessionExtensions
    {
        public static void SetKey<T>(this ISession session, string key, T value)
        {
            // session.SetString(key, System.Text.Json.JsonSerializer.Serialize(value));
            session.SetString(key, Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        public static T GetKey<T>(this ISession session, string key)
        {
            string value = session.GetString(key);
            // return value == null ? default : System.Text.Json.JsonSerializer.Deserialize<T>(value);
            return value == null ? default : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }

        public static bool HasKey(this ISession session, string key)
        {
            return session.HasKey(key);
        }

        public static void RemoveKey(this ISession session, string key)
        {
            session.Remove(key);
        }
    }

    public static class SessionExtension
    {
        /// <summary>
        /// Use this extension at the beginning ConfigureServices method. 
        /// It also requires UseSession in the Configure method.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection _AddSession(this IServiceCollection services)
        {
            // The IDistributedCache implementation is used as a backing store for session.
            // (in-memory implementation of IDistributedCache)
            services.AddDistributedMemoryCache();

            // HttpContext.Session is available after session state is configured.
            // HttpContext.Session can't be accessed before UseSession has been called.
            services.AddSession(options =>
            {
                options.Cookie.Name = Constants.Session_Cookie_Name;
                options.IdleTimeout = Constants.Session_IdleTimeout;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }

        /// <summary>
        /// Call _UseSession after UseRouting and before UseEndpoints. 
        /// https://docs.microsoft.com/tr-tr/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1#order
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder _UseSession(this IApplicationBuilder app)
        {
            app.UseSession();

            return app;
        }
    }
}
