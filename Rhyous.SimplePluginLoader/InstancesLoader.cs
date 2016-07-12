﻿// See License at the end of the file

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class InstancesLoader<T> : ILoadInstancesOfType<T> where T : class
    {
        public static bool ThrowExceptionsOnLoad = false;

        public List<T> LoadInstances(Assembly assembly)
        {
            return LoadTypes(assembly);
        }

        public List<T> LoadTypes(Assembly assembly)
        {
            var listOfT = new List<T>();
            if (assembly != null)
            {
                Type[] objTypes = assembly.GetTypes();
                IEnumerable<Type> typesToLoad = GetTypesToLoad(objTypes);
                if (typesToLoad == null)
                    return null;
                try { listOfT.AddRange(typesToLoad.Select(Activator.CreateInstance).OfType<T>()); }
                catch { if (ThrowExceptionsOnLoad) throw; }
            }
            return listOfT;
        }

        private static IEnumerable<Type> GetTypesToLoad(Type[] objTypes)
        {
            IEnumerable<Type> typesToLoad = null;
            if (typeof(T).IsClass)
            {
                typesToLoad = objTypes.Where(objType => objType.IsSameOrSubclassAs(typeof(T)));
            }
            if (typeof(T).IsInterface)
            {
                typesToLoad = objTypes.Where(objType => objType.GetInterfaces().Contains(typeof(T)));
            }
            return typesToLoad;
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