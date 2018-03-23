// See License at the end of the file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class InstancesLoader<T> : ILoadInstancesOfType<T>
        where T : class
    {
        public static bool ThrowExceptionsOnLoad = false;
        
        internal IPluginLoaderLogger Logger;

        public InstancesLoader(IPluginLoaderLogger logger) { Logger = logger; }

        public List<T> LoadInstances(Assembly assembly)
        {
            return LoadTypes(assembly);
        }

        public List<Type> GetPluginTypes(Assembly assembly)
        {
            if (assembly == null)
                return null;
            return assembly.GetTypes().Where(o => o.IsPluginType<T>())?.ToList() ?? null;
        }

        public List<T> LoadTypes(Assembly assembly)
        {
            try
            {
                var typesToLoad = GetPluginTypes(assembly);
                if (typesToLoad == null)
                    return null;
                var listOfT = new List<T>();
                foreach (var typeToLoad in typesToLoad.Where(t => t.IsInstantiable()))
                {
                    var obj = Create(typeToLoad);
                    if (obj == null)
                        continue;
                    listOfT.Add(obj);
                    Logger?.WriteLine(PluginLoaderLogLevel.Info, $"A plugin type was found and added: {obj}");
                }
                return listOfT;
            }
            catch (Exception e)
            {
                if (ThrowExceptionsOnLoad)
                {
                    var e2 = new PluginTypeLoadException("Failed to load plugin types. See inner exception.", e);
                    Logger?.WriteLine(PluginLoaderLogLevel.Debug, e2.Message);
                    throw e2;
                }
                else
                {
                    Logger?.WriteLines(PluginLoaderLogLevel.Debug, $"Exception occurred loading a plugin type.", e.Message);
                    return null;
                }
            }
        }

        private static T Create(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = typeof(T).GetGenericArguments();
                if (genericArgs == null || !genericArgs.Any())
                    return null;
                return CreateGenericType(type, genericArgs);
            }
            var obj = Activator.CreateInstance(type);
            return obj as T;
        }

        private static T CreateGenericType(Type genericType, params Type[] genericParams)
        {
            Type constructedType = genericType.MakeGenericType(genericParams);
            var methodInfo = typeof(Activator).GetGenericMethod("CreateInstance");
            var genMethod = methodInfo.MakeGenericMethod(constructedType);
            var obj = genMethod.Invoke(null, null);
            return obj as T;
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