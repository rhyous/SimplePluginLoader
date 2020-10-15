// See License at the end of the file

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class Plugin<T> : IPlugin<T>
    {
        private readonly ITypeLoader<T> _TypeLoader;
        private readonly IAssemblyLoader _AssemblyLoader;

        public Plugin(ITypeLoader<T> typeLoader,
                      IPluginDependencyResolver dependencyResolver,
                      IAssemblyLoader assemblyLoader)
        {
            _TypeLoader = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader));
            DependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            if (DependencyResolver.Plugin == null)
                DependencyResolver.Plugin = this;
            _AssemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrWhiteSpace(File))
                    throw new PluginPathUndefinedException("Please define Plugin.File and Plugin.Directory properties.");
                return Path.GetFileNameWithoutExtension(File);
            }
        }

        public IPluginDependencyResolver DependencyResolver { get; }

        [ExcludeFromCodeCoverage]
        public string Directory { get; set; }

        public string File
        {
            get { return _File; }
            set
            {
                _File = value;
                _Assembly = null;
            }
        } private string _File;

        public string FilePdb
        {
            get
            {
                var length = File.LastIndexOf(".", StringComparison.Ordinal);
                return length > 0 ? File.Substring(0, length) + ".pdb" : null;
            }
        }

        public string FullPath
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(Directory) || string.IsNullOrWhiteSpace(File))
                    throw new PluginPathUndefinedException("Please define Plugin.File and Plugin.Directory properties.");
                return Path.Combine(Directory, File); 
            }
        }

        public string FullPathPdb
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Directory) || string.IsNullOrWhiteSpace(File))
                    throw new PluginPathUndefinedException("Please define Plugin.File and Plugin.Directory properties.");
                return Path.Combine(Directory, FilePdb);
            }
        }

        public IAssembly Assembly
        {
            get { return _Assembly ?? (_Assembly = _AssemblyLoader.TryLoad(FullPath, FullPathPdb)); }
            internal set { _Assembly = value; }
        } private IAssembly _Assembly;

        public List<Type> PluginTypes
        {
            get { return _PluginTypes ?? (_PluginTypes = GetPluginTypes()); }
            internal set { _PluginTypes = value; }
        } private List<Type> _PluginTypes;

        internal List<Type> GetPluginTypes()
        {
            DependencyResolver.AddDependencyResolver();
            var types = _TypeLoader.Load(Assembly);
            if ((types == null || !types.Any()) && DependencyResolver.Plugin == this)
                DependencyResolver.RemoveDependencyResolver();
            return types;
        }

        public List<T> CreatePluginObjects(IPluginObjectCreator<T> pluginObjectCreator) => PluginTypes?.Select(t => CreatePluginObject(t, pluginObjectCreator)).Where(o => o != null).ToList();

        public T CreatePluginObject(Type t, IPluginObjectCreator<T> pluginObjectCreator)
        {
            return pluginObjectCreator.Create(this, t);
        }

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
                // Remove should already be done by _DependencyResolver.Dispose(), but if a custom IDependencyResolver is 
                // used, we can't guarantee that, so let's both unregister and dispose of it. It doesn't cause any issue 
                // if a registration with an event is removed twice.
                DependencyResolver.RemoveDependencyResolver();
                DependencyResolver.Dispose();
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
