namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public interface IDependencyRegistrar<TContainerType>
    {
        void Register(TContainerType containerBuilder);
    }
}
