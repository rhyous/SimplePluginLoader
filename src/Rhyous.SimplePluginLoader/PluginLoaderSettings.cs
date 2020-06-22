﻿using Rhyous.SimplePluginLoader.Extensions;
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
            _AppSettings = appSettings;
        }

        #region Public Settings
        public bool ThrowExceptionsOnLoad { get { return _ThrowExceptionsOnLoad ?? (_ThrowExceptionsOnLoad = _AppSettings.Settings.Get(ThrowExceptionsOnLoadKey).ToBool(false)).Value; } }
        private bool? _ThrowExceptionsOnLoad;

        public string DefaultPluginDirectory { get { return _DefaultPluginDirectory ?? (_DefaultPluginDirectory = _AppSettings.Settings.Get(PluginDirectoryKey)); } }
        private string _DefaultPluginDirectory;

        public IEnumerable<string> SharedPaths { get { return _SharedPaths ?? (_SharedPaths = GetSharedBinPaths()); } }
        private IEnumerable<string> _SharedPaths;

        public bool LoadDependenciesProactively { get { return _LoadDependenciesProactively ?? (_LoadDependenciesProactively = _AppSettings.Settings.Get(LoadDependenciesProactivelyKey).ToBool(false)).Value; } }
        private bool? _LoadDependenciesProactively;

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
