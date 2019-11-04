namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    public interface IUser : IBaseEntity<int>
    {
        string Username { get; set; }
    }
}
