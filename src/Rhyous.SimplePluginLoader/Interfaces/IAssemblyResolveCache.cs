using System.Collections.Concurrent;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyResolveCache
    {
        ConcurrentDictionary<string, IAssembly> Cache { get; }
    }
}