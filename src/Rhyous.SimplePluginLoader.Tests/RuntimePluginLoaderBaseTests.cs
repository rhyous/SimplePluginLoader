using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using Rhyous.Collections;

namespace Rhyous.SimplePluginLoader.Tests
{

    [TestClass]
    public class RuntimePluginLoaderBaseTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginCacheFactory<Org>> _MockPluginCacheFactory;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void InstanceLoader_TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginCacheFactory = _MockRepository.Create<IPluginCacheFactory<Org>>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private TestPluginLoader CreateInstanceLoader()
        {
            return new TestPluginLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginCacheFactory.Object,
                _MockPluginLoaderLogger.Object);
        }

        private TestPluginLoader2 CreateInstance2Loader()
        {
            return new TestPluginLoader2(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginCacheFactory.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_DefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins";
            _MockPluginLoaderSettings.Setup(m=>m.DefaultPluginDirectory).Returns((string)null);
            var loader = CreateInstanceLoader();

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_UseAppSettingsForDefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"c:\plugins";
            _MockPluginLoaderSettings.Setup(m => m.DefaultPluginDirectory).Returns(@"c:\plugins");
            var loader = CreateInstanceLoader();

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_DefaultPluginPathWithSubFolder_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins\Sub2"; 
            _MockPluginLoaderSettings.Setup(m => m.DefaultPluginDirectory).Returns((string)null);
            var loader = CreateInstance2Loader();

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_UseAppSettingsAndPluginSubFolderForDefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"c:\plugins\Sub2";
            _MockPluginLoaderSettings.Setup(m => m.DefaultPluginDirectory).Returns(@"c:\plugins");
            var loader = CreateInstance2Loader();

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
            _MockRepository.VerifyAll();
        }
    }
}