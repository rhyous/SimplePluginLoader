// See License at the end of the file

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class InstanceLoader<T> : IInstanceLoader<T>
        where T : class
    {
        private readonly IObjectCreator<T> _ObjectCreator;
        private readonly ITypeLoader<T> _TypeLoader;
        private readonly IPluginLoaderSettings _Settings;
        internal IPluginLoaderLogger Logger;

        public InstanceLoader(IObjectCreator<T> objectCreator, 
                               ITypeLoader<T> typeLoader,
                               IPluginLoaderSettings settings,
                               IPluginLoaderLogger logger)
        {
            _ObjectCreator = objectCreator ?? throw new ArgumentNullException(nameof(objectCreator));
            _TypeLoader = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader));
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Logger = logger;
        }

        public IPlugin<T> Plugin { get => _ObjectCreator.Plugin; set => _ObjectCreator.Plugin = value; }

        public List<T> Load(IAssembly assembly)
        {
            if (assembly == null)
                return null;
            var typesToLoad = _TypeLoader.Load(assembly);
            if (typesToLoad == null || !typesToLoad.Any())
                return null;
            var listOfT = new List<T>();
            foreach (var typeToLoad in typesToLoad.Where(t => t.IsInstantiable()))
            {
                try
                {
                    var obj = _ObjectCreator.Create(typeToLoad);
                    if (obj == null)
                        continue;
                    listOfT.Add(obj);
                    Logger?.WriteLine(PluginLoaderLogLevel.Info, $"A plugin type was found and added: {obj}");
                }
                catch (Exception e)
                {
                    if (_Settings.ThrowExceptionsOnLoad)
                    {
                        var e2 = new PluginTypeLoadException($"Failed to load plugin type: {typeToLoad.Name}. See inner exception.", e);
                        Logger?.Log(e2);
                        throw e2;
                    }
                    else
                    {
                        Logger?.Write(PluginLoaderLogLevel.Info, $"Exception occurred loading the plugin of type: {typeToLoad.Name}.");
                        Logger?.Log(e);
                    }
                }
            }
            return listOfT;
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