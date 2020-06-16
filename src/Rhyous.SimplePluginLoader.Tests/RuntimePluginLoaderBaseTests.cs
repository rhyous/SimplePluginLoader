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
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPluginDependencyResolver<Org>> _MockPluginDependencyResolver;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void InstanceLoader_TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockTypeLoader = _MockRepository.Create<ITypeLoader<Org>>();
            _MockInstanceLoaderFactory = _MockRepository.Create<IInstanceLoaderFactory<Org>>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockPluginDependencyResolver = _MockRepository.Create<IPluginDependencyResolver<Org>>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private TestPluginLoader CreateInstanceLoader()
        {
            return new TestPluginLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockTypeLoader.Object,
                _MockInstanceLoaderFactory.Object,
                _MockAssemblyLoader.Object,
                _MockPluginDependencyResolver.Object,
                _MockPluginLoaderLogger.Object);
        }

        private TestPluginLoader2 CreateInstance2Loader()
        {
            return new TestPluginLoader2(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockTypeLoader.Object,
                _MockInstanceLoaderFactory.Object,
                _MockAssemblyLoader.Object,
                _MockPluginDependencyResolver.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_DefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins";
            var loader = CreateInstanceLoader();
            _MockPluginLoaderSettings.Setup(m=>m.DefaultPluginDirectory).Returns((string)null);

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
            _MockPluginLoaderSettings.Setup(m => m.DefaultPluginDirectory).Returns(@"c:\plugins");

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
            var loader = CreateInstance2Loader();
            _MockPluginLoaderSettings.Setup(m => m.DefaultPluginDirectory).Returns((string)null);

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