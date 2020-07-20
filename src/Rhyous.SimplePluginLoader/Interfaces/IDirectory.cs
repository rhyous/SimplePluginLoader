using System;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    internal interface IDirectory
    {
        bool Exists(string path);
        string[] GetFiles(string path, string searchPattern);
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    }
}