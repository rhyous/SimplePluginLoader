using Autofac;
using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public static class ContainerBuilderExtensions
    { 
        /// <summary>
        /// This method provides Just-in-time (JIT) dependency registration for the plugin itself.
        /// </summary>
        /// <param name="type">The type to </param>
        /// <param name="builder">The Autofac ContainerBuidler</param>
        public static Type RegisterType(this ContainerBuilder builder, Type type, Type type2)
        {
            Type typeToLoad;
            if (type.IsInterface)
            {
                return type; // It must already be registered or resolve will fail.
            }
            if (type2.IsGenericTypeDefinition)
            {
                if (type.IsGenericTypeDefinition)
                {
                    builder.RegisterGeneric(type.GetGenericTypeDefinition()).As(type2).IfNotRegistered(type2).SingleInstance();
                    return type2;
                }
                else
                {
                    var genericArgs = type2.GetGenericArguments();
                    if (genericArgs == null || !genericArgs.Any())
                        return null;
                    typeToLoad = type2.MakeGenericType(genericArgs);
                    builder.RegisterType(type).As(typeToLoad);
                    return typeToLoad;
                }
            }
            else if (type2.IsGenericType)
            {
                if (type.IsGenericTypeDefinition)
                {
                    var genericArgs = type2.GetGenericArguments();
                    if (genericArgs == null || !genericArgs.Any())
                        return null;
                    typeToLoad = type.MakeGenericType(genericArgs);
                    builder.RegisterType(typeToLoad);
                    return typeToLoad;
                }
                else
                {
                    builder.RegisterType(type).As(type2).IfNotRegistered(type2).SingleInstance();
                    return type2;
                }
            }
            else
            {
                builder.RegisterType(type);
                return type;
            }
        }
    }
}