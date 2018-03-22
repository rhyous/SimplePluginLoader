using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var logger = new PluginLoaderLogger();
            var pluginLoader = new PluginLoader<ITool>(logger);
            var plugins = pluginLoader.LoadPlugins();
            tools.AddRange(plugins.AllObjects);

            var pluginLoaderCaveMan = new PluginLoader<ICaveManTool<Hammer>>(logger);
            var caveManPlugins = pluginLoaderCaveMan.LoadPlugins();
            tools.AddRange(caveManPlugins.AllObjects.Select(o => o.Tool));
            
            ShowPrompt(tools);
            int input = ReadLine(tools);
            while (input != 0)
            {
                Console.WriteLine(tools[input - 1].DoWork());
                ShowPrompt(tools);
                input = ReadLine(tools);
            }
        }

        private static void ShowPrompt(List<ITool> tools)
        {
            Console.WriteLine("Which tool do you want to use:");
            Console.WriteLine("0. Exit");
            for (int i = 0; i < tools.Count; )
            {
                var tool = tools[i];
                Console.WriteLine("{0}. {1}", ++i, tool.Name);
            }
            Console.Write("Choose an option> ");
        }

        private static int ReadLine(List<ITool> tools)
        {
            int input;
            while (!int.TryParse(Console.ReadLine(), out input) || input > tools.Count)
            {
                Console.WriteLine("Invalid entry!");
                ShowPrompt(tools);
                ReadLine(tools);
            }
            return input;
        }
    }
}
