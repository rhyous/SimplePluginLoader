// See License at the end of the file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class Plugin<T> : IPlugin<T>
    {
        private readonly ITypeLoader<T> _TypeLoader;
        private readonly IPluginObjectCreator<T> _PluginObjectCreator;
        private readonly IPluginDependencyResolver _DependencyResolver;
        private readonly IAssemblyLoader _AssemblyLoader;

        public Plugin(ITypeLoader<T> typeLoader,
                      IPluginObjectCreator<T> pluginObjectCreator,
                      IPluginDependencyResolver dependencyResolver,
                      IAssemblyLoader assemblyLoader)
        {
            _TypeLoader = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader));
            _PluginObjectCreator = pluginObjectCreator ?? throw new ArgumentNullException(nameof(pluginObjectCreator));
            _PluginObjectCreator.Plugin = this;
            _DependencyResolver = dependencyResolver ?? throw new ArgumentNullException(nameof(dependencyResolver));
            _DependencyResolver.Plugin = this;
            _AssemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
        }

        public string Name { get { return Path.GetFileNameWithoutExtension(File); } }

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
            get { return Path.Combine(Directory, File); }
        }

        public string FullPathPdb
        {
            get { return Path.Combine(Directory, FilePdb); }
        }

        public IAssembly Assembly
        {
            get { return _Assembly ?? (_Assembly = _AssemblyLoader.Load(FullPath, FullPathPdb)); }
            set { _Assembly = value; }
        } private IAssembly _Assembly;

        public List<Type> PluginTypes
        {
            get { return _PluginTypes ?? (_PluginTypes = GetPluginTypes()); }
            set { _PluginTypes = value; }
        } private List<Type> _PluginTypes;

        private List<Type> GetPluginTypes()
        {
            _DependencyResolver.AddDependencyResolver();
            var types = _TypeLoader.Load(Assembly);
            if (types == null)
                _DependencyResolver.RemoveDependencyResolver();
            return types;
        }

        public List<T> CreatePluginObjects() => PluginTypes?.Select(t => CreatePluginObject(t)).Where(o => o != null).ToList();

        public T CreatePluginObject(Type t) => _PluginObjectCreator.Create(t);

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
                _AssemblyLoader.Dispose();
                _DependencyResolver.Dispose();
                // Remove should already be done but if a custom IDependencyResolver is used, let's verify.
                // It doesn't cause any issue if it is removed twice.
                _DependencyResolver.RemoveDependencyResolver();
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
