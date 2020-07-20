using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutofacPluginObjectCreator<T> : IPluginObjectCreator<T>
    {
        private readonly IComponentContext _ComponentContext;
        private readonly IPluginDependencyRegistrar _PluginDependecyRegistrar;

        /// <summary>
        /// The AutofacPluginObjectCreator constructor
        /// </summary>
        /// <param name="componentContext">An Autofac ComponentContext object.</param>
        /// <param name="pluginDependecyRegistrar">An instance of a IPluginDependencyRegistrar 
        /// that can be used to dynamically register objects in a plugin just-in-time or 
        /// right before the lugin is loaded.</param>
        public AutofacPluginObjectCreator(IComponentContext componentContext,
                                          IPluginDependencyRegistrar pluginDependecyRegistrar)
            : base()
        {
            _ComponentContext = componentContext;
            _PluginDependecyRegistrar = pluginDependecyRegistrar;
        }

        /// <summary>
        /// Create an instance of the given type using Autofac with 
        /// Just-in-Time (JIT) regisration and resolving.
        /// </summary>
        /// <param name="plugin">The plugin</param>
        /// <param name="type">The type to create.</param>
        /// <returns>An instantiated instance of T.</returns>
        public T Create(IPlugin<T> plugin, Type type)
        {
            var scope = _ComponentContext.Resolve<ILifetimeScope>();
            Type typeToLoad = null;

            using (var pluginScope = scope.BeginLifetimeScope((builder) =>
            {
                typeToLoad = RegisterTypeMethod(builder, type, typeof(T));
                _PluginDependecyRegistrar.RegisterPluginDependencies(builder, plugin, type);
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