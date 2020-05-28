using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class TypeLoaderTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;
        private Mock<IAssembly> _MockAssembly;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockAssembly = _MockRepository.Create<IAssembly>();
        }

        private TypeLoader<ITestPlugin> CreateTypeLoader()
        {
            return new TypeLoader<ITestPlugin>(
                _MockPluginLoaderSettings.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void TypeLoader_Load_Test()
        {
            // Arrange
            var typeLoader = CreateTypeLoader();
            var types = new Type[] { typeof(TestPlugin), typeof(Org) };
            _MockAssembly.Setup(m => m.GetTypes())
                         .Returns(types);
            _MockPluginLoaderSettings.Setup(m => m.ThrowExceptionsOnLoad)
                                     .Returns(false);

            // Act
            var result = typeLoader.Load(_MockAssembly.Object);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(typeof(TestPlugin), result[0]);
            _MockRepository.VerifyAll();
        }
    }
}
