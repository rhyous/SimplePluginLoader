namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    public class UserService : Service<User, IUser, int>
    {
        public override IUser Get(int id)
        {
            return new User();
        }
    }
}
