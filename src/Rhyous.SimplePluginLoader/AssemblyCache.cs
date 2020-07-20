using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{

    public class AssemblyCache : IAssemblyCache
    {
        private readonly IAppDomain _AppDomain;
        private readonly IAssemblyNameReader _AssemblyNameReader;
        private readonly IPluginLoaderLogger _Logger;

        public AssemblyCache(IAppDomain appDomain,
                             IAssemblyNameReader assemblyNameReader,
                             IPluginLoaderLogger logger)
        {
            _AppDomain = appDomain;
            _AssemblyNameReader = assemblyNameReader;
            Assemblies = new ConcurrentDictionary<string, IAssembly>();
            _Logger = logger;
        }

        public ConcurrentDictionary<string, IAssembly> Assemblies { get; }

        public IAssembly Add(string dll, string assemblyVersion, IAssembly assembly)
        {
            if (string.IsNullOrWhiteSpace(dll)) { throw new ArgumentException(nameof(dll)); }
            if (assembly == null) { throw new ArgumentNullException(nameof(assembly)); }
            if (string.IsNullOrWhiteSpace(assemblyVersion)) { assemblyVersion = assembly.GetName().Version?.ToString(); }

            var key = GetKey(dll, assemblyVersion);
            Assemblies.TryAdd(key, assembly); // Since it is a ConcurrentDictionary another assembly could already be added by another thread.
            Assemblies.TryGetValue(key, out assembly); // Get the loaded assembly in case a different thread loaded one.
            return assembly;
        }

        public IAssembly FindAlreadyLoadedAssembly(string dll, string version = null)
        {
            version = (string.IsNullOrWhiteSpace(version))
                ? _AssemblyNameReader.GetAssemblyName(dll)?.Version?.ToString()
                : version;

            var key = GetKey(dll, version);

            // Find and return already loaded assembly
            if (Assemblies.TryGetValue(key, out IAssembly cachedAssembly))
            {
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Found already loaded plugin assembly: {key}");
                return cachedAssembly;
            }

            // See if the Assembly was loaded into the AppDomain already
            // This could happen if it was loaded by some other system or reference
            var alreadyLoddedassemblies = _AppDomain.GetAssemblies();
            if (alreadyLoddedassemblies != null && alreadyLoddedassemblies.Any())
            {
                var dllAssemblyName = _AssemblyNameReader.GetAssemblyName(dll);
                if (dllAssemblyName == null)
                    return null;
                foreach (var alreadyLoddedassembly in alreadyLoddedassemblies)
                {
                    var loadedAssemblyName = alreadyLoddedassembly.GetName();
                    if (loadedAssemblyName.FullName == dllAssemblyName.FullName && loadedAssemblyName.Version.ToString() == version)
                    {
                        key = GetKey(dll, alreadyLoddedassembly.GetName().Version.ToString());
                        _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Loading plugin assembly from dll: {key}");
                        Assemblies.TryAdd(key, alreadyLoddedassembly);
                        return alreadyLoddedassembly;
                    }
                }
            }

            // No assembly found, return null
            return null;
        }

        internal string GetKey(string dll, string version = null)
        {
            return (string.IsNullOrWhiteSpace(version))
                ? string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(dll), File.GetLastWriteTime(dll).ToFileTimeUtc())
                : string.Format("{0}_{1}_{2}", Path.GetFileNameWithoutExtension(dll), version, File.GetLastWriteTime(dll).ToFileTimeUtc());
        }

        #region Wrappers -  While this could come from the constructor, these are only used in Unit Tests, so we hide them internally
        [ExcludeFromCodeCoverage]
        internal IFile File
        {
            get { return _File ?? (_File = FileWrapper.Instance); }
            set { _File = value; }
        } private IFile _File;
        #endregion
    }
}