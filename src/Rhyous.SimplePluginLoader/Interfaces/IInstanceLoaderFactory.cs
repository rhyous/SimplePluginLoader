namespace Rhyous.SimplePluginLoader
{
    public interface IInstanceLoaderFactory<T>
        where T : class
    {
        IInstanceLoader<T> Create();
    }
}