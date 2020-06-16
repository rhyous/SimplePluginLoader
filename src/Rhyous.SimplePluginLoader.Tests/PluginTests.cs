using Castle.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using T = Rhyous.SimplePluginLoader.Tests.Org;


namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<ITypeLoader<T>> _MockTypeLoader;
        private Mock<IInstanceLoader<T>> _MockInstanceLoader;
        private Mock<IPluginDependencyResolver<T>> _MockPluginDependencyResolver;
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockTypeLoader = _MockRepository.Create<ITypeLoader<T>>();
            _MockInstanceLoader = _MockRepository.Create<IInstanceLoader<T>>();
            _MockPluginDependencyResolver = _MockRepository.Create<IPluginDependencyResolver<T>>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();

        }

        private Plugin<T> CreatePlugin(bool includeMocks = false)
        {
            if (includeMocks)
            {
                _MockInstanceLoader.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
                _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
                _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>()));
            }
            return new Plugin<T>(
                _MockAppDomain.Object,
                _MockTypeLoader.Object,
                _MockInstanceLoader.Object,
                _MockPluginDependencyResolver.Object,
                _MockAssemblyLoader.Object,
                _MockPluginLoaderLogger.Object);
        }

        #region Constructor

        [TestMethod]
        public void Plugin_Constructor_NullAppDomain_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    null,
                    _MockTypeLoader.Object,
                    _MockInstanceLoader.Object,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object,
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullTypeLoader_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockAppDomain.Object,
                    null,
                    _MockInstanceLoader.Object,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object,
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullInstanceLoader_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockAppDomain.Object,
                    _MockTypeLoader.Object,
                    null,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object,
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullPluginDependencyResolver_Test()
        {
            _MockInstanceLoader.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockAppDomain.Object,
                    _MockTypeLoader.Object,
                    _MockInstanceLoader.Object,
                    null,
                    _MockAssemblyLoader.Object,
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullAssemblyLoader_Test()
        {
            _MockInstanceLoader.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
            _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockAppDomain.Object,
                    _MockTypeLoader.Object,
                    _MockInstanceLoader.Object,
                    _MockPluginDependencyResolver.Object,
                    null,
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullLogger_Ignored_Test()
        {
            // Arrange
            _MockInstanceLoader.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
            _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());

            // Act
            new Plugin<T>(
                _MockAppDomain.Object,
                _MockTypeLoader.Object,
                _MockInstanceLoader.Object,
                _MockPluginDependencyResolver.Object,
                _MockAssemblyLoader.Object,
                null);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_Constructor_Test()
        {
            // Arrange
            // Act
            CreatePlugin(true);

            // Assert
            _MockRepository.VerifyAll();
        }

        #endregion

        [TestMethod]
        public void Plugin_PdbTest()
        {
            // Arrange
            var dll = @"C:\test\library.dll";
            var pdb = @"C:\test\library.pdb";

            // Act
            var plugin = CreatePlugin(true);
            plugin.File = dll;

            // Assert
            Assert.AreEqual(pdb, plugin.FilePdb);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_PdbTestInvalidPath()
        {
            // Arrange
            var dll = @"Invalid";

            // Act
            var plugin = CreatePlugin(true);
            plugin.File = dll;

            // Assert
            Assert.AreEqual(null, plugin.FilePdb);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_RemoveDependencyResolver_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            bool wasCalled = false;
            ResolveEventHandler handler = (object sender, ResolveEventArgs args) => 
            {
                wasCalled = true;
                return Assembly.GetExecutingAssembly(); 
            };

            // Act
            plugin.RemoveDependencyResolver();

            // Assert
            Assert.IsFalse(wasCalled, "The removed handle should not be called.");
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_Dispose_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            bool wasCalled = false;
            ResolveEventHandler handler = (object sender, ResolveEventArgs args) =>
            {
                wasCalled = true;
                return Assembly.GetExecutingAssembly();
            };

            _MockAssemblyLoader.Setup(m => m.Dispose());
            _MockPluginDependencyResolver.Setup(m => m.Dispose());

            // Act
            plugin.Dispose();

            // Assert
            Assert.IsFalse(wasCalled, "The removed handle should not be called.");
            _MockRepository.VerifyAll();
        }
    }
}