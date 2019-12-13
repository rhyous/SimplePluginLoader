using System;
using System.Collections.Generic;
using System.Reflection;

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
        Assembly Assembly { get; set; }
        List<T> PluginObjects { get; set; }
        List<Type> PluginTypes { get; set; }
        ILoadInstancesOfType<T> Loader { get; set; }
    }
}
