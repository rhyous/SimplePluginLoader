using System;
using System.Diagnostics.CodeAnalysis;

namespace Rhyous.SimplePluginLoader.Attributes
{
    [ExcludeFromCodeCoverage]
    public class PluginAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
