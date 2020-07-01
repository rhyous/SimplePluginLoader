namespace Rhyous.SimplePluginLoader
{
    public interface IPluginFinder<T> where T : class
    {
        T FindPlugin(string pluginName, string dir);
    }
}