using Ninject;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// This class registers dependencies of a plugin with Ninject.
    /// </summary>
    public class PluginDependencyRegistrar : IPluginDependencyRegistrar
    {
        private readonly IKernel _RootScope;

        /// <summary>
        /// The constructor for PluginDependencyRegistrar{T}
        /// </summary>
        /// <param name="rootScope"></param>
        public PluginDependencyRegistrar(IKernel rootScope)
        {
            _RootScope = rootScope;
        }

        /// <summary>
        /// This method provides Just-in-time (JIT) dependency registration for a plugin's dependencies.
        /// </summary>
        /// <param name="scope">The Ninject ContainerBuilder</param>
        /// <param name="plugin">The plugin that could contain an IDependencyRegistrar[ContainerBuilder] module..</param>
        /// <param name="type">The type to resolve.</param>
        public void RegisterPluginDependencies(IKernel scope, IPlugin plugin, Type type)
        {
            var dependencyRegistrarPluginLoader = _RootScope.Get<IPluginLoader<IDependencyRegistrar<IKernel>>>();
            IPlugin<IDependencyRegistrar<IKernel>> registrarPlugin = null;
            if (plugin != null && !string.IsNullOrWhiteSpace(plugin.FullPath))
                registrarPlugin = dependencyRegistrarPluginLoader.LoadPlugin(plugin.FullPath);
            if (registrarPlugin != null)
            {
                if (registrarPlugin.PluginTypes == null || !registrarPlugin.PluginTypes.Any())
                    return;
                var registrars = registrarPlugin.CreatePluginObjects(_RootScope.Get<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>());
                if (registrars == null)
                    return;
                foreach (var dependencyRegistrar in registrars)
                {
                    dependencyRegistrar.Register(scope);
                }
            }
        }
    }
}