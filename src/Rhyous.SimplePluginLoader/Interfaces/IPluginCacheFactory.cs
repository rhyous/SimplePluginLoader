namespace Rhyous.SimplePluginLoader
{
    public interface IPluginCacheFactory<T> : ICacheFactory<string, IPlugin<T>>
    { }
}