using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// This wraps the minimal parts of System.IO.Directory needed by PluginLoader
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class DirectoryWrapper : IDirectory
    {
        #region Singleton
        private static readonly Lazy<DirectoryWrapper> Lazy = new Lazy<DirectoryWrapper>(() => new DirectoryWrapper());
        public static DirectoryWrapper Instance { get { return Lazy.Value; } }
        internal DirectoryWrapper() { }
        #endregion

        public bool Exists(string path) => Directory.Exists(path);

        public string[] GetFiles(string path, string searchPattern)
                        => Directory.GetFiles(path, searchPattern);

        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
                        => Directory.GetFiles(path, searchPattern, searchOption);

        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}
