using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class AssemblyExtensionsTests
    {
        private MockRepository _MockRepository;
        private Mock<IAssembly> _MockAssembly;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;


        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAssembly = _MockRepository.Create<IAssembly>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_NullAssembly()
        {
            // Arrange
            IAssembly assembly = null;
            bool throwException = false;

            // Act
            var result = assembly.TryGetTypes(throwException,
                                              _MockPluginLoaderLogger.Object);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_ReturnsTypes()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };
            _MockAssembly.Setup(m => m.GetTypes()).Returns(types);
            IAssembly assembly = _MockAssembly.Object;
            bool throwException = false;

            // Act
            var result = assembly.TryGetTypes(throwException,
                                              _MockPluginLoaderLogger.Object);

            // Assert
            Assert.AreEqual(types, result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_GetTypesThrows()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };
            var exception1 = new Exception("Message 1");
            var loaderExceptions = new[] { exception1 };
            var reflectionTypeLoadException = new ReflectionTypeLoadException(types, loaderExceptions);
            _MockAssembly.Setup(m => m.GetTypes()).Throws(reflectionTypeLoadException);

            _MockPluginLoaderLogger.Setup(m => m.Log(It.IsAny<Exception>()));

            IAssembly assembly = _MockAssembly.Object;
            bool throwException = false;

            // Act
            var result = assembly.TryGetTypes(throwException,
                                              _MockPluginLoaderLogger.Object);

            // Assert
            CollectionAssert.AreEqual(types, result.ToArray());
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_GetTypesThrows_TwoLoaderExceptions()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };
            var exception1 = new Exception("Message1");
            var exception2 = new Exception("Message2");
            var loaderExceptions = new[] { exception1, exception2 };
            var reflectionTypeLoadException = new ReflectionTypeLoadException(types, loaderExceptions);
            _MockAssembly.Setup(m => m.GetTypes()).Throws(reflectionTypeLoadException);

            _MockPluginLoaderLogger.Setup(m => m.Log(exception1));
            _MockPluginLoaderLogger.Setup(m => m.Log(exception2));

            IAssembly assembly = _MockAssembly.Object;
            bool throwException = false;

            // Act
            var result = assembly.TryGetTypes(throwException,
                                              _MockPluginLoaderLogger.Object);

            // Assert
            CollectionAssert.AreEqual(types, result.ToArray());
            _MockRepository.VerifyAll();
        }

        class TestException : Exception
        {
            public TestException(string message) : base(message)
            {
            }
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_GetTypesThrows_OneLoaderException_RethrowTrue()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };
            var exception1 = new TestException("Message1");
            var loaderExceptions = new[] { exception1 };
            var reflectionTypeLoadException = new ReflectionTypeLoadException(types, loaderExceptions);
            _MockAssembly.Setup(m => m.GetTypes()).Throws(reflectionTypeLoadException);

            _MockPluginLoaderLogger.Setup(m => m.Log(exception1));

            IAssembly assembly = _MockAssembly.Object;
            bool throwException = true;

            // Act
            // Assert
            Assert.ThrowsException<TestException>(() =>
            {
                assembly.TryGetTypes(throwException,
                                     _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_GetTypesThrows_TwoLoaderExceptions_RethrowTrue()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };
            var exception1 = new Exception("Message1");
            var exception2 = new Exception("Message2");
            var loaderExceptions = new[] { exception1, exception2 };
            var reflectionTypeLoadException = new ReflectionTypeLoadException(types, loaderExceptions);
            _MockAssembly.Setup(m => m.GetTypes()).Throws(reflectionTypeLoadException);

            _MockPluginLoaderLogger.Setup(m => m.Log(exception1));
            _MockPluginLoaderLogger.Setup(m => m.Log(exception2));

            IAssembly assembly = _MockAssembly.Object;
            bool throwException = true;

            // Act
            // Assert
            Assert.ThrowsException<ReflectionTypeLoadException>(() =>
            {
                assembly.TryGetTypes(throwException,
                                     _MockPluginLoaderLogger.Object);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyExtensions_TryGetTypes_GetTypesThrows_TwoLoaderExceptions_Rethrow_LoggerNull_True()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };
            var exception1 = new Exception("Message1");
            var exception2 = new Exception("Message2");
            var loaderExceptions = new[] { exception1, exception2 };
            var reflectionTypeLoadException = new ReflectionTypeLoadException(types, loaderExceptions);
            _MockAssembly.Setup(m => m.GetTypes()).Throws(reflectionTypeLoadException);

            IAssembly assembly = _MockAssembly.Object;
            bool throwException = true;

            // Act
            // Assert
            Assert.ThrowsException<ReflectionTypeLoadException>(() =>
            {
                assembly.TryGetTypes(throwException, null);
            });
            _MockRepository.VerifyAll();
        }
    }
}
