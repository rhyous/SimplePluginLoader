// See License at the end of the file

using Rhyous.SimplePluginLoader.Attributes;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class PluginFinder<T> : IDisposable, IPluginFinder<T>
    {
        private const string DllExtension = "*.dll";
        private readonly IPluginLoader<T> _PluginLoader;
        private readonly IPluginObjectCreator<T> _PluginObjectCreator;
        private readonly IPluginLoaderLogger _Logger;

        public PluginFinder(IPluginLoader<T> pluginLoader,
                            IPluginObjectCreator<T> pluginObjectCreator,
                            IPluginLoaderLogger logger)
        {
            _PluginLoader = pluginLoader ?? throw new ArgumentNullException(nameof(pluginLoader));
            _PluginObjectCreator = pluginObjectCreator ?? throw new ArgumentNullException(nameof(pluginObjectCreator));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Find a plugin by name. The plugin must implement a Name property.
        ///     public string Name { get; set; }
        /// The Dependency resolver is only loaded for the found plugin.
        /// </summary>
        /// <param name="pluginName">The plugin name</param>
        /// <param name="dir">The directory to search</param>
        /// <returns>A found plugin of type T.</returns>
        public T FindPlugin(string pluginName, string dir, IPluginObjectCreator<T> pluginObjectCreator = null)
        {
            if (string.IsNullOrWhiteSpace(pluginName))
                return default(T);
            if (string.IsNullOrWhiteSpace(dir))
                return default(T);
            _Logger?.WriteLine(PluginLoaderLogLevel.Info, $"Attempting to find plugin: {pluginName}; from path: {dir}");
            var plugins = _PluginLoader.LoadPlugins(Directory.GetFiles(dir, DllExtension));
            if (plugins == null || !plugins.Any())
                return default(T);
            pluginObjectCreator = pluginObjectCreator ?? _PluginObjectCreator;
            foreach (var plugin in plugins)
            {
                // Find by attribute (New preferred method)
                foreach (var type in plugin.PluginTypes)
                {
                    var pluginAttribute = type.GetCustomAttribute<PluginAttribute>();
                    if (pluginAttribute != null && pluginName.Equals(pluginAttribute.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return plugin.CreatePluginObject(type, pluginObjectCreator);
                    }
                }

                // Find by a Named property on instance of class (legacy way)
                var pluginObjects = plugin.CreatePluginObjects(pluginObjectCreator);
                if (pluginObjects == null)
                    continue;
                foreach (var obj in pluginObjects)
                {
                    dynamic namedObj = obj;
                    if (namedObj != null)
                    {
                        if (namedObj.Name.Equals(pluginName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return obj; 
                        }
                    }
                }
            }
            return default(T);
        }
        
        #region IDisposable
        bool _Disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_Disposed)
                return;

            if (disposing)
            {
            }
            _Disposed = true;
        }
        #endregion

        internal IDirectory Directory
        {
            get { return _Directory ?? (_Directory = new DirectoryWrapper()); }
            set { _Directory = value; }
        } private IDirectory _Directory;

    }
}


#region License
/*
Simple Plugin Loader - A library that makes loading plugins quick and easy. It
                       creates instances of interfaces or base classes from
                       plugins with a few lines of code.

Copyright (c) 2012, Jared Barneck (Rhyous)
All rights reserved.
 
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
 
1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
3. Use of the source code or binaries that in any way provides a competing
   solution, whether open source or commercial, is prohibited unless permission
   is specifically granted under a separate license by Jared Barneck (Rhyous).
4. Forking for personal or internal, or non-competing commercial use is 
   allowed. Distributing compiled releases as part of your non-competing 
   project is allowed.
5. Public copies, or forks, of source is allowed, but from such, public
   distribution of compiled releases is forbidden.
6. Source code enhancements or additions are the property of the author until
   the source code is contributed to this project. By contributing the source
   code to this project, the author immediately grants all rights to the
   contributed source code to Jared Barneck (Rhyous).
 
THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion