namespace Rhyous.SimplePluginLoader
{
    public interface IPluginLoaderFactory<T> where T : class
    {
        IPluginLoader<T> Create(IPluginPaths pluginPaths);
    }
}