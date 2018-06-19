using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.SimplePluginLoader.Tests.TestClasses;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginDependencyResolverTests
    {
        [TestMethod]
        public void GetPaths_BasicTest()
        {
            // Arrange
            var plugin = new Plugin<IOrg>() { Directory = @"c:\my\plugins", File = "MyPlugin.dll"};
            var resolver = new PluginDependencyResolver<IOrg>(plugin);
            resolver.SharedBinPathManager.SharePathProviderMethod = () => { return @"c:\bin;c:\sharedbin\;c:\Libs"; };
            var expectedPaths = new List<string> {
                "",
                "c:\\my\\plugins",
                "c:\\my\\plugins\\bin",
                "c:\\my\\plugins\\MyPlugin",
                @"c:\bin",
                @"c:\sharedbin\",
                @"c:\Libs" };

            // Act
            var actualpaths = resolver.Paths;

            // Assert
            CollectionAssert.AreEqual(expectedPaths, actualpaths);
        }
    }
}
