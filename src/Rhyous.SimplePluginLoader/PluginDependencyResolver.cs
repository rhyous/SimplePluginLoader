// See License at the end of the file

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Rhyous.SimplePluginLoader
{
    [Serializable]
    public class PluginDependencyResolver : IPluginDependencyResolver
    {
        private readonly IAppDomain _AppDomain;
        private readonly IPluginLoaderSettings _Settings;
        private readonly IAssemblyLoader _AssemblyLoader;
        private readonly IPluginLoaderLogger _Logger;
        private readonly IWaiter _Waiter;
        private readonly IAssemblyResolveCache _AssemblyResolveCache;
        internal readonly Locked<bool> IsRegisteredWithAssemblyResolve = new Locked<bool>();
        internal readonly ConcurrentDictionary<string, List<string>> _AttemptedPaths = new ConcurrentDictionary<string, List<string>>();

        public PluginDependencyResolver(IAppDomain appDomain,
                                        IPluginLoaderSettings settings,
                                        IAssemblyLoader assemblyLoader,
                                        IWaiter waiter,
                                        IAssemblyResolveCache assemblyResolveCache,
                                        IPluginLoaderLogger logger)
        {
            _AppDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _AssemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
            _Waiter = waiter;
            _AssemblyResolveCache = assemblyResolveCache;
            _Logger = logger;
        }

        public IPlugin Plugin { get; set; }

        public List<string> Paths
        {
            get { return _Paths ?? (_Paths = Plugin?.GetPaths(_Settings)); }
            set { _Paths = value; }
        } private List<string> _Paths;

        public void AddDependencyResolver()
        {
            if (!IsRegisteredWithAssemblyResolve.Value) // What if two threads hit this at the same time?
            {
                _AppDomain.AssemblyResolve += AssemblyResolveHandler; // Add
                IsRegisteredWithAssemblyResolve.Value = true;
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Added AssemblyResolver for plugin: {Plugin.Name}");
            }
        }

        public void RemoveDependencyResolver()
        {
            _AppDomain.AssemblyResolve -= AssemblyResolveHandler; // If it is already removed, there is no harm in removing it again.
            IsRegisteredWithAssemblyResolve.Value = false;
            _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Removed AssemblyResolver for plugin: {Plugin?.Name ?? "unknown"}.");
        }

        public Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            if (Plugin == null)
            {
                RemoveDependencyResolver();
                return null;
            }
            var assemblyDetails = args.Name.Split(", ".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            var file = assemblyDetails.First();
            var version = assemblyDetails.FirstOrDefault(ad => ad.StartsWith("Version="))?
                                         .Split("=".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                                         .Skip(1)?.First();
            var key = $"{file}_{version}";
            _Waiter.Wait(key);
            if (_AssemblyResolveCache.Cache.TryGetValue(key, out IAssembly cachedAssembly))
            {
                _Waiter.InProgress[key] = false;
                return cachedAssembly.Instance;
            }
            var paths = GetPathsToSearch(Paths, args, key);
            if (paths == null || !paths.Any())
            {
                _Waiter.InProgress[key] = false;
                return null;
            }
            foreach (var path in paths)
            {
                _AttemptedPaths[args.Name].Add(path);
                if (!Directory.Exists(path))
                {
                    Paths.Remove(path);
                    _Waiter.InProgress[key] = false;
                    continue;
                }
                var dll = System.IO.Path.Combine(path, file + ".dll");
                var pdb = System.IO.Path.Combine(path, file + ".pdb");
                var assembly = (string.IsNullOrWhiteSpace(version))
                    ? _AssemblyLoader.TryLoad(dll, pdb)
                    : _AssemblyLoader.TryLoad(dll, pdb, version);
                if (assembly != null)
                {
                    _Waiter.InProgress[key] = false;
                    _AssemblyResolveCache.Cache.TryAdd(key, assembly);
                    return assembly.Instance;
                }
            }
            _Waiter.InProgress[key] = false;
            return null;
        }

        private List<string> GetPathsToSearch(List<string> allPaths, ResolveEventArgs args, string key)
        {
            var paths = allPaths?.Where(Directory.Exists).ToList();
            if (paths == null || !paths.Any())
            {
                return null;
            }
            if (!_AttemptedPaths.TryGetValue(args.Name, out List<string> alreadyTriedPaths))
            {
                alreadyTriedPaths = new List<string>();
                if (!_AttemptedPaths.TryAdd(args.Name, alreadyTriedPaths))
                    _AttemptedPaths.TryGetValue(args.Name, out alreadyTriedPaths);
            }
            // Use a copy of alreadyTriedPaths in case another action gets and changes the list
            foreach (var path in alreadyTriedPaths.ToList())
            {
                paths.Remove(path);
            }
            return paths;
        }

        internal IDirectory Directory
        {
            get { return _Directory ?? (_Directory = DirectoryWrapper.Instance); }
            set { _Directory = value; }
        } private IDirectory _Directory;

        #region IDisposable        
        private bool _DisposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_DisposedValue)
            {
                if (disposing)
                {
                    _AppDomain.AssemblyResolve -= AssemblyResolveHandler;
                }
                _DisposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
