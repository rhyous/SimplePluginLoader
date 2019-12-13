using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    public class SharedBinPathManager
    {
        private const string PluginSharedBinPaths = "PluginSharedBinPaths";

        public IEnumerable<string> SharedPaths { get { return GetSharedBinPaths(); } }

        public string SettingValue { get { return SharePathProviderMethod(); } }

        internal List<string> GetSharedBinPaths()
        {
            var sharedPaths = new List<string>();
            if (!string.IsNullOrWhiteSpace(SettingValue))
            {
                var splitPaths = SettingValue?.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)?
                                              .Select(s => s.Trim())
                                              .Where(s => !string.IsNullOrWhiteSpace(s))
                                              .ToList();
                sharedPaths.AddRange(splitPaths);
            }
            return sharedPaths;
        }

        internal Func<string> SharePathProviderMethod
        {
            get { return _SharePathProviderMethod ?? (_SharePathProviderMethod = () => { return ConfigurationManager.AppSettings.Get(PluginSharedBinPaths); }); }
            set { _SharePathProviderMethod = value; }
        } private Func<string> _SharePathProviderMethod;
    }
}
