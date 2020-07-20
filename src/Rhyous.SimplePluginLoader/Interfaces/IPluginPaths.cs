using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader
{
    public interface IPluginPaths
    {
        IEnumerable<string> Paths { get; }
    }
}
