namespace Rhyous.SimplePluginLoader
{
    public class PluginDependencyResolverCacheFactory : CacheFactory<string, IPluginDependencyResolver>,
                                                        IPluginDependencyResolverCacheFactory
    {

        public PluginDependencyResolverCacheFactory(IPluginDependencyResolverObjectCreator objectCreator, 
                                                    IPluginLoaderLogger logger)
            :base(objectCreator, logger)
        {
        }
    }
}
