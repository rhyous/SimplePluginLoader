using System;

namespace Rhyous.SimplePluginLoader
{
    internal interface IFile
    {
        DateTime GetLastWriteTime(string path);
        bool Exists(string path);
        byte[] ReadAllBytes(string path);
    }
}