namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    public interface IServiceLoader<T>
    {
        T Load();
    }
}
