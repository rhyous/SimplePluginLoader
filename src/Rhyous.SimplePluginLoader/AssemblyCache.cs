using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyCache : IAssemblyCache
    {
        #region Singleton

        private static readonly Lazy<AssemblyCache> Lazy = new Lazy<AssemblyCache>(() => new AssemblyCache());

        public static AssemblyCache Instance { get { return Lazy.Value; } }

        internal AssemblyCache()
        {
        }

        #endregion  
        public IDictionary<string, IAssembly> Assemblies
        {
            get { return _Assemblies.Value; }
        } private readonly Lazy<ConcurrentDictionary<string, IAssembly>> _Assemblies = new Lazy<ConcurrentDictionary<string, IAssembly>>();
    }
}