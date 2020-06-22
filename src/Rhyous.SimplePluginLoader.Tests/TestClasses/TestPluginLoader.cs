namespace Rhyous.SimplePluginLoader.Tests
{
    class TestPluginLoader : RuntimePluginLoaderBase<Org>
    {
        public TestPluginLoader(IAppDomain appDomain,
                                IPluginLoaderSettings settings,
                                ITypeLoader<Org> typeLoader,
                                IInstanceLoaderFactory<Org> instanceLoaderFactory,
                                IAssemblyLoader assemblyLoader,
                                IPluginDependencyResolver<Org> pluginDependencyResolver,
                                IPluginLoaderLogger logger) 
            : base(appDomain,  settings, typeLoader, instanceLoaderFactory, assemblyLoader, pluginDependencyResolver, logger)
        {
        }

        public override string PluginSubFolder { get; }
    }

    class TestPluginLoader2 : RuntimePluginLoaderBase<Org>
    {
        public TestPluginLoader2(IAppDomain appDomain, 
                                 IPluginLoaderSettings settings,
                                 ITypeLoader<Org> typeLoader,
                                 IInstanceLoaderFactory<Org> instanceLoaderFactory,
                                 IAssemblyLoader assemblyLoader,
                                 IPluginDependencyResolver<Org> pluginDependencyResolver,
                                 IPluginLoaderLogger logger)
            : base(appDomain, settings, typeLoader, instanceLoaderFactory, assemblyLoader, pluginDependencyResolver, logger)
        {
        }

        public override string PluginSubFolder => "Sub2";
    }
}