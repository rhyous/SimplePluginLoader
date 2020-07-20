using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class ObjectCreator<T> : IObjectCreator<T>
    {
        public virtual T Create(Type type = null)
        {
            type = type ?? typeof(T);
            var obj = (type.IsGenericType && type.IsGenericTypeDefinition)
                      ? CreateGenericType(type)
                      : Activator.CreateInstance(type);
            return (T)obj;
        }

        private static T CreateGenericType(Type genericType)
        {
            var genericArgs = typeof(T).GetGenericArguments(); // T can never be a Generic Type definition
            Type constructedType = genericType.MakeGenericType(genericArgs);
            var methodInfo = typeof(Activator).GetGenericMethod("CreateInstance");
            var genMethod = methodInfo.MakeGenericMethod(constructedType);
            var obj = genMethod.Invoke(null, null);
            return (T)obj;
        }
    }
}