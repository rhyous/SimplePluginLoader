using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class PluginDependencyRegistrar : IPluginDependencyRegistrar
    {
        private readonly IComponentContext _ComponentContext;

        public PluginDependencyRegistrar(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        /// <summary>
        /// This method provides Just-in-time (JIT) dependency registration for a plugin's dependencies.
        /// </summary>
        /// <param name="builder">The Autofac ContainerBuilder</param>
        /// <param name="plugin">The plugin that could contain an IDependencyRegistrar[ContainerBuilder] module..</param>
        /// <param name="type">The type to resolve.</param>
        public void RegisterPluginDependencies(ContainerBuilder builder, IPlugin plugin, Type type)
        {
            var dependencyRegistrarPluginLoader = _ComponentContext.Resolve<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            IPlugin<IDependencyRegistrar<ContainerBuilder>> registrarPlugin = null;
            if (plugin != null && !string.IsNullOrWhiteSpace(plugin.FullPath))
                registrarPlugin = dependencyRegistrarPluginLoader.LoadPlugin(plugin.FullPath);
            if (registrarPlugin != null)
            {
                if (registrarPlugin.PluginTypes == null || !registrarPlugin.PluginTypes.Any())
                    return;
                var registrars = registrarPlugin.CreatePluginObjects();
                if (registrars == null)
                    return;
                foreach (var dependencyRegistrar in registrars)
                {
                    dependencyRegistrar.Register(builder);
                }
            }
        }
    }
}