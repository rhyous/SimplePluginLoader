// See License at the end of the file

using System;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public static class TypeExtensions
    {
        public static bool IsSameOrSubclassAs(this Type left, Type right)
        {
            return left == right || left.IsSubclassOf(right);
        }

        public static bool IsPluginType<T>(this Type type)
        {
            return type.IsSameOrSubclassAs(typeof(T)) || typeof(T).IsAssignableFrom(type) || typeof(T).IsGenericInterfaceOf(type);
        }

        public static bool IsTypeOf(this Type type, Type type2)
        {
            return type.IsSameOrSubclassAs(type2) || type2.IsAssignableFrom(type) || type2.IsGenericInterfaceOf(type) || type2.IsGenericTypeDefinitionOf(type);
        }

        public static bool IsGenericInterfaceOf(this Type left, Type right)
        {
            if (!left.IsGenericType || !left.IsInterface || !right.IsGenericType)
            {
                if (right.BaseType != null)
                    return left.IsGenericInterfaceOf(right.BaseType);
                return false;
            }
            var rightInterfaces = right.GetInterfaces();
            if (rightInterfaces.Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == left))
                return true;
            var leftGenArgs = left.GetGenericArguments().ToList();
            var rightGenArgs = right.GetGenericArguments().ToList();
            if (rightInterfaces.Any(itype => itype.IsGenericType && itype.GetGenericTypeDefinition() == left.GetGenericTypeDefinition()))
            {
                if (!left.ContainsGenericParameters && right.ContainsGenericParameters)
                    return true;
            }
            return false;
        }

        public static bool IsGenericTypeDefinitionOf(this Type left, Type right)
        {
            if (left.IsGenericType && left.IsGenericTypeDefinition && right.IsGenericType)
            {
                return (right.GetGenericTypeDefinition() == left)
                    || (right.BaseType.IsGenericType && right.IsGenericTypeDefinition && right.GetGenericTypeDefinition() == left);
            }
            return false;
        }

        public static bool IsInstantiable(this Type type)
        {
            return !type.IsInterface && !type.IsAbstract && (!type.IsGenericType || !type.GetGenericArguments().Any(t => t.IsInterface || t.IsAbstract));
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (string.IsNullOrWhiteSpace(methodName))
                throw new ArgumentNullException("methodName");
            return type.GetMethods().FirstOrDefault(m => m.Name == methodName && m.IsGenericMethod == true);
        }

        /// <summary>
        /// Type.DeclaringType could be a compile-time generated type if the DeclaryingType is a dynamically generated object or method.
        /// If what you are after is the class you defined the dynamically generated object or method, then you can use this method.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>The first DeclaringType that is not a System.RuntimeType</returns>
        public static Type GetFixedDeclaringType(this Type t)
        {
            if (t.DeclaringType == null)
                return t;
            while (t.GetType() == _RunTimeType) 
                return GetFixedDeclaringType(t.DeclaringType);
            return t;
        }

        /// <summary>
        /// Type.DeclaringType could be a compile-time generated type if the DeclaryingType is a dynamically generated object or method.
        /// If what you are after is the class you defined the dynamically generated object or method, then you can use this method.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>The first DeclaringType that is not a System.RuntimeType</returns>
        public static Type GetFixedDeclaringType(this MethodInfo mi)
        {
            if (mi.DeclaringType == null)
                return mi.DeclaringType;
            if (mi.DeclaringType.GetType() == _RunTimeType)
                return GetFixedDeclaringType(mi.DeclaringType);
            return mi.DeclaringType;
        }

        /// <summary> System.RuntimeType is internal so we can't use it directly</summary>
        private static Type _RunTimeType = Type.GetType("System.RuntimeType");
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