using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// The inerfaces that helps ILoadPlugins know here to go to load plugins.
    /// </summary>
    /// <typeparam name="T">The type of plugin to load.</typeparam>
    public interface IPluginLoader<T>
        where T : class
    {
        /// <summary>
        /// The collection of loaded plugins.
        /// </summary>
        PluginCollection<T> PluginCollection { get; }
        /// <summary>
        /// The plugin loader.
        /// </summary>
        ILoadPlugins<T> PluginLoader { get; }
        /// <summary>
        /// A list of loaded plugins.
        /// </summary>
        List<T> Plugins { get; }
        /// <summary>
        /// The subfolder underneath the Plugins directory to search.
        /// </summary>
        string PluginSubFolder { get; }
    }
}