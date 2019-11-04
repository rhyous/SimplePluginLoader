using Autofac;
using Rhyous.SimplePluginLoader.DependencyInjection;

namespace Tool.Plugin1
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<LocalizerModule>();
        }
    }
} 