using System;

namespace Rhyous.SimplePluginLoader
{
    public interface IPluginObjectCreator<T>
    {
        T Create(IPlugin<T> plugin, Type type = null);
    }
}