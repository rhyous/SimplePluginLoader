using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using Rhyous.SimplePluginLoader.Tests.TestClasses;
using System;

namespace Rhyous.SimplePluginLoader.Tests.Factories
{
    [TestClass]
    public class PluginObjectCreatorTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;
        private Mock<IPlugin> _MockPlugin;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockPlugin = _MockRepository.Create<IPlugin>();
        }

        private PluginObjectCreator<T> CreatePluginObjectCreator<T>(bool nullLogger = false)
            where T : class
        {
            return new PluginObjectCreator<T>(
                _MockPluginLoaderSettings.Object,
                nullLogger ? null : _MockPluginLoaderLogger.Object)
            { Plugin = _MockPlugin.Object };
        }

        #region Non-generic

        [TestMethod]
        public void ObjectCreator_Create_Constructorless_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Org>();

            // Act
            var result = objectCreator.Create();

            // Assert
            Assert.AreEqual(typeof(Org), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_EmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Org2>();

            // Act
            var result = objectCreator.Create();

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_CreatorIsForParentEmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Org>();
            var type = typeof(Org2);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_BaseCreateReturnsNull()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Org>();
            var type = typeof(Org2);
            objectCreator.BaseCreateMethod = (Type t) => { return null; };
            _MockPluginLoaderSettings.Setup(m=>m.ThrowExceptionsOnLoad).Returns(true);
            _MockPluginLoaderLogger.Setup(m=>m.Log(It.IsAny<PluginTypeLoadException>()));

            // Act
            // Assert
            Assert.ThrowsException<PluginTypeLoadException>(() =>
            {
                objectCreator.Create(type);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_BaseCreateReturnsNull_NullLogger()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Org>(true);
            var type = typeof(Org2);
            objectCreator.BaseCreateMethod = (Type t) => { return null; };
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(true);

            // Act
            // Assert
            Assert.ThrowsException<PluginTypeLoadException>(() =>
            {
                objectCreator.Create(type);
            });
            _MockRepository.VerifyAll();
        }
        #endregion

        #region Non-generic Interface parent

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_Constructorless_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IOrg>();
            var type = typeof(Org);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_EmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IOrg>();
            var type = typeof(Org2);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_CreatorIsForParentEmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IOrg>();
            var type = typeof(Org2);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_NoEmptyConstructor_Model_Setting_ThrowsExceptionOnLoad_True()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IRuntimePluginLoader<Org>>();
            _MockPlugin.Setup(m => m.Name).Returns("MyPlugin");
            Type type = typeof(TestPluginLoader);
            _MockPluginLoaderLogger.Setup(m => m.Log(It.IsAny<PluginTypeLoadException>()));
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(true);
            // Act
            // Assert
            Assert.ThrowsException<PluginTypeLoadException>(() =>
            {
                objectCreator.Create(type);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_NoEmptyConstructor_Model_Setting_ThrowsExceptionOnLoad_False()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IRuntimePluginLoader<Org>>();
            _MockPlugin.Setup(m=>m.Name).Returns("MyPlugin");
            Type type = typeof(TestPluginLoader);
            _MockPluginLoaderLogger.Setup(m => m.Log(It.IsAny<PluginTypeLoadException>()));
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(false);

            // Act
            var actual = objectCreator.Create(type);

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region Generic
        [TestMethod]
        public void ObjectCreator_Create_GenericWithParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Entity<Org>>();
            var type = typeof(Entity<Org>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_GenericWithoutParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<Entity<Org>>();
            var type = typeof(Entity<>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_Interface_GenericWithParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IEntity<Org>>();
            var type = typeof(Entity<Org>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_Interface_GenericWithoutParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = CreatePluginObjectCreator<IEntity<Org>>();
            var type = typeof(Entity<>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
