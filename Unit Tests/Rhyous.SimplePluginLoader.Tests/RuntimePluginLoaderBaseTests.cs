using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using Rhyous.Collections;

namespace Rhyous.SimplePluginLoader.Tests
{

    [TestClass]
    public class RuntimePluginLoaderBaseTests
    {
        [TestMethod]
        public void RuntimePluginLoaderBase_DeafaultPluginPath_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins";
            var mockAppDomain = new Mock<IAppDomain>();
            var mockObjectCreator = new Mock<IObjectCreator<Org>>();
            var mockPluginLoaderLogger = new Mock<IPluginLoaderLogger>();
            var loader = new TestPluginLoader(mockAppDomain.Object, mockObjectCreator.Object, mockPluginLoaderLogger.Object);

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_UseAppSettingsForDefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"c:\plugins";
            var mockAppDomain = new Mock<IAppDomain>();
            var mockObjectCreator = new Mock<IObjectCreator<Org>>();
            var mockPluginLoaderLogger = new Mock<IPluginLoaderLogger>();
            var loader = new TestPluginLoader(mockAppDomain.Object, mockObjectCreator.Object, mockPluginLoaderLogger.Object);
            var nvc = new NameValueCollection { { RuntimePluginLoaderBase<Org>.PluginDirConfig, expected } };
            loader.AppSettings = nvc;

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_DeafaultPluginPathWithSubFolder_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins\Sub2";
            var mockAppDomain = new Mock<IAppDomain>();
            var mockObjectCreator = new Mock<IObjectCreator<Org>>();
            var mockPluginLoaderLogger = new Mock<IPluginLoaderLogger>();
            var loader = new TestPluginLoader2(mockAppDomain.Object, mockObjectCreator.Object, mockPluginLoaderLogger.Object);

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_UseAppSettingsAndPluginSubFolderForDefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"c:\plugins\Sub2";
            var mockAppDomain = new Mock<IAppDomain>();
            var mockObjectCreator = new Mock<IObjectCreator<Org>>();
            var mockPluginLoaderLogger = new Mock<IPluginLoaderLogger>();
            var loader = new TestPluginLoader2(mockAppDomain.Object, mockObjectCreator.Object, mockPluginLoaderLogger.Object);
            var nvc = new NameValueCollection { { RuntimePluginLoaderBase<Org>.PluginDirConfig, @"c:\plugins" } };
            loader.AppSettings = nvc;

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}