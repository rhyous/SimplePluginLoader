using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using T = Rhyous.SimplePluginLoader.Tests.Org;


namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginTests
    {
        private MockRepository _MockRepository;

        private Mock<ITypeLoader<T>> _MockTypeLoader;
        private Mock<IPluginObjectCreator<T>> _MockPluginObjectCreator;
        private Mock<IPluginDependencyResolver> _MockPluginDependencyResolver;
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockTypeLoader = _MockRepository.Create<ITypeLoader<T>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<T>>();
            _MockPluginDependencyResolver = _MockRepository.Create<IPluginDependencyResolver>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();

        }

        private Plugin<T> CreatePlugin(bool includeMocks = false)
        {
            if (includeMocks)
            {
                _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
                _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
                _MockPluginDependencyResolver.Setup(m => m.Plugin).Returns((IPlugin)null);
            }
            return new Plugin<T>(
                _MockTypeLoader.Object,
                _MockPluginObjectCreator.Object,
                _MockPluginDependencyResolver.Object,
                _MockAssemblyLoader.Object);
        }

        #region Constructor

        [TestMethod]
        public void Plugin_Constructor_NullTypeLoader_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    null,
                    _MockPluginObjectCreator.Object,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullInstanceLoader_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockTypeLoader.Object,
                    null,
                    _MockPluginDependencyResolver.Object,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullPluginDependencyResolver_Test()
        {
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockTypeLoader.Object,
                    _MockPluginObjectCreator.Object,
                    null,
                    _MockAssemblyLoader.Object);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_NullAssemblyLoader_Test()
        {
            _MockPluginObjectCreator.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            _MockPluginDependencyResolver.SetupSet(m => m.Plugin = It.IsAny<IPlugin>());
            _MockPluginDependencyResolver.Setup(m => m.Plugin).Returns((IPlugin)null);
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new Plugin<T>(
                    _MockTypeLoader.Object,
                    _MockPluginObjectCreator.Object,
                    _MockPluginDependencyResolver.Object,
                    null);
            });
        }

        [TestMethod]
        public void Plugin_Constructor_Test()
        {
            // Arrange            
            // Act
            CreatePlugin(true);

            // Assert
            _MockRepository.VerifyAll();
        }

        #endregion

        #region FilePdb
        [TestMethod]
        public void Plugin_FilePdb_Test()
        {
            // Arrange
            var dll = @"C:\test\library.dll";
            var pdb = @"C:\test\library.pdb";

            // Act
            var plugin = CreatePlugin(true);
            plugin.File = dll;

            // Assert
            Assert.AreEqual(pdb, plugin.FilePdb);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_FilePdb_InvalidPath_Test()
        {
            // Arrange
            var dll = @"Invalid";

            // Act
            var plugin = CreatePlugin(true);
            plugin.File = dll;

            // Assert
            Assert.AreEqual(null, plugin.FilePdb);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region Dispose
        [TestMethod]
        public void Plugin_Dispose_Test()
        {
            // Arrange
            _MockPluginDependencyResolver.Setup(m => m.RemoveDependencyResolver());
            var plugin = CreatePlugin(true);
            bool wasCalled = false;
            ResolveEventHandler handler = (object sender, ResolveEventArgs args) =>
            {
                wasCalled = true;
                return Assembly.GetExecutingAssembly();
            };

            _MockPluginDependencyResolver.Setup(m => m.Dispose());

            // Act
            plugin.Dispose();

            // Assert
            Assert.IsFalse(wasCalled, "The removed handle should not be called.");
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_Dispose_Called_Twice()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            _MockPluginDependencyResolver.Setup(m => m.RemoveDependencyResolver());
            _MockPluginDependencyResolver.Setup(m => m.Dispose());

            // Act
            plugin.Dispose();
            plugin.Dispose();

            // Assert
            _MockPluginDependencyResolver.Verify(m => m.RemoveDependencyResolver(), Times.Once);
            _MockPluginDependencyResolver.Verify(m => m.Dispose(), Times.Once);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region CreatePluginObjects
        [TestMethod]
        public void Plugin_CreatePluginObjects_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.PluginTypes = new List<Type> { typeof(Org2), null, typeof(Org) };
            _MockPluginObjectCreator.Setup(m => m.Create(typeof(Org2))).Returns(new Org2());
            _MockPluginObjectCreator.Setup(m => m.Create(null)).Returns((Org)null);
            _MockPluginObjectCreator.Setup(m => m.Create(typeof(Org))).Returns(new Org());

            // Act
            var actual = plugin.CreatePluginObjects();

            // Assert
            Assert.AreEqual(2, actual.Count);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_CreatePluginObjects_PluginTypes_Null_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            var mockAssembly = _MockRepository.Create<IAssembly>();
            plugin.Assembly = mockAssembly.Object;
            _MockTypeLoader.Setup(m=>m.Load(mockAssembly.Object)).Returns((List<Type>)null);
            _MockPluginDependencyResolver.Setup(m => m.AddDependencyResolver());
            _MockPluginDependencyResolver.Setup(m => m.RemoveDependencyResolver());
            _MockPluginDependencyResolver.Setup(m => m.Plugin).Returns(plugin);


            // Act
            var actual = plugin.CreatePluginObjects();

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region Assembly

        [TestMethod]
        public void Plugin_Assembly_AlreadySet_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            var mockAssembly = _MockRepository.Create<IAssembly>();
            plugin.Assembly = mockAssembly.Object;

            // Act
            var assembly = plugin.Assembly;

            // Assert
            Assert.AreEqual(mockAssembly.Object, assembly);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_Assembly_Null()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            var path = @"C:\Path\to";
            plugin.Directory = path;
            var file = @"MyAssembly.dll";
            var pdb = @"MyAssembly.pdb";
            plugin.File = file;
            var mockAssembly = _MockRepository.Create<IAssembly>();
            _MockAssemblyLoader.Setup(m=>m.TryLoad($@"{path}\{file}", $@"{path}\{pdb}", null))
                               .Returns(mockAssembly.Object);

            // Act
            var assembly = plugin.Assembly;

            // Assert
            Assert.AreEqual(mockAssembly.Object, assembly);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region PluginTypes

        [TestMethod]
        public void Plugin_PluginTypes_AlreadySet_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.PluginTypes = new List<Type>();

            // Act
            var types = plugin.PluginTypes;

            // Assert
            IEnumerableAssert.IsNullOrEmpty(types, "Empty");
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [ListTNullOrEmpty(typeof(Type))]
        public void Plugin_PluginTypes_NullOrEmptyTypesReturned_Test(List<Type> list, string testTitle)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            _MockPluginDependencyResolver.Setup(m => m.AddDependencyResolver());
            var mockAssembly = _MockRepository.Create<IAssembly>();
            plugin.Assembly = mockAssembly.Object;
            _MockTypeLoader.Setup(m => m.Load(mockAssembly.Object)).Returns(list);
            _MockPluginDependencyResolver.Setup(m => m.RemoveDependencyResolver());
            _MockPluginDependencyResolver.Setup(m => m.Plugin).Returns(plugin);

            // Act
            var types = plugin.PluginTypes;

            // Assert
            IEnumerableAssert.IsNullOrEmpty(types, testTitle);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [ListTNullOrEmpty(typeof(Type))]
        public void Plugin_PluginTypes_NullOrEmptyTypesReturned_SubPlugin_Test(List<Type> list, string testTitle)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            _MockPluginDependencyResolver.Setup(m => m.AddDependencyResolver());
            var mockAssembly = _MockRepository.Create<IAssembly>();
            plugin.Assembly = mockAssembly.Object;
            _MockTypeLoader.Setup(m => m.Load(mockAssembly.Object)).Returns(list);
            var mockPlugin = _MockRepository.Create<IPlugin>();
            _MockPluginDependencyResolver.Setup(m => m.Plugin).Returns(mockPlugin.Object);

            // Act
            var types = plugin.PluginTypes;

            // Assert
            IEnumerableAssert.IsNullOrEmpty(types, testTitle);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region FullPath
        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void Plugin_FullPath_DirectoryNull_Test(string dir)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.Directory = dir;
            plugin.File = "MyAssembly.dll";

            // Act
            // Assert
            Assert.ThrowsException<PluginPathUndefinedException>(() => 
            {
                var actual = plugin.FullPath;
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void Plugin_FullPath_FileNull_Test(string file)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.Directory = @"C:\Path\to";
            plugin.File = file;

            // Act
            // Assert
            Assert.ThrowsException<PluginPathUndefinedException>(() =>
            {
                var actual = plugin.FullPath;
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_FullPath_DirectoryAndFileSet_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.Directory = @"C:\Path\to";
            plugin.File = "MyAssembly.dll";

            // Act
            var actual = plugin.FullPath;

            // Assert
            Assert.AreEqual(@"C:\Path\to\MyAssembly.dll", actual);
            _MockRepository.VerifyAll();
        }
        #endregion


        #region FullPathPdb

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void Plugin_FullPathPdb_DirectoryNull_Test(string directory)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.Directory = directory;
            plugin.File = "MyAssembly.dll";

            // Act
            // Assert
            Assert.ThrowsException<PluginPathUndefinedException>(() =>
            {
                var actual = plugin.FullPathPdb;
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void Plugin_FullPathPdb_FileNull_Test(string file)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.Directory = @"C:\Path\to";
            plugin.File = file;

            // Act
            // Assert
            Assert.ThrowsException<PluginPathUndefinedException>(() =>
            {
                var actual = plugin.FullPathPdb;
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_FullPathPdb_DirectoryAndFileSet_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.Directory = @"C:\Path\to";
            plugin.File = "MyAssembly.dll";

            // Act
            var actual = plugin.FullPathPdb;

            // Assert
            Assert.AreEqual(@"C:\Path\to\MyAssembly.pdb", actual);
            _MockRepository.VerifyAll();
        }
        #endregion


        #region Name

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void Plugin_Name_FileNullEmptyOrWhitespace_Test(string file)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.File = file;

            // Act
            // Assert
            Assert.ThrowsException<PluginPathUndefinedException>(() =>
            {
                var actual = plugin.Name;
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void Plugin_Name_FileSet_Test()
        {
            // Arrange
            var plugin = CreatePlugin(true);
            plugin.File = "MyAssembly.dll";

            // Act
            var actual = plugin.Name;

            // Assert
            Assert.AreEqual("MyAssembly", actual);
            _MockRepository.VerifyAll();
        }
        #endregion


        #region GetPluginTypes
        [TestMethod]
        [ListTNullOrEmpty(typeof(Type))]
        public void Plugin_GetPluginTypes_NullOrEmptyTypesReturned_Test(List<Type> list, string testTitle)
        {
            // Arrange
            var plugin = CreatePlugin(true);
            _MockPluginDependencyResolver.Setup(m => m.AddDependencyResolver());
            var mockAssembly = _MockRepository.Create<IAssembly>();
            plugin.Assembly = mockAssembly.Object;
            _MockTypeLoader.Setup(m => m.Load(mockAssembly.Object)).Returns(list);
            _MockPluginDependencyResolver.Setup(m => m.RemoveDependencyResolver());
            _MockPluginDependencyResolver.Setup(m => m.Plugin).Returns(plugin);

            // Act
            var types = plugin.GetPluginTypes();

            // Assert
            IEnumerableAssert.IsNullOrEmpty(types, testTitle);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}