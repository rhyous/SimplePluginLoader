namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    public interface IOrganization : IBaseEntity<int>
    {
        string Name { get; set; }
    }
}
