﻿// See License at the end of the file

using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyLoader<T> : IAssemblyBuilder
        where T : class
    {
        private Plugin<T> _Plugin;
        internal IPluginLoaderLogger Logger;

        public AssemblyLoader(Plugin<T> plugin, IPluginLoaderLogger logger)
        {
            _Plugin = plugin;
            Logger = logger;
        }

        public AppDomain Domain
        {
            get { return _Domain ?? (_Domain = AppDomain.CurrentDomain); }
        } private AppDomain _Domain;

        public virtual Assembly Load(string dll, string pdb)
        {
            if (Path.IsPathRooted(dll))
            {
                return TryLoad(dll, pdb);
            }
            var defaultDirs = new PluginPaths(_Plugin.AssemblyBuilder.Domain.BaseDirectory, Logger).GetDefaultPluginDirectories();
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

        public Assembly TryLoad(string dll, string pdb)
        {
            var assembly = FindAlreadyLoadedAssembly(dll);
            if (assembly != null)
                return assembly;
            lock (AssemblyDictionary.IsLocked)
            {
                assembly = FindAlreadyLoadedAssembly(dll);
                if (assembly != null)
                    return assembly;
                assembly = Domain.TryLoad(dll, pdb, Logger);
                if (assembly == null)
                    return null;
                try
                {
                    AssemblyDictionary.Assemblies.Add(GetKey(dll), assembly);
                }
                catch (Exception e)
                {
                    Logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                    throw;
                }
            }
            return assembly;
        }

        public Assembly TryLoad(string dll, string pdb, string version)
        {
            var assembly = FindAlreadyLoadedAssembly(dll, version);
            if (assembly != null)
                return assembly;
            lock (AssemblyDictionary.IsLocked)
            {
                assembly = FindAlreadyLoadedAssembly(dll, version);
                if (assembly != null)
                    return assembly;
                assembly = Domain.TryLoad(dll, pdb, Logger);
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
                    Logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                    throw;
                }
            }
            return assembly;
        }

        private Assembly FindAlreadyLoadedAssembly(string dll)
        {
            Assembly assembly = AssemblyDictionary.Assemblies.TryGetValue(GetKey(dll), out assembly) ? assembly : null;
            if (assembly == null)
                assembly = Domain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == Path.GetFileName(dll));
            return assembly;
        }

        private Assembly FindAlreadyLoadedAssembly(string dll, string version)
        {
            Assembly assembly = AssemblyDictionary.Assemblies.TryGetValue(GetKey(dll, version), out assembly) ? assembly : null;
            if (assembly == null)
                assembly = Domain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == Path.GetFileName(dll) && a.GetName().Version.ToString() == version);
            return assembly;
        }

        private static string GetKey(string dll)
        {
            return string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(dll), File.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        private static string GetKey(string dll, string version)
        {
            return string.Format("{0}_{1}_{2}", Path.GetFileNameWithoutExtension(dll), version, File.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        internal AssemblyDictionary AssemblyDictionary
        {
            get { return _AssemblyDictionary ?? (_AssemblyDictionary = AssemblyDictionary.GetInstance(Domain, Logger)); }
            set { _AssemblyDictionary = value; }
        } private AssemblyDictionary _AssemblyDictionary;
        
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