using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IPlugin : IDisposable
    {
        string Name { get; }
        string Directory { get; set; }
        string File { get; set; }
        string FilePdb { get; }
        string FullPath { get; }
        string FullPathPdb { get; }
        IAssembly Assembly { get; }
        List<Type> PluginTypes { get; }
        IPluginDependencyResolver DependencyResolver { get; }
    }

    public interface IPlugin<T> : IPlugin
    {
        List<T> CreatePluginObjects(IPluginObjectCreator<T> pluginObjectCreator);
        T CreatePluginObject(Type t, IPluginObjectCreator<T> pluginObjectCreator);
    }
}
