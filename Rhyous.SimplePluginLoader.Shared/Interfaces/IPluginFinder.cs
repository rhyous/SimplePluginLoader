namespace Rhyous.SimplePluginLoader
{
    public interface IPluginFinder<T> where T : class
    {
        Plugin<T> FoundPlugin { get; set; }
        T FoundPluginObject { get; set; }

        T FindPlugin(string pluginName, string dir);
    }
}