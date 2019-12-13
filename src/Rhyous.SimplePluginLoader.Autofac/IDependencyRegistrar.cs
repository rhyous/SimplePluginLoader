namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public interface IDependencyRegistrar<ContainerType>
    {
        void Register(ContainerType containerBuilder);
    }
}
