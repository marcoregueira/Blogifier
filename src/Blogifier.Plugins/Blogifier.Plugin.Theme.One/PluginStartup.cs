using Blogifier.Core.Plugins;
using Blogifier.Core.Providers;
using Blogifier.Core.Web;
using Blogifier.Core.Web.Theme;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Blogifier.Plugin.Theme.One
{
    public class PluginStartup : IPluginBootstrapper
    {
        private IConfigurationSection? _configSection;
        private readonly OneThemeSettings _settings;

        public PluginStartup()
        {
            _settings = LoadPluginConfiguration();
        }

        public void AddServices(IServiceCollection services)
        {
            Console.WriteLine("ThemeOne AddServices");


            // ADD LOCAL SERVICES
            services.AddSingleton<IWidgetListProvider, LocalWidgetProvider>();

            // REGISTER OPTIONS
            services.AddOptions<OneThemeSettings>();
            services.AddSingleton<IConfigureOptions<OneThemeSettings>>(s =>
                new ConfigureOptions<OneThemeSettings>(o => _configSection.Bind(o)));

            // REGISTER CONTROLLERS AND COMPONENTS
            var themeAssembly = GetType().Assembly;
            var location = Path.GetDirectoryName(themeAssembly.Location);

            services.AddControllersWithViews()
                  .AddApplicationPart(themeAssembly)
                  .AddRazorOptions(options => options.ViewLocationExpanders.Add(new StaticLocationExtender("Theme")));

            // services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
            // {
            //     options.FileProviders.Add(new PhysicalFileProvider(location));
            // });
        }

        public void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            var folder = Path.GetDirectoryName(Path.GetDirectoryName(env.ContentRootPath));
            var storage = serviceProvider.GetService<IStorageProvider>()
                ?? throw new NullReferenceException("IStorageProvider is not registered");

            var pluginsRoot = storage.FindFolderInRoute(env.ContentRootPath, "plugins");
            var themeAssembly = GetType().Assembly;
            var location = Path.GetDirectoryName(themeAssembly.Location);
            var localwwwroot = Path.Combine(pluginsRoot.path, Path.Combine(location, "wwwroot"));

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(localwwwroot),
                RequestPath = "/_content/Blogifier.Plugin.Theme.One"
            });
        }

        public void ConfigureHost(IWebHostBuilder webHostBuilder)
        {
        }

        public void ConfigureKestrel(KestrelServerOptions kestrel)
        {
        }

        #region Private methods

        private OneThemeSettings LoadPluginConfiguration()
        {
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

            var settings = new OneThemeSettings();
            _configSection = config.GetSection("OneTheme");
            _configSection.Bind(settings);

            return settings;
        }

        #endregion Private methods
    }
}
