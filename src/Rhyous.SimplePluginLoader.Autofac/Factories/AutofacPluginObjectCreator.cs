using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class AutofacPluginObjectCreator<T> : IPluginObjectCreator<T>
    {
        private readonly IComponentContext _ComponentContext;
        private readonly IPluginDependencyRegistrar _PluginDependecyRegistrar;

        public AutofacPluginObjectCreator(IComponentContext componentContext,
                                          IPluginDependencyRegistrar pluginDependecyRegistrar)
            : base()
        {
            _ComponentContext = componentContext;
            _PluginDependecyRegistrar = pluginDependecyRegistrar;
        }

        public IPlugin Plugin { get; set; }


        /// <summary>
        /// Create an instance of the given type using Autofac with 
        /// Just-in-Time (JIT) regisration and resolving.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns></returns>
        public T Create(Type type)
        {
            var scope = _ComponentContext.Resolve<ILifetimeScope>();
            Type typeToLoad = null;

            using (var pluginScope = scope.BeginLifetimeScope((builder) =>
            {
                typeToLoad = RegisterTypeMethod(builder, type, typeof(T));
                _PluginDependecyRegistrar.RegisterPluginDependencies(builder, Plugin, type);
            }))
            {
                if (typeToLoad == null || typeToLoad.IsGenericTypeDefinition)
                    return default(T);
                return (T)pluginScope.Resolve(typeToLoad);
            }
        }

        /// <summary>This wrapper aroudn the extension method is used to add ease of Unit Testing</summary>
        internal Func<ContainerBuilder, Type, Type, Type> RegisterTypeMethod = ContainerBuilderExtensions.RegisterType;
    }
}