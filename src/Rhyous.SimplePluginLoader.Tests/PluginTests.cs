using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using T = Rhyous.SimplePluginLoader.Tests.Org;


namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginTests
    {
        private MockRepository _MockRepository;

        private Mock<ITypeLoader<T>> _MockTypeLoader;
        private Mock<IPluginObjectCreator<T>> _MockPluginObjectCreator;
        private Mock<IPluginDependencyResolver> _MockPluginDependencyResolver;
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockTypeLoader = _MockRepository.Create<ITypeLoader<T>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<T>>();
            _MockPluginDependencyResolver = _MockRepository.Create<IPluginDependencyResolver>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();

        }

        private Plugin<T> CreatePlugin(bool includeMocks = false)
        {
            if (includeMocks)
            {
                _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin<T>>());
            }
            return new Plugin<T>(
                _MockTypeLoader.Object,
                _MockPluginObjectCreator.Object,
                _MockPluginDependencyResolver.Object,
                _MockAssemblyLoader.Object);
        }

        #region Constructor

        [TestMethod]
        public void Plugin_Constructor_NullTypeLoader_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    null,
                    _MockPluginObjectCreator.Object,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullInstanceLoader_Test()
        {
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockTypeLoader.Object,
                    null,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullPluginDependencyResolver_Test()
        {
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockTypeLoader.Object,
                    _MockPluginObjectCreator.Object,
                    null,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullAssemblyLoader_Test()
        {
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockTypeLoader.Object,
                    _MockPluginObjectCreator.Object,
                    _MockPluginDependencyResolver.Object,
                    null);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_Test()
        {
            // Arrange
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            
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
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());

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
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());

            // Act
            var plugin = CreatePlugin(true);
            plugin.File = dll;

            // Assert
            Assert.AreEqual(null, plugin.FilePdb);
            _MockRepository.VerifyAll();
        }        

        [TestMethod]
        public void Plugin_Dispose_Test()
        {
            // Arrange
            _MockPluginDependencyResolver.Setup(m => m.RemoveDependencyResolver());
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
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

        [TestMethod]
        public void Plugin_CreatePluginObjects_Test()
        {
            // Arrange
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            var plugin = CreatePlugin(true);
            plugin.PluginTypes = new List<Type> { typeof(Org2), null, typeof(Org) };
            _MockPluginObjectCreator.Setup(m => m.Create(typeof(Org2))).Returns(new Org2());
            _MockPluginObjectCreator.Setup(m => m.Create(null)).Returns((Org)null);
            _MockPluginObjectCreator.Setup(m => m.Create(typeof(Org))).Returns(new Org());

            // Act
            var actual = plugin.CreatePluginObjects();

            // Assert
            Assert.AreEqual(2, actual.Count);
            _MockRepository.VerifyAll();
        }
    }
}