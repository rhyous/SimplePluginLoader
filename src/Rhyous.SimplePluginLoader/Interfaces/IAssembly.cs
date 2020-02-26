using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssembly
    {
        Assembly Instance { get; }
        IEnumerable<CustomAttributeData> CustomAttributes { get; }
        IEnumerable<TypeInfo> DefinedTypes { get; }
        MethodInfo EntryPoint { get; }
        string EscapedCodeBase { get; }
        IEnumerable<Type> ExportedTypes { get; }
        string FullName { get; }
        bool GlobalAssemblyCache { get; }
        long HostContext { get; }
        string ImageRuntimeVersion { get; }
        bool IsDynamic { get; }
        bool IsFullyTrusted { get; }
        string Location { get; }
        Module ManifestModule { get; }
        IEnumerable<Module> Modules { get; }
        bool ReflectionOnly { get; }
        string CodeBase { get; }
        SecurityRuleSet SecurityRuleSet { get; }
        object CreateInstance(string typeName, bool ignoreCase);
        object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes);
        object CreateInstance(string typeName);
        bool Equals(object o);
        object[] GetCustomAttributes(Type attributeType, bool inherit);
        object[] GetCustomAttributes(bool inherit);
        IList<CustomAttributeData> GetCustomAttributesData();
        Type[] GetExportedTypes();
        FileStream GetFile(string name);
        FileStream[] GetFiles();
        FileStream[] GetFiles(bool getResourceModules);
        int GetHashCode();
        Module[] GetLoadedModules(bool getResourceModules);
        Module[] GetLoadedModules();
        ManifestResourceInfo GetManifestResourceInfo(string resourceName);
        string[] GetManifestResourceNames();
        Stream GetManifestResourceStream(string name);
        Stream GetManifestResourceStream(Type type, string name);
        Module GetModule(string name);
        Module[] GetModules();
        Module[] GetModules(bool getResourceModules);
        AssemblyName GetName();
        AssemblyName GetName(bool copiedName);
        void GetObjectData(SerializationInfo info, StreamingContext context);
        AssemblyName[] GetReferencedAssemblies();
        IAssembly GetSatelliteAssembly(CultureInfo culture, Version version);
        IAssembly GetSatelliteAssembly(CultureInfo culture);
        Type GetType(string name, bool throwOnError, bool ignoreCase);
        Type GetType(string name);
        Type GetType(string name, bool throwOnError);
        Type[] GetTypes();
        bool IsDefined(Type attributeType, bool inherit);
        Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore);
        Module LoadModule(string moduleName, byte[] rawModule);
        string ToString();
    }
}
