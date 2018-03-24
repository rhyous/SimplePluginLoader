﻿using System;
using System.Configuration.Assemblies;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Principal;

namespace Rhyous.SimplePluginLoader
{
    public class AppDomainWrapper : IAppDomain
    {
        private AppDomain _AppDomain;
        public AppDomainWrapper(AppDomain appDomain) { _AppDomain = appDomain; }

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
        public event UnhandledExceptionEventHandler UnhandledException
        {
            add { _AppDomain.UnhandledException += value; }
            remove { _AppDomain.UnhandledException -= value; }
        }
        public event ResolveEventHandler TypeResolve
        {
            add { _AppDomain.TypeResolve += value; }
            remove { _AppDomain.TypeResolve -= value; }
        }
        public event AssemblyLoadEventHandler AssemblyLoad
        {
            add { _AppDomain.AssemblyLoad += value; }
            remove { _AppDomain.AssemblyLoad -= value; }
        }

        public event ResolveEventHandler AssemblyResolve
        {
            add { _AppDomain.AssemblyResolve += value; }
            remove { _AppDomain.AssemblyResolve -= value; }
        }

        public event EventHandler DomainUnload
        {
            add { _AppDomain.DomainUnload += value; }
            remove { _AppDomain.DomainUnload -= value; }
        }

        public event EventHandler<FirstChanceExceptionEventArgs> FirstChanceException
        {
            add { _AppDomain.FirstChanceException += value; }
            remove { _AppDomain.FirstChanceException -= value; }
        }

        public event EventHandler ProcessExit
        {
            add { _AppDomain.ProcessExit += value; }
            remove { _AppDomain.ProcessExit -= value; }
        }

        public event ResolveEventHandler ReflectionOnlyAssemblyResolve
        {
            add { _AppDomain.ReflectionOnlyAssemblyResolve += value; }
            remove { _AppDomain.ReflectionOnlyAssemblyResolve -= value; }
        }

        public event ResolveEventHandler ResourceResolve
        {
            add { _AppDomain.ResourceResolve += value; }
            remove { _AppDomain.ResourceResolve -= value; }
        }
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

        public Assembly[] GetAssemblies() => _AppDomain.GetAssemblies();

        public object GetData(string name) => _AppDomain.GetData(name);

        public object InitializeLifetimeService() => _AppDomain.InitializeLifetimeService();

        public bool? IsCompatibilitySwitchSet(string value) => _AppDomain.IsCompatibilitySwitchSet(value);

        public bool IsDefaultAppDomain() => _AppDomain.IsDefaultAppDomain();

        public bool IsFinalizingForUnload() => _AppDomain.IsFinalizingForUnload();

        public Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore) => _AppDomain.Load(rawAssembly, rawSymbolStore);

        public Assembly Load(AssemblyName assemblyRef) => _AppDomain.Load(assemblyRef);

        public Assembly Load(string assemblyString) => _AppDomain.Load(assemblyString);

        public Assembly Load(byte[] rawAssembly) => _AppDomain.Load(rawAssembly);

        public Assembly[] ReflectionOnlyGetAssemblies() => _AppDomain.ReflectionOnlyGetAssemblies();

        public void SetData(string name, object data) => _AppDomain.SetData(name, data);

        public void SetPrincipalPolicy(PrincipalPolicy policy) => _AppDomain.SetPrincipalPolicy(policy);
        
        public void SetThreadPrincipal(IPrincipal principal) => _AppDomain.SetThreadPrincipal(principal);
    }
}