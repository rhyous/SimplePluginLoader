using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{

    public class User : IUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }
}
