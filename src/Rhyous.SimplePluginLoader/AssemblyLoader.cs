// See License at the end of the file

using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyLoader : IAssemblyLoader
    {
        private readonly IPluginLoaderLogger _Logger;
        private readonly IAppDomain _AppDomain;
        private readonly IPluginLoaderSettings _Settings;
        private readonly IAssemblyCache _AssemblyCache;
        private readonly IAssemblyNameReader _AssemblyNameReader;

        public AssemblyLoader(IAppDomain appDomain,
                              IPluginLoaderSettings settings, 
                              IAssemblyCache assemblyCache,
                              IAssemblyNameReader assemblyNameReader,
                              IPluginLoaderLogger logger)
        {
            _AppDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _AssemblyCache = assemblyCache ?? throw new ArgumentNullException(nameof(assemblyCache));
            _AssemblyNameReader = assemblyNameReader;
            _Logger = logger;
        }

        public virtual IAssembly Load(string dll, string pdb)
        {
            if (Path.IsPathRooted(dll))
            {
                return TryLoad(dll, pdb);
            }
            var defaultDirs = new PluginPaths(_AppDomain.BaseDirectory, null, _AppDomain, _Logger).GetDefaultPluginDirectories();
            foreach (var path in defaultDirs)
            {
                var dllPath = Path.Combine(path, dll);
                var assembly = TryLoad(dllPath, pdb);
                if (assembly != null)
                {
                    return assembly;
                }
            }
            return null;
        }

        public IAssembly TryLoad(string dll, string pdb)
        {
            var assemblyName = _AssemblyNameReader.GetAssemblyName(dll);
            if (assemblyName == null)
                return null;
            return TryLoad(dll, pdb, assemblyName.Version.ToString());
        }

        public IAssembly TryLoad(string dll, string pdb, string version)
        {
            IAssembly assembly = FindAlreadyLoadedAssembly(dll, version);
            if (assembly != null)
                return assembly;
            assembly = _AppDomain.TryLoad(dll, pdb, _Logger);
            if (assembly == null)
                return null;
            var assemblyVersion = assembly.GetName().Version.ToString();
            if (assemblyVersion != version)
                return null;
            try
            {
                _AssemblyCache.Assemblies.Add(GetKey(dll, assemblyVersion), assembly);
            }
            catch (Exception e)
            {
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                throw;
            }
            if (_Settings.LoadDependenciesProactively)
            {
                var dir = Path.GetDirectoryName(dll);
                if (!dir.EndsWith("bin"))
                    ProactivelyLoadDependencies(Path.Combine(dir, "bin"));
            }
            return assembly;
        }

        internal IAssembly FindAlreadyLoadedAssembly(string dll, string version)
        {
            var key = GetKey(dll, version);
            if (_AssemblyCache.Assemblies.TryGetValue(key, out IAssembly assembly))
            {
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Found already loaded plugin assembly: {key}");
            }
            else
            {
                var assemblyName = _AssemblyNameReader.GetAssemblyName(dll);
                if (assemblyName == null)
                    return null;
                key = GetKey(dll, assemblyName.Version.ToString());
                assembly = _AssemblyCache.Assemblies.TryGetValue(key, out assembly) ? assembly : null;
                if (assembly != null && assembly.GetName().Version.ToString() == version)
                {
                    _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Found already loaded plugin assembly: {key}");
                    return assembly;
                }
                assembly = _AppDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == assemblyName.FullName && a.GetName().Version.ToString() == version);
                if (assembly != null)
                {
                    key = GetKey(dll, assembly.GetName().Version.ToString());
                    _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Loading plugin assembly from dll: {key}");
                    _AssemblyCache.Assemblies.Add(key, assembly);
                }
            }
            return assembly;
        }

        internal string GetKey(string dll)
        {
            return string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(dll), Directory.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        internal string GetKey(string dll, string version)
        {
            return string.Format("{0}_{1}_{2}", Path.GetFileNameWithoutExtension(dll), version, Directory.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        internal void ProactivelyLoadDependencies(string binPath)
        {
            if (!Directory.Exists(binPath))
                return;
            foreach (var file in Directory.GetFiles(binPath, "*.dll"))
            {
                var dll = file;
                var assemblyName = _AssemblyNameReader.GetAssemblyName(dll);
                if (assemblyName == null)
                    continue;
                var pdb = file.Substring(0, file.Length - 3) + ".pdb";
                var version = assemblyName.Version.ToString();
                if (string.IsNullOrWhiteSpace(version))
                    TryLoad(dll, pdb);
                else
                    TryLoad(dll, pdb, version);
            }
        }

        internal IDirectory Directory
        {
            get { return _Directory ?? (_Directory = DirectoryWrapper.Instance); }
            set { _Directory = value; }
        } private IDirectory _Directory;

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
                //AppDomain.Unload(Domain); // When we get it working in a separate AppDomain
            }
            _disposed = true;
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