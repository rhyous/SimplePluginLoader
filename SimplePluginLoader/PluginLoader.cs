using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SimplePluginLoader
{
    /// <summary>
    /// A singleton that load plugins
    /// </summary>
    public class PluginLoader<T> : ILoadPlugins<T> where T : class
    {
        private const string DefaultPluginDirectory = "Plugins";
        private const string DefaultDllSearchString = "*.dll";

        public string DefaultAppName
        {
            get { return _DefaultAppName ?? (_DefaultAppName = Path.GetFileName(Assembly.GetEntryAssembly().Location)); }
        } private string _DefaultAppName;

        #region Methods

        /// <summary>
        /// Support multiple plugin directories, one relative to running path, 
        /// one in the uses profile, and one in ApplicationData.  
        /// </summary>
        public PluginCollection<T> LoadPlugins()
        {
            var dirs = GetDefaultPluginDirectories(DefaultAppName, DefaultPluginDirectory);
            return LoadPlugins(dirs);
        }

        /// <summary>
        /// Returns a "Plugins" directory relative to the executable, a directory in the user profile
        /// and a directory in ApplicationData.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="pluginDirectory"></param>
        /// <returns>A list of default directories</returns>
        public static IEnumerable<string> GetDefaultPluginDirectories(string appName, string pluginDirectory)
        {
            string[] dirs = { pluginDirectory,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), appName, pluginDirectory),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName, pluginDirectory)
            };

            var dirList = new List<string> { pluginDirectory };
            foreach (var dir in dirs.Where(dir => !dirList.Contains(dir)))
            {
                dirList.Add(dir);
            }
            return dirList;
        }

        /// <summary>
        /// Loads any plugins found in the specified directories
        /// </summary>
        /// <param name="dirs"></param>
        public PluginCollection<T> LoadPlugins(IEnumerable<string> dirs)
        {
            var plugins = new PluginCollection<T>();
            foreach (var dir in dirs.Where(Directory.Exists))
            {
                plugins.AddRange(LoadPlugins(Directory.GetFiles(dir, DefaultDllSearchString)));
            }
            return plugins;
        }

        /// <summary>
        /// Loads the plugins from files specificied. 
        /// </summary>
        public PluginCollection<T> LoadPlugins(string[] pluginFiles)
        {
            return new PluginCollection<T>(pluginFiles.Select(LoadPlugin).Where(plugin => plugin != null));
        }

        /// <summary>
        /// Loads the single plugin specificied. 
        /// </summary>
        public Plugin<T> LoadPlugin(string pluginFile)
        {
            if (!File.Exists(pluginFile))
                return null;
            return new Plugin<T>
            {
                Directory = Path.GetDirectoryName(pluginFile),
                File = Path.GetFileName(pluginFile),
                Assembly = Assembly.LoadFrom(pluginFile)
            };
        }
        #endregion
    }
}

#region Fork and Contribute License
/*
SimplePluginLoader - Easily support plugins in to your project.

Copyright (c) 2012, Jared Abram Barneck (Rhyous)
All rights reserved.
 
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
 
1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.
3. Use of the source code or binaries for a competing project, whether open
   source or commercial, is prohibited unless permission is specifically
   granted under a separate license by Jared Abram Barneck (Rhyous).
4. Forking for personal or internal, or non-competing commercial use is 
   allowed. Distributing compiled releases as part of your non-competing 
   project is allowed.
5. Public copies, or forks, of source is allowed, but from such, public
   distribution of compiled releases is forbidden.
6. Source code enhancements or additions are the property of the author until
   the source code is contributed to this project. By contributing the source
   code to this project, the author immediately grants all rights to the
   contributed source code to Jared Abram Barneck (Rhyous).
 
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