using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IPlugin<T> : IDisposable
    {
        string Name { get; }
        string Directory { get; set; }
        string File { get; set; }
        string FilePdb { get; }
        string FullPath { get; }
        string FullPathPdb { get; }
        IAssembly Assembly { get; set; }
        List<T> PluginObjects { get; set; }
        List<Type> PluginTypes { get; set; }
    }
}
