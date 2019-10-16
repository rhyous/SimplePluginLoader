using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;

namespace Rhyous.SimplePluginLoader.Tests
{

    [TestClass]
    public class RuntimePluginLoaderBaseTests
    {
        [TestMethod]
        public void RuntimePluginLoaderBase_Deafault_PluginPath()
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
        public void RuntimePluginLoaderBase_UseAppSettingsForDefault_PluginPath()
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
    }
}