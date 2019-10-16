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
}