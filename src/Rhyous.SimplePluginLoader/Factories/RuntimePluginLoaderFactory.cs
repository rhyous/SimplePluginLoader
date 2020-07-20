using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Factories
{
    public class RuntimePluginLoaderFactory : IRuntimePluginLoaderFactory
    {
        #region Singleton

        public static RuntimePluginLoaderFactory Instance { get; set; } = new RuntimePluginLoaderFactory();

        internal RuntimePluginLoaderFactory() { }

        public SingletonObjects Singletons { get; set; }

        #endregion

        public IRuntimePluginLoader<T> Create<TRuntimePluginLoader, T>(params object[] dependencies)
            where TRuntimePluginLoader : IRuntimePluginLoader<T>
            where T : class
        {
            if (Singletons == null)
                Singletons = new SingletonObjects();
            var typeLoader = new TypeLoader<T>(Singletons.Settings, Singletons.Logger);
            var pluginCacheFactory = new PluginCacheFactory<T>(typeLoader, 
                                                               Singletons.PluginDependencyResolverFactory,
                                                               Singletons.AssemblyLoader, 
                                                               Singletons.Logger);
            var pluginLoaderFactory = new PluginLoaderFactory<T>(pluginCacheFactory);
            var pluginObjectCreator = new PluginObjectCreator<T>(Singletons.Settings, new ObjectCreator<T>(), Singletons.Logger);
            IRuntimePluginLoader<T> runtimePluginLoader = (dependencies == null || !dependencies.Any())
                ? Activator.CreateInstance(typeof(TRuntimePluginLoader),
                                           Singletons.AppDomain,
                                           Singletons.Settings,
                                           pluginLoaderFactory,
                                           pluginObjectCreator,
                                           Singletons.PluginPaths,
                                           Singletons.Logger) as IRuntimePluginLoader<T>
                : Activator.CreateInstance(typeof(TRuntimePluginLoader),
                                           Singletons.AppDomain,
                                           Singletons.Settings,
                                           pluginLoaderFactory,
                                           pluginObjectCreator,
                                           Singletons.PluginPaths,
                                           Singletons.Logger,
                                           dependencies) as IRuntimePluginLoader<T>;
            return runtimePluginLoader;
        }

        public class SingletonObjects
        {
            public IPluginLoaderSettings Settings
            {
                get => _Settings ?? (_Settings = PluginLoaderSettings.Default);
                set => _Settings = value;
            } private IPluginLoaderSettings _Settings;

            public IPluginLoaderLogger Logger => PluginLoaderLogger.Instance;
            public IAppDomain AppDomain => new AppDomainWrapper(System.AppDomain.CurrentDomain, Logger);
            public IAppSettings AppSettings => new AppSettings();
            public IAssemblyNameReader AssemblyNameReader => new AssemblyNameReader();
            public IAssemblyCache AssemblyCache => new AssemblyCache(AppDomain, AssemblyNameReader, Logger);
            public AssemblyLoader AssemblyLoader => new AssemblyLoader(AppDomain, Settings, AssemblyCache, AssemblyNameReader, Logger);
            public IPluginPaths PluginPaths => new AppPluginPaths(Settings.AppName, Settings.PluginFolder, AppDomain, Logger);
            public IPluginDependencyResolverObjectCreator PluginDependencyResolverObjectCreator 
                         => new PluginDependencyResolverObjectCreator(AppDomain, Settings, AssemblyLoader, Logger);
            public IPluginDependencyResolverCacheFactory PluginDependencyResolverFactory 
                        => new PluginDependencyResolverCacheFactory(PluginDependencyResolverObjectCreator, Logger);
        }
    }
}
