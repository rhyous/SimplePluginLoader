using Ninject;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An interface defining how to implement a DependencyRegistrar,
    /// which is used for registering dependencies of an object with
    /// Ninject.
    /// </summary>
    /// <typeparam name="TContainerType"></typeparam>
    public interface IDependencyRegistrar<TContainerType>
    {
        /// <summary>
        /// The register method, which is used for registering dependencies
        /// of an object with Ninject.
        /// </summary>
        /// <param name="kernel"></param>
        void Register(TContainerType kernel);
    }
}
