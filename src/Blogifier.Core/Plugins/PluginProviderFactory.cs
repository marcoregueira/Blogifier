namespace Blogifier.Core.Plugins
{
    public static class PluginProviderFactory
    {
        public static IPluginProvider PluginProvider { get; set; }

        static PluginProviderFactory()
        {
            PluginProvider = new PluginEnvironment();
        }
    }
}

