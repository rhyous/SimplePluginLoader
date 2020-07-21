using Rhyous.SimplePluginLoader.Autofac.Tests;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    class TestRuntimePluginLoader : RuntimePluginLoaderBase<IOrganization>
    {
        public TestRuntimePluginLoader(IAppDomain appDomain,
                                       IPluginLoaderSettings settings,
                                       IPluginLoaderFactory<IOrganization> pluginLoaderFactory,
                                       IPluginObjectCreator<IOrganization> pluginObjectCreator,
                                       IPluginPaths pluginPaths,
                                       IPluginLoaderLogger logger) 
            : base(appDomain,  settings, pluginLoaderFactory, pluginObjectCreator, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder { get; }
    }

    class TestRuntimePluginLoader2 : RuntimePluginLoaderBase<IOrganization>
    {
        public TestRuntimePluginLoader2(IAppDomain appDomain, 
                                        IPluginLoaderSettings settings,
                                        IPluginLoaderFactory<IOrganization> pluginLoaderFactory,
                                        IPluginObjectCreator<IOrganization> pluginObjectCreator,
                                        IPluginPaths pluginPaths,
                                        IPluginLoaderLogger logger)
            : base(appDomain, settings, pluginLoaderFactory, pluginObjectCreator, pluginPaths, logger)
        {
        }

        public override string PluginSubFolder => "Sub2";
    }
}