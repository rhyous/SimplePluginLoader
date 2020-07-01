using System;

namespace Rhyous.SimplePluginLoader
{
    public class PluginObjectCreator<T> : ObjectCreator<T>, IPluginObjectCreator<T>
        where T : class
    {
        private readonly IPluginLoaderSettings _Settings;
        private readonly IPluginLoaderLogger _Logger;

        public PluginObjectCreator(IPluginLoaderSettings settings,
                                   IPluginLoaderLogger logger)
        {
            _Settings = settings;
            _Logger = logger;
        }

        public IPlugin Plugin { get; set; }

        public override T Create(Type type = null)
        {
            if (type == null)
                return null;
            T obj = null;
            try
            {
                obj = base.Create(type);
                if (obj == null)
                {
                    throw new PluginTypeLoadException($"ObjectCreator attempted to load plugin {type.Name} but returned null.");
                }
                return obj;
            }
            catch (Exception e)
            {
                LogException(type, e);
            }
            return obj;
        }

        private void LogException(Type typeToLoad, Exception e)
        {
            var e2 = (e as PluginTypeLoadException) ?? new PluginTypeLoadException($"Failed to load plugin type: {typeToLoad.Name}. See inner exception.", e);
            _Logger?.Log(e2);
            e.LogReflectionTypeLoadExceptions(_Logger);
            if (_Settings.ThrowExceptionsOnLoad)
            {
                throw e2;
            }
        }
    }
}
