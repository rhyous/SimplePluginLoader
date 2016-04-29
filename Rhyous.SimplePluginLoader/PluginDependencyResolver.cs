using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    [Serializable]
    public class PluginDependencyResolver<T> : IPluginDependencyResolver
        where T : class
    {
        public PluginDependencyResolver(Plugin<T> plugin)
        {
            Plugin = plugin;
        }

        public Plugin<T> Plugin { get; set; }

        public Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            var paths = new List<string>();
            paths.Add("");                                             // Try current path
            paths.Add(Plugin.Directory);                               // Try plugin directory
            paths.Add(Path.Combine(Plugin.Directory, "bin"));          // Try plugin/bin directory
            paths.Add(Path.Combine(Plugin.Directory, Plugin.Name));    // Try plugin/<pluginName> directory

            var file = args.Name.Split(',').First();
            foreach (var path in paths)
            {
                var dll = Path.Combine(path, file + ".dll");
                var pdb = Path.Combine(path, file + ".pdb");
                var assembly = Plugin.AssemblyBuilder.TryLoad(dll, pdb);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            return null;
        }
    }
}
