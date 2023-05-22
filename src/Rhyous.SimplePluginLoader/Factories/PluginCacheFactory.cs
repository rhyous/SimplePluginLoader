using System;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    public class PluginCacheFactory<T> : CacheFactory<string, IPlugin<T>>, IPluginCacheFactory<T>
    {
        private readonly ITypeLoader<T> _TypeLoader;
        private readonly IPluginDependencyResolverCacheFactory _DependencyResolverCacheFactory;
        private readonly IAssemblyLoader _AssemblyLoader;

        public PluginCacheFactory(ITypeLoader<T> typeLoader,
                                  IPluginDependencyResolverCacheFactory dependencyResolverCacheFactory,
                                  IAssemblyLoader assemblyLoader,
                                  IPluginLoaderLogger logger)
                    : base(null, logger)
        {
            _TypeLoader = typeLoader;
            _DependencyResolverCacheFactory = dependencyResolverCacheFactory;
            _AssemblyLoader = assemblyLoader;
        }

        public override IPlugin<T> Create(string pluginFile, Type t)
        {
            if (_Cache.TryGetValue(pluginFile, out IPlugin<T> plugin))
                return plugin;
            CreatePlugin(pluginFile);
            _Cache.TryGetValue(pluginFile, out plugin);
            return plugin;
        }

        /// <summary>
        /// The method intentionally has no return value. If two threds call CreatePlugin
        /// at the same time, then only one will be added by TryAdd. So instead, of returning
        /// returning the created Plugin, it simply calls TryAdd.
        /// </summary>
        /// <param name="pluginFile"></param>
        private void CreatePlugin(string pluginFile)
        {
            Plugin<T> plugin;
            var pluginDir = Path.GetDirectoryName(pluginFile);
            var binDir = $"{Path.DirectorySeparatorChar}bin";
            if (pluginDir.EndsWith(binDir, StringComparison.OrdinalIgnoreCase))
                pluginDir = pluginDir.Substring(0, pluginDir.Length - binDir.Length);
            var resolver = _DependencyResolverCacheFactory.Create(pluginDir, typeof(PluginDependencyResolver));
            plugin = new Plugin<T>(_TypeLoader, resolver, _AssemblyLoader)
            {
                Directory = Path.GetDirectoryName(pluginFile),
                File = Path.GetFileName(pluginFile)
            };
            _Cache.TryAdd(pluginFile, plugin);
        }
    }
}