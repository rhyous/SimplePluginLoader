using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockAssemblyNameReader = _MockRepository.Create<IAssemblyNameReader>();
            _MockLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private AssemblyLoader CreateAssemblyLoader(AssemblyCache assemblyCache)
        {
            return new AssemblyLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                assemblyCache,
                _MockAssemblyNameReader.Object,
                _MockLogger.Object);
        }

        [TestMethod]
        public void FindAlreadyLoadedAssembly_NothingLoaded()
        {
            // Arrange
            var logLines = new List<string>();
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName("file.dll"))
                                   .Returns((string inDll) => { return new AssemblyNameReader().GetAssemblyName(inDll); });
            var assemblyDictionary = new AssemblyCache();
            var loader = CreateAssemblyLoader(assemblyDictionary);
            var dll = "file.dll";
            var version = "1.0.0.1";
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };

            // Act
            var actual = loader.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void FindAlreadyLoadedAssembly_OnlyDifferentVersionLoaded()
        {
            // Arrange
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var logLines = new List<string>();
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });
            _MockAppDomain.Setup(m => m.GetAssemblies()).Returns(appDomain.GetAssemblies());
            var assemblyCache = new AssemblyCache();
            var loader = CreateAssemblyLoader(assemblyCache); ;
            var dll = "file2.dll";
            var oldversion = "1.0.0.0";
            var version = "1.0.0.1";
            var key = loader.GetKey(dll, oldversion);
            var testAssembly = appDomain.GetAssemblies().First();
            assemblyCache.Assemblies.Add(key, testAssembly);
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(It.IsAny<string>())).Returns(assemblyName);

            // Act
            var actual = loader.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void FindAlreadyLoadedAssembly_AlreadyLoaded()
        {
            // Arrange
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var logLines = new List<string>();
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });

            var assemblyCache = new AssemblyCache();
            var loader = CreateAssemblyLoader(assemblyCache);
            var dll = "file3.dll";
            var version = "1.0.0.1";
            var key = loader.GetKey(dll, version);
            var testAssembly = appDomain.GetAssemblies().First();
            assemblyCache.Assemblies.Add(key, testAssembly);
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };
            _MockAssemblyNameReader.Setup(m => m.GetAssemblyName(It.IsAny<string>())).Returns(assemblyName);

            // Act
            var actual = loader.FindAlreadyLoadedAssembly(dll, version);

            // Assert
            Assert.AreEqual(actual, testAssembly);
        }

        //[TestMethod]
        //public void Load_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var assemblyLoader = CreateAssemblyLoader();
        //    string dll = null;
        //    string pdb = null;

        //    // Act
        //    var result = assemblyLoader.Load(dll, pdb);

        //    // Assert
        //    Assert.Fail();
        //    _MockRepository.VerifyAll();
        //}

        //[TestMethod]
        //public void TryLoad_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var assemblyLoader = CreateAssemblyLoader();
        //    string dll = null;
        //    string pdb = null;

        //    // Act
        //    var result = assemblyLoader.TryLoad(
        //        dll,
        //        pdb);

        //    // Assert
        //    Assert.Fail();
        //    _MockRepository.VerifyAll();
        //}

        //[TestMethod]
        //public void TryLoad_StateUnderTest_ExpectedBehavior1()
        //{
        //    // Arrange
        //    var assemblyLoader = CreateAssemblyLoader();
        //    string dll = null;
        //    string pdb = null;
        //    string version = null;

        //    // Act
        //    var result = assemblyLoader.TryLoad(
        //        dll,
        //        pdb,
        //        version);

        //    // Assert
        //    Assert.Fail();
        //    _MockRepository.VerifyAll();
        //}

        //[TestMethod]
        //public void Dispose_StateUnderTest_ExpectedBehavior()
        //{
        //    // Arrange
        //    var assemblyLoader = CreateAssemblyLoader();

        //    // Act
        //    assemblyLoader.Dispose();

        //    // Assert
        //    Assert.Fail();
        //    _MockRepository.VerifyAll();
        //}
    }
}
