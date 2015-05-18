using System.Collections.Generic;
using System.Reflection;

namespace SimplePluginLoader
{
    public interface ILoadInstancesOfType<T>
    {
        List<T> LoadInstances(Assembly assembly);
    }
}
