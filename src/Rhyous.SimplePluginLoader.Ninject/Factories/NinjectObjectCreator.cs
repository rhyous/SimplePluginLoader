using Ninject;
using Ninject.Extensions.ChildKernel;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An Ninject object creator. It used registration and resolve to create the objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NinjectObjectCreator<T> : IObjectCreator<T>
    {
        private readonly IKernel _Scope;

        /// <summary>
        /// NinjectObjectCreator constructor
        /// </summary>
        /// <param name="lifetimeScope">An Ninject ComponentContenxt object.</param>
        public NinjectObjectCreator(IKernel lifetimeScope)
        {
            _Scope = lifetimeScope;
        }

        /// <summary>
        /// Create an instance of the given type using Ninject with 
        /// Just-in-Time (JIT) regisration and resolving.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns></returns>
        public T Create(Type type)
        {
            var scope = new ChildKernel(_Scope);
            var pluginScope = new ChildKernel(scope);
            Type typeToLoad = RegisterTypeMethod(scope, type, typeof(T));
            if (typeToLoad == null || typeToLoad.IsGenericTypeDefinition)
                return default;
            return (T)pluginScope.Get(typeToLoad);
        }

        /// <summary>This wrapper aroudn the extension method is used to add ease of Unit Testing</summary>
        internal Func<IKernel, Type, Type, Type> RegisterTypeMethod = KernelExtensions.RegisterType;
    }
}