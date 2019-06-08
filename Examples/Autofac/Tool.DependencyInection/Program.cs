using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;
using Tool.Tools;
using Autofac;

namespace Tool
{
    class Program
    {
        static void Main()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<PluginLoaderLogger>()
                   .As<IPluginLoaderLogger>()
                   .SingleInstance();
            builder.Register(c => AppDomain.CurrentDomain)
                   .As<AppDomain>()
                   .SingleInstance();
            builder.Register(c => new AppDomainWrapper(c.Resolve<AppDomain>()))
                   .As<IAppDomain>()
                   .SingleInstance();
            builder.RegisterType<ObjectCreator<ITool>>()
                   .As<IObjectCreator<ITool>>()
                   .SingleInstance();
            builder.Register(c => new PluginLoader<ITool>(null,
                                     c.Resolve<IAppDomain>(),
                                     c.Resolve<IObjectCreator<ITool>>(),
                                     c.Resolve<IPluginLoaderLogger>()))
                   .As<ILoadPlugins<ITool>>()
                   .SingleInstance();
            builder.RegisterType<ObjectCreator<ICaveManTool<Hammer>>>()
                   .As<IObjectCreator<ICaveManTool<Hammer>>>()
                   .SingleInstance();
            builder.Register(c => new PluginLoader<ICaveManTool<Hammer>>(null,
                                     c.Resolve<IAppDomain>(),
                                     c.Resolve<IObjectCreator<ICaveManTool<Hammer>>>(),
                                     c.Resolve<IPluginLoaderLogger>()))
                   .As<ILoadPlugins<ICaveManTool<Hammer>>>()
                   .SingleInstance();
            var container = builder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var pluginLoader = scope.Resolve<ILoadPlugins<ITool>>();
                var tools = new List<ITool>
                {
                    new Hammer()
                };

                var plugins = pluginLoader.LoadPlugins();
                tools.AddRange(plugins.AllObjects);

                var pluginLoaderCaveMan = scope.Resolve<ILoadPlugins<ICaveManTool<Hammer>>>();
                var caveManPlugins = pluginLoaderCaveMan.LoadPlugins();
                tools.AddRange(caveManPlugins.AllObjects);

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
            for (int i = 0; i < tools.Count;)
            {
                var tool = tools[i];
                Console.WriteLine("{0}. {1}", ++i, tool.Name);
            }
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
