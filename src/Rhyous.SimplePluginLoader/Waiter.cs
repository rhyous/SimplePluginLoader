using System.Collections.Concurrent;
using System.Threading;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// Sometimes the same action might be happening in the same object on multiple threads.
    /// This allows for using a key to mark a task in progress. Another task doing the same
    /// thing will simply wait up to 100 milliseconds for the primary task to complete.
    /// </summary>
    public class Waiter
        : IWaiter
    {
        const int MaxWaitTimeInMilliseconds = 100;
        private readonly IPluginLoaderLogger _Logger;

        public Waiter(IPluginLoaderLogger logger)
        {
            InProgress = new ConcurrentDictionary<string, bool>();
            _Logger = logger;
        }

        public ConcurrentDictionary<string, bool> InProgress { get; }

        public void Wait(string key)
        {
            if (InProgress.TryGetValue(key, out bool alreadyResolving) && alreadyResolving)
            {
                Wait2(key);
            }
            else
            {
                if (InProgress.TryAdd(key, true))
                {
                    _Logger.WriteLine(PluginLoaderLogLevel.Debug, $"Thread {Thread.CurrentThread.ManagedThreadId} is searching for {key}.");
                }
                else
                {
                    Wait2(key);
                }
            }
        }

        private void Wait2(string key)
        {
            _Logger.WriteLine(PluginLoaderLogLevel.Debug, $"Thread {Thread.CurrentThread.ManagedThreadId} is waiting {key}.");
            // wait for other other resolvers to finish
            int i = 0;
            while (InProgress[key] && i++ < MaxWaitTimeInMilliseconds)
            {
                Thread.Sleep(1);
            }
        }
    }
}