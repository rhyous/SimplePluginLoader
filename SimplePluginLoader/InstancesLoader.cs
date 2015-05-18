using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePluginLoader
{
    public class InstancesLoader<T> : ILoadInstancesOfType<T> where T : class
    {
        public List<T> LoadInstances(Assembly assembly)
        {
            return LoadTypes(assembly);
        }

        public List<T> LoadTypes(Assembly assembly)
        {
            var listOfT = new List<T>();
            if (assembly != null)
            {
                Type[] objTypes = assembly.GetTypes();
                IEnumerable<Type> typesToLoad = GetTypesToLoad(objTypes);
                if (typesToLoad == null)
                    return null;
                listOfT.AddRange(typesToLoad.Select(Activator.CreateInstance).OfType<T>());
            }
            return listOfT;
        }

        private static IEnumerable<Type> GetTypesToLoad(Type[] objTypes)
        {
            IEnumerable<Type> typesToLoad = null;
            if (typeof(T).IsClass)
            {
                typesToLoad = objTypes.Where(objType => objType.IsSameOrSubclassAs(typeof(T)));
            }
            if (typeof(T).IsInterface)
            {
                typesToLoad = objTypes.Where(objType => objType.GetInterfaces().Contains(typeof(T)));
            }
            return typesToLoad;
        }
    }
}
