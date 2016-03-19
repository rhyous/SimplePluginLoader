using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class PluginFinder<T> where T : class
    {
        private const string DllExtension = "*.dll";

        /// <summary>
        /// Find a plugin by name. The plugin must implement IName.
        /// </summary>
        /// <param name="pluginName">The plugin name</param>
        /// <param name="dir">The directory to search</param>
        /// <returns></returns>
        public static T FindPlugin(string pluginName, string dir)
        {
            var pluginLoader = new PluginLoader<T>();
            var plugins = pluginLoader.LoadPlugins(Directory.GetFiles(dir, DllExtension));
            return (from pluginList in plugins
                    from plugin in pluginList.PluginObjects
                    let namedPlugin = plugin as IName
                    where namedPlugin != null && namedPlugin.Name == pluginName
                    select plugin).FirstOrDefault();
        }
    }
}
