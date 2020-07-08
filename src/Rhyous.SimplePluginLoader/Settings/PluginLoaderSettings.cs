using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class PluginLoaderSettings : IPluginLoaderSettings
    {
        internal const string PluginSharedBinPathsKey = "PluginSharedBinPaths";
        internal const string PluginDirectoryKey = "PluginDirectory";
        internal const string ThrowExceptionsOnLoadKey = "ThrowExceptionsOnLoad";
        internal const string LoadDependenciesProactivelyKey = "LoadDependenciesProactively";

        public static PluginLoaderSettings Default = _Default ?? (_Default = new PluginLoaderSettings(new AppSettings()));
        private static PluginLoaderSettings _Default;

        private readonly IAppSettings _AppSettings;

        public PluginLoaderSettings(IAppSettings appSettings)
        {
            _AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            DefaultPluginDirectory = _AppSettings.Settings.Get(PluginDirectoryKey);
            ThrowExceptionsOnLoad = _AppSettings.Settings.Get(ThrowExceptionsOnLoadKey).ToBool(false);
            LoadDependenciesProactively = _AppSettings.Settings.Get(LoadDependenciesProactivelyKey).ToBool(false);
            SharedPaths = GetSharedBinPaths();
        }

        #region Public Settings
        public virtual string Company => "Rhyous";
        public virtual string AppName => "App1";
        public virtual string PluginFolder => "Plugins";

        public virtual bool ThrowExceptionsOnLoad { get; }

        public virtual bool ThrowExceptionIfNoPluginFound { get; }

        public virtual string DefaultPluginDirectory { get; }

        public virtual IEnumerable<string> SharedPaths { get; }

        public virtual bool LoadDependenciesProactively { get; }

        #endregion

        internal List<string> GetSharedBinPaths()
        {
            var semicolonSeparatedValues = _AppSettings.Settings.Get(PluginSharedBinPathsKey);
            if (string.IsNullOrWhiteSpace(semicolonSeparatedValues))
                return null;

            var sharedPaths = new List<string>();
            var splitPaths = semicolonSeparatedValues.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)?
                                          .Select(s => s.Trim())
                                          .Where(s => !string.IsNullOrWhiteSpace(s))
                                          .ToList();
            sharedPaths.AddRange(splitPaths);
            return sharedPaths;
        }
    }
}
