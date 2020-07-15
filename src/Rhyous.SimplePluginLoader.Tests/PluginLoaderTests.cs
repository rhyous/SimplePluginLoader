using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginLoaderTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<ITypeLoader<IOrg>> _MockTypeLoader;
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPluginCacheFactory<IOrg>> _MockCacheFactory;
        private Mock<IPluginLoaderLogger> _MockLogger;
        private Mock<IDirectory> _MockDirectory;
        private Mock<IFile> _MockFile;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockTypeLoader = _MockRepository.Create<ITypeLoader<IOrg>>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockCacheFactory = _MockRepository.Create<IPluginCacheFactory<IOrg>>();
            _MockLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockDirectory = _MockRepository.Create<IDirectory>();
            _MockFile = _MockRepository.Create<IFile>();
        }

        private PluginLoader<IOrg> CreatePluginLoader(string appName, string subFolder)
        {
            return new PluginLoader<IOrg>(
                new AppPluginPaths(appName, subFolder, _MockAppDomain.Object, _MockLogger.Object),                
                _MockCacheFactory.Object)
            {
                Directory = _MockDirectory.Object,
                File = _MockFile.Object
            };
        }

        [TestMethod]
        public void PluginLoader_Create_Static_OnlyAppName_Test()
        {
            // Arrange
            string appName = "MyApp";
            
            // Act
            var result = PluginLoader<IOrg>.Create(appName);

            // Assert
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoader_Create_Static_AllParams_Test()
        {
            // Arrange
            string appName = "MyApp";
            AppPluginPaths paths = null;
            var mockSettings = _MockRepository.Create<IPluginLoaderSettings>();
            var mockPluginObjectCreatorFactory = _MockRepository.Create<IPluginObjectCreatorFactory<IOrg>>();
            var mockAssemblyCache = _MockRepository.Create<IAssemblyCache>();
            var mockAssemblyNameReader = _MockRepository.Create<IAssemblyNameReader>();
            var mockPluginDependencyResolverObjectCreator = _MockRepository.Create<IPluginDependencyResolverObjectCreator>();
            var mockPluginDependencyResolverFactory = _MockRepository.Create<IPluginDependencyResolverCacheFactory>();
            var mockPluginCacheFactory = _MockRepository.Create<IPluginCacheFactory<IOrg>>();
            var mockLogger = _MockRepository.Create<IPluginLoaderLogger>();

            // Act
            var result = PluginLoader<IOrg>.Create(
                appName,
                paths,
                _MockAppDomain.Object,
                mockSettings.Object,
                _MockTypeLoader.Object,
                mockPluginObjectCreatorFactory.Object,
                mockAssemblyCache.Object,
                mockAssemblyNameReader.Object,
                _MockAssemblyLoader.Object,
                mockPluginDependencyResolverObjectCreator.Object,
                mockPluginDependencyResolverFactory.Object,
                mockPluginCacheFactory.Object,
                mockLogger.Object);

            // Assert
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoader_LoadPlugins_CallingTwiceEvenFromSeparateInstancesReturnsSamePluginInstance()
        {
            // Arrange
            var path = @"Plugins\Sub";
            _MockAppDomain.Setup(m => m.BaseDirectory).Returns(@"C:\Plugins\");
            
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>()));
            _MockDirectory.Setup(m => m.Exists(path)).Returns(true);
            var currentPathPluginDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
            _MockDirectory.Setup(m => m.GetFiles(currentPathPluginDir, "*.dll", SearchOption.AllDirectories))
                          .Returns(new[] { "plugin1.dll", "plugin2.dll" });
            _MockFile.Setup(m => m.Exists("plugin1.dll")).Returns(true);
            _MockFile.Setup(m => m.Exists("plugin2.dll")).Returns(true);

            var userPluginDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "MyApp", path);
            _MockDirectory.Setup(m => m.Exists(userPluginDir)).Returns(true);
            _MockDirectory.Setup(m => m.GetFiles(userPluginDir, "*.dll", SearchOption.AllDirectories))
                          .Returns(new string[0]);

            var appDataPluginDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyApp", path);
            _MockDirectory.Setup(m => m.Exists(appDataPluginDir)).Returns(true);
            _MockDirectory.Setup(m => m.GetFiles(appDataPluginDir, "*.dll", SearchOption.AllDirectories))
                          .Returns(new string[0]);

            var basePluginDir = Path.Combine(_MockAppDomain.Object.BaseDirectory, path);
            _MockDirectory.Setup(m => m.Exists(basePluginDir)).Returns(true);
            _MockDirectory.Setup(m => m.GetFiles(basePluginDir, "*.dll", SearchOption.AllDirectories))
                          .Returns(new string[0]);

            var mockPlugin1 = new Mock<IPlugin<IOrg>>();
            var mockPlugin2 = new Mock<IPlugin<IOrg>>();

            _MockCacheFactory.Setup(m => m.Create("plugin1.dll", typeof(Plugin<IOrg>))).Returns(mockPlugin1.Object);
            _MockCacheFactory.Setup(m => m.Create("plugin2.dll", typeof(Plugin<IOrg>))).Returns(mockPlugin2.Object);

            var pluginLoader1 = CreatePluginLoader("MyApp", "Sub");

            var pluginLoader2 = CreatePluginLoader("MyApp", "Sub");

            // Act
            var result1 = pluginLoader1.LoadPlugins();
            var result2 = pluginLoader2.LoadPlugins();

            // Assert
            Assert.AreEqual(result1[0], result2[0]);
            Assert.AreEqual(result1[1], result2[1]);
            _MockRepository.VerifyAll();
        }
    }
}
