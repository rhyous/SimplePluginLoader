using Autofac;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An interface defining how to implement a DependencyRegistrar,
    /// which is used for registering dependencies of a plugin object 
    /// with Autofac.
    /// </summary>
    public interface IPluginDependencyRegistrar
    {
        /// <summary>
        /// The register method, which is used for registering dependencies
        /// of a plugin object with Autofac.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="plugin"></param>
        /// <param name="type"></param>
        void RegisterPluginDependencies(ContainerBuilder builder, IPlugin plugin, Type type);
    }
}