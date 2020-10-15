namespace Rhyous.SimplePluginLoader.Tests
{
    class TestRuntimePluginLoader : RuntimePluginLoaderBase<Org>
    {
        public TestRuntimePluginLoader(IAppDomain appDomain,
                                IPluginLoaderSettings settings,
                                IPluginLoaderFactory<Org> pluginLoaderFactory,
                                IPluginObjectCreator<Org> pluginObjectCreator,
                                IPluginPaths pluginPaths,
                                IPluginLoaderLogger logger) 
            : base(appDomain,  settings, pluginLoaderFactory, pluginObjectCreator, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder { get; }
    }

    class TestRuntimePluginLoader2 : RuntimePluginLoaderBase<Org>
    {
        public TestRuntimePluginLoader2(IAppDomain appDomain, 
                                 IPluginLoaderSettings settings,
                                 IPluginLoaderFactory<Org> pluginLoaderFactory,
                                 IPluginObjectCreator<Org> pluginObjectCreator,
                                 IPluginPaths pluginPaths,
                                 IPluginLoaderLogger logger)
            : base(appDomain, settings, pluginLoaderFactory, pluginObjectCreator, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder => "Sub2";
    }
}