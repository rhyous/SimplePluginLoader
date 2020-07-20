using System.Collections.Generic;
using System;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// The plugin loader interface.
    /// </summary>
    public interface IRuntimePluginLoader
    {
        /// <summary>
        /// The subfolder underneath the any plugin directories to search.
        /// </summary>
        /// <example>If Plugins directory was: 'c:\plugins' and PluginSubFolder was 'Tools', then the path should become: 'c:\plugins\tools'</example>
        string PluginSubFolder { get; }
        /// <summary>
        /// A group name for the plugins in the subfolder. This is usually the same name as the PluginSubFolder.
        /// </summary>
        string PluginGroup { get; }
        /// <summary>
        /// A list of loaded plugin types.
        /// </summary>
        List<Type> PluginTypes { get; }
    }

    /// <summary>
    /// The generic RuntimePluginLoader interface.
    /// This is the interface to use to implement a RuntimePluginLoader.
    /// </summary>
    /// <typeparam name="T">The type of plugin to load.</typeparam>
    public interface IRuntimePluginLoader<T> : IRuntimePluginLoader
        where T : class
    {
        /// <summary>
        /// Instantiated instances of the plugins. If multiple dlls have plugins, there could be multiple collections.
        /// </summary>
        PluginCollection<T> PluginCollection { get; }
        /// <summary>
        /// The actual plugin Loader.
        /// </summary>
        IPluginLoader<T> PluginLoader { get; }

        IList<T> CreatePluginObjects(IPluginObjectCreator<T> pluginObjectCreator = null);
    }
}