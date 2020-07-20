using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface ITypeLoader<T>
    {
        List<Type> Load(IAssembly assembly);
    }
}