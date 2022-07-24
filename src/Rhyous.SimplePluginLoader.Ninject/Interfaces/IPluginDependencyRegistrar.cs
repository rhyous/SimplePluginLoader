using Ninject;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An interface defining how to implement a DependencyRegistrar,
    /// which is used for registering dependencies of a plugin object 
    /// with Ninject.
    /// </summary>
    public interface IPluginDependencyRegistrar
    {
        /// <summary>
        /// The register method, which is used for registering dependencies
        /// of a plugin object with Ninject.
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="plugin"></param>
        /// <param name="type"></param>
        void RegisterPluginDependencies(IKernel kernel, IPlugin plugin, Type type);
    }
}