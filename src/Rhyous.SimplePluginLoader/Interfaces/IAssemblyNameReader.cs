using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyNameReader
    {
        AssemblyName GetAssemblyName(string dll);
    }
}