using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class AutofacObjectCreator<T> : Module, IObjectCreator<T>
        where T : class
    {
        private readonly IComponentContext _ComponentContext;

        public AutofacObjectCreator(IComponentContext componentContext)
        {
            _ComponentContext = componentContext ?? throw new ArgumentNullException(nameof(componentContext));
        }

        public IPlugin<T> Plugin { get; set; }

        internal Type TypeToLoad { get; set; }

        /// <summary>
        /// Create an instance of the given type using Autofac with 
        /// Just-in-Time (JIT) regisration and resolving.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns></returns>
        public T Create(Type type)
        {
            var scope = _ComponentContext.Resolve<ILifetimeScope>();
            using (var pluginScope = scope.BeginLifetimeScope((builder) =>
            {
                RegisterPluginDependencies(type, builder);
                RegisterPluginType(type, builder);
            }))
            {
                if (TypeToLoad.IsGenericTypeDefinition)
                    return null;
                return pluginScope.Resolve(TypeToLoad) as T;
            }
        }

        /// <summary>
        /// This method provides Just-in-time (JIT) dependency registration for a plugin's dependencies.
        /// </summary>
        /// <param name="type">The type to </param>
        /// <param name="builder">The Autofac ContainerBuidler</param>
        internal void RegisterPluginDependencies(Type type, ContainerBuilder builder)
        {
            var dependencyRegistrarPluginLoader = _ComponentContext.Resolve<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            IPlugin<IDependencyRegistrar<ContainerBuilder>> registrarPlugin = null;
            if (Plugin != null && !string.IsNullOrWhiteSpace(Plugin.FullPath))
                registrarPlugin = dependencyRegistrarPluginLoader.LoadPlugin(Plugin.FullPath);
            if (registrarPlugin != null)
            {
                foreach (var dependencyRegistrar in registrarPlugin.PluginObjects)
                {
                    if (type.Assembly.FullName == dependencyRegistrar.GetType().Assembly.FullName)
                        dependencyRegistrar.Register(builder);
                }
            }
        }

        /// <summary>
        /// This method provides Just-in-time (JIT) dependency registration for the plugin itself.
        /// </summary>
        /// <param name="type">The type to </param>
        /// <param name="builder">The Autofac ContainerBuidler</param>
        internal void RegisterPluginType(Type type, ContainerBuilder builder)
        {
            if (type.IsInterface)
            {
                TypeToLoad = type;
                return; // It must already be registered or resolve will fail.
            }
            if (typeof(T).IsGenericTypeDefinition)
            {
                if (type.IsGenericTypeDefinition)
                {
                    builder.RegisterGeneric(type.GetGenericTypeDefinition()).As(typeof(T)).IfNotRegistered(typeof(T)).SingleInstance();
                    TypeToLoad = typeof(T);
                }
                else
                {
                    var genericArgs = typeof(T).GetGenericArguments();
                    if (genericArgs == null || !genericArgs.Any())
                        return;
                    TypeToLoad = typeof(T).MakeGenericType(genericArgs);
                    builder.RegisterType(type).As(TypeToLoad);
                }
            }
            else if (typeof(T).IsGenericType)
            {
                if (type.IsGenericTypeDefinition)
                {
                    var genericArgs = typeof(T).GetGenericArguments();
                    if (genericArgs == null || !genericArgs.Any())
                        return;
                    TypeToLoad = type.MakeGenericType(genericArgs);
                    builder.RegisterType(TypeToLoad);
                }
                else
                {
                    builder.RegisterType(type).As<T>().IfNotRegistered(typeof(T)).SingleInstance();
                    TypeToLoad = typeof(T);
                }
            }
            else
            {
                builder.RegisterType(type);
                TypeToLoad = type;
            }
        }
    }
}