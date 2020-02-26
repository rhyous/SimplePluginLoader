using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class AutofacObjectCreatorFactory<T> : IObjectCreatorFactory<T> 
        where T : class
    {
        private readonly IComponentContext _ComponentContext;

        public AutofacObjectCreatorFactory(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        public IObjectCreator<T> Create()
        {
            return _ComponentContext.Resolve<IObjectCreator<T>>();
        }
    }
}