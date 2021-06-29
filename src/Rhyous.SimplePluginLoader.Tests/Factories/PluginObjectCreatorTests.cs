using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using Rhyous.SimplePluginLoader.Tests.TestClasses;
using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests.Factories
{
    [TestClass]
    public class PluginObjectCreatorTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }


        private PluginObjectCreator<T> CreatePluginObjectCreator<T>(bool nullLogger = false, Mock<IObjectCreator<T>> mockObjectCreator = null)
        {
            mockObjectCreator = mockObjectCreator ?? _MockRepository.Create<IObjectCreator<T>>();
            return new PluginObjectCreator<T>(
                _MockPluginLoaderSettings.Object,
                mockObjectCreator.Object,
                nullLogger ? null : _MockPluginLoaderLogger.Object);
        }

        #region Create

        [TestMethod]
        public void ObjectCreator_Create_TypeIsNull_ObjectCreatorReturns_Model()
        {
            // Arrange
            var mockObjectCreator = _MockRepository.Create<IObjectCreator<Org>>();
            var type = typeof(Org);
            mockObjectCreator.Setup(m => m.Create(type)).Returns(new Org());
            var objectCreator = CreatePluginObjectCreator<Org>(false, mockObjectCreator);
            var mockPlugin = _MockRepository.Create<IPlugin<Org>>();

            // Act
            var result = objectCreator.Create(mockPlugin.Object);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(type, result.GetType());
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_TypeIsNull_ObjectCreatorThrows_Model()
        {
            // Arrange
            var mockObjectCreator = _MockRepository.Create<IObjectCreator<Org>>();
            mockObjectCreator.Setup(m => m.Create(typeof(Org))).Throws(new MissingMethodException());
            var objectCreator = CreatePluginObjectCreator<Org>(false, mockObjectCreator);
            var mockPlugin = _MockRepository.Create<IPlugin<Org>>();
            mockPlugin.Setup(m => m.Name).Returns("OrgPlugin");
            _MockPluginLoaderLogger.Setup(m => m.Log(It.IsAny<PluginTypeLoadException>()));
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(false);

            // Act
            var result = objectCreator.Create(mockPlugin.Object);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_TypeSpecified_ObjectCreatorReturns_Model()
        {
            // Arrange
            var mockObjectCreator = _MockRepository.Create<IObjectCreator<Org>>();
            var type = typeof(Org2);
            mockObjectCreator.Setup(m => m.Create(typeof(Org2))).Returns(new Org2());
            var objectCreator = CreatePluginObjectCreator<Org>(false, mockObjectCreator);
            var mockPlugin = _MockRepository.Create<IPlugin<Org>>();

            // Act
            var result = objectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(type, result.GetType());
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_TypeSpecified_ObjectCreatorThrows_Model()
        {
            // Arrange
            var mockObjectCreator = _MockRepository.Create<IObjectCreator<Org>>();
            var type = typeof(Org2);
            mockObjectCreator.Setup(m => m.Create(type)).Throws(new MissingMethodException());
            var objectCreator = CreatePluginObjectCreator<Org>(false, mockObjectCreator);
            var mockPlugin = _MockRepository.Create<IPlugin<Org>>();
            mockPlugin.Setup(m => m.Name).Returns("OrgPlugin");
            _MockPluginLoaderLogger.Setup(m => m.Log(It.IsAny<PluginTypeLoadException>()));
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(false);

            // Act
            var result = objectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_TypeSpecified_ObjectCreatorReturnsNull_Model()
        {
            // Arrange
            var mockObjectCreator = _MockRepository.Create<IObjectCreator<Org>>();
            var type = typeof(Org2);
            mockObjectCreator.Setup(m => m.Create(typeof(Org2))).Returns((Org)null);
            var objectCreator = CreatePluginObjectCreator<Org>(false, mockObjectCreator);
            var mockPlugin = _MockRepository.Create<IPlugin<Org>>(); 
            mockPlugin.Setup(m => m.Name).Returns("OrgPlugin");
            _MockPluginLoaderLogger.Setup(m => m.Log(It.IsAny<PluginTypeLoadException>()));
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(true);

            // Act
            // Assert
            Assert.ThrowsException<PluginTypeLoadException>(() => 
            {
                objectCreator.Create(mockPlugin.Object, type);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ObjectCreator_Create_TypeSpecified_ObjectCreatorReturnsNull_LoggerNull_Model()
        {
            // Arrange
            var mockObjectCreator = _MockRepository.Create<IObjectCreator<Org>>();
            var type = typeof(Org2);
            mockObjectCreator.Setup(m => m.Create(typeof(Org2))).Returns((Org)null);
            var objectCreator = CreatePluginObjectCreator<Org>(true, mockObjectCreator);
            var mockPlugin = _MockRepository.Create<IPlugin<Org>>();
            mockPlugin.Setup(m => m.Name).Returns("OrgPlugin");
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad).Returns(true);

            // Act
            // Assert
            Assert.ThrowsException<PluginTypeLoadException>(() =>
            {
                objectCreator.Create(mockPlugin.Object, type);
            });
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
