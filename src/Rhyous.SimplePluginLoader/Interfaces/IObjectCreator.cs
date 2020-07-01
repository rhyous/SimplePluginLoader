using System;

namespace Rhyous.SimplePluginLoader
{
    public interface IObjectCreator<T>
    {
        T Create(Type type = null);
    }
}