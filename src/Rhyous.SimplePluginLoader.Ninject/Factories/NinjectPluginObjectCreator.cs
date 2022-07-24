using Ninject;
using Ninject.Extensions.ChildKernel;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NinjectPluginObjectCreator<T> : IPluginObjectCreator<T>
    {
        private readonly IKernel _Scope;
        private readonly IPluginDependencyRegistrar _PluginDependecyRegistrar;

        /// <summary>
        /// The NinjectPluginObjectCreator constructor
        /// </summary>
        /// <param name="componentContext">An Ninject ComponentContext object.</param>
        /// <param name="pluginDependecyRegistrar">An instance of a IPluginDependencyRegistrar 
        /// that can be used to dynamically register objects in a plugin just-in-time or 
        /// right before the lugin is loaded.</param>
        public NinjectPluginObjectCreator(IKernel componentContext,
                                          IPluginDependencyRegistrar pluginDependecyRegistrar)
            : base()
        {
            _Scope = componentContext;
            _PluginDependecyRegistrar = pluginDependecyRegistrar;
        }

        /// <summary>
        /// Create an instance of the given type using Ninject with 
        /// Just-in-Time (JIT) regisration and resolving.
        /// </summary>
        /// <param name="plugin">The plugin</param>
        /// <param name="type">The type to create.</param>
        /// <returns>An instantiated instance of T.</returns>
        public T Create(IPlugin<T> plugin, Type type)
        {
            var pluginScope = new ChildKernel(_Scope);
            Type typeToLoad = RegisterTypeMethod(pluginScope, type, typeof(T));
            _PluginDependecyRegistrar.RegisterPluginDependencies(pluginScope, plugin, type);
            if (typeToLoad == null || typeToLoad.IsGenericTypeDefinition)
                return default;
            return (T)pluginScope.Get(typeToLoad);
        }

        /// <summary>This wrapper aroudn the extension method is used to add ease of Unit Testing</summary>
        internal Func<IKernel, Type, Type, Type> RegisterTypeMethod = KernelExtensions.RegisterType;
    }
}