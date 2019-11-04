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
            builder.RegisterType<PluginLoaderLogger>()
                   .As<IPluginLoaderLogger>()
                   .SingleInstance();
            builder.RegisterInstance(AppDomain.CurrentDomain).SingleInstance();
            builder.RegisterType<AppDomainWrapper>().As<IAppDomain>().SingleInstance();
            builder.RegisterType<AutofacObjectCreator<ITool>>()
                   .As<IObjectCreator<ITool>>()
                   .SingleInstance();
            builder.Register(c => new PluginLoader<ITool>(null,
                                     c.Resolve<IAppDomain>(),
                                     c.Resolve<IObjectCreator<ITool>>(),
                                     c.Resolve<IPluginLoaderLogger>()))
                   .As<IPluginLoader<ITool>>()
                   .SingleInstance();
            builder.RegisterType<PluginPaths>()
                   .WithParameter("appName", "sectionName")
                   .WithParameter("PluginSubFolder", null);
            builder.RegisterType<PluginLoader<IDependencyRegistrar<ContainerBuilder>>>()
               .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>()
               .SingleInstance();
            builder.RegisterType<Hammer>();
            builder.RegisterType<AutofacObjectCreator<ICaveManTool<Hammer>>>()
                   .As<IObjectCreator<ICaveManTool<Hammer>>>()
                   .SingleInstance();
            builder.RegisterType<PluginLoader<ICaveManTool<Hammer>>>()
                   .As<IPluginLoader<ICaveManTool<Hammer>>>()
                   .WithParameter("paths", null)
                   .SingleInstance();
            var container = builder.Build();
            using (var globalScope = container.BeginLifetimeScope())
            {
                var pluginLoader = globalScope.Resolve<IPluginLoader<ITool>>();
                var tools = new List<ITool> { new Hammer() };

                var plugins = pluginLoader.LoadPlugins();
                tools.AddRange(plugins.AllObjects);

                var pluginLoaderCaveMan = globalScope.Resolve<IPluginLoader<ICaveManTool<Hammer>>>();
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
