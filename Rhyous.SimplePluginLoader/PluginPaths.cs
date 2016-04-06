using System;
using System.Collections.Generic;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    public class PluginPaths
    {
        public const string DefaultPluginDirectory = "Plugins";
        public const string DefaultDllSearchString = "*.dll";

        public PluginPaths(string appName)
        {
            AppName = appName;
        }

        public string AppName { get; set; }

        /// <summary>
        /// The name of the subdirectory where plugins are stored. The default is:
        /// Plugins
        /// </summary>
        public string PluginDirectoryName
        {
            get
            {
                return (string.IsNullOrWhiteSpace(_PluginDirectoryName))
                    ? (_PluginDirectoryName = DefaultPluginDirectory)
                    : _PluginDirectoryName;
            }
            set { _PluginDirectoryName = value; }
        } private string _PluginDirectoryName;

        public string UserProfilePlugins
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), AppName, PluginDirectoryName); }
        }

        public string ApplicationDataPlugins
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName, PluginDirectoryName); }
        }

        public string RelativePathPlugins
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginDirectoryName); }
        }

        public IEnumerable<string> GetDefaultPluginDirectories()
        {
            return new[] { PluginDirectoryName, UserProfilePlugins, ApplicationDataPlugins, RelativePathPlugins };
        }
    }
}
