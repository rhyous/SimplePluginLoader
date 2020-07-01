using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class AutofacPluginObjectCreatorFactory<T> : IPluginObjectCreatorFactory<T> 
    {
        private readonly IComponentContext _ComponentContext;

        public AutofacPluginObjectCreatorFactory(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        public IPluginObjectCreator<T> Create()
        {
            return _ComponentContext.Resolve<IPluginObjectCreator<T>>();
        }
    }
}