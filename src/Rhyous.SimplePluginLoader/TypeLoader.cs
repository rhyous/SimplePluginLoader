using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public class TypeLoader<T> : ITypeLoader<T>
        where T : class
    {
        private readonly IPluginLoaderSettings _Settings;
        private readonly IPluginLoaderLogger _Logger;

        public TypeLoader(IPluginLoaderSettings settings,
                          IPluginLoaderLogger logger)
        {
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _Logger = logger;
        }

        public List<Type> Load(IAssembly assembly)
        {
            try
            {
                if (assembly == null)
                    return null;
                var allAssemblyTypes = assembly.TryGetTypes(_Settings.ThrowExceptionsOnLoad, _Logger);
                var pluginTypes = new List<Type>();
                foreach (var type in allAssemblyTypes)
                {
                    if (type.IsPluginType<T>())
                        pluginTypes.Add(type);
                }
                return pluginTypes;
            }
            catch (Exception e)
            {
                return HandleException(e);
            }
        }

        private List<Type> HandleException(Exception e)
        {
            if (_Settings.ThrowExceptionsOnLoad)
            {
                var e2 = new PluginTypeLoadException($"Failed to load plugin types of type {typeof(T).Name}. See inner exception.", e);
                _Logger?.Log(e2);
                throw e2;
            }
            else
            {
                _Logger?.Write(PluginLoaderLogLevel.Info, $"Exception occurred loading a plugin type.");
                _Logger?.Log(e);
                e.LogReflectionTypeLoadExceptions(_Logger);
                return null;
            }
        }
    }
}
