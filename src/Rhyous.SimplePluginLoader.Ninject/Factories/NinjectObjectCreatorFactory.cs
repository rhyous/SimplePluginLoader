using Ninject;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for creating NinjectObjectCreators
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    public class NinjectObjectCreatorFactory<T> : IObjectCreatorFactory<T> 
    {
        private readonly IKernel _ComponentContext;

        /// <summary>
        /// NinjectObjectCreatorFactory constructor
        /// </summary>
        /// <param name="lifetimeScope">An Ninject LifetimeScope object.</param>
        public NinjectObjectCreatorFactory(IKernel lifetimeScope)
        {
            _ComponentContext = lifetimeScope;
        }

        /// <summary>
        /// This uses the Ninject LifetimeScope to resolve (i.e. instantiate) a 
        /// concrete instance of IObjectCreator{T}.
        /// </summary>
        /// <returns></returns>
        public IObjectCreator<T> Create()
        {
            return _ComponentContext.Get<IObjectCreator<T>>();
        }
    }
}