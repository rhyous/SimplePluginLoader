// See License at the end of the file

using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyLoader<T> : IAssemblyBuilder
        where T : class
    {
        private IPluginLoaderLogger _Logger;
        private IAppDomain _AppDomain;
        private bool LoadDependenciesProactively = ConfigurationManager.AppSettings["LoadDependenciesProactively"].ToBool(false);

        public AssemblyLoader(IAppDomain appDomain, IPluginLoaderLogger logger)
        {
            _AppDomain = appDomain;
            _Logger = logger;
        }

        public virtual IAssembly Load(string dll, string pdb)
        {
            if (Path.IsPathRooted(dll))
            {
                return TryLoad(dll, pdb);
            }
            var defaultDirs = new PluginPaths(_AppDomain.BaseDirectory, _AppDomain, null, _Logger).GetDefaultPluginDirectories();
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
            var assemblyName = AssemblyNameReader.GetAssemblyName(dll);
            if (assemblyName == null)
                return null;
            return TryLoad(dll, pdb, assemblyName.Version.ToString());
        }

        public IAssembly TryLoad(string dll, string pdb, string version)
        {
            IAssembly assembly = null;
            lock (AssemblyDictionary.IsLocked)
            {
                assembly = FindAlreadyLoadedAssembly(dll, version);
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
                    AssemblyDictionary.Assemblies.Add(GetKey(dll, assemblyVersion), assembly);
                }
                catch (Exception e)
                { 
                    _Logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                    throw;
                }
            }
            if (LoadDependenciesProactively)
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
            if (!AssemblyDictionary.Assemblies.TryGetValue(key, out IAssembly assembly))
            {                var assemblyName = AssemblyNameReader.GetAssemblyName(dll);
                if (assemblyName == null)
                    return null;
                key = GetKey(dll, assemblyName.Version.ToString());
                assembly = AssemblyDictionary.Assemblies.TryGetValue(key, out assembly) ? assembly : null;
                if (assembly != null && assembly.GetName().Version.ToString() == version)
                    return assembly;
                assembly = _AppDomain.GetAssemblies().FirstOrDefault(a => a.GetName().FullName == assemblyName.FullName && a.GetName().Version.ToString() == version);
                if (assembly != null)
                {
                    key = GetKey(dll, assembly.GetName().Version.ToString());
                    AssemblyDictionary.Assemblies.Add(key, assembly);
                }
            }
            return assembly;
        }

        internal static string GetKey(string dll)
        {
            return string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(dll), File.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        internal static string GetKey(string dll, string version)
        {
            return string.Format("{0}_{1}_{2}", Path.GetFileNameWithoutExtension(dll), version, File.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        internal AssemblyDictionary AssemblyDictionary
        {
            get { return _AssemblyDictionary ?? (_AssemblyDictionary = AssemblyDictionary.GetInstance(_AppDomain, _Logger)); }
            set { _AssemblyDictionary = value; }
        } private AssemblyDictionary _AssemblyDictionary;

        internal void ProactivelyLoadDependencies(string binPath)
        {
            if (!Directory.Exists(binPath))
                return;
            foreach (var file in Directory.GetFiles(binPath, "*.dll"))
            {
                var dll = file;
                var assemblyName = AssemblyNameReader.GetAssemblyName(dll);
                if (assemblyName == null)
                    continue;
                var pdb = file.Substring(0, file.Length - 3) + ".pdb";
                var version = assemblyName.Version.ToString();
                var assembly = (string.IsNullOrWhiteSpace(version))
                    ? TryLoad(dll, pdb)
                    : TryLoad(dll, pdb, version);
            }
        }

        public IAssemblyNameReader AssemblyNameReader
        {
            get { return _AssemblyNameReader ?? (_AssemblyNameReader = new AssemblyNameReader()); }
            set { _AssemblyNameReader = value; }
        } private IAssemblyNameReader _AssemblyNameReader;


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