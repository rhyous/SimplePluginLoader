namespace Rhyous.SimplePluginLoader
{
    public class PluginObjectCreatorFactory<T> : IPluginObjectCreatorFactory<T>
    where T : class
    {
        private readonly IPluginLoaderSettings _Settings;
        private readonly IPluginLoaderLogger _Logger;

        public PluginObjectCreatorFactory(IPluginLoaderSettings settings,
                                          IPluginLoaderLogger logger)
        {
            _Settings = settings;
            _Logger = logger;
        }

        public IPluginObjectCreator<T> Create()
        {
            return new PluginObjectCreator<T>(_Settings, _Logger);
        }
    }
}