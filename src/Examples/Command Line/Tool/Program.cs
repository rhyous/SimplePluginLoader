using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tool.Tools;

namespace Tool
{
    class Program
    {
        static void Main()
        {
            var tools = new List<ITool>
            {
                new Hammer()
            };
            
            // Common objects
            var logger = new PluginLoaderLogger();
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain, logger);
            var appSettings = new AppSettings();
            var settings = new PluginLoaderSettings(appSettings);
            var assemblyNameReader = new AssemblyNameReader();
            var assemblyLoader = new AssemblyLoader(appDomain, settings, AssemblyCache.Instance, assemblyNameReader, logger);
            string appName = "Tool.CommandLine";
            string appSubFolder = null;
            var pluginPaths = new PluginPaths(appName, appSubFolder, appDomain, logger);
            var pluginDependencyResolverObjectCreator = new PluginDependencyResolverObjectCreator(appDomain, settings, assemblyLoader, logger);
            var pluginDependencyResolverFactory = new PluginDependencyResolverCacheFactory(pluginDependencyResolverObjectCreator, logger);

            // ITool plugin loader objects
            var typeLoader = new TypeLoader<ITool>(settings, logger);
            var toolPluginObjectCreatorFactory = new PluginObjectCreatorFactory<ITool>(settings, logger);
            var pluginCacheFactory = new PluginCacheFactory<ITool>(typeLoader, toolPluginObjectCreatorFactory, pluginDependencyResolverFactory, assemblyLoader, logger);
            var pluginLoader = new PluginLoader<ITool>(pluginPaths, pluginCacheFactory);
            var plugins = pluginLoader.LoadPlugins();
            tools.AddRange(plugins.CreatePluginObjects());

            // ICaveManTool<Hammer> plugin loader objects
            var caveManTypeLoader = new TypeLoader<ICaveManTool<Hammer>>(PluginLoaderSettings.Default, logger);
            var caveManToolObjectCreatorFactory = new PluginObjectCreatorFactory<ICaveManTool<Hammer>>(settings, logger);
            var caveManPluginCacheFactory = new PluginCacheFactory<ICaveManTool<Hammer>>(caveManTypeLoader, caveManToolObjectCreatorFactory, pluginDependencyResolverFactory, assemblyLoader, logger);
            var pluginLoaderCaveMan = new PluginLoader<ICaveManTool<Hammer>>(pluginPaths, caveManPluginCacheFactory);
            var caveManPlugins = pluginLoaderCaveMan.LoadPlugins();
            tools.AddRange(caveManPlugins.CreatePluginObjects());
            
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
