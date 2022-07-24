using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// This class holds extension methods for the Autofac ContainerBuilder object.
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// This method figures out how to register a runtime type based on two passed in types.
        /// It is especially useful for registering a GenericTypeDefinition.
        /// </summary>
        /// <param name="builder">The ContainerBuilder that allows for Autofac object registration.</param>
        /// <param name="regType">The type to register. Must be a concrete type else it must already be registered and this method will do noth.</param>
        /// <param name="asType">The plugin type.</param>
        /// <returns>The type to resolve. This could be the regType, the asType, or a custom built generic type.</returns>
        public static Type RegisterType(this ContainerBuilder builder, Type regType, Type asType)
        {
            if (!regType.IsTypeOf(asType) && !regType.IsGenericTypeDefinitionOf(asType))
                throw new TypeMismatchException($"Type {regType} must be a type of {asType} or visa versa.");

            if (regType.IsInterface)
                return regType; // It must already be registered or resolve will fail.

            if (asType.IsGenericTypeDefinition && regType.IsGenericTypeDefinition)
            {
                builder.RegisterGeneric(regType.GetGenericTypeDefinition()).As(asType).InstancePerLifetimeScope();
                return null;
            }

            if (asType.IsGenericTypeDefinition && !regType.IsGenericTypeDefinition)
            {
                builder.RegisterType(regType);
                return regType;
            }

            if (asType.IsGenericType && regType.IsGenericTypeDefinition)
            {
                var genericArgs = asType.GetGenericArguments();
                if (genericArgs == null || !genericArgs.Any())
                    return null; // this may not be reachable code
                var typeToLoad = regType.MakeGenericType(genericArgs);
                builder.RegisterType(typeToLoad);
                return typeToLoad;
            }

            if (asType.IsGenericType && !regType.IsGenericTypeDefinition)
            {
                builder.RegisterType(regType).As(regType).InstancePerLifetimeScope();
                builder.RegisterType(regType).As(asType).InstancePerLifetimeScope();
                return regType;
            }

            builder.RegisterType(regType);
            return regType;
        }
    }
}