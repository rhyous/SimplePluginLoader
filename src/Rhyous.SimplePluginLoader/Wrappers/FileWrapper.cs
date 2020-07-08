using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    [ExcludeFromCodeCoverage]
    internal class FileWrapper : IFile
    {
        #region Singleton
        private static readonly Lazy<FileWrapper> Lazy = new Lazy<FileWrapper>(() => new FileWrapper());
        public static IFile Instance { get { return Lazy.Value; } }
        internal FileWrapper() { }
        #endregion

        public bool Exists(string path) => File.Exists(path);

        public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

        public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
    }
}
