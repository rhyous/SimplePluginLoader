using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IPluginLoaderSettings
    {
        string DefaultPluginDirectory { get; }
        bool ThrowExceptionsOnLoad { get; }
        bool LoadDependenciesProactively { get; }
        IEnumerable<string> SharedPaths { get; }
    }
}