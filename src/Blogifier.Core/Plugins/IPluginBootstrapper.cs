
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Blogifier.Core.Plugins
{
    /// <summary>
    /// Plugin startup hooks
    /// </summary>
    public interface IPluginBootstrapper
    {
        // Called from Program.cs
        public void ConfigureHost(IWebHostBuilder webHostBuilder);

        // Called from Program.cs
        public void ConfigureKestrel(KestrelServerOptions kestrel);

        // Called from Startup.cs
        public void AddServices(IServiceCollection services);

        // Called from Startup.cs
        public void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider);
    }
}
