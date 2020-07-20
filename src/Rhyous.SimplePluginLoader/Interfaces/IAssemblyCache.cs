using System.Collections.Concurrent;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyCache
    {
        ConcurrentDictionary<string, IAssembly> Assemblies { get; }
        IAssembly Add(string dll, string assemblyVersion, IAssembly assembly);
        IAssembly FindAlreadyLoadedAssembly(string dll, string version = null);
    }
}