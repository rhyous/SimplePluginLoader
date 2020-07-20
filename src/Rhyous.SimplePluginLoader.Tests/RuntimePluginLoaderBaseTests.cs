using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Tests
{

    [TestClass]
    public class RuntimePluginLoaderBaseTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginLoaderFactory<Org>> _MockPluginLoaderFactory;
        private Mock<IPluginObjectCreator<Org>> _MockPluginObjectCreator;
        private Mock<IPluginPaths> _MockPluginPaths;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;
        private List<string> _Paths;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderFactory = _MockRepository.Create<IPluginLoaderFactory<Org>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<Org>>();
            _MockPluginPaths = _MockRepository.Create<IPluginPaths>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _Paths = new List<string>
            {
                @"c:\my\path",
                @"c:\some\other\path"
            };
        }

        private TestPluginLoader CreateInstanceLoader()
        {
            return new TestPluginLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginLoaderFactory.Object,
                _MockPluginObjectCreator.Object,
                _MockPluginPaths.Object,
                _MockPluginLoaderLogger.Object);
        }

        private TestPluginLoader2 CreateInstance2Loader()
        {
            return new TestPluginLoader2(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginLoaderFactory.Object,
                _MockPluginObjectCreator.Object,
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
                                   _MockPluginLoaderFactory.Object,
                                   _MockPluginObjectCreator.Object,
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
                                   _MockPluginLoaderFactory.Object,
                                   _MockPluginObjectCreator.Object,
                                   _MockPluginPaths.Object,
                                   _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_Constructor_Null_PluginLoaderFactory_Throws_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var loader = new TestPluginLoader(
                                   _MockAppDomain.Object,
                                   _MockPluginLoaderSettings.Object,
                                   null,
                                   _MockPluginObjectCreator.Object,
                                   _MockPluginPaths.Object,
                                   _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_Constructor_Null_PluginObjectCreator_Throws_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                var loader = new TestPluginLoader(
                                   _MockAppDomain.Object,
                                   _MockPluginLoaderSettings.Object,
                                   _MockPluginLoaderFactory.Object,
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
                                   _MockPluginLoaderFactory.Object,
                                   _MockPluginObjectCreator.Object,
                                   null,
                                   _MockPluginLoaderLogger.Object);

            // Assert
            _MockRepository.VerifyAll();
        }
        #endregion

        #region PluginCollection
        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Test()
        {
            // Arrange
            _MockPluginPaths.Setup(m => m.Paths).Returns(_Paths);
            var mockPlugin1 = _MockRepository.Create<IPlugin<Org>>();
            var mockPlugin2 = _MockRepository.Create<IPlugin<Org>>();
            var plugins = new PluginCollection<Org> { mockPlugin1.Object, mockPlugin2.Object }; 
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns(plugins);
            var loader = CreateInstance2Loader();
            _MockPluginLoaderFactory.Setup(m=>m.Create(It.IsAny<IPluginPaths>()))
                                    .Returns(mockPluginLoader.Object);

            // Act
            var actual = loader.PluginCollection;

            // Assert
            Assert.AreEqual(plugins, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Null_Test()
        {
            // Arrange
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns((PluginCollection<Org>)null);
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(false);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins\Sub2{Environment.NewLine}c:\MyApp\Plugins\Sub2";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            var actual = loader.PluginCollection;

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Empty_Test()
        {
            // Arrange
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns(new PluginCollection<Org>());
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(false);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins\Sub2{Environment.NewLine}c:\MyApp\Plugins\Sub2";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            var actual = loader.PluginCollection;

            // Assert
            Assert.AreEqual(0, actual.Count);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginCollection_Null_ThrowExceptionIfNoPluginFound_True_Throws()
        {
            // Arrange
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;           
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns((PluginCollection<Org>)null);
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(true);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins\Sub2{Environment.NewLine}c:\MyApp\Plugins\Sub2";
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
            _MockPluginPaths.Setup(m => m.Paths).Returns(_Paths);
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
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns(new PluginCollection<Org>());
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(false);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins\Sub2{Environment.NewLine}c:\MyApp\Plugins\Sub2";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            // Act
            var actual = loader.PluginTypes;

            // Assert
            Assert.AreEqual(0, actual.Count);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void RuntimePluginLoaderBase_PluginTypes_Null_Test()
        {
            // Arrange
            var paths = new List<string> { @"c:\plugins", @"c:\MyApp\Plugins" };
            _MockPluginPaths.Setup(m => m.Paths).Returns(paths);
            var loader = CreateInstance2Loader();
            var mockPluginLoader = _MockRepository.Create<IPluginLoader<Org>>();
            loader.PluginLoader = mockPluginLoader.Object;
            mockPluginLoader.Setup(m => m.LoadPlugins()).Returns((PluginCollection<Org>)null);
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionIfNoPluginFound).Returns(true);
            var msg = $@"No Sub2 plugins were found in these directories:{Environment.NewLine}c:\plugins\Sub2{Environment.NewLine}c:\MyApp\Plugins\Sub2";
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
        public void RuntimePluginLoaderBase_PluginLoader_FactoryReturnsPluginLoader_Test()
        {
            // Arrange
            _MockPluginPaths.Setup(m => m.Paths).Returns(_Paths);
            var loader = CreateInstance2Loader();
            var expectedPaths = new List<string>
            {
                $@"c:\my\path\{loader.PluginSubFolder}",
                $@"c:\some\other\path\{loader.PluginSubFolder}"
            };
            var mockPluginCacheFactory = _MockRepository.Create<IPluginCacheFactory<Org>>();
            IPluginPaths actualPluginPaths = null;
            _MockPluginLoaderFactory.Setup(m => m.Create(It.IsAny<PluginPaths>()))
                .Returns((IPluginPaths pluginPaths) => 
                {
                    actualPluginPaths = pluginPaths;
                    return new PluginLoader<Org>(pluginPaths, mockPluginCacheFactory.Object);
                });

            // Act
            var actual = loader.PluginLoader;

            // Assert
            Assert.IsNotNull(actual);
            CollectionAssert.AreEqual(expectedPaths, actualPluginPaths.Paths.ToList());
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}