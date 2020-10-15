using System.Collections.Concurrent;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// This caches the Assembly and version by the requested name as a string
    /// </summary>
    public class AssemblyResolveCache : IAssemblyResolveCache
    {
        public ConcurrentDictionary<string, IAssembly> Cache { get; } = new ConcurrentDictionary<string, IAssembly>();
    }
}