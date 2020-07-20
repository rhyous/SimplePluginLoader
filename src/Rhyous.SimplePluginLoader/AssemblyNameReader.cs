using System;
using System.Diagnostics.CodeAnalysis;
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

        #region Wrappers -  While this could come from the constructor, these are only used in Unit Tests, so we hide them internally
        [ExcludeFromCodeCoverage]
        internal IFile File
        {
            get { return _File ?? (_File = FileWrapper.Instance); }
            set { _File = value; }
        } private IFile _File;
        #endregion
    }
}