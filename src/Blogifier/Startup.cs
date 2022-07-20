using System;
using System.IO;

using Blogifier.Core.Extensions;
using Blogifier.Core.Plugins;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Blogifier
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration()
                  .Enrich.FromLogContext()
                  .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                  .CreateLogger();

            Log.Warning("Application start");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Console.WriteLine("Start configure services: ", env);

            services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });

            services.AddAuthentication(auth =>
                auth.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            services.AddCors(o => o.AddPolicy("BlogifierPolicy", cors =>
                cors.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

            services.AddBlogDatabase(Configuration);
            services.AddBlogProviders();
            services.AddControllersWithViews();

            services.AddMvc();

            services.AddPlugins();

            //services.AddTransient<ViewLocationExtender>();
            //services.AddOptions<RazorViewEngineOptions>().Configure<ViewLocationExtender>((options, expander) => options.ViewLocationExpanders.Add(expander));

            Log.Warning("Done configure services");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseSerilogRequestLogging();

            app.UsePlugins(env, serviceProvider);
            app.UseBlazorFrameworkFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseCors("BlogifierPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = "/data",
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Data/public"))
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                      name: "default",
                      pattern: "{controller=Home}/{action=Index}/{id?}"
                 );


                endpoints.MapRazorPages();
                endpoints.MapFallbackToFile("admin/{*path:nonfile}", "index.html");
                endpoints.MapFallbackToFile("account/{*path:nonfile}", "index.html");
            });
        }
    }
}
