using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyDictionary : IAssemblyDictionary
    {
        #region Singleton

        private static readonly Lazy<AssemblyDictionary> Lazy = new Lazy<AssemblyDictionary>(() => new AssemblyDictionary());
        
        internal readonly object IsLocked = new object();

        public static AssemblyDictionary Instance { get { return Lazy.Value; } }

        internal AssemblyDictionary()
        {
        }

        #endregion

        public Dictionary<string, Assembly> Assemblies
        {
            get { return _Assemblies.Value; }
        } private Lazy<Dictionary<string, Assembly>> _Assemblies = new Lazy<Dictionary<string, Assembly>>();
    }
}
