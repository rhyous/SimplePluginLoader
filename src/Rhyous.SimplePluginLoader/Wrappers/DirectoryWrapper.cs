using System;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    internal interface IDirectory
    {
        bool Exists(string path);
        string[] GetFiles(string path, string searchPattern);
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
        bool FileExists(string path);
        DateTime GetLastWriteTime(string path);
    }

    /// <summary>
    /// This wraps the minimal parts of System.IO.Directory needed by PluginLoader
    /// </summary>
    internal class DirectoryWrapper : IDirectory
    {
        #region Singleton

        private static readonly Lazy<DirectoryWrapper> Lazy = new Lazy<DirectoryWrapper>(() => new DirectoryWrapper());

        public static DirectoryWrapper Instance { get { return Lazy.Value; } }

        internal DirectoryWrapper()
        {
        }

        #endregion
        public bool Exists(string path) => Directory.Exists(path);

        public string[] GetFiles(string path, string searchPattern)
                        => Directory.GetFiles(path, searchPattern);
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
                        => Directory.GetFiles(path, searchPattern, searchOption);

        public bool FileExists(string path) => File.Exists(path);

        public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);
    }
}
