using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using TValue = Rhyous.SimplePluginLoader.Tests.IOrg;
using TKey = System.String;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class CacheFactoryTests
    {
        private MockRepository _MockRepository;

        private Mock<IObjectCreator<TValue>> _MockObjectCreator;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockObjectCreator = _MockRepository.Create<IObjectCreator<TValue>>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private CacheFactory<TKey, TValue> CreateFactory()
        {
            return new CacheFactory<TKey, TValue>(
                _MockObjectCreator.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void CacheFactory_Create_Key_Null_Test()
        {
            // Arrange
            var factory = CreateFactory();
            TKey key = default(TKey);
            Type t = typeof(Org);

            // Act
            var result = factory.Create(key, t);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void CacheFactory_Create_Type_Null_Test()
        {
            // Arrange
            var factory = CreateFactory();
            TKey key = "Org1";
            Type t = null;

            // Act
            var result = factory.Create(key, t);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void CacheFactory_Create_Type_NotInstantiable_Test()
        {
            // Arrange
            var factory = CreateFactory();
            TKey key = "Org1";
            Type t = typeof(IOrg);

            // Act
            var result = factory.Create(key, t);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void CacheFactory_Create_ItemNotCached_ItemCreated_Test()
        {
            // Arrange
            var factory = CreateFactory();
            TKey key = "Org1";
            Type typeToCreate = typeof(Org);
            Type typeOfTValue = typeof(TValue);
            var org1 = new Org();
            _MockObjectCreator.Setup(m=>m.Create(typeOfTValue))
                              .Returns(org1);

            // Act
            var result = factory.Create(key, typeToCreate);

            // Assert
            Assert.AreEqual(org1, result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void CacheFactory_Create_ItemNotCached_OjbectCreatorThrows_Test()
        {
            // Arrange
            var factory = CreateFactory();
            TKey key = "Org1";
            Type typeToCreate = typeof(Org);
            Type typeOfTValue = typeof(TValue);
            var org1 = new Org();
            var msg = "Exception occurred creating an object.";
            var exception = new Exception(msg);
            _MockObjectCreator.Setup(m => m.Create(typeOfTValue))
                              .Throws(exception);
            _MockPluginLoaderLogger.Setup(m=>m.Log(exception));

            // Act
            // Assert
            Assert.ThrowsException<Exception>(() => 
            {
                factory.Create(key, typeToCreate);
            });            
            _MockRepository.VerifyAll();
        }
    }
}