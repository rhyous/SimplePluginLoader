namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    public interface IBaseEntity<TId>
    {
        TId Id { get; set; }
    }
}
