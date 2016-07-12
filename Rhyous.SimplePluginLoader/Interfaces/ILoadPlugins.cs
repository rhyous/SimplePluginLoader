// See License at the end of the file

namespace Rhyous.SimplePluginLoader
{
    interface ILoadPlugins<T> where T : class
    {
        PluginCollection<T> LoadPlugins();
    }
}

#region Fork and Contribute License
