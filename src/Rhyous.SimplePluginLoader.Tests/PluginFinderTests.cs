using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.Collections;
using Rhyous.SimplePluginLoader;
using System;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginFinderTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoader<IOrg>> _MockPluginLoader;
        private Mock<IPluginObjectCreator<IOrg>> _MockPluginObjectCreator;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoader = _MockRepository.Create<IPluginLoader<IOrg>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<IOrg>>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private PluginFinder<IOrg> CreatePluginFinder()
        {
            return new PluginFinder<IOrg>(
                _MockPluginLoader.Object,
                _MockPluginObjectCreator.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void PluginFinder_FindPlugin_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var pluginFinder = CreatePluginFinder();
            string pluginName = null;
            string dir = null;
            IPluginObjectCreator<IOrg> pluginObjectCreator = null;

            // Act
            var result = pluginFinder.FindPlugin(pluginName, dir, pluginObjectCreator);

            // Assert
            Assert.Fail();
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginFinder_Dispose_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var pluginFinder = CreatePluginFinder();

            // Act
            pluginFinder.Dispose();

            // Assert
            Assert.IsTrue((bool)pluginFinder.GetType().GetFieldInfo("_Disposed").GetValue(pluginFinder));
            _MockRepository.VerifyAll();
        }
    }
}
