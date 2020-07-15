using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for creating AutofacObjectCreators
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    public class AutofacObjectCreatorFactory<T> : IObjectCreatorFactory<T> 
    {
        private readonly IComponentContext _ComponentContext;

        /// <summary>
        /// AutofacObjectCreatorFactory constructor
        /// </summary>
        /// <param name="componentContext">An Autofac ComponentContext object.</param>
        public AutofacObjectCreatorFactory(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        /// <summary>
        /// This uses the Autofac ComponentContext to resolve (i.e. instantiate) a 
        /// concrete instance of IObjectCreator{T}.
        /// </summary>
        /// <returns></returns>
        public IObjectCreator<T> Create()
        {
            return _ComponentContext.Resolve<IObjectCreator<T>>();
        }
    }
}