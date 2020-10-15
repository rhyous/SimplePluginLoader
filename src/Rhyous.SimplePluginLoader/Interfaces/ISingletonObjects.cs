namespace Rhyous.SimplePluginLoader
{
    public interface ISingletonObjects
    {
        IAppDomain AppDomain { get; set; }
        IAppSettings AppSettings { get; set; }
        IAssemblyCache AssemblyCache { get; set; }
        AssemblyLoader AssemblyLoader { get; set; }
        IAssemblyNameReader AssemblyNameReader { get; set; }
        IPluginLoaderLogger Logger { get; set; }
        IPluginDependencyResolverCacheFactory PluginDependencyResolverFactory { get; set; }
        IPluginDependencyResolverObjectCreator PluginDependencyResolverObjectCreator { get; set; }
        IPluginPaths PluginPaths { get; set; }
        IPluginLoaderSettings Settings { get; set; }
    }
}