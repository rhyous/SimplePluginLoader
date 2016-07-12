// See License at the end of the file

using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface ILoadInstancesOfType<T>
    {
        List<T> LoadInstances(Assembly assembly);
    }
}
