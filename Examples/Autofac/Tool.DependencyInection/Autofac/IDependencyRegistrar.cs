namespace Tool.DependencyInjection
{
    public interface IDependencyRegistrar<ContainerType>
    {
        void Register(ContainerType containerBuilder);
    }
}
