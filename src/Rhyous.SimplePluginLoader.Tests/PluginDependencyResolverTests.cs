using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginDependencyResolverTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IPluginLoaderSettings> _MockIPluginLoaderSettings;
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPlugin> _MockPlugin;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockIPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockPlugin = _MockRepository.Create<IPlugin>();
        }

        private PluginDependencyResolver<T> CreatePluginDependencyResolver<T>()
            where T : class
        {
            return new PluginDependencyResolver<T>(_MockAppDomain.Object, _MockIPluginLoaderSettings.Object, _MockAssemblyLoader.Object)
            {
                Plugin = _MockPlugin.Object
            };
        }

        #region Constructor

        [TestMethod]
        public void PluginDependencyResolver_Constructor_NullAppDomain_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new PluginDependencyResolver<Org>(
                    null,
                    _MockIPluginLoaderSettings.Object,
                    _MockAssemblyLoader.Object);                
            });
        }

        [TestMethod]
        public void PluginDependencyResolver_Constructor_NullIPluginLoaderSettings_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new PluginDependencyResolver<Org>(
                    _MockAppDomain.Object,
                    null,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void PluginDependencyResolver_Constructor_NullAssemblyLoade_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new PluginDependencyResolver<Org>(
                    _MockAppDomain.Object,
                    _MockIPluginLoaderSettings.Object,
                    null);
            });
        }
        #endregion

        [TestMethod]
        public void PluginDependencyResolver_GetPaths_BasicTest()
        {
            // Arrange
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m => m.SharedPaths).Returns(new[] { @"c:\bin", @"c:\sharedbin\", @"c:\Libs" });
            var expectedPaths = new List<string> {
                "",
                "c:\\my\\plugins",
                "c:\\my\\plugins\\bin",
                "c:\\my\\plugins\\MyPlugin",
                @"c:\bin",
                @"c:\sharedbin\",
                @"c:\Libs" };

            var resolver = CreatePluginDependencyResolver<IOrg>();

            // Act
            var actualpaths = resolver.Paths;

            // Assert
            CollectionAssert.AreEqual(expectedPaths, actualpaths);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_Dispose_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var pluginDependencyResolver = CreatePluginDependencyResolver<Org>();

            // Act
            pluginDependencyResolver.Dispose();

            // Assert
            _MockRepository.VerifyAll();
        }

        #region AssemblyResolveHandler
        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_NullPlugin()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            var pluginDependencyResolver = new PluginDependencyResolver<Org>(_MockAppDomain.Object, _MockIPluginLoaderSettings.Object, _MockAssemblyLoader.Object);

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_NullSharedPaths()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m=>m.SharedPaths).Returns((IEnumerable<string>)null);

            var pluginDependencyResolver = CreatePluginDependencyResolver<Org>();

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_EmptyPaths()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            var pluginDependencyResolver = CreatePluginDependencyResolver<Org>();
            pluginDependencyResolver.Paths = new List<string>();

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_AllPathsAlreadyTried()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m => m.SharedPaths).Returns((IEnumerable<string>)null);
            var pluginDependencyResolver = CreatePluginDependencyResolver<Org>();
            pluginDependencyResolver._AttemptedPaths = new ConcurrentDictionary<string, List<string>>();
            var pathList = new List<string> {
                                "",
                                "c:\\my\\plugins" ,
                                "c:\\my\\plugins\\bin" ,
                                "c:\\my\\plugins\\MyPlugin" ,
                                @"c:\bin" ,
                                @"c:\sharedbin\" ,
                                @"c:\Libs"
                            };
            pluginDependencyResolver._AttemptedPaths.TryAdd("name", pathList);

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            var pluginDependencyResolver = CreatePluginDependencyResolver<Org>();
            pluginDependencyResolver.Paths = new List<string>();


            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
