using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
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
        /// <summary>
        /// If you set this appSettings in the app.config or the web.config, the DefaultPluginDirectory will
        /// be the configured path.
        /// </summary>
        public const string PluginDirConfig = "PluginDirectory";

        public static string AppRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        // After creating a concrete (non-generic) version of this class, you need to update these values.
        // RuntimePluginLoaderBase<MyPlugins>.Company = "ABC, Inc."
        public static string Company = "Rhyous";
        public static string AppName = "App1";
        public static string PluginFolder = "Plugins";

        public virtual bool ThrowExceptionIfNoPluginFound => true;

        private readonly IAppDomain _AppDomain;
        private readonly IObjectCreator<T> _ObjectCreator;
        private readonly IPluginLoaderLogger _Logger;

        public RuntimePluginLoaderBase(IAppDomain appDomain, IObjectCreator<T> objectCreator, IPluginLoaderLogger logger)
        {
            _AppDomain = appDomain;
            _ObjectCreator = objectCreator;
            _Logger = logger;
        }

        /// <inheritdoc />
        public virtual string DefaultPluginDirectory
        {
            get
            {
                var _DefaultPluginDirectory = AppSettings.Get(PluginDirConfig);
                if (_DefaultPluginDirectory == null)
                    _DefaultPluginDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Company, AppName, PluginFolder);
                if (!string.IsNullOrWhiteSpace(PluginSubFolder))
                    _DefaultPluginDirectory = Path.Combine(_DefaultPluginDirectory, PluginSubFolder);
                return _DefaultPluginDirectory;
            }
        } internal string _DefaultPluginDirectory;

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

        public virtual IPluginLoader<T> PluginLoader
        {
            get { return _PluginLoader ?? new PluginLoader<T>(new PluginPaths(AppName, _AppDomain, DefaultPluginDirectory, _Logger), _AppDomain, _ObjectCreator, _Logger); }
            set { _PluginLoader = value; }
        } private IPluginLoader<T> _PluginLoader;

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

        /// <summary>
        /// Allows for replacing this during unit tests
        /// </summary>
        internal NameValueCollection AppSettings
        {
            get { return _AppSettings ?? (_AppSettings = ConfigurationManager.AppSettings); }
            set { _AppSettings = value; }
        } private NameValueCollection _AppSettings;
    }
}