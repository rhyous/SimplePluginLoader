using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyBuilder
    {
        Assembly Build(string dll, string pdb);
    }
}
