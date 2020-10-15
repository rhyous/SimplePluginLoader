using System.Collections.Concurrent;

namespace Rhyous.SimplePluginLoader
{
    public interface IWaiter
    {
        ConcurrentDictionary<string, bool> InProgress { get; }

        void Wait(string key);
    }
}