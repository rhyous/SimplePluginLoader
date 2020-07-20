using Autofac;
using Rhyous.SimplePluginLoader;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;
using System.Collections.Generic;
using Tool.Tools;

namespace Tool.DependencyInjection
{
    class Program
    {
        static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<SimplePluginLoaderModule>();
            builder.RegisterGeneric(typeof(CustomPluginObjectCreator<>))
                   .As(typeof(IPluginObjectCreator<>));

            builder.RegisterInstance(new StaticPluginPaths(new[] { "Plugins" }))
                   .As<IPluginPaths>();

            // ITool Plugin Loader registrations
            builder.RegisterType<PluginLoader<ITool>>()
                   .As<IPluginLoader<ITool>>()
                   .SingleInstance();

            // ICaveManTool<Hammer>> Plugin Loader registrations
            builder.RegisterType<Hammer>();
            builder.RegisterType<PluginLoader<ICaveManTool<Hammer>>>()
                   .As<IPluginLoader<ICaveManTool<Hammer>>>()
                   .SingleInstance();

            var container = builder.Build();
            using (var globalScope = container.BeginLifetimeScope())
            {
                var tools = new List<ITool> { new Hammer() };

                using (var iToolScope = globalScope.BeginLifetimeScope(b=>
                {
                    b.RegisterType<ScopedSetting1>().As<IScopedSetting>();
                }))
                {
                    var pluginLoader = iToolScope.Resolve<IPluginLoader<ITool>>();
                    var pluginObjectCreator = iToolScope.Resolve<IPluginObjectCreator<ITool>>();
                    var plugins = pluginLoader.LoadPlugins();
                    tools.AddRange(plugins.CreatePluginObjects(pluginObjectCreator));
                }

                using (var caveManScope = globalScope.BeginLifetimeScope(b =>
                {
                    b.RegisterType<ScopedSetting2>().As<IScopedSetting>();
                }))
                {
                    var pluginLoaderCaveMan = caveManScope.Resolve<IPluginLoader<ICaveManTool<Hammer>>>();
                    var pluginObjectCreatorCaveMan = caveManScope.Resolve<IPluginObjectCreator<ICaveManTool<Hammer>>>();
                    var caveManPlugins = pluginLoaderCaveMan.LoadPlugins();
                    tools.AddRange(caveManPlugins.CreatePluginObjects(pluginObjectCreatorCaveMan));
                }

                ShowPrompt(tools);
                int input = ReadLine(tools);
                while (input != 0)
                {
                    if (input <= tools.Count)
                        Console.WriteLine(tools[input - 1].DoWork());
                    ShowPrompt(tools);
                    input = ReadLine(tools);
                }
            }
        }

        private static void ShowPrompt(List<ITool> tools)
        {
            Console.WriteLine("Which tool do you want to use:");
            Console.WriteLine("0. Exit");
            var expectedToolCount = 8;
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
