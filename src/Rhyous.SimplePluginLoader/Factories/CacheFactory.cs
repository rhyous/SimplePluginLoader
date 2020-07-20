using System;
using System.Collections.Concurrent;

namespace Rhyous.SimplePluginLoader
{
    public class CacheFactory<TKey, TValue> : ICacheFactory<TKey, TValue>
    {
        protected readonly ConcurrentDictionary<TKey, TValue> _Cache = new ConcurrentDictionary<TKey, TValue>();
        protected readonly IObjectCreator<TValue> _ObjectCreator;
        protected readonly IPluginLoaderLogger _Logger;

        public CacheFactory(IObjectCreator<TValue> objectCreator,
                            IPluginLoaderLogger logger)
        {
            _ObjectCreator = objectCreator;
            _Logger = logger;
        }

        public virtual TValue Create(TKey key, Type t)
        {
            if (key == null || t == null || !t.IsInstantiable())
                return default(TValue);
            if (!_Cache.TryGetValue(key, out TValue value))
            {
                try
                {
                    value = _ObjectCreator.Create(typeof(TValue));
                    _Cache.TryAdd(key, value);
                }
                catch (Exception e)
                {
                    _Logger.Log(e);
                    throw;
                }
            }
            return value;
        }
    }
}