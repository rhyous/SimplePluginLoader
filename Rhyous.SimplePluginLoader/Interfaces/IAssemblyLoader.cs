using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyBuilder
    {
        Assembly Load(string dll, string pdb);
    }
}
