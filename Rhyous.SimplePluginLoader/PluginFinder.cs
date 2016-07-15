using System;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    public class PluginFinder<T> : IDisposable, IPluginFinder<T>
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
            FoundPlugin = null;
            FoundPluginObject = null;
            var plugins = PluginLoader.LoadPlugins(Directory.GetFiles(dir, DllExtension));
            foreach (var plugin in plugins)
            {
                foreach (var obj in plugin.PluginObjects)
                {
                    var namedObj = obj as IName;
                    if (namedObj != null)
                    {
                        if (namedObj.Name.Equals(pluginName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            FoundPlugin = plugin;
                            return (FoundPluginObject = obj); 
                        }
                    }
                }
            }
            return null;
        }

        public Plugin<T> FoundPlugin { get; set; }

        public T FoundPluginObject { get; set; }

        public ILoadPlugins<T> PluginLoader
        {
            get { return _PluginLoader ?? (_PluginLoader = new PluginLoader<T>()); }
            set { _PluginLoader = value; } // Allows for use of a custom plugin
        } private ILoadPlugins<T> _PluginLoader;


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
