using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Blogifier.Core.Plugins
{
    public static class WebHostPluginExtensions
    {
        public static IWebHostBuilder UsePlugins(this IWebHostBuilder hostBuilder)
        {
            PluginProviderFactory.PluginProvider.ConfigureHost(hostBuilder);
            return hostBuilder;
        }

        public static IServiceCollection AddPlugins(this IServiceCollection services)
        {
            PluginProviderFactory.PluginProvider.AddServices(services);
            return services;
        }

        public static IApplicationBuilder UsePlugins(this IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            PluginProviderFactory.PluginProvider.ConfigureApp(app, env, serviceProvider);
            return app;
        }
    }
}
