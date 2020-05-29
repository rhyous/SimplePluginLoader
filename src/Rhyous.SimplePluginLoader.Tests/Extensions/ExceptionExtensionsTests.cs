using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Tests.Extensions
{
    [TestClass]
    public class ExceptionExtensionsTests
    {
        private MockRepository _MockRepository;
        private Mock<IPluginLoaderLogger> _MockLogger;


        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);
            _MockLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        [TestMethod]
        public void ExceptionExtensions_LogReflectionTypeLoadExceptions_Exception_NotLogged_Test()
        {
            // Arrange
            var e = new Exception();

            // Act
            e.LogReflectionTypeLoadExceptions(_MockLogger.Object);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ExceptionExtensions_LogReflectionTypeLoadExceptions_ReflectionTypeLoadException_NoLoaderExceptions_NothingLogged_Test()
        {
            // Arrange
            var e = new ReflectionTypeLoadException(new[] { typeof(string) }, new Exception[] { });

            // Act
            e.LogReflectionTypeLoadExceptions(_MockLogger.Object);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ExceptionExtensions_LogReflectionTypeLoadExceptions_ReflectionTypeLoadException_Logged_Test()
        {
            // Arrange
            var loaderException = new Exception();
            var e = new ReflectionTypeLoadException(new[] { typeof(string) }, new[] { loaderException });
            _MockLogger.Setup(m => m.Log(It.IsAny<Exception>()));

            // Act
            e.LogReflectionTypeLoadExceptions(_MockLogger.Object);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void ExceptionExtensions_LogReflectionTypeLoadExceptions_ReflectionTypeLoadException_TwoLogs_Test()
        {
            // Arrange
            var loaderException1 = new Exception();
            var loaderException2 = new Exception();
            var e = new ReflectionTypeLoadException(new[] { typeof(string) }, new[] { loaderException1, loaderException2 });
            _MockLogger.Setup(m => m.Log(It.IsAny<Exception>()));

            // Act
            e.LogReflectionTypeLoadExceptions(_MockLogger.Object);

            // Assert
            _MockRepository.VerifyAll();
            _MockLogger.Verify(m => m.Log(It.IsAny<Exception>()), Times.Exactly(2));
        }
    }
}
