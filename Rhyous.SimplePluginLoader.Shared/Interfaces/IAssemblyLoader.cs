﻿using System;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public interface IAssemblyBuilder : IDisposable
    {
        AppDomain Domain { get; }
        Assembly Load(string dll, string pdb);
        Assembly TryLoad(string dll, string pdb);
        Assembly TryLoad(string dll, string pdb, string version);
    }
}