namespace Rhyous.SimplePluginLoader.Tests
{
    class TestPluginLoader : RuntimePluginLoaderBase<Org>
    {
        public TestPluginLoader(IAppDomain appDomain, IObjectCreator<Org> objectCreator, IPluginLoaderLogger logger) 
            : base(appDomain, objectCreator, logger)
        {
        }

        public override string PluginSubFolder { get; }
    }

    class TestPluginLoader2 : RuntimePluginLoaderBase<Org>
    {
        public TestPluginLoader2(IAppDomain appDomain, IObjectCreator<Org> objectCreator, IPluginLoaderLogger logger)
            : base(appDomain, objectCreator, logger)
        {
        }

        public override string PluginSubFolder => "Sub2";
    }
}