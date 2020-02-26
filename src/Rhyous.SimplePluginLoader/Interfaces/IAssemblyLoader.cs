using System;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyBuilder : IDisposable
    {
        IAssembly Load(string dll, string pdb);
        IAssembly TryLoad(string dll, string pdb);
        IAssembly TryLoad(string dll, string pdb, string version);
    }
}