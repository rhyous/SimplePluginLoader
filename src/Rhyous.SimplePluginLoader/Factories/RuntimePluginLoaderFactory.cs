using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class RuntimePluginLoaderFactory : IRuntimePluginLoaderFactory
    {
        #region Singleton

        public static IRuntimePluginLoaderFactory Instance { get; set; } = new RuntimePluginLoaderFactory();

        internal RuntimePluginLoaderFactory() { }

        public ISingletonObjects Singletons { get; set; }

        #endregion

        public IRuntimePluginLoader<T> Create<TRuntimePluginLoader, T>(params object[] dependencies)
            where TRuntimePluginLoader : class, IRuntimePluginLoader<T>
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
    }
}
