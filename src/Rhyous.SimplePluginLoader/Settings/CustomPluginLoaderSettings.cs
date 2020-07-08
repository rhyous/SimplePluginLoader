using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// This is used to create completely custom settings
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CustomPluginLoaderSettings : IPluginLoaderSettings
    {
        public string Company { get; set; }
        public string AppName { get; set; }
        public string PluginFolder { get; set; }
        public string DefaultPluginDirectory { get; set; }
        public bool ThrowExceptionsOnLoad { get; set; }
        public bool ThrowExceptionIfNoPluginFound { get; }
        public bool LoadDependenciesProactively { get; set; }
        public IEnumerable<string> SharedPaths { get; set; }
    }
}