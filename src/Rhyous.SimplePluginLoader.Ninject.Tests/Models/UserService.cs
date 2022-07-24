namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public interface IUserService : IService<User, IUser, int> { }
    public class UserService : Service<User, IUser, int>, IUserService
    {
        public override IUser Get(int id)
        {
            return new User();
        }
    }
}
