using System;
using System.Configuration.Assemblies;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Principal;

namespace Rhyous.SimplePluginLoader
{
    public interface IAppDomain
    {
        string BaseDirectory { get; }
        string DynamicDirectory { get; }
        string FriendlyName { get; }
        int Id { get; }
        bool IsFullyTrusted { get; }
        bool IsHomogenous { get; }
        bool ShadowCopyFiles { get; }
        string RelativeSearchPath { get; }
        TimeSpan MonitoringTotalProcessorTime { get; }
        long MonitoringSurvivedMemorySize { get; }
        long MonitoringTotalAllocatedMemorySize { get; }
        event UnhandledExceptionEventHandler UnhandledException;
        event ResolveEventHandler TypeResolve;
        event AssemblyLoadEventHandler AssemblyLoad;
        event ResolveEventHandler AssemblyResolve;
        int AssemblyResolveSubscriberCount { get; }

        event EventHandler DomainUnload;
        event EventHandler<FirstChanceExceptionEventArgs> FirstChanceException;
        event EventHandler ProcessExit;
        event ResolveEventHandler ReflectionOnlyAssemblyResolve;
        event ResolveEventHandler ResourceResolve;

        string ApplyPolicy(string assemblyName);
        int ExecuteAssembly(string assemblyFile, string[] args, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm);
        int ExecuteAssembly(string assemblyFile);
        int ExecuteAssembly(string assemblyFile, string[] args);
        int ExecuteAssemblyByName(string assemblyName);
        int ExecuteAssemblyByName(string assemblyName, params string[] args);
        int ExecuteAssemblyByName(AssemblyName assemblyName, params string[] args);
        IAssembly[] GetAssemblies();
        object GetData(string name);
        Type GetType();
        object InitializeLifetimeService();
        bool? IsCompatibilitySwitchSet(string value);
        bool IsDefaultAppDomain();
        bool IsFinalizingForUnload();
        IAssembly Load(byte[] rawAssembly, byte[] rawSymbolStore);
        IAssembly Load(AssemblyName assemblyRef);
        IAssembly Load(string assemblyString);
        IAssembly Load(byte[] rawAssembly);
        IAssembly[] ReflectionOnlyGetAssemblies();
        void SetData(string name, object data);
        void SetPrincipalPolicy(PrincipalPolicy policy);
        void SetThreadPrincipal(IPrincipal principal);
        string ToString();
    }
}
