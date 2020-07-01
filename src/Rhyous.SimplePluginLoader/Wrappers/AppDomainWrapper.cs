using System;
using System.Configuration.Assemblies;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Principal;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// This method wraps an AppDomain, allowing for interface-based design in consumers of an AppDomain.
    /// </summary>
    /// <remarks>This wraps the events by subscribing and then hosting its own copy of each event. This code does
    /// not provide a way to trigger such events, as doing so would trigger only subscribers to the 
    /// wrapper and not subscribers directly the events in the concrete AppDomain being wrapped.</remarks>
    public class AppDomainWrapper : IAppDomain
    {
        private readonly AppDomain _AppDomain;
        private readonly IPluginLoaderLogger _Logger;

        public AppDomainWrapper(AppDomain appDomain, IPluginLoaderLogger logger = null)
        {
            _AppDomain = appDomain;
            _Logger = logger;
            _AppDomain.AssemblyLoad += OnAssemblyLoad;
            _AppDomain.AssemblyResolve += OnAssemblyResolve;
            _AppDomain.DomainUnload += OnDomainUnload;
            _AppDomain.FirstChanceException += OnFirstChanceException;
            _AppDomain.ProcessExit += OnProcessExit;
            _AppDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
            _AppDomain.ResourceResolve += OnResourceResolve;
            _AppDomain.TypeResolve += OnTypeResolve;
            _AppDomain.UnhandledException += OnUnhandledException;
            _AppDomain.UnhandledException += OnUnhandledException;
        }

        public string BaseDirectory => _AppDomain.BaseDirectory;

        public string DynamicDirectory => _AppDomain.DynamicDirectory;

        public string FriendlyName => _AppDomain.FriendlyName;

        public int Id => _AppDomain.Id;

        public bool IsFullyTrusted => _AppDomain.IsFullyTrusted;

        public bool IsHomogenous => _AppDomain.IsHomogenous;

        public bool ShadowCopyFiles => _AppDomain.ShadowCopyFiles;

        public string RelativeSearchPath => _AppDomain.RelativeSearchPath;

        public TimeSpan MonitoringTotalProcessorTime => _AppDomain.MonitoringTotalProcessorTime;

        public long MonitoringSurvivedMemorySize => _AppDomain.MonitoringSurvivedMemorySize;

        public long MonitoringTotalAllocatedMemorySize => _AppDomain.MonitoringTotalAllocatedMemorySize;

        #region Events
        public event AssemblyLoadEventHandler AssemblyLoad;
        internal void OnAssemblyLoad(object sender, AssemblyLoadEventArgs args) => AssemblyLoad?.Invoke(sender, args);

        public event ResolveEventHandler AssemblyResolve
        {
            add 
            {
                _AssemblyResolve += value;
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"{value.Method.DeclaringType.Name}.{value.Method.Name} subscribed to AppDomain.AssemblyResolve.");
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Total subscriptions: {AssemblyResolveSubscriberCount}");
            }
            remove 
            {
                _AssemblyResolve -= value;
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"{value.Method.DeclaringType.Name}.{value.Method.Name} unsubscribed to AppDomain.AssemblyResolve.");
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Total subscriptions: {AssemblyResolveSubscriberCount}");
            } 
        } private event ResolveEventHandler _AssemblyResolve;

        internal Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (ResolveEventHandler handler in _AssemblyResolve.GetInvocationList())
            {
                var assembly = handler?.Invoke(sender, args);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }
        public int AssemblyResolveSubscriberCount => _AssemblyResolve.GetInvocationList().Length;

        public event EventHandler DomainUnload;
        internal void OnDomainUnload(object sender, EventArgs args) => DomainUnload?.Invoke(sender, args);

        public event EventHandler<FirstChanceExceptionEventArgs> FirstChanceException;
        internal void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs args) => FirstChanceException?.Invoke(sender, args);

        public event EventHandler ProcessExit;
        internal void OnProcessExit(object sender, EventArgs args) => ProcessExit?.Invoke(sender, args);

        public event ResolveEventHandler ReflectionOnlyAssemblyResolve;
        internal Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            foreach (ResolveEventHandler handler in ReflectionOnlyAssemblyResolve.GetInvocationList())
            {
                var assembly = handler?.Invoke(sender, args);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }        

        public event ResolveEventHandler ResourceResolve;
        internal Assembly OnResourceResolve(object sender, ResolveEventArgs args)
        {
            foreach (ResolveEventHandler handler in ResourceResolve.GetInvocationList())
            {
                var assembly = handler?.Invoke(sender, args);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }

        public event ResolveEventHandler TypeResolve;
        internal Assembly OnTypeResolve(object sender, ResolveEventArgs args)
        {
            foreach (ResolveEventHandler handler in TypeResolve.GetInvocationList())
            {
                var assembly = handler?.Invoke(sender, args);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }

        public event UnhandledExceptionEventHandler UnhandledException;
        internal void OnUnhandledException(object sender, UnhandledExceptionEventArgs args) => UnhandledException?.Invoke(sender, args);

        #endregion

        public string ApplyPolicy(string assemblyName) => _AppDomain.ApplyPolicy(assemblyName);
        
        public int ExecuteAssembly(string assemblyFile, string[] args, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
        {
            return _AppDomain.ExecuteAssembly(assemblyFile, args, hashValue, hashAlgorithm);
        }

        public int ExecuteAssembly(string assemblyFile) => _AppDomain.ExecuteAssembly(assemblyFile);

        public int ExecuteAssembly(string assemblyFile, string[] args) => _AppDomain.ExecuteAssembly(assemblyFile, args);

        public int ExecuteAssemblyByName(string assemblyName) => _AppDomain.ExecuteAssemblyByName(assemblyName);

        public int ExecuteAssemblyByName(string assemblyName, params string[] args) => _AppDomain.ExecuteAssemblyByName(assemblyName, args);

        public int ExecuteAssemblyByName(AssemblyName assemblyName, params string[] args) => _AppDomain.ExecuteAssemblyByName(assemblyName, args);

        public IAssembly[] GetAssemblies() => _AppDomain.GetAssemblies().Select(a=> new AssemblyWrapper(a)).ToArray();

        public object GetData(string name) => _AppDomain.GetData(name);

        public object InitializeLifetimeService() => _AppDomain.InitializeLifetimeService();

        public bool? IsCompatibilitySwitchSet(string value) => _AppDomain.IsCompatibilitySwitchSet(value);

        public bool IsDefaultAppDomain() => _AppDomain.IsDefaultAppDomain();

        public bool IsFinalizingForUnload() => _AppDomain.IsFinalizingForUnload();

        public IAssembly Load(byte[] rawAssembly, byte[] rawSymbolStore) 
            => new AssemblyWrapper(_AppDomain.Load(rawAssembly, rawSymbolStore));

        public IAssembly Load(AssemblyName assemblyRef) => new AssemblyWrapper(_AppDomain.Load(assemblyRef));

        public IAssembly Load(string assemblyString) => new AssemblyWrapper(_AppDomain.Load(assemblyString));

        public IAssembly Load(byte[] rawAssembly) => new AssemblyWrapper(_AppDomain.Load(rawAssembly));

        public IAssembly[] ReflectionOnlyGetAssemblies() 
            => _AppDomain.ReflectionOnlyGetAssemblies().Select(a=> new AssemblyWrapper(a)).ToArray();

        public void SetData(string name, object data) => _AppDomain.SetData(name, data);

        public void SetPrincipalPolicy(PrincipalPolicy policy) => _AppDomain.SetPrincipalPolicy(policy);
        
        public void SetThreadPrincipal(IPrincipal principal) => _AppDomain.SetThreadPrincipal(principal);
    }
}