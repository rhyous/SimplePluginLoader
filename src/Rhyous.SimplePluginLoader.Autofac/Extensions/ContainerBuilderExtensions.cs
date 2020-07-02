using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public static class ContainerBuilderExtensions
    {
        public static Type RegisterType(this ContainerBuilder builder, Type regType, Type asType)
        {
            if (!regType.IsTypeOf(asType) && !regType.IsGenericTypeDefinitionOf(asType))
                throw new TypeMismatchException($"Type {regType} must be a type of {asType} or visa versa.");

            if (regType.IsInterface)
                return regType; // It must already be registered or resolve will fail.

            if (asType.IsGenericTypeDefinition && regType.IsGenericTypeDefinition)
            {
                builder.RegisterGeneric(regType.GetGenericTypeDefinition()).As(asType).IfNotRegistered(asType).SingleInstance();
                return asType;
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
                builder.RegisterType(regType).As(asType).IfNotRegistered(asType).SingleInstance();
                return asType;
            }

            builder.RegisterType(regType);
            return regType;
        }
    }
}