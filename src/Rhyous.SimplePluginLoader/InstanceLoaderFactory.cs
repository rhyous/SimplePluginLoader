using System;

namespace Rhyous.SimplePluginLoader
{
    public class InstanceLoaderFactory<T> : IInstanceLoaderFactory<T>
        where T : class
    {
        private readonly IObjectCreatorFactory<T> _ObjectCreatorFactory;
        private readonly ITypeLoader<T> _TypeLoader;
        private readonly IPluginLoaderSettings _Settings;
        private readonly IPluginLoaderLogger _Logger;

        public InstanceLoaderFactory(IObjectCreatorFactory<T> objectCreatorFactory,
                                     ITypeLoader<T> typeLoader,
                                     IPluginLoaderSettings settings,
                                     IPluginLoaderLogger logger)
        {
            _ObjectCreatorFactory = objectCreatorFactory ?? throw new ArgumentNullException(nameof(_ObjectCreatorFactory));
            _TypeLoader = typeLoader ?? throw new ArgumentNullException(nameof(typeLoader));
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _Logger = logger;
        }

        public IInstanceLoader<T> Create()
        {
            return new InstanceLoader<T>(_ObjectCreatorFactory.Create(), _TypeLoader, _Settings, _Logger);
        }
    }
}