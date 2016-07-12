namespace Rhyous.SimplePluginLoader
{
    interface ILoadPlugins<T> where T : class
    {
        PluginCollection<T> LoadPlugins();
    }
}