using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class InstanceLoaderTests
    {
        private MockRepository _MockRepository;

        private Mock<IObjectCreator<ITestPlugin>> _MockObjectCreator;
        private Mock<ITypeLoader<ITestPlugin>> _MockTypeLoader;
        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;
        private Mock<IAssembly> _MockAssembly;

        [TestInitialize]
        public void InstanceLoader_TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockObjectCreator = _MockRepository.Create<IObjectCreator<ITestPlugin>>();
            _MockTypeLoader = _MockRepository.Create<ITypeLoader<ITestPlugin>>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockAssembly = _MockRepository.Create<IAssembly>();
        }

        private InstanceLoader<ITestPlugin> CreateInstanceLoader()
        {
            return new InstanceLoader<ITestPlugin>(
                _MockObjectCreator.Object,
                _MockTypeLoader.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginLoaderLogger.Object);
        }

        #region LoadInstances
        [TestMethod]
        public void InstanceLoader_LoadInstances_Constructor_ObjectCreator_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new InstanceLoader<ITestPlugin>(null,
                                                 _MockTypeLoader.Object, 
                                                 _MockPluginLoaderSettings.Object,
                                                 _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void InstanceLoader_LoadInstances_Constructor_TypeLoader_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new InstanceLoader<ITestPlugin>(_MockObjectCreator.Object,
                                                 null,
                                                 _MockPluginLoaderSettings.Object,
                                                 _MockPluginLoaderLogger.Object);
            });
        }



        [TestMethod]
        public void InstanceLoader_LoadInstances_Constructor_Settings_Null()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new InstanceLoader<ITestPlugin>(_MockObjectCreator.Object,
                                                 _MockTypeLoader.Object,
                                                 null,
                                                 _MockPluginLoaderLogger.Object);
            });
        }
        #endregion

        #region Load
        [TestMethod]
        public void InstanceLoader_LoadInstances_Assembly_Null()
        {
            // Arrange
            var instanceLoader = CreateInstanceLoader();
            IAssembly assembly = null;

            // Act
            var result = instanceLoader.Load(assembly);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void InstanceLoader_LoadTypes_Assembly_Test()
        {
            // Arrange
            var instanceLoader = CreateInstanceLoader();
            var expectedTestPlugin = new TestPlugin();
            _MockObjectCreator.Setup(m => m.Create(It.IsAny<Type>()))
                              .Returns(expectedTestPlugin);
            var types = new List<Type> { typeof(TestPlugin)};
            _MockTypeLoader.Setup(m => m.Load(_MockAssembly.Object))
                           .Returns(types);
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), 
                                                           It.IsAny<string>()));
                                   

            // Act
            var result = instanceLoader.Load(_MockAssembly.Object);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(typeof(TestPlugin), result[0].GetType());
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
