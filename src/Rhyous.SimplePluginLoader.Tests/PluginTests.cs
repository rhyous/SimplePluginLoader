using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginTests
    {
        [TestMethod]
        public void PdbTest()
        {
            var dll = @"C:\test\library.dll";
            var pdb = @"C:\test\library.pdb";
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var iTestPluginObjectCreator = new ObjectCreator<ITestPlugin>();
            var logger = new PluginLoaderLogger();
            var typeLoader = new TypeLoader<ITestPlugin>(PluginLoaderSettings.Default, logger);
            var instanceLoader = new InstanceLoader<ITestPlugin>(iTestPluginObjectCreator, typeLoader, PluginLoaderSettings.Default, logger);
            var plugin = new Plugin<ITestPlugin>(appDomain, typeLoader, instanceLoader, logger)
            {
                File = dll
            };
            Assert.AreEqual(pdb, plugin.FilePdb);
        }

        [TestMethod]
        public void PdbTestInvalidPath()
        {
            var dll = @"Invalid";
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var iTestPluginObjectCreator = new ObjectCreator<ITestPlugin>();
            var logger = new PluginLoaderLogger();
            var typeLoader = new TypeLoader<ITestPlugin>(PluginLoaderSettings.Default, logger);
            var instanceLoader = new InstanceLoader<ITestPlugin>(iTestPluginObjectCreator, typeLoader, PluginLoaderSettings.Default, logger);
            var plugin = new Plugin<ITestPlugin>(appDomain, typeLoader, instanceLoader, logger)
            {
                File = dll
            };

            Assert.AreEqual(null, plugin.FilePdb);
        }
    }
}
