using System;
using System.Collections.Generic;
using System.Windows;
using Rhyous.SimplePluginLoader;
using Tool;
using Tool.Tools;

namespace Tools.Ui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static List<ITool> Tools
        {
            get { return _Tools ?? (_Tools = new List<ITool>()); }
            set { _Tools = value; }
        } private static List<ITool> _Tools;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Tools.Add(new Hammer());
            var appSettings = new AppSettings();
            var logger = PluginLoaderLogger.Factory(appSettings);
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain, logger);
            var settings = new PluginLoaderSettings(appSettings);
            var typeLoader = new TypeLoader<ITool>(PluginLoaderSettings.Default, logger);
            var assemblyNameReader = new AssemblyNameReader();
            var assemblyCache = new AssemblyCache(appDomain, assemblyNameReader, logger);
            var assemblyLoader = new AssemblyLoader(appDomain, settings, assemblyCache, assemblyNameReader, logger);
            var waiter = new Waiter(logger);
            var assemblyResolveCache = new AssemblyResolveCache();
            var pluginDependencyObjectCreator = new PluginDependencyResolverObjectCreator(appDomain, settings, assemblyLoader, waiter, assemblyResolveCache, logger);
            var pluginDependencyResolverFactory = new PluginDependencyResolverCacheFactory(pluginDependencyObjectCreator, logger);
            var pluginCacheFactory = new PluginCacheFactory<ITool>(typeLoader, pluginDependencyResolverFactory, assemblyLoader, logger);
            var pluginPaths = new StaticPluginPaths(new[] { "Plugins" });
            var pluginLoader = new PluginLoader<ITool>(pluginPaths, pluginCacheFactory);
            var plugins = pluginLoader.LoadPlugins();
            var objectCreator = new ObjectCreator<ITool>();
            var toolPluginObjectCreator = new PluginObjectCreator<ITool>(settings, objectCreator, logger);
            Tools.AddRange(plugins.CreatePluginObjects(toolPluginObjectCreator));
        }
    }
}
