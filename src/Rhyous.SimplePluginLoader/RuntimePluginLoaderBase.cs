using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// A base class for loading plugins from a default Plugins directory.
    /// This just makes it easy for a consuming application to mark the plugin directory and get plugins.
    /// It allows for finding plugins that are global or per user.
    /// </summary>
    /// <typeparam name="T">The type of the plugin to load.</typeparam>
    public abstract class RuntimePluginLoaderBase<T> : IRuntimePluginLoader<T>
        where T : class
    {
        private readonly IAppDomain _AppDomain;
        private readonly IPluginLoaderSettings _Settings;
        private readonly IPluginLoaderFactory<T> _PluginLoaderFactory;
        private readonly IPluginObjectCreator<T> _PluginObjectCreator;
        private readonly IPluginPaths _PluginPaths;
        protected readonly IPluginLoaderLogger _Logger;
        private readonly bool _DisconnectResolverOnPluginLoad;
        private readonly bool _DisconnectResolverOnPluginTypeLoad;

        public RuntimePluginLoaderBase(IAppDomain appDomain,
                                       IPluginLoaderSettings settings,
                                       IPluginLoaderFactory<T> pluginLoaderFactory,
                                       IPluginObjectCreator<T> pluginObjectCreator,
                                       IPluginPaths pluginPaths = null,
                                       IPluginLoaderLogger logger = null,
                                       bool disconnectResolverOnPluginLoad = false,
                                       bool disconnectResolverOnPluginTypeLoad = false)
        {
            _AppDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _PluginLoaderFactory = pluginLoaderFactory ?? throw new ArgumentNullException(nameof(pluginLoaderFactory));
            _PluginObjectCreator = pluginObjectCreator ?? throw new ArgumentNullException(nameof(pluginObjectCreator));
            pluginPaths = pluginPaths ?? new AppPluginPaths(_Settings.AppName, GetDefaultPluginDirectory(), _AppDomain, _Logger);
            _PluginPaths = string.IsNullOrWhiteSpace(PluginSubFolder)
                         ? pluginPaths
                         : new PluginPaths { Paths = pluginPaths.Paths.Select(p => Path.Combine(p, PluginSubFolder)) };
            _Logger = logger;
            _DisconnectResolverOnPluginLoad = disconnectResolverOnPluginLoad;
            _DisconnectResolverOnPluginTypeLoad = disconnectResolverOnPluginTypeLoad;
        }

        /// <inheritdoc />
        private string GetDefaultPluginDirectory()
        {
            var _DefaultPluginDirectory = _Settings.DefaultPluginDirectory;
            if (_DefaultPluginDirectory == null)
                _DefaultPluginDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                                       _Settings.Company, _Settings.AppName, _Settings.PluginFolder);
            if (!string.IsNullOrWhiteSpace(PluginSubFolder))
                _DefaultPluginDirectory = Path.Combine(_DefaultPluginDirectory, PluginSubFolder);
            return _DefaultPluginDirectory;
        }

        /// <inheritdoc />       
        public abstract string PluginSubFolder { get; }

        /// <inheritdoc />
        public virtual string PluginGroup => PluginSubFolder;

        /// <summary>
        /// Instantiated instances of the plugins. If multiple dlls have plugins, there could be
        /// multiple collections.
        /// </summary>
        public virtual PluginCollection<T> PluginCollection
        {
            get { return _PluginCollection ?? (_PluginCollection = GetPluginLibraries()); }
            internal set { _PluginCollection = value; } // Set exposed as internal for unit tests
        } private PluginCollection<T> _PluginCollection;

        public virtual IPluginLoader<T> PluginLoader
        {
            get { return _PluginLoader ?? _PluginLoaderFactory.Create(_PluginPaths); }
            internal set { _PluginLoader = value; }
        } private IPluginLoader<T> _PluginLoader;

        /// <summary>
        /// Gets only the types of a plugin, not an actual instance.
        /// </summary>
        public virtual List<Type> PluginTypes { get { return _PluginTypes ?? (_PluginTypes = GetPluginTypes()); } }
        private List<Type> _PluginTypes;

        private List<Type> GetPluginTypes() 
        {
            var types = PluginCollection?.SelectMany(p => p.PluginTypes).ToList();
            if (_DisconnectResolverOnPluginTypeLoad)
            {
                foreach (var plugin in PluginCollection)
                {
                    plugin.DependencyResolver.RemoveDependencyResolver();
                }
            }
            return types;
        }

        /// <summary>
        /// This populates PluginCollection.
        /// </summary>
        /// <returns></returns>
        protected virtual PluginCollection<T> GetPluginLibraries()
        {
            var plugins = PluginLoader.LoadPlugins();
            if (plugins == null || !plugins.Any())
            {
                var msg = $"No {PluginGroup} plugins were found in these directories:{Environment.NewLine}{string.Join(Environment.NewLine, _PluginPaths.Paths)}";
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, msg);
                if (_Settings.ThrowExceptionIfNoPluginFound)
                {
                    throw new RuntimePluginLoaderException(msg);
                }
                return plugins;
            }
            return plugins;
        }

        public virtual IList<T> CreatePluginObjects(IPluginObjectCreator<T> pluginObjectCreator = null)
        {
            var pluginObjects = PluginCollection?.CreatePluginObjects(pluginObjectCreator ?? _PluginObjectCreator);
            if (_DisconnectResolverOnPluginLoad)
            {
                foreach (var plugin in PluginCollection)
                {
                    plugin.DependencyResolver.RemoveDependencyResolver();
                }
            }
            return pluginObjects;
        }
    }
}