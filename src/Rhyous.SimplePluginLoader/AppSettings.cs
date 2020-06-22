using System.Collections.Specialized;
using System.Configuration;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// If you want only one logger, use this static singleton.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        public NameValueCollection Settings
        {
            get { return _Settings ?? (_Settings = ConfigurationManager.AppSettings); }
            set { _Settings = value; }
        } private NameValueCollection _Settings;
    }
}