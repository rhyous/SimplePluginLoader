using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for creating AutofacObjectCreators
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    public class AutofacObjectCreatorFactory<T> : IObjectCreatorFactory<T> 
    {
        private readonly ILifetimeScope _ComponentContext;

        /// <summary>
        /// AutofacObjectCreatorFactory constructor
        /// </summary>
        /// <param name="lifetimeScope">An Autofac LifetimeScope object.</param>
        public AutofacObjectCreatorFactory(ILifetimeScope lifetimeScope)
        {
            _ComponentContext = lifetimeScope;
        }

        /// <summary>
        /// This uses the Autofac LifetimeScope to resolve (i.e. instantiate) a 
        /// concrete instance of IObjectCreator{T}.
        /// </summary>
        /// <returns></returns>
        public IObjectCreator<T> Create()
        {
            return _ComponentContext.Resolve<IObjectCreator<T>>();
        }
    }
}