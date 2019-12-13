using System;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyBuilder : IDisposable
    {
        Assembly Load(string dll, string pdb);
        Assembly TryLoad(string dll, string pdb);
        Assembly TryLoad(string dll, string pdb, string version);
    }
}