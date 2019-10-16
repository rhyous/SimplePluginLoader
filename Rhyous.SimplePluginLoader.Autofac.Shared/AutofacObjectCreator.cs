using Autofac;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class AutofacObjectCreator<T> : Module, IObjectCreator<T>
        where T : class
    {
        private readonly IComponentContext _ComponentContext;

        public AutofacObjectCreator(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

        public Plugin<T> Plugin { get; set; }

        public T Create(Type type)
        {
            var scope = _ComponentContext.Resolve<ILifetimeScope>();
            var dependencyRegistrarPluginLoader = scope.Resolve<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            var registrarPlugin = dependencyRegistrarPluginLoader.LoadPlugin(Plugin.FullPath);
            using (var pluginScope = scope.BeginLifetimeScope((builder) =>
            {
                foreach (var dependencyRegistrar in registrarPlugin.PluginObjects)
                {
                    if (type.Assembly.FullName == dependencyRegistrar.GetType().Assembly.FullName)
                        dependencyRegistrar.Register(builder);
                }
                if (!type.IsGenericTypeDefinition)
                    builder.RegisterType(type);
                else
                    builder.RegisterGeneric(type.GetGenericTypeDefinition());
            }))
            {
                if (type.IsGenericTypeDefinition)
                    return null;
                return pluginScope.Resolve(type) as T;
            }
        }
    }
}