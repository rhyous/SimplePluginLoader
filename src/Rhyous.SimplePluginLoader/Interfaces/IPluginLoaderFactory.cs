namespace Rhyous.SimplePluginLoader
{
    public interface IPluginLoaderFactory<T>
    {
        IPluginLoader<T> Create(IPluginPaths pluginPaths);
    }
}