using Blogifier.Core.Plugins;
using Blogifier.Core.Providers;
using Blogifier.Core.Web;
using Blogifier.Core.Web.Theme;
using Blogifier.Plugin.Gpdr.Config;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Blogifier.Plugin.Gpdr
{
    public class PluginStartup : IPluginBootstrapper
    {
        private GpdrSettings _gpdrSettings;

        public PluginStartup()
        {
            _gpdrSettings = LoadPluginConfiguration();
        }

        public void AddServices(IServiceCollection services)
        {
            Console.WriteLine("GPDR AddServices");
            var pluginAssembly = GetType().Assembly;

            services.AddSingleton<IWidgetListProvider, LocalWidgetProvider>();

            services.AddMvc().AddApplicationPart(GetType().Assembly);
            services.AddControllersWithViews()
                  .AddApplicationPart(pluginAssembly)
                  .AddRazorOptions(options => options.ViewLocationExpanders.Add(new StaticLocationExtender("ViewsGpdr")));


            //services.Configure<StaticFileOptions>(x => x.FileProvider =
            //    new CompositeFileProvider(x.FileProvider, new EmbeddedFileProvider(this.GetType().Assembly, "Blogifier.Plugin.Gpdr")));
        }

        public void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            var folder = Path.GetDirectoryName(Path.GetDirectoryName(env.ContentRootPath));
            var storage = serviceProvider.GetService<IStorageProvider>();
            var pluginsRoot = storage.FindFolderInRoute(env.ContentRootPath, "plugins");

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new EmbeddedFileProvider(this.GetType().Assembly),
            //    RequestPath = "/plugins/gpdr"
            //});

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(pluginsRoot.path, "gpdr/Content")),
            //    RequestPath = "/plugins/gpdr"
            //});

            var themeAssembly = GetType().Assembly;
            var location = Path.GetDirectoryName(themeAssembly.Location);
            var localwwwroot = Path.Combine(pluginsRoot.path, Path.Combine(location, "wwwroot"));
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(localwwwroot),
                RequestPath = "/plugins/gpdr"
            });


        }

        public void ConfigureHost(IWebHostBuilder webHostBuilder)
        {
        }

        public void ConfigureKestrel(KestrelServerOptions kestrel)
        {
        }

        #region Private methods

        private GpdrSettings LoadPluginConfiguration()
        {
            // Read plugin config to a temporary object

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var location = typeof(PluginStartup).Assembly.GetFiles()[0].Name;
            var path = Path.GetDirectoryName(location);

            Console.WriteLine("Loading config files in " + path);
            var config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var settings = new GpdrSettings();
            var _gpdrConfigSection = config.GetSection("Gpdr");
            _gpdrConfigSection.Bind(settings);

            return settings;
        }

        #endregion Private methods
    }
}
