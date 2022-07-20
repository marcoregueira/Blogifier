using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;


namespace Blogifier.Core.Plugins
{
    // BASIC REFERENCE FOR THIS PART
    // https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support

    public class PluginEnvironment : IPluginProvider
    {
        private const string PLUGINS_FILE = "Data/plugins.txt";
        private const string PLUGINS_ENVIRONMENT_VARIABLE = "Enabled_plugins";
        private string _baseDirectory;
        private List<IPluginBootstrapper> _plugins = new List<IPluginBootstrapper>();

        public PluginEnvironment()
        {
            _baseDirectory = Environment.CurrentDirectory;
        }

        public void ConfigureKestrel(KestrelServerOptions k)
        {
            foreach (var plugin in _plugins)
                plugin.ConfigureKestrel(k);
        }

        public void ConfigureHost(IWebHostBuilder hostBuilder)
        {
            foreach (var plugin in _plugins)
                plugin.ConfigureHost(hostBuilder);
        }

        void IPluginBootstrapper.AddServices(IServiceCollection services)
        {
            foreach (var plugin in _plugins)
                plugin.AddServices(services);
        }

        public void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            foreach (var plugin in _plugins)
                plugin.ConfigureApp(app, env, serviceProvider);
        }

        public IPluginProvider LoadPlugins()
        {
            var pluginPaths = GetPluginList();

            _plugins = pluginPaths.SelectMany(pluginPath =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePluginBootstrapper(pluginAssembly);
            }).ToList();

            return this;
        }

        private List<string> GetPluginList()
        {
            var enabledPlugins = new List<string>();
            var pluginFile = Path.Combine(_baseDirectory, PLUGINS_FILE);
            if (File.Exists(pluginFile))
            {
                var lines = File.ReadAllLines(pluginFile);
                var pluginsInFile = lines
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Where(x => !x.StartsWith("*"))
                    .Where(x => !x.StartsWith("-"))
                    .Where(x => !x.StartsWith(";"))
                    .Where(x => !x.StartsWith("#"));

                enabledPlugins.AddRange(pluginsInFile);

                Console.WriteLine("Plugins in file", string.Join(",", pluginsInFile));
            }
            else
            {
                Console.WriteLine(pluginFile + " not found");
            }

            var environmentPlugins = Environment.GetEnvironmentVariable(PLUGINS_ENVIRONMENT_VARIABLE)?.Split(";");
            if (environmentPlugins != null)
            {
                enabledPlugins.AddRange(environmentPlugins);
            }

            Console.WriteLine("Requested plugins: " + string.Join(",", enabledPlugins));
            var pluginFolder = _baseDirectory;
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // For now this will suffice
            // In the future, we could load this from the settings file.
            // That means some additional changes to program.cs to load configuration explicitely
            pluginFolder = environment == "Development"
                ? Path.Combine("..", "..", "plugins")
                : "plugins";

            return enabledPlugins
                .Distinct()
                .Select(x => Path.Combine(pluginFolder, x))
                .ToList();
        }

        private Assembly LoadPlugin(string relativePath)
        {
            // Navigate up to the solution root
            var pluginLocation = Path.GetFullPath(Path.Combine(_baseDirectory, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            Console.WriteLine($"Loading plugin: {pluginLocation}");
            var loadContext = new PluginLoaderInfraestructure(pluginLocation);
            return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
        }

        private static IEnumerable<IPluginBootstrapper> CreatePluginBootstrapper(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPluginBootstrapper).IsAssignableFrom(type))
                {
                    var result = Activator.CreateInstance(type) as IPluginBootstrapper;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
                throw new ApplicationException(
                    $"Can't find any type which implements IPluginBootstrapper in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }
    }
}
