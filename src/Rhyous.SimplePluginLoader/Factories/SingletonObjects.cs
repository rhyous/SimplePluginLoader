namespace Rhyous.SimplePluginLoader
{
    public class SingletonObjects : ISingletonObjects
    {
        public SingletonObjects()
        {
            Settings = PluginLoaderSettings.Default;
            AppSettings = new AppSettings();
            Logger = PluginLoaderLogger.Factory(AppSettings);
            AppDomain = new AppDomainWrapper(System.AppDomain.CurrentDomain, Logger);
            AssemblyNameReader = new AssemblyNameReader();
            AssemblyCache = new AssemblyCache(AppDomain, AssemblyNameReader, Logger);
            AssemblyLoader = new AssemblyLoader(AppDomain, Settings, AssemblyCache, AssemblyNameReader, Logger);
            PluginPaths = new AppPluginPaths(Settings.AppName, Settings.PluginFolder, AppDomain, Logger);
            Waiter = new Waiter(Logger);
            AssemblyResolveCache = new AssemblyResolveCache();
            PluginDependencyResolverObjectCreator = new PluginDependencyResolverObjectCreator(AppDomain, Settings, AssemblyLoader, Waiter, AssemblyResolveCache, Logger);
            PluginDependencyResolverFactory = new PluginDependencyResolverCacheFactory(PluginDependencyResolverObjectCreator, Logger);
        }

        public virtual IPluginLoaderSettings Settings { get; set; }
        public virtual IPluginLoaderLogger Logger { get; set; }
        public virtual IAppDomain AppDomain { get; set; }
        public virtual IAppSettings AppSettings { get; set; }
        public virtual IAssemblyNameReader AssemblyNameReader { get; set; }
        public virtual IAssemblyCache AssemblyCache { get; set; }
        public virtual AssemblyLoader AssemblyLoader { get; set; }
        public virtual IPluginPaths PluginPaths { get; set; }
        public virtual IWaiter Waiter { get; set; }
        public virtual IAssemblyResolveCache AssemblyResolveCache { get; set; }
        public virtual IPluginDependencyResolverObjectCreator PluginDependencyResolverObjectCreator { get; set; }
        public virtual IPluginDependencyResolverCacheFactory PluginDependencyResolverFactory { get; set; }
    }
}
