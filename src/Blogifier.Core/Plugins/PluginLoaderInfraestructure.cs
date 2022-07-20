using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Blogifier.Core.Plugins
{
    // BASIC REFERENCE FOR THIS PART
    // https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support

    public class PluginLoaderInfraestructure : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoaderInfraestructure(string pluginPath)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
