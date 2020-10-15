using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;
using Tool.Tools;

namespace Tool
{
    public class CavemanHammerRuntimePluginLoader : RuntimePluginLoaderBase<ICaveManTool<Hammer>>
    {
        public CavemanHammerRuntimePluginLoader(IAppDomain appDomain, 
                                                IPluginLoaderSettings settings,
                                                IPluginLoaderFactory<ICaveManTool<Hammer>> pluginLoaderFactory,
                                                IPluginObjectCreator<ICaveManTool<Hammer>> pluginObjectCreator,
                                                IPluginPaths pluginPaths = null, 
                                                IPluginLoaderLogger logger = null) 
            : base(appDomain, settings, pluginLoaderFactory, pluginObjectCreator, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder { get; }
    }

    class Program
    {
        static void Main()
        {
            var tools = new List<ITool>
            {
                new Hammer()
            };

            // Common objects
            var logger = PluginLoaderLogger.Instance;
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain, logger);
            var appSettings = new AppSettings();
            PluginLoaderSettings.Default = new MyPluginLoaderSettings(appSettings);
            var settings = PluginLoaderSettings.Default;
            var assemblyNameReader = new AssemblyNameReader();
            var assemblyCache = new AssemblyCache(appDomain, assemblyNameReader, logger);
            var assemblyLoader = new AssemblyLoader(appDomain, settings, assemblyCache, assemblyNameReader, logger);
            var pluginPaths = new AppPluginPaths(settings.AppName, settings.PluginFolder, appDomain, logger);
            var waiter = new Waiter(logger);
            var assemblyResolveCache = new AssemblyResolveCache();
            var pluginDependencyResolverObjectCreator = new PluginDependencyResolverObjectCreator(appDomain, settings, assemblyLoader, waiter, assemblyResolveCache, logger);
            var pluginDependencyResolverFactory = new PluginDependencyResolverCacheFactory(pluginDependencyResolverObjectCreator, logger);

            // ITool plugin loader objects
            var typeLoader = new TypeLoader<ITool>(settings, logger);
            var objectCreator = new ObjectCreator<ITool>();
            var pluginCacheFactory = new PluginCacheFactory<ITool>(typeLoader, pluginDependencyResolverFactory, assemblyLoader, logger);
            var pluginLoader = new PluginLoader<ITool>(pluginPaths, pluginCacheFactory);
            var plugins = pluginLoader.LoadPlugins();
            var toolPluginObjectCreator = new PluginObjectCreator<ITool>(settings, objectCreator, logger);
            tools.AddRange(plugins.CreatePluginObjects(toolPluginObjectCreator));

            // ICaveManTool<Hammer> plugin loader objects - using RuntimePluginLoader
            var caveManHammerRuntimePluginLoader = RuntimePluginLoaderFactory.Instance.Create<CavemanHammerRuntimePluginLoader, ICaveManTool<Hammer>>();
            var caveManObjectCreator = new ObjectCreator<ICaveManTool<Hammer>>();
            var caveManToolPluginObjectCreator = new PluginObjectCreator<ICaveManTool<Hammer>>(settings, caveManObjectCreator, logger);
            tools.AddRange(caveManHammerRuntimePluginLoader.PluginCollection.CreatePluginObjects(caveManToolPluginObjectCreator));

            ShowPrompt(tools);
            // Only 4 plugins will show as this doesn't support plugins with Constructor parameters
            int input = ReadLine(tools);
            while (input != 0)
            {
                if (input <= tools.Count)
                    Console.WriteLine(tools[input - 1].DoWork());
                ShowPrompt(tools);
                input = ReadLine(tools);
            }
        }

        private static void ShowPrompt(List<ITool> tools)
        {
            Console.WriteLine("Which tool do you want to use:");
            Console.WriteLine("0. Exit");
            var expectedToolCount = 5;
            int i = 0;
            for (; i < tools.Count;)
            {
                var tool = tools[i];
                Console.WriteLine("{0}. {1}", ++i, tool.Name);
            }
            if (i != expectedToolCount)
                Console.WriteLine($"Something went wrong. You should see {expectedToolCount} tools here.");
            Console.Write("Choose an option> ");
        }

        private static int ReadLine(List<ITool> tools)
        {
            int input;
            if (!int.TryParse(Console.ReadLine(), out input) || input > tools.Count)
            {
                Console.WriteLine("Invalid entry!");
            }
            return input;
        }
    }
}
