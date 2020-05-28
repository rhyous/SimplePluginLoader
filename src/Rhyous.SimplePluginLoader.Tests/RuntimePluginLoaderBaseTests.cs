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
        private Mock<ITypeLoader<Org>> _MockTypeLoader;
        private Mock<IInstanceLoaderFactory<Org>> _MockInstanceLoaderFactory;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void InstanceLoader_TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockTypeLoader = _MockRepository.Create<ITypeLoader<Org>>();
            _MockInstanceLoaderFactory = _MockRepository.Create<IInstanceLoaderFactory<Org>>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private TestPluginLoader CreateInstanceLoader()
        {
            return new TestPluginLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockTypeLoader.Object,
                _MockInstanceLoaderFactory.Object,
                _MockPluginLoaderLogger.Object);
        }

        private TestPluginLoader2 CreateInstance2Loader()
        {
            return new TestPluginLoader2(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockTypeLoader.Object,
                _MockInstanceLoaderFactory.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_DeafaultPluginPath_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins";
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
            var loader = CreateInstanceLoader();
            var nvc = new NameValueCollection { { RuntimePluginLoaderBase<Org>.PluginDirConfig, expected } };
            loader.AppSettings = nvc;

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_DeafaultPluginPathWithSubFolder_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins\Sub2";
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
            var loader = CreateInstance2Loader();
            var nvc = new NameValueCollection { { RuntimePluginLoaderBase<Org>.PluginDirConfig, @"c:\plugins" } };
            loader.AppSettings = nvc;

            // Act
            var actual = loader.DefaultPluginDirectory;

            // Assert
            Assert.AreEqual(expected, actual);
            _MockRepository.VerifyAll();

        }
    }
}