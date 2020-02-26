using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface ILoadFromAssembly<TPlugin, TResult>
        where TPlugin : class
        where TResult : class
    {
        List<TResult> Load(IAssembly assembly);
    }
}