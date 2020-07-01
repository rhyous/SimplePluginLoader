using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyCache
    {
        IDictionary<string, IAssembly> Assemblies { get; }
    }
}