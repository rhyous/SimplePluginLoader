﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginTests
    {
        [TestMethod]
        public void PdbTests()
        {
            var dll = @"C:\test\library.dll";
            var pdb = @"C:\test\library.pdb";
            Plugin<ITestPlugin> plugin = new Plugin<ITestPlugin> { File = dll };

            Assert.AreEqual(pdb, plugin.FilePdb);
        }
    }
}