using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for creating AutofacPluginObjectCreator{T} instances.
    /// </summary>
    /// <typeparam name="T">The type used in an IPluginObjectCreator{T} object.</typeparam>
    public class AutofacPluginObjectCreatorFactory<T> : IPluginObjectCreatorFactory<T> 
    {
        private readonly IComponentContext _ComponentContext;

        /// <summary>
        /// The AutofacPluginObjectCreatorFactory constructor.
        /// </summary>
        /// <param name="componentContext">An Autofac ComponentContext object.</param>
        public AutofacPluginObjectCreatorFactory(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        /// <summary>
        /// Create an instance of IPluginObjectCreator{T} using the Autofac 
        /// ComponentContext to resolve.
        /// </summary>
        /// <returns>An instantiated instance of IPluginObjectCreator{T}.</returns>
        public IPluginObjectCreator<T> Create()
        {
            return _ComponentContext.Resolve<IPluginObjectCreator<T>>();
        }
    }
}