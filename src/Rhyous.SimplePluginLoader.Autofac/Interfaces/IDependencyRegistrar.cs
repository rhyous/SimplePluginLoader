namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An interface defining how to implement a DependencyRegistrar,
    /// which is used for registering dependencies of an object with
    /// Autofac.
    /// </summary>
    /// <typeparam name="TContainerType"></typeparam>
    public interface IDependencyRegistrar<TContainerType>
    {
        /// <summary>
        /// The register method, which is used for registering dependencies
        /// of an object with Autofac.
        /// </summary>
        /// <param name="containerBuilder"></param>
        void Register(TContainerType containerBuilder);
    }
}
