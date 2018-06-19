using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class SharedBinPathManagerTests
    {
        [TestMethod]
        public void SharedBinPathManager_BasicTest()
        {
            // Arrange
            var pathManager = new SharedBinPathManager();
            pathManager.SharePathProviderMethod = () => { return @"c:\bin;c:\sharedbin\;c:\Libs"; };            
            var expectedPaths = new List<string> { @"c:\bin", @"c:\sharedbin\", @"c:\Libs" };

            // Act
            var actualpaths = pathManager.GetSharedBinPaths();

            // Assert
            Assert.IsTrue(actualpaths.SequenceEqual(expectedPaths));
        }

        [TestMethod]
        public void SharedBinPathManager_MultipleSemicolonsIgnored()
        {
            // Arrange
            var pathManager = new SharedBinPathManager();
            pathManager.SharePathProviderMethod = () => { return @"c:\bin;;;;;;c:\sharedbin\;c:\Libs"; };
            var expectedPaths = new List<string> { @"c:\bin", @"c:\sharedbin\", @"c:\Libs" };

            // Act
            var actualpaths = pathManager.GetSharedBinPaths();

            // Assert
            Assert.IsTrue(actualpaths.SequenceEqual(expectedPaths));
        }

        [TestMethod]
        public void SharedBinPathManager_ValueOnlyHasSemicolons()
        {
            // Arrange
            var pathManager = new SharedBinPathManager();
            pathManager.SharePathProviderMethod = () => { return @";;;;;;"; };

            // Act
            var actualpaths = pathManager.GetSharedBinPaths();

            // Assert
            Assert.IsFalse(actualpaths.Any());
        }

        [TestMethod]
        public void SharedBinPathManager_ValueOnlyHasSemicolonsAndWhitespace()
        {
            // Arrange
            var pathManager = new SharedBinPathManager();
            pathManager.SharePathProviderMethod = () => { return @";  ;;   ;;      ;"; };

            // Act
            var actualpaths = pathManager.GetSharedBinPaths();

            // Assert
            Assert.IsFalse(actualpaths.Any());
        }
    }
}
