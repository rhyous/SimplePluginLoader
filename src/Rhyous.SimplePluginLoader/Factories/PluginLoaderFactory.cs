namespace Rhyous.SimplePluginLoader
{
    public class PluginLoaderFactory<T> : IPluginLoaderFactory<T> 
        where T : class
    {
        private readonly IPluginCacheFactory<T> _PluginCacheFactory;

        public PluginLoaderFactory(IPluginCacheFactory<T> pluginCacheFactory)
        {
            _PluginCacheFactory = pluginCacheFactory;
        }

        public IPluginLoader<T> Create(IPluginPaths pluginPaths)
        {
            return new PluginLoader<T>(pluginPaths, _PluginCacheFactory);
        }
    }
}