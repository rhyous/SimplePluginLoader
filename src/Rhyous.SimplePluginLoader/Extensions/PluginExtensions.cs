using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public static class PluginExtensions
    {
        public static List<string> GetPaths(this IPlugin plugin, IPluginLoaderSettings settings)
        {
            if (plugin == null || string.IsNullOrWhiteSpace(plugin.FullPath))
                return null;
            var paths = new List<string>
                {
                    "",                                                // Try current path
                    plugin.Directory,                                  // Try plugin directory
                    Path.Combine(plugin.Directory, "bin"),             // Try plugin\bin directory
                    Path.Combine(plugin.Directory, plugin.Name),       // Try plugin\<pluginName> directory
                    Path.Combine(plugin.Directory, plugin.Name, "bin") // Try plugin\<pluginName>\bin directory
                };

            // This allows for two plugins that share a dll to have Copy Local set to false, and both look to the same folder
            if (settings?.SharedPaths?.Any() != null)
                paths.AddRange(settings.SharedPaths);

            return paths;
        }
    }
}
