namespace Rhyous.SimplePluginLoader
{
    public interface IPluginFinder<T>
    {
        T FindPlugin(string pluginName, string dir, IPluginObjectCreator<T> pluginObjectCreator = null);
    }
}