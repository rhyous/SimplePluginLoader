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
            var logger = new PluginLoaderLogger();
            var appSettings = new AppSettings();
            var settings = new PluginLoaderSettings(appSettings);
            var toolObjectCreatorFactory = new ObjectCreatorFactory<ITool>();
            var typeLoader = new TypeLoader<ITool>(PluginLoaderSettings.Default, logger);
            var instanceLoaderFactory = new InstanceLoaderFactory<ITool>(toolObjectCreatorFactory, typeLoader, PluginLoaderSettings.Default, logger);
            var assemblyLoader = new AssemblyLoader(appDomain, settings, logger);
            var pluginLoader = new PluginLoader<ITool>(null, appDomain, PluginLoaderSettings.Default, typeLoader, instanceLoaderFactory,
                                                       assemblyLoader, logger);
            var plugins = pluginLoader.LoadPlugins();
            Tools.AddRange(plugins.AllObjects);
        }
    }
}
