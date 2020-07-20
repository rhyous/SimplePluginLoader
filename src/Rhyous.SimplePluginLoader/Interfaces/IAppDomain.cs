using System;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// This wraps AppDomin. It only wrap the minimal methods needed by SimplePluginLoader.
    /// </summary>
    public interface IAppDomain
    {
        string BaseDirectory { get; }
        event ResolveEventHandler AssemblyResolve;
        int AssemblyResolveSubscriberCount { get; }
        IAssembly[] GetAssemblies();
        IAssembly Load(byte[] rawAssembly);

        #region Custom Methods

        IAssembly TryLoad(byte[] rawAssembly);
        IAssembly TryLoad(byte[] rawAssembly, byte[] rawSymbolStore);
        IAssembly TryLoad(string dll, string pdb);

        #endregion
    }
}
