using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.Collections;
using Rhyous.SimplePluginLoader;
using Rhyous.UnitTesting;
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
        private Mock<IDirectory> _MockDirector;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoader = _MockRepository.Create<IPluginLoader<IOrg>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<IOrg>>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockDirector = _MockRepository.Create<IDirectory>();
        }

        private PluginFinder<IOrg> CreatePluginFinder()
        {
            return new PluginFinder<IOrg>(
                _MockPluginLoader.Object,
                _MockPluginObjectCreator.Object,
                _MockPluginLoaderLogger.Object)
            {
                Directory = _MockDirector.Object
            };
        }

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void PluginFinder_FindPlugin_NameNullEmptyOrWhitespace_ExpectedBehavior(string s)
        {
            // Arrange
            var pluginFinder = CreatePluginFinder();
            string pluginName = s;
            string dir = @"C:\some\path";
            IPluginObjectCreator<IOrg> pluginObjectCreator = null;

            // Act
            var result = pluginFinder.FindPlugin(pluginName, dir, pluginObjectCreator);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void PluginFinder_FindPlugin_DirNullEmptyOrWhitespace_ExpectedBehavior(string s)
        {
            // Arrange
            var pluginFinder = CreatePluginFinder();
            string pluginName = "MyPlugin";
            string dir = s;
            IPluginObjectCreator<IOrg> pluginObjectCreator = null;

            // Act
            var result = pluginFinder.FindPlugin(pluginName, dir, pluginObjectCreator);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginFinder_Dispose_Test()
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
