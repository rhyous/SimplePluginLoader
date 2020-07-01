using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for creating AutofacObjectCreators
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutofacObjectCreatorFactory<T> : IObjectCreatorFactory<T> 
    {
        private readonly IComponentContext _ComponentContext;

        public AutofacObjectCreatorFactory(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        public IObjectCreator<T> Create()
        {
            return _ComponentContext.Resolve<IPluginObjectCreator<T>>();
        }
    }
}