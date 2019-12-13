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
            Plugin<ITestPlugin> plugin = new Plugin<ITestPlugin>(appDomain, iTestPluginObjectCreator, logger)
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
            Plugin<ITestPlugin> plugin = new Plugin<ITestPlugin>(appDomain, iTestPluginObjectCreator, logger)
            {
                File = dll
            };

            Assert.AreEqual(null, plugin.FilePdb);
        }
    }
}
