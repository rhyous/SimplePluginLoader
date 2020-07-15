﻿using Autofac;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An Autofac object creator. It used registration and resolve to create the objects.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutofacObjectCreator<T> : IObjectCreator<T>
    {
        private readonly IComponentContext _ComponentContext;

        /// <summary>
        /// AutofacObjectCreator constructor
        /// </summary>
        /// <param name="componentContext">An Autofac ComponentContenxt object.</param>
        public AutofacObjectCreator(IComponentContext componentContext)
        {
            _ComponentContext = componentContext;
        }

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