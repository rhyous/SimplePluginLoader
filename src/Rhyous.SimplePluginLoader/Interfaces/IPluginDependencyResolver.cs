using System;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IPluginDependencyResolver : IDisposable
    {
        Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args);
        IPlugin Plugin { get; set; }

        void AddDependencyResolver();

        void RemoveDependencyResolver();
    }
}