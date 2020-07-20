using System;

namespace Rhyous.SimplePluginLoader
{
    public interface ICacheFactory<TKey, TValue>
    {
        TValue Create(TKey key, Type t);
    }
}