using System;

namespace Rhyous.SimplePluginLoader
{
    public interface ITypeLoader<T> : ILoadFromAssembly<T, Type>
        where T : class
    {
    }
}