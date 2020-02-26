namespace Rhyous.SimplePluginLoader
{
    public interface IInstanceLoader<T> : ILoadFromAssembly<T, T>
        where T : class
    {
        IPlugin<T> Plugin { get; set; }
    }
}