namespace Rhyous.SimplePluginLoader
{
    public interface IRuntimePluginLoaderFactory
    {
        IRuntimePluginLoader<T> Create<TRuntimePluginLoader, T>(params object[] dependencies)
            where TRuntimePluginLoader : IRuntimePluginLoader<T>
            where T : class;
    }
}
