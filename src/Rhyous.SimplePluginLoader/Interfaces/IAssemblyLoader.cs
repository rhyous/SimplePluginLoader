using System;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyLoader
    {
        IAssembly TryLoad(string dll, string pdb, string version = null);
    }
}