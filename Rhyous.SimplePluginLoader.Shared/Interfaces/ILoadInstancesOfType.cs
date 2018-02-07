using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface ILoadInstancesOfType<T>
    {
        List<Type> GetPluginTypes(Assembly assembly);
        List<T> LoadInstances(Assembly assembly);
    }
}