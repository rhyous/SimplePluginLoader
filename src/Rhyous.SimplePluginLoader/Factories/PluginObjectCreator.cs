using System;
using System.Diagnostics.CodeAnalysis;

namespace Rhyous.SimplePluginLoader
{
    public class PluginObjectCreator<T> : IPluginObjectCreator<T>
        where T : class
    {
        private readonly IPluginLoaderSettings _Settings;
        private readonly IPluginLoaderLogger _Logger;
        private readonly IObjectCreator<T> _ObjectCreator;

        public PluginObjectCreator(IPluginLoaderSettings settings,
                                   IObjectCreator<T> objectCreator,
                                   IPluginLoaderLogger logger)
        {
            _Settings = settings;
            _Logger = logger;
            _ObjectCreator = objectCreator;
        }

        public T Create(IPlugin<T> plugin, Type type = null)
        {
            if (type == null)
                type = typeof(T);
            T obj = null;
            try
            {
                obj = _ObjectCreator.Create(type);
                if (obj == null)
                {
                    throw new PluginTypeLoadException($"ObjectCreator for {plugin.Name} plugin attempted to load plugin {type.Name} but returned null.");
                }
                return obj;
            }
            catch (Exception e)
            {
                LogException(plugin, type, e);
            }
            return obj;
        }

        private void LogException(IPlugin<T> plugin, Type typeToLoad, Exception e)
        {
            var e2 = (e as PluginTypeLoadException) ?? new PluginTypeLoadException($"{plugin.Name} plugin failed to load plugin type: {typeToLoad.Name}. See inner exception.", e);
            _Logger?.Log(e2);
            e.LogReflectionTypeLoadExceptions(_Logger);
            if (_Settings.ThrowExceptionsOnLoad)
            {
                throw e2;
            }
        }
    }
}
