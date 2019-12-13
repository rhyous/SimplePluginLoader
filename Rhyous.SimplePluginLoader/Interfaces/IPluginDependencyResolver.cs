using System;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    interface IPluginDependencyResolver
    {
        Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args);
    }
}