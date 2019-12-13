using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyDictionary
    {
        IDictionary<string, Assembly> Assemblies { get; }
    }
}