using Autofac;
using Rhyous.SimplePluginLoader.DependencyInjection;

namespace Tool.DependencyInjection
{
    public class CustomPluginObjectCreator<T> : AutofacPluginObjectCreator<T>
    {
        public CustomPluginObjectCreator(IComponentContext componentContext,
                                         IPluginDependencyRegistrar pluginDependecyRegistrar,
                                         IScopedSetting setting)
            : base(componentContext, pluginDependecyRegistrar)
        {
            Setting = setting;
        }

        public IScopedSetting Setting { get; }
    }
}
