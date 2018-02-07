using System.Collections.Generic;
using System;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// The plugin loader interface.
    /// </summary>
    /// <typeparam name="T">The type of plugin to load.</typeparam>
    public interface IRuntimePluginLoader<T>
        where T : class
    {
        /// <summary>
        /// The default parent directory for all plugins.
        /// </summary>
        string DefaultPluginDirectory { get; }
        /// <summary>
        /// The subfolder underneath the Plugins directory to search.
        /// </summary>
        string PluginSubFolder { get; }
        /// <summary>
        /// A group name for the plugins in the subfolder. This is usually the same name as the PluginSubFolder.
        /// </summary>
        string PluginGroup { get; }
        /// <summary>
        /// Instantiated instances of the plugins. If multiple dlls have plugins, there could be multiple collections. To get a flattened list, use the Plugins property.
        /// </summary>
        PluginCollection<T> PluginCollection { get; }
        /// <summary>
        /// The actual plugin Loader.
        /// </summary>
        ILoadPlugins<T> PluginLoader { get; }
        /// <summary>
        /// The plugin loader.
        /// </summary>
        List<Type> PluginTypes { get; }
        /// <summary>
        /// A list of loaded plugins.
        /// </summary>
        List<T> Plugins { get; }
    }
}