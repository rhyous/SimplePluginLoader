using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Specialized;
using Rhyous.Collections;
using System.Collections.Generic;
using Rhyous.UnitTesting;
using System;

namespace Rhyous.SimplePluginLoader.Tests
{

    [TestClass]
    public class RuntimePluginLoaderBaseTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginCacheFactory<Org>> _MockPluginCacheFactory;
        private Mock<IPluginPaths> _MockPluginPaths;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginCacheFactory = _MockRepository.Create<IPluginCacheFactory<Org>>();
            _MockPluginPaths = _MockRepository.Create<IPluginPaths>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private TestPluginLoader CreateInstanceLoader()
        {
            return new TestPluginLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginCacheFactory.Object,
                _MockPluginPaths.Object,
                _MockPluginLoaderLogger.Object);
        }

        private TestPluginLoader2 CreateInstance2Loader()
        {
            return new TestPluginLoader2(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginCacheFactory.Object,
                _MockPluginPaths.Object,
                _MockPluginLoaderLogger.Object);
        }

        #region Constructor tests
        [TestMethod]
        public void RuntimePluginLoaderBase_Constructor_Null_AppDomain_Throws_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var loader = new TestPluginLoader(
                                   null,
                                   _MockPluginLoaderSettings.Object,
                                   _MockPluginCacheFactory.Object,
                                   _MockPluginPaths.Object,
                                   _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_Constructor_Null_Settings_Throws_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var loader = new TestPluginLoader(
                                   _MockAppDomain.Object,
                                   null,
                                   _MockPluginCacheFactory.Object,
                                   _MockPluginPaths.Object,
                                   _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_Constructor_Null_PluginCache_Throws_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var loader = new TestPluginLoader(
                                   _MockAppDomain.Object,
                                   _MockPluginLoaderSettings.Object,
                                   null,
                                   _MockPluginPaths.Object,
                                   _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }


        [TestMethod]
        public void RuntimePluginLoaderBase_Constructor_Null_PluginPaths_CreatesNew_Test()
        {
            // Arrange
            _MockPluginLoaderSettings.Setup(m => m.DefaultPluginDirectory).Returns((string)null);
            _MockPluginLoaderSettings.Setup(m => m.Company).Returns("Rhyous");
            _MockPluginLoaderSettings.Setup(m => m.AppName).Returns("App1");
            _MockPluginLoaderSettings.Setup(m => m.PluginFolder).Returns("Plugins");

            // Act
            var loader = new TestPluginLoader(
                                   _MockAppDomain.Object,
                                   _MockPluginLoaderSettings.Object,
                                   _MockPluginCacheFactory.Object,
                                   null,
                                   _MockPluginLoaderLogger.Object);

            // Assert
            _MockRepository.VerifyAll();
        }
        #endregion

        #region DefaultPluginDirectory
        [TestMethod]
        public void RuntimePluginLoaderBase_DefaultPluginPath_Test()
        {
            // Arrange
            var expected = @"C:\ProgramData\Rhyous\App1\Plugins";
            _MockPluginLoaderSettings.Setup(m=>m.DefaultPluginDirectory).Returns((string)null);
            _MockPluginLoaderSettings.Setup(m => m.Company).Returns("Rhyous");
            _MockPluginLoaderSettings.Setup(m => m.AppName).Returns("App1");
            _MockPluginLoaderSettings.Setup(m => m.PluginFolder).Returns("Plugins");
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
            _MockPluginLoaderSettings.Setup(m => m.Company).Returns("Rhyous");
            _MockPluginLoaderSettings.Setup(m => m.AppName).Returns("App1");
            _MockPluginLoaderSettings.Setup(m => m.PluginFolder).Returns("Plugins");
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
        #endregion

        #region PluginCollection
        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            var mockPlugin1 = _MockRepository.Create<IPlugin<Org>>();
            var mockPlugin2 = _MockRepository.Create<IPlugin<Org>>();
            var plugins = new PluginCollection<Org> { mockPlugin1.Object, mockPlugin2.Object };
            mockPluginLoader.Setup(m=>m.LoadPlugins()).Returns(plugins);

            // Act
            var actual = loader.PluginCollection;

            // Assert
            Assert.AreEqual(plugins, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Empty_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns(new PluginCollection<Org>());
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(false);
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins{Environment.NewLine}c:\MyApp\Plugins";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            var actual = loader.PluginCollection;

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Null_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;           
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns((PluginCollection<Org>)null);
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(true);
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m=>m.Paths).Returns(paths);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins{Environment.NewLine}c:\MyApp\Plugins";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            // Assert
            Assert.ThrowsException<RuntimePluginLoaderException>(() =>
            {
                var actual = loader.PluginCollection;
            });
            _MockRepository.VerifyAll();
        }
        #endregion

        #region PluginTypes
        [TestMethod]
        public void RuntimePluginLoaderBase_PluginTypes_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            
            var mockPlugin1 = _MockRepository.Create<IPlugin<Org>>();
            var plugin1Types = new List<Type> { typeof(Org) };
            mockPlugin1.Setup(m=>m.PluginTypes).Returns(plugin1Types);

            var mockPlugin2 = _MockRepository.Create<IPlugin<Org>>();
            var plugin2Types = new List<Type> { typeof(Org2) };
            mockPlugin2.Setup(m => m.PluginTypes).Returns(plugin2Types);

            var plugins = new PluginCollection<Org> { mockPlugin1.Object, mockPlugin2.Object };
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns(plugins);

            // Act
            var actual = loader.PluginTypes;

            // Assert
            CollectionAssert.AreEqual(new[] { typeof(Org), typeof(Org2)}, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginTypes_Empty_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns(new PluginCollection<Org>());
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(false);
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins{Environment.NewLine}c:\MyApp\Plugins";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            var actual = loader.PluginTypes;

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginTypes_Null_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns((PluginCollection<Org>)null);
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(true);
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins{Environment.NewLine}c:\MyApp\Plugins";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            // Assert
            Assert.ThrowsException<RuntimePluginLoaderException>(() =>
            {
                var actual = loader.PluginTypes;
            });
            _MockRepository.VerifyAll();
        }
        #endregion

        #region PluginLoader
        [TestMethod]
        public void RuntimePluginLoaderBase_PluginLoader_Test()
        {
            // Arrange
            var loader = CreateInstance2Loader();

            // Act
            var actual = loader.PluginLoader;

            // Assert
            Assert.IsNotNull(actual);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}