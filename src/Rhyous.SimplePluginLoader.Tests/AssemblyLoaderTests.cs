using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.Tests.TestClasses;
using Rhyous.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class AssemblyLoaderTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IAssemblyNameReader> _MockAssemblyNameReader;
        private Mock<IPluginLoaderLogger> _MockLogger;
        private Mock<IAssemblyCache> _MockAssemblyCache;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockAssemblyNameReader = _MockRepository.Create<IAssemblyNameReader>();
            _MockLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockAssemblyCache = _MockRepository.Create<IAssemblyCache>();
        }

        private AssemblyLoader CreateAssemblyLoader(bool nullLogger = false)
        {
            return new AssemblyLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockAssemblyCache.Object,
                _MockAssemblyNameReader.Object,
                nullLogger ? null : _MockLogger.Object);
        }

        #region Constructor
        [TestMethod]
        public void AssemblyLoader_Constuctor_Null_AppDomain()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new AssemblyLoader(null,
                                   _MockPluginLoaderSettings.Object,
                                   _MockAssemblyCache.Object,
                                   _MockAssemblyNameReader.Object,
                                   _MockLogger.Object);
            });
        }

        [TestMethod]
        public void AssemblyLoader_Constuctor_Null_PluginLoaderSettings()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new AssemblyLoader(_MockAppDomain.Object,
                                   null,
                                   _MockAssemblyCache.Object,
                                   _MockAssemblyNameReader.Object,
                                   _MockLogger.Object);
            });
        }

        [TestMethod]
        public void AssemblyLoader_Constuctor_Null_AssemblyCache()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new AssemblyLoader(_MockAppDomain.Object,
                                   _MockPluginLoaderSettings.Object,
                                   null,
                                   _MockAssemblyNameReader.Object,
                                   _MockLogger.Object);
            });
        }

        [TestMethod]
        public void AssemblyLoader_Constuctor_Null_AssemblyNameReader()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new AssemblyLoader(_MockAppDomain.Object,
                                   _MockPluginLoaderSettings.Object,
                                   _MockAssemblyCache.Object,
                                   null,
                                   _MockLogger.Object);
            });
        }
        #endregion

        #region TryLoad
        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void AssemblyLoader_TryLoad_Dll_NullEmptyOrWhitespace_Throws(string s)
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = s;
            string pdb = @"c:\path\to\my.pdb";

            // Act
            // Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                assemblyLoader.TryLoad(dll, pdb);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void AssemblyLoader_TryLoad_Pdb_NullEmptyOrWhitespace_Throws(string s)
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = s;

            // Act
            // Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                assemblyLoader.TryLoad(dll, pdb);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_TryLoad_NoVersion_NoAssemblyNameFound_LoadedHasNoVersion_Test()
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = @"c:\path\to\my.pdb";
            string version = null;
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns((AssemblyName)null);
            _MockAssemblyCache.Setup(m=>m.FindAlreadyLoadedAssembly(dll, version))
                              .Returns((IAssembly)null);
            var mockAssembly = _MockRepository.Create<IAssembly>();
            _MockAppDomain.Setup(m=>m.TryLoad(dll, pdb)).Returns(mockAssembly.Object);
            var assemblyName = new AssemblyName("my");
            mockAssembly.Setup(m=>m.GetName()).Returns(assemblyName);
            _MockAssemblyCache.Setup(m => m.Add(dll, (string)null, mockAssembly.Object))
                              .Returns(mockAssembly.Object);
            _MockPluginLoaderSettings.Setup(m=>m.LoadDependenciesProactively)
                                     .Returns(false);
            // Act
            var actual = assemblyLoader.TryLoad(dll, pdb);

            // Assert
            Assert.AreEqual(mockAssembly.Object, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_TryLoad_NoVersion_NoAssemblyNameFound_LoadedHasNoAssemblyName_Test()
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = @"c:\path\to\my.pdb";
            string version = null;
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns((AssemblyName)null);
            _MockAssemblyCache.Setup(m => m.FindAlreadyLoadedAssembly(dll, version))
                              .Returns((IAssembly)null);
            var mockAssembly = _MockRepository.Create<IAssembly>();
            _MockAppDomain.Setup(m => m.TryLoad(dll, pdb)).Returns(mockAssembly.Object);
            mockAssembly.Setup(m => m.GetName()).Returns((AssemblyName)null);
            _MockAssemblyCache.Setup(m => m.Add(dll, (string)null, mockAssembly.Object))
                              .Returns(mockAssembly.Object);
            _MockPluginLoaderSettings.Setup(m => m.LoadDependenciesProactively)
                                     .Returns(false);
            // Act
            var actual = assemblyLoader.TryLoad(dll, pdb);

            // Assert
            Assert.AreEqual(mockAssembly.Object, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_TryLoad_Version_AssemblyNameFound_AssemblyAlreadyCached_Test()
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = @"c:\path\to\my.pdb";
            string version = "2.0.0.1";
            var assemblyName = new AssemblyName("My.Assembly") { Version = new Version(version) };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns(assemblyName);
            var mockAssembly = _MockRepository.Create<IAssembly>();
            _MockAssemblyCache.Setup(m => m.FindAlreadyLoadedAssembly(dll, version))
                              .Returns(mockAssembly.Object);

            // Act
            var actual = assemblyLoader.TryLoad(dll, pdb);

            // Assert
            Assert.AreEqual(mockAssembly.Object, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_TryLoad_Version_AssemblyNameFound_NotCached_TryLoadReturnsNull_Test()
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = @"c:\path\to\my.pdb";
            string version = "2.0.0.1";
            var assemblyName = new AssemblyName("My.Assembly") { Version = new Version(version) };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns(assemblyName);
            _MockAssemblyCache.Setup(m => m.FindAlreadyLoadedAssembly(dll, version))
                              .Returns((IAssembly)null);
            _MockAppDomain.Setup(m => m.TryLoad(dll, pdb)).Returns((IAssembly)null);

            // Act
            var actual = assemblyLoader.TryLoad(dll, pdb);

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_TryLoad_Version_AssemblyLoadedAndCached_Test()
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = @"c:\path\to\my.pdb";
            string version = "2.0.0.1";
            _MockAssemblyCache.Setup(m => m.FindAlreadyLoadedAssembly(dll, version))
                              .Returns((IAssembly)null);

            var mockAssembly = _MockRepository.Create<IAssembly>();
            _MockAppDomain.Setup(m => m.TryLoad(dll, pdb)).Returns(mockAssembly.Object);

            var assemblyName = new AssemblyName("My.Assembly") { Version = new Version(version) };
            mockAssembly.Setup(m => m.GetName()).Returns(assemblyName);

            _MockAssemblyCache.Setup(m => m.Add(dll, version, mockAssembly.Object))
                              .Returns(mockAssembly.Object);
            _MockPluginLoaderSettings.Setup(m => m.LoadDependenciesProactively).Returns(false);

            // Act
            var actual = assemblyLoader.TryLoad(dll, pdb, version);

            // Assert
            Assert.AreEqual(mockAssembly.Object, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_TryLoad_Version_DifferentVersionAssemblyLoadedAndCached_Test()
        {
            // Arrange
            var assemblyLoader = CreateAssemblyLoader();
            string dll = @"c:\path\to\my.dll";
            string pdb = @"c:\path\to\my.pdb";
            string version = "2.0.0.1";
            string dllAssemblyVersion = "2.1.2.0";
            _MockAssemblyCache.Setup(m => m.FindAlreadyLoadedAssembly(dll, version))
                              .Returns((IAssembly)null);

            var mockAssembly = _MockRepository.Create<IAssembly>();
            _MockAppDomain.Setup(m => m.TryLoad(dll, pdb)).Returns(mockAssembly.Object);

            var assemblyName = new AssemblyName("My.Assembly") { Version = new Version(dllAssemblyVersion) };
            mockAssembly.Setup(m => m.GetName()).Returns(assemblyName);

            _MockAssemblyCache.Setup(m => m.Add(dll, dllAssemblyVersion, mockAssembly.Object))
                              .Returns(mockAssembly.Object);

            // Act
            var actual = assemblyLoader.TryLoad(dll, pdb, version);

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
