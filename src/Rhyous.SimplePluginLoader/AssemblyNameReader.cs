using System;
using System.IO;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class AssemblyNameReader : IAssemblyNameReader
    {
        public AssemblyName GetAssemblyName(string dll)
        {
            if (!File.Exists(dll))
                return null;
            try { return AssemblyName.GetAssemblyName(dll); }
            catch (Exception) { return null; }
        }
    }
}