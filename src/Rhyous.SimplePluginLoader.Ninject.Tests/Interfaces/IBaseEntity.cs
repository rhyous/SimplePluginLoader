namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public interface IBaseEntity<TId>
    {
        TId Id { get; set; }
    }
}
