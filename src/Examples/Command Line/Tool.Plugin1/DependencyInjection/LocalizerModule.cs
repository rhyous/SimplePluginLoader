using Autofac;
using Example.Dependency;
using Interfaces.Localization;

namespace Tool.Plugin1
{
    public class LocalizerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new Localizer())
                   .As<ILocalizer>()
                   .SingleInstance();
            builder.RegisterType<Rock>();
        }
    }
} 