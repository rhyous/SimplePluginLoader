﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class AppPluginPaths : IPluginPaths
    {
        public const string DefaultPluginDirectory = "Plugins";
        public const string DefaultDllSearchString = "*.dll";

        private readonly IAppDomain _AppDomain;
        private readonly string _PluginSubFolder;
        private readonly IPluginLoaderLogger _Logger;

        public AppPluginPaths(string appName,
                           string pluginSubFolder,
                           IAppDomain appDomain,
                           IPluginLoaderLogger logger)
        {
            AppName = !string.IsNullOrWhiteSpace(appName) ? appName : throw new ArgumentException("message", nameof(appName));
            _PluginSubFolder = pluginSubFolder; // Optional
            _AppDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            _Logger = logger; // Optional
        }

        public string AppName { get; set; }

        public IEnumerable<string> Paths => GetDefaultPluginDirectories();

        /// <summary>
        /// The path to the directory where plugins are stored. The default is:
        /// Plugins
        /// </summary>
        public string PluginDirectoryName
        {
            get { return GetPluginDirectory(); }
            set { _PluginDirectoryName = value; }
        } private string _PluginDirectoryName;

        private string GetPluginDirectory()
        {
            return (string.IsNullOrWhiteSpace(_PluginDirectoryName))
                    ? (_PluginDirectoryName = string.IsNullOrWhiteSpace(_PluginSubFolder) || DefaultPluginDirectory.EndsWith(_PluginSubFolder, StringComparison.OrdinalIgnoreCase)
                                         ? DefaultPluginDirectory
                                         : Path.Combine(DefaultPluginDirectory, _PluginSubFolder))
                    : _PluginDirectoryName;
        }        

        public string UserProfilePlugins
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), AppName, PluginDirectoryName); }
        }

        public string ApplicationDataPlugins
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, PluginDirectoryName); }
        }

        public string RelativePathPlugins
        {
            get { return Path.Combine(_AppDomain.BaseDirectory, PluginDirectoryName); }
        }

        public IEnumerable<string> GetDefaultPluginDirectories()
        {
            var distinctPaths = new[] { PluginDirectoryName, UserProfilePlugins, ApplicationDataPlugins, RelativePathPlugins }.Distinct();
            _Logger?.WriteLine(PluginLoaderLogLevel.Info, "Paths: " + string.Join(Environment.NewLine, distinctPaths));
            return distinctPaths;
        }
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