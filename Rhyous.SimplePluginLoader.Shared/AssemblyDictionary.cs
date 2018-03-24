﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyDictionary : IAssemblyDictionary
    {
        #region Singleton

        internal static readonly object InstanceLock = new object();
        internal readonly object IsLocked = new object();
        internal IPluginLoaderLogger Logger;

        public static AssemblyDictionary GetInstance(IAppDomain domain, IPluginLoaderLogger logger)
        {
            if (_Instance != null)
                return _Instance;
            lock (InstanceLock)
            {
                if (_Instance != null)
                    return _Instance;
                return _Instance = new AssemblyDictionary(domain, logger);
            }
        } private static AssemblyDictionary _Instance;

        internal AssemblyDictionary(IAppDomain domain, IPluginLoaderLogger logger)
        {
            Logger = logger;
            domain.AssemblyLoad += OnAssemblyLoad;
        }

        private void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Logger?.WriteLines(PluginLoaderLogLevel.Info, "An assembly was loaded", args.LoadedAssembly.CodeBase, args.LoadedAssembly.Location);
        }
        #endregion

        public Dictionary<string, Assembly> Assemblies
        {
            get { return _Assemblies.Value; }
        } private Lazy<Dictionary<string, Assembly>> _Assemblies = new Lazy<Dictionary<string, Assembly>>();
    }
}