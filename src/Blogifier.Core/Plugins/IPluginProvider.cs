namespace Blogifier.Core.Plugins
{
    public interface IPluginProvider : IPluginBootstrapper
    {
        IPluginProvider LoadPlugins();
    }
}
