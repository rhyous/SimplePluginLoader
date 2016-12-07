using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyDictionary
    {
        Dictionary<string, Assembly> Assemblies { get; }
    }
}