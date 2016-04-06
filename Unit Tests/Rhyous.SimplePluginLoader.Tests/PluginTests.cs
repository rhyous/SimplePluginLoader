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
            Plugin<ITestPlugin> plugin = new Plugin<ITestPlugin> { File = dll };

            Assert.AreEqual(pdb, plugin.FilePdb);
        }

        [TestMethod]
        public void PdbTestInvalidPath()
        {
            var dll = @"Invalid";
            Plugin<ITestPlugin> plugin = new Plugin<ITestPlugin> { File = dll };

            Assert.AreEqual(null, plugin.FilePdb);
        }
    }
}
