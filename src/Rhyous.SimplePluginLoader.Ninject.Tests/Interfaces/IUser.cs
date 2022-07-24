namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public interface IUser : IBaseEntity<int>
    {
        string Username { get; set; }
    }
}
