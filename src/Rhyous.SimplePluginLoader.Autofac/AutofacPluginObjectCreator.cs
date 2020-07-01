using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class AutofacPluginObjectCreator<T> : IPluginObjectCreator<T>
    {
        private readonly IComponentContext _ComponentContext;

        public AutofacPluginObjectCreator(IComponentContext componentContext)
            : base()
        {
            _ComponentContext = componentContext ?? throw new ArgumentNullException(nameof(componentContext));
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
                typeToLoad = builder.RegisterType(type, typeof(T));
                RegisterPluginDependencies(builder, type);
            }))
            {
                if (typeToLoad == null || typeToLoad.IsGenericTypeDefinition)
                    return default(T);
                return (T)pluginScope.Resolve(typeToLoad);
            }
        }

        /// <summary>
        /// This method provides Just-in-time (JIT) dependency registration for a plugin's dependencies.
        /// </summary>
        /// <param name="type">The type to </param>
        /// <param name="builder">The Autofac ContainerBuidler</param>
        internal void RegisterPluginDependencies(ContainerBuilder builder, Type type)
        {
            var dependencyRegistrarPluginLoader = _ComponentContext.Resolve<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            IPlugin<IDependencyRegistrar<ContainerBuilder>> registrarPlugin = null;
            if (Plugin != null && !string.IsNullOrWhiteSpace(Plugin.FullPath))
                registrarPlugin = dependencyRegistrarPluginLoader.LoadPlugin(Plugin.FullPath);
            if (registrarPlugin != null)
            {
                if (registrarPlugin.PluginTypes == null || !registrarPlugin.PluginTypes.Any())
                    return;
                var registrars = registrarPlugin.CreatePluginObjects();
                foreach (var dependencyRegistrar in registrars)
                {
                    if (type.Assembly.FullName == dependencyRegistrar.GetType().Assembly.FullName)
                        dependencyRegistrar.Register(builder);
                }
            }
        }
    }
}