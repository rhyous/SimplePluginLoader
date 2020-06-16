using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginLoaderSettingsTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppSettings> _MockAppSettings;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppSettings = _MockRepository.Create<IAppSettings>();
        }

        private PluginLoaderSettings CreatePluginLoaderSettings()
        {
            return new PluginLoaderSettings(
                _MockAppSettings.Object);
        }

        [TestMethod]
        public void SharedBinPathManager_BasicTest()
        {
            // Arrange
            var appSettings = new NameValueCollection { { PluginLoaderSettings.PluginSharedBinPathsKey, @"c:\bin;c:\sharedbin\;c:\Libs" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(appSettings);
            var expectedPaths = new List<string> { @"c:\bin", @"c:\sharedbin\", @"c:\Libs" };
            var settings = new PluginLoaderSettings(_MockAppSettings.Object);

            // Act
            var actualpaths = settings.GetSharedBinPaths();

            // Assert
            Assert.IsTrue(actualpaths.SequenceEqual(expectedPaths));
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void SharedBinPathManager_MultipleSemicolonsIgnored()
        {
            // Arrange
            var appSettings = new NameValueCollection { { PluginLoaderSettings.PluginSharedBinPathsKey, @"c:\bin;;;;;;c:\sharedbin\;c:\Libs" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(appSettings);
            var expectedPaths = new List<string> { @"c:\bin", @"c:\sharedbin\", @"c:\Libs" };
            var settings = new PluginLoaderSettings(_MockAppSettings.Object);

            // Act
            var actualpaths = settings.GetSharedBinPaths();

            // Assert
            Assert.IsTrue(actualpaths.SequenceEqual(expectedPaths));
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void SharedBinPathManager_ValueOnlyHasSemicolons()
        {
            // Arrange
            var appSettings = new NameValueCollection { { PluginLoaderSettings.PluginSharedBinPathsKey, @";;;;;;" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(appSettings);
            var settings = new PluginLoaderSettings(_MockAppSettings.Object);

            // Act
            var actualpaths = settings.GetSharedBinPaths();

            // Assert
            Assert.IsFalse(actualpaths.Any());
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void SharedBinPathManager_ValueOnlyHasSemicolonsAndWhitespace()
        {
            // Arrange
            var appSettings = new NameValueCollection { { PluginLoaderSettings.PluginSharedBinPathsKey, @";  ;;   ;;      ;" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(appSettings);
            var settings = new PluginLoaderSettings(_MockAppSettings.Object);

            // Act
            var actualpaths = settings.GetSharedBinPaths();

            // Assert
            Assert.IsFalse(actualpaths.Any());
            _MockRepository.VerifyAll();
        }
    }
}