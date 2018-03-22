// See License at the end of the file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// A singleton that load plugins
    /// </summary>
    public class PluginLoader<T> : ILoadPlugins<T>
        where T : class
    {
        public List<Plugin<T>> Plugins
        {
            get { return _Plugins.Value; }
        } private Lazy<List<Plugin<T>>> _Plugins = new Lazy<List<Plugin<T>>>(true);

        public string DefaultAppName
        {
            get { return _DefaultAppName ?? (_DefaultAppName = Path.GetFileName(Domain.BaseDirectory)); }
        } private string _DefaultAppName;

        public PluginPaths Paths
        {
            get { return _Paths ?? (_Paths = new PluginPaths(DefaultAppName, Logger)); }
        } private PluginPaths _Paths;

        public AppDomain Domain
        {
            get { return _Domain ?? (_Domain = AppDomain.CurrentDomain); }
        } private AppDomain _Domain;

        public IPluginLoaderLogger Logger
        {
            get { return _Logger ?? (_Logger = new PluginLoaderLogger()); }
            set { _Logger = value; }
        } private IPluginLoaderLogger _Logger;

        #region Constructors

        public PluginLoader() { }

        public PluginLoader(string pluginDirectory) { Paths.PluginDirectoryName = pluginDirectory; }
        
        public PluginLoader(IPluginLoaderLogger logger) { Logger = logger; }

        public PluginLoader(string pluginDirectory, IPluginLoaderLogger logger)
        {
            Paths.PluginDirectoryName = pluginDirectory;
            Logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Support multiple plugin directories, one relative to running path, 
        /// one in the uses profile, and one in ApplicationData.  
        /// </summary>
        public PluginCollection<T> LoadPlugins()
        {
            var dirs = Paths.GetDefaultPluginDirectories();
            return LoadPlugins(dirs);
        }

        /// <summary>
        /// Loads any plugins found in the specified directories
        /// </summary>
        /// <param name="dirs"></param>
        public PluginCollection<T> LoadPlugins(IEnumerable<string> dirs)
        {
            var plugins = new PluginCollection<T>();
            var fullPaths = new List<string>();
            foreach (var dir in dirs.Where(Directory.Exists))
            {
                var info = new DirectoryInfo(dir);
                if (fullPaths.Contains(info.FullName))
                    continue;
                fullPaths.Add(info.FullName);
                var allfiles = Directory.GetFiles(info.FullName, PluginPaths.DefaultDllSearchString, SearchOption.AllDirectories);
                var foundplugins = LoadPlugins(allfiles.Where(s => !Path.GetDirectoryName(s).EndsWith(@"\bin")).ToArray());
                plugins.AddRange(foundplugins);
            }
            return plugins;
        }

        /// <summary>
        /// Loads the plugins from files specificied. 
        /// </summary>
        public PluginCollection<T> LoadPlugins(string[] pluginFiles)
        {
            var plugins = new PluginCollection<T>();
            foreach (var pluginFile in pluginFiles)
            {
                var plugin = LoadPlugin(pluginFile);
                if (plugin == null)
                    continue;
                plugin.PluginObjects = plugin.Loader.LoadInstances(plugin.Assembly);
                if (plugin.PluginObjects?.Count == 0)
                    continue;
                plugins.Add(plugin);
            }
            return plugins;
        }

        /// <summary>
        /// Loads the single plugin specificied. 
        /// </summary>
        public Plugin<T> LoadPlugin(string pluginFile)
        {
            if (!File.Exists(pluginFile))
                return null;
            var plugin = new Plugin<T>(Logger)
            {
                Directory = Path.GetDirectoryName(pluginFile),
                File = Path.GetFileName(pluginFile)
            };
            Plugins.Add(plugin);
            return plugin;
        }
        #endregion
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