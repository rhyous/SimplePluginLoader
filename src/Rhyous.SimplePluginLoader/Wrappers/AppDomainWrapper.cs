using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

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
        private readonly ConcurrentDictionary<ResolveEventHandler, byte> Handlers = new ConcurrentDictionary<ResolveEventHandler, byte>();

        public AppDomainWrapper(AppDomain appDomain, IPluginLoaderLogger logger = null)
        {
            _AppDomain = appDomain;
            _Logger = logger;
            _AppDomain.AssemblyResolve += OnAssemblyResolve;
        }

        [ExcludeFromCodeCoverage]
        public string BaseDirectory => _AppDomain.BaseDirectory;

        #region Events

        public event ResolveEventHandler AssemblyResolve
        {
            add
            {
                if (Handlers.TryGetValue(value, out _))
                {
                    _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"{value.Method.GetFixedDeclaringType().Name} tried to subscribed to AppDomain.AssemblyResolve, but was already subscribed.");
                    return;
                }
                _AssemblyResolve += value;
                Handlers.TryAdd(value, 1);
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"{value.Method.GetFixedDeclaringType().Name} subscribed to AppDomain.AssemblyResolve.");
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Total subscriptions: {AssemblyResolveSubscriberCount}");
            }
            remove
            {
                _AssemblyResolve -= value;
                Handlers.TryRemove(value, out _);
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"{value.Method.GetFixedDeclaringType().Name} unsubscribed to AppDomain.AssemblyResolve.");
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, $"Total subscriptions: {AssemblyResolveSubscriberCount}");
            }
        } private event ResolveEventHandler _AssemblyResolve;

        internal Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (_AssemblyResolve == null)
                return null;
            foreach (ResolveEventHandler handler in _AssemblyResolve.GetInvocationList())
            {
                var assembly = handler.Invoke(sender, args);
                if (assembly != null)
                    return assembly;
            }
            return null;
        }

        public int AssemblyResolveSubscriberCount => _AssemblyResolve?.GetInvocationList().Length ?? 0;
        #endregion

        public IAssembly[] GetAssemblies() => _AppDomain.GetAssemblies().Select(a => new AssemblyWrapper(a)).ToArray();

        [ExcludeFromCodeCoverage]
        public IAssembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
            => new AssemblyWrapper(_AppDomain.Load(rawAssembly, rawSymbolStore));

        [ExcludeFromCodeCoverage]
        public IAssembly Load(byte[] rawAssembly) => new AssemblyWrapper(_AppDomain.Load(rawAssembly));

        #region Custom Methods
        public IAssembly TryLoad(byte[] rawAssembly)
        {
            try { return Load(rawAssembly); }
            catch (Exception e)
            {
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                return null;
            }
        }

        public IAssembly TryLoad(byte[] rawAssembly, byte[] rawSymbolStore)
        {
            try { return Load(rawAssembly, rawSymbolStore); }
            catch (Exception e)
            {
                _Logger?.WriteLine(PluginLoaderLogLevel.Debug, e.Message);
                return null;
            }
        }

        public IAssembly TryLoad(string dll, string pdb)
        {
            if (File.Exists(dll))
            {
                var assembly = File.Exists(pdb)
                    ? TryLoad(File.ReadAllBytes(dll), File.ReadAllBytes(pdb)) // Allow debugging
                    : TryLoad(File.ReadAllBytes(dll));
                return assembly;
            }
            return null;
        }
        #endregion

        #region Wrappers -  While this could come from the constructor, these are only used in Unit Tests, so we hide them internally
        [ExcludeFromCodeCoverage]
        internal IDirectory Directory
        {
            get { return _Directory ?? (_Directory = DirectoryWrapper.Instance); }
            set { _Directory = value; }
        } private IDirectory _Directory;

        [ExcludeFromCodeCoverage]
        internal IFile File
        {
            get { return _File ?? (_File = FileWrapper.Instance); }
            set { _File = value; }
        } private IFile _File;
        #endregion
    }
}