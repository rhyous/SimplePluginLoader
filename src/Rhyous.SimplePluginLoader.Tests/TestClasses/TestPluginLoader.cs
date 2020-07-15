namespace Rhyous.SimplePluginLoader.Tests
{
    class TestPluginLoader : RuntimePluginLoaderBase<Org>
    {
        public TestPluginLoader(IAppDomain appDomain,
                                IPluginLoaderSettings settings,
                                IPluginLoaderFactory<Org> pluginLoaderFactory,
                                IPluginPaths pluginPaths,
                                IPluginLoaderLogger logger) 
            : base(appDomain,  settings, pluginLoaderFactory, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder { get; }
    }

    class TestPluginLoader2 : RuntimePluginLoaderBase<Org>
    {
        public TestPluginLoader2(IAppDomain appDomain, 
                                 IPluginLoaderSettings settings,
                                 IPluginLoaderFactory<Org> pluginLoaderFactory,
                                 IPluginPaths pluginPaths,
                                 IPluginLoaderLogger logger)
            : base(appDomain, settings, pluginLoaderFactory, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder => "Sub2";
    }
}