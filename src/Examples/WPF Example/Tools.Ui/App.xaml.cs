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
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var logger = PluginLoaderLogger.Instance;
            var appSettings = new AppSettings();
            var settings = new PluginLoaderSettings(appSettings);
            var toolPluginObjectCreatorFactory = new PluginObjectCreatorFactory<ITool>(settings, logger);
            var typeLoader = new TypeLoader<ITool>(PluginLoaderSettings.Default, logger);
            var assemblyNameReader = new AssemblyNameReader();
            var assemblyCache = new AssemblyCache(appDomain, assemblyNameReader, logger);
            var assemblyLoader = new AssemblyLoader(appDomain, settings, assemblyCache, assemblyNameReader, logger);
            var pluginDependencyObjectCreator = new PluginDependencyResolverObjectCreator(appDomain, settings, assemblyLoader, logger);
            var pluginDependencyResolverFactory = new PluginDependencyResolverCacheFactory(pluginDependencyObjectCreator, logger);
            var pluginCacheFactory = new PluginCacheFactory<ITool>(typeLoader, toolPluginObjectCreatorFactory, pluginDependencyResolverFactory, assemblyLoader, logger);
            var pluginPaths = new StaticPluginPaths(new[] { "Plugins" });
            var pluginLoader = new PluginLoader<ITool>(pluginPaths, pluginCacheFactory);
            var plugins = pluginLoader.LoadPlugins();
            Tools.AddRange(plugins.CreatePluginObjects());
        }
    }
}
