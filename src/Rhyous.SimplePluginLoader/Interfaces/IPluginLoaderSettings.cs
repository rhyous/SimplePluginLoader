using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IPluginLoaderSettings
    {
        /// <summary>
        /// Your Company name. Set this to your company.
        /// </summary>
        string Company { get; }

        /// <summary>
        /// Your Application name
        /// </summary>
        string AppName { get; }

        /// <summary>
        /// The name of the plugin folder when adding it to a relative path.
        /// </summary>
        string PluginFolder { get; }

        /// <summary>
        /// The directory to the plugin folder
        /// </summary>
        string DefaultPluginDirectory { get; }

        /// <summary>
        /// Whether to throw an exception or not if a plugin fails to load.
        /// </summary>
        bool ThrowExceptionsOnLoad { get; }

        /// <summary>
        /// Whether to throw an exception or not if a RuntimePluginLoader fails to load any plugins.
        /// </summary>
        bool ThrowExceptionIfNoPluginFound { get; }

        /// <summary>
        /// Whether to proactively load dependent C# dlls.
        /// This will not load dependent C++ or other p-invokes or similar dlls.
        /// </summary>
        bool LoadDependenciesProactively { get; }

        /// <summary>
        /// The location of dependencies shared by multiple plugins.
        /// If two plugins share the same assembly, it can be place here
        /// so two copies don't have to be deployed.
        /// </summary>
        IEnumerable<string> SharedPaths { get; }
    }
}