using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using Rhyous.SimplePluginLoader.Tests.TestClasses;
using Rhyous.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class AssemblyCacheTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IAssemblyNameReader> _MockAssemblyNameReader;
        private Mock<IPluginLoaderLogger> _MockLogger;
        private Mock<IFile> _MockFile;
        private Mock<IAssembly> _MockAssembly;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockAssemblyNameReader = _MockRepository.Create<IAssemblyNameReader>();
            _MockLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockFile = _MockRepository.Create<IFile>();
            _MockAssembly = _MockRepository.Create<IAssembly>();
        }

        private AssemblyCache CreateAssemblyCache(bool nullLogger = false)
        {
            return new AssemblyCache(
                _MockAppDomain.Object,
                _MockAssemblyNameReader.Object,
                nullLogger ? null : _MockLogger.Object)
            { File = _MockFile.Object };
        }

        #region Add
        [TestMethod]
        [StringIsNullEmptyOrWhitespace]
        public void AssemblyLoader_Add_Dll_NullEmptyOrWhitespace(string s)
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            string dll = s;
            string version = "2.0.0.1";
            IAssembly assembly = _MockAssembly.Object;

            // Act
            // Assert
            Assert.ThrowsException<ArgumentException>(() =>
            {
                assemblyCache.Add(dll, version, assembly);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_Add_Assembly_Null()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            string dll = @"c:\Path\To\MyAssembly.dll";
            string version = "2.0.0.1";
            IAssembly assembly = null;

            // Act
            // Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                assemblyCache.Add(dll, version, assembly);
            });
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_Add_Assembly_NoVersion_AssemblyHasVersion()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            string name = "MyAssembly";
            string dll = $@"c:\Path\To\{name}.dll";
            string version = null;
            var assemblyVersion = new Version("2.0.0.1");
            IAssembly assembly = _MockAssembly.Object;
            var assemblyName = new AssemblyName() { Version = assemblyVersion };
            _MockAssembly.Setup(m => m.GetName()).Returns(assemblyName);
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.Add(dll, version, assembly);

            // Assert
            Assert.IsTrue(assemblyCache.Assemblies.TryGetValue($"{name}_{assemblyVersion}_{lastFileWriteTime.ToFileTimeUtc()}", out _));
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_Add_Assembly_NoVersion_AssemblyMissingVersion()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            string name = "MyAssembly";
            string dll = $@"c:\Path\To\{name}.dll";
            string version = null;
            IAssembly assembly = _MockAssembly.Object;
            var assemblyName = new AssemblyName() { };
            _MockAssembly.Setup(m => m.GetName()).Returns(assemblyName);
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.Add(dll, version, assembly);

            // Assert
            Assert.IsTrue(assemblyCache.Assemblies.TryGetValue($"{name}_{lastFileWriteTime.ToFileTimeUtc()}", out _));
            _MockRepository.VerifyAll();
        }
        #endregion

        #region FindAlreadyLoadedAssembly

        public static IEnumerable<object[]> NullOrEmptyAssemblies
        {
            get => new[] {
                new [] { (IAssembly[])null },
                new [] { new IAssembly[0] }
            };
        }

        [TestMethod]
        [ArrayNullOrEmpty(typeof(IAssembly[]))]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_NothingLoaded_NullOrEmptyAssembliesInAppDomain(IAssembly[] assemblies, string testTitle)
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            var dll = "file.dll";
            var version = "1.0.0.1";
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };
            _MockAppDomain.Setup(m => m.GetAssemblies()).Returns(assemblies);

            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.IsNull(actual, testTitle);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_NothingLoaded_AssemblyInAppDomain()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            var name = "MyAssembly";
            var dll = $"{name}.dll";
            var version = "1.0.0.1";
            var assemblyName = new AssemblyName(name) { Version = new Version(version) };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns(assemblyName);
            _MockAssembly.Setup(m => m.GetName()).Returns(assemblyName);
            _MockAppDomain.Setup(m => m.GetAssemblies()).Returns(new[] { _MockAssembly.Object });
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var msg = $"Loading plugin assembly from dll: {name}_{version}_{lastFileWriteTime.ToFileTimeUtc()}";
            _MockLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, msg));

            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(_MockAssembly.Object, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_NothingLoaded_AssemblyInAppDomain_NullLogger()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache(true);
            var name = "MyAssembly";
            var dll = $"{name}.dll";
            var version = "1.0.0.1";
            var assemblyName = new AssemblyName(name) { Version = new Version(version) };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns(assemblyName);
            _MockAssembly.Setup(m => m.GetName()).Returns(assemblyName);
            _MockAppDomain.Setup(m => m.GetAssemblies()).Returns(new[] { _MockAssembly.Object });
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var msg = $"Loading plugin assembly from dll: {name}_{version}_{lastFileWriteTime.ToFileTimeUtc()}";

            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(_MockAssembly.Object, actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_OnlyDifferentVersionLoaded()
        {
            // Arrange
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain, _MockLogger.Object);
            var logLines = new List<string>();
            _MockAppDomain.Setup(m => m.GetAssemblies()).Returns(appDomain.GetAssemblies());
            var assemblyCache = CreateAssemblyCache();
            var assembly = "file2";
            var dll = $"{assembly}.dll";
            var oldversion = "1.0.0.0";
            var version = "1.0.0.1";
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var key = $"{assembly}_{oldversion}_{lastFileWriteTime.ToFileTimeUtc()}";
            var testAssembly = appDomain.GetAssemblies().First();
            assemblyCache.Assemblies.TryAdd(key, testAssembly);
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(It.IsAny<string>())).Returns(assemblyName);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.IsNull(actual);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_AlreadyLoaded()
        {
            // Arrange
            var logLines = new List<string>();
            var assemblyCache = CreateAssemblyCache();
            var assembly = "file3";
            var dll = $"{assembly}.dll";
            var version = "1.0.0.1";
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var key = $"{assembly}_{version}_{lastFileWriteTime.ToFileTimeUtc()}";
            assemblyCache.Assemblies.TryAdd(key, _MockAssembly.Object);
            var assemblyName = new AssemblyName { Version = new Version(version) };
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), $"Found already loaded plugin assembly: {key}"));

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(actual, _MockAssembly.Object);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_AlreadyLoaded_VersionNull()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            var assembly = "file3";
            var dll = $"{assembly}.dll";
            string version = null;
            string assemblyVersion = "1.0.0.1";
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var key = $"{assembly}_{assemblyVersion}_{lastFileWriteTime.ToFileTimeUtc()}";
            assemblyCache.Assemblies.TryAdd(key, _MockAssembly.Object);
            var assemblyName = new AssemblyName { Version = new Version(assemblyVersion) };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns(assemblyName);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), $"Found already loaded plugin assembly: {key}"));

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(actual, _MockAssembly.Object);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_AlreadyLoaded_GetNameNull()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            var assembly = "file3";
            var dll = $"{assembly}.dll";
            string version = null;
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var key = $"{assembly}_{lastFileWriteTime.ToFileTimeUtc()}";
            assemblyCache.Assemblies.TryAdd(key, _MockAssembly.Object);
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns((AssemblyName)null);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), $"Found already loaded plugin assembly: {key}"));

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(actual, _MockAssembly.Object);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_AlreadyLoaded_GetName_VersionNull()
        {
            // Arrange
            var assemblyCache = CreateAssemblyCache();
            var assembly = "file3";
            var dll = $"{assembly}.dll";
            string version = null;
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var key = $"{assembly}_{lastFileWriteTime.ToFileTimeUtc()}";
            assemblyCache.Assemblies.TryAdd(key, _MockAssembly.Object);
            var assemblyName = new AssemblyName();
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(dll))
                                   .Returns(assemblyName);
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), $"Found already loaded plugin assembly: {key}"));

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(actual, _MockAssembly.Object);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyLoader_FindAlreadyLoadedAssembly_AlreadyLoaded_NullLogger()
        {
            // Arrange
            var logLines = new List<string>();
            var assemblyCache = CreateAssemblyCache(true);
            var assembly = "file3";
            var dll = $"{assembly}.dll";
            var version = "1.0.0.1";
            var lastFileWriteTime = new DateTime(2020, 6, 27, 10, 11, 12);
            var key = $"{assembly}_{version}_{lastFileWriteTime.ToFileTimeUtc()}";
            assemblyCache.Assemblies.TryAdd(key, _MockAssembly.Object);
            var assemblyName = new AssemblyName { Version = new Version(version) };
            _MockFile.Setup(m => m.GetLastWriteTime(dll)).Returns(lastFileWriteTime);

            // Act
            var actual = assemblyCache.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(actual, _MockAssembly.Object);
            _MockRepository.VerifyAll();
        }
        #endregion
    }
}
