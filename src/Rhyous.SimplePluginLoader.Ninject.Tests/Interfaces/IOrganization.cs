namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public interface IOrganization : IBaseEntity<int>
    {
        string Name { get; set; }
    }
}
