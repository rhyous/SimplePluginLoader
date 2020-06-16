using System.Collections.Specialized;

namespace Rhyous.SimplePluginLoader
{
    public interface IAppSettings
    {
        NameValueCollection  Settings { get; }
    }
}