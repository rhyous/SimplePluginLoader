using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class ObjectCreator<T> : IObjectCreator<T>
        where T : class
    {
        public T Create(Type type)
        {
            if (type.IsGenericType)
            {
                var genericArgs = typeof(T).GetGenericArguments();
                if (genericArgs == null || !genericArgs.Any())
                    return null;
                return CreateGenericType(type, genericArgs);
            }
            var obj = Activator.CreateInstance(type);
            return obj as T;
        }

        private static T CreateGenericType(Type genericType, params Type[] genericParams)
        {
            Type constructedType = genericType.MakeGenericType(genericParams);
            var methodInfo = typeof(Activator).GetGenericMethod("CreateInstance");
            var genMethod = methodInfo.MakeGenericMethod(constructedType);
            var obj = genMethod.Invoke(null, null);
            return obj as T;
        }
    }
}
