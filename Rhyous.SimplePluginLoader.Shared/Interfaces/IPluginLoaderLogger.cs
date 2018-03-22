namespace Rhyous.SimplePluginLoader
{
    public interface IPluginLoaderLogger
    {
        void Write(PluginLoaderLogLevel level, string msg);
        void WriteLine(PluginLoaderLogLevel level, string msg);
    }
}
