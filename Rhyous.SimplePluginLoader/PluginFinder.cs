using System;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class PluginFinder<T> : IDisposable
        where T : class
    {
        private const string DllExtension = "*.dll";

        /// <summary>
        /// Find a plugin by name. The plugin must implement IName.
        /// The Dependency resolve is only loaded for the found plugin.
        /// </summary>
        /// <param name="pluginName">The plugin name</param>
        /// <param name="dir">The directory to search</param>
        /// <returns></returns>
        public T FindPlugin(string pluginName, string dir)
        {
            var pluginLoader = new PluginLoader<T>();
            var plugins = pluginLoader.LoadPlugins(Directory.GetFiles(dir, DllExtension));
            FoundPluginObject = null;
            foreach (var plugin in plugins)
            {
                foreach (var obj in plugin.PluginObjects)
                {
                    var namedObj = obj as IName;
                    if (namedObj != null)
                    {
                        if (namedObj.Name.Equals(pluginName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            FoundPluginObject = obj;
                            break;
                        }
                    }
                }
                if (FoundPluginObject != null)
                {
                    FoundPlugin = plugin;
                    plugin.AddDependencyResolver();
                }
            }
            return FoundPluginObject;
        }

        public Plugin<T> FoundPlugin { get; set; }

        public T FoundPluginObject { get; set; }

        #region IDisposable
        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (FoundPlugin != null)
                    FoundPlugin.Dispose();
            }
            _disposed = true;
        }
        #endregion
    }
}
