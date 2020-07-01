using System;

namespace Rhyous.SimplePluginLoader
{
    public class PluginDependencyResolverObjectCreator : IPluginDependencyResolverObjectCreator
    {

            private readonly IAppDomain _AppDomain;
        private readonly IPluginLoaderSettings _Settings;
        private readonly IAssemblyLoader _AssemblyLoader;
        private readonly IPluginLoaderLogger _Logger;

        public PluginDependencyResolverObjectCreator(IAppDomain appDomain,
                                                     IPluginLoaderSettings settings,
                                                     IAssemblyLoader assemblyLoader,
                                                     IPluginLoaderLogger logger)
        {
            _AppDomain = appDomain ?? throw new ArgumentNullException(nameof(appDomain));
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _AssemblyLoader = assemblyLoader ?? throw new ArgumentNullException(nameof(assemblyLoader));
            _Logger = logger;
        }

        public IPluginDependencyResolver Create(Type type = null)
        {
            return new PluginDependencyResolver(_AppDomain, _Settings, _AssemblyLoader, _Logger);
        }
    }
}
