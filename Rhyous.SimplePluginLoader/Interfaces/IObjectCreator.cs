using System;

namespace Rhyous.SimplePluginLoader
{
    public interface IObjectCreator<T>
        where T : class
    {
        IPlugin<T> Plugin { get; set; }
        T Create(Type type);
    }
}
