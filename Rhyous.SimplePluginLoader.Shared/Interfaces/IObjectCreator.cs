using System;

namespace Rhyous.SimplePluginLoader
{
    public interface IObjectCreator<T>
        where T : class
    {
        T Create(Type type);
    }
}
