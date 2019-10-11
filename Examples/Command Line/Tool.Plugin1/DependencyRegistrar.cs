using Autofac;
using Example.Dependency;
using Interfaces.Localization;
using Tool.DependencyInjection;

namespace Tool.Plugin1
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.Register(c => new Localizer())
                .As<ILocalizer>()
                .SingleInstance();
            containerBuilder.RegisterType<Rock>();
        }
    }
}
