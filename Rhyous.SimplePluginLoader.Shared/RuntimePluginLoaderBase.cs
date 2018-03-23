using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// A base class for loading plugins from a default Plugins directory.
    /// This just makes it easy for a consuming application to mark the plugin directory and get plugins.
    /// </summary>
    /// <typeparam name="T">The type of the plugin to load.</typeparam>
    public abstract class RuntimePluginLoaderBase<T> : IRuntimePluginLoader<T>
        where T : class
    {
        public const string PluginDirConfig = "PluginDirectory";

        public static string AppRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        // After creating a concrete (non-generic) version of this class, you need to update these values.
        // RuntimePluginLoaderBase<MyPlugins>.Company = "ABC, Inc."
        public static string Company = "Rhyous";
        public static string AppName = "App1";
        public static string PluginFolder = "Plugins";

        public virtual bool ThrowExceptionIfNoPluginFound => true;

        public RuntimePluginLoaderBase() { }
        public RuntimePluginLoaderBase(IPluginLoaderLogger logger) { _Logger = logger; }

        /// <inheritdoc />
        public virtual string DefaultPluginDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Company, AppName, PluginFolder);

        /// <inheritdoc />
        public abstract string PluginSubFolder { get; }

        /// <inheritdoc />
        public virtual string PluginGroup => PluginSubFolder;

        /// <summary>
        /// Instantiated instances of the plugins. If multiple dlls have plugins, there could be multiple collections. To get a flattened list, use the Plugins property.
        /// </summary>
        public virtual PluginCollection<T> PluginCollection
        {
            get { return _PluginCollection ?? (_PluginCollection = GetPluginLibraries()); }
            internal set { _PluginCollection = value; } // Set exposed as internal for unit tests
        } private PluginCollection<T> _PluginCollection;

        public virtual ILoadPlugins<T> PluginLoader
        {
            get { return _PluginLoader ?? new PluginLoader<T>(DefaultPluginDirectory, Logger); }
            internal set { _PluginLoader = value; } // Set exposed as internal for unit tests
        } private ILoadPlugins<T> _PluginLoader;

        /// <summary>
        /// Gets only the types of a plugin, not an actual instance.
        /// </summary>
        public virtual List<Type> PluginTypes { get { return _PluginTypes ?? (_PluginTypes = PluginCollection?.SelectMany(p => p.PluginTypes).ToList()); } }
        private List<Type> _PluginTypes;

        /// <summary>
        /// Gets instantiated instances of the plugins. This is a flattened list of plugins.
        /// </summary>
        public virtual List<T> Plugins { get { return _Plugins ?? (_Plugins = PluginCollection?.Where(p=>p.PluginObjects != null && p.PluginObjects.Any()).SelectMany(p => p?.PluginObjects)?.ToList()); } }
        private List<T> _Plugins;

        /// <summary>
        /// This populates PluginCollection.
        /// </summary>
        /// <returns></returns>
        protected virtual PluginCollection<T> GetPluginLibraries()
        {
            var plugins = PluginLoader.LoadPlugins();
            if (plugins == null || !plugins.Any())
            {
                if (ThrowExceptionIfNoPluginFound)
                    throw new Exception($"No {PluginSubFolder} plugin found.");
                return null;
            }
            return plugins;
        }
        
        public IPluginLoaderLogger Logger
        {
            get { return _Logger ?? (_Logger = new PluginLoaderLogger()); }
            set { _Logger = value; }
        } private IPluginLoaderLogger _Logger;
    }
}