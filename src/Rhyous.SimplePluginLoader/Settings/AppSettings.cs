using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// If you want only one logger, use this static singleton.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AppSettings : IAppSettings
    {
        public NameValueCollection Settings
        {
            get { return _Settings ?? (_Settings = ConfigurationManager.AppSettings); }
        } private NameValueCollection _Settings;
    }
}