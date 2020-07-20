using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace Rhyous.SimplePluginLoader
{
    [ExcludeFromCodeCoverage]
    public class AssemblyWrapper : IAssembly
    {
        public Assembly Instance { get; }
        
        public AssemblyWrapper(Assembly assembly)
        {
            Instance = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public IEnumerable<CustomAttributeData> CustomAttributes => Instance.CustomAttributes;

        public IEnumerable<TypeInfo> DefinedTypes => Instance.DefinedTypes;

        public MethodInfo EntryPoint => Instance.EntryPoint;

        public string EscapedCodeBase => Instance.EscapedCodeBase;

        public IEnumerable<Type> ExportedTypes => Instance.ExportedTypes;

        public string FullName => Instance.FullName;

        public bool GlobalAssemblyCache => Instance.GlobalAssemblyCache;

        public long HostContext => Instance.HostContext;

        public string ImageRuntimeVersion => Instance.ImageRuntimeVersion;

        public bool IsDynamic => Instance.IsDynamic;

        public bool IsFullyTrusted => Instance.IsFullyTrusted;

        public string Location => Instance.Location;

        public Module ManifestModule => Instance.ManifestModule;

        public IEnumerable<Module> Modules => Instance.Modules;

        public bool ReflectionOnly => Instance.ReflectionOnly;

        public string CodeBase => Instance.CodeBase;

        public SecurityRuleSet SecurityRuleSet => Instance.SecurityRuleSet;

        public object CreateInstance(string typeName, bool ignoreCase) => Instance.CreateInstance(typeName, ignoreCase);

        public object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes) => Instance.CreateInstance(typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes);

        public object CreateInstance(string typeName) => Instance.CreateInstance(typeName);

        public object[] GetCustomAttributes(Type attributeType, bool inherit) => Instance.GetCustomAttributes(attributeType, inherit);

        public object[] GetCustomAttributes(bool inherit) => Instance.GetCustomAttributes(inherit);

        public IList<CustomAttributeData> GetCustomAttributesData() => Instance.GetCustomAttributesData();

        public Type[] GetExportedTypes() => Instance.GetExportedTypes();

        public FileStream GetFile(string name) => Instance.GetFile(name);

        public FileStream[] GetFiles() => Instance.GetFiles();

        public FileStream[] GetFiles(bool getResourceModules) => Instance.GetFiles(getResourceModules);

        public Module[] GetLoadedModules(bool getResourceModules) => Instance.GetLoadedModules(getResourceModules);

        public Module[] GetLoadedModules() => Instance.GetLoadedModules();

        public ManifestResourceInfo GetManifestResourceInfo(string resourceName) => Instance.GetManifestResourceInfo(resourceName);

        public string[] GetManifestResourceNames() => Instance.GetManifestResourceNames();

        public Stream GetManifestResourceStream(string name) => Instance.GetManifestResourceStream(name);

        public Stream GetManifestResourceStream(Type type, string name) => Instance.GetManifestResourceStream(type, name);

        public Module GetModule(string name) => Instance.GetModule(name);

        public Module[] GetModules() => Instance.GetModules();

        public Module[] GetModules(bool getResourceModules) => Instance.GetModules(getResourceModules);

        public AssemblyName GetName() => Instance.GetName();

        public AssemblyName GetName(bool copiedName) => Instance.GetName(copiedName);

        public void GetObjectData(SerializationInfo info, StreamingContext context) => Instance.GetObjectData(info, context);

        public AssemblyName[] GetReferencedAssemblies() => Instance.GetReferencedAssemblies();

        public IAssembly GetSatelliteAssembly(CultureInfo culture, Version version) => new AssemblyWrapper(Instance.GetSatelliteAssembly(culture, version));

        public IAssembly GetSatelliteAssembly(CultureInfo culture) => new AssemblyWrapper(Instance.GetSatelliteAssembly(culture));

        public Type GetType(string name, bool throwOnError, bool ignoreCase) => Instance.GetType(name, throwOnError, ignoreCase);

        public Type GetType(string name) => Instance.GetType(name);

        public Type GetType(string name, bool throwOnError) => Instance.GetType(name, throwOnError);

        public Type[] GetTypes() => Instance.GetTypes();

        public bool IsDefined(Type attributeType, bool inherit) => Instance.IsDefined(attributeType, inherit);

        public Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore) => Instance.LoadModule(moduleName, rawModule, rawSymbolStore);

        public Module LoadModule(string moduleName, byte[] rawModule) => Instance.LoadModule(moduleName, rawModule);
    }
}
