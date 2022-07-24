namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public interface IServiceLoader<T>
    {
        T Load();
    }
}
