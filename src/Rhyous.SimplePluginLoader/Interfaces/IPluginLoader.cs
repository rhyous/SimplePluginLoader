using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IPluginLoader<T> where T : class
    {
        /// <summary>
        /// Load plugins.  
        /// </summary> 
        /// <returns>A collection of loaded plugins.</returns>
        PluginCollection<T> LoadPlugins();

        /// <summary>
        /// Load plugins from the list of files.
        /// </summary>
        /// <param name="pluginFiles">A list of files.</param>
        /// <returns>A collection of loaded plugins.</returns>
        PluginCollection<T> LoadPlugins(IEnumerable<string> dirs);

        /// <summary>
        /// Load plugins from the list of files.
        /// </summary>
        /// <param name="pluginFiles">A list of files.</param>
        /// <returns>A collection of loaded plugins.</returns>
        PluginCollection<T> LoadPlugins(string[] pluginFiles);

        /// <summary>
        /// Load a single plugin from a specified file.
        /// </summary>
        /// <param name="pluginFile">The plugin file.</param>
        /// <returns>A single Plugin.</returns>
        IPlugin<T> LoadPlugin(string pluginFile);
    }
}