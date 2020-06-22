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
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private AssemblyLoader CreateAssemblyLoader()
        {
            return new AssemblyLoader(
                _MockAppDomain.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void FindAlreadyLoadedAssembly_NothingLoaded()
        {
            // Arrange
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var logLines = new List<string>();
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });
            var loader = new AssemblyLoader(appDomain, _MockPluginLoaderSettings.Object, _MockPluginLoaderLogger.Object);
            var assemblyDictionary = AssemblyDictionary.GetInstance(appDomain, _MockPluginLoaderLogger.Object);
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
            var mockLogger = new Mock<IPluginLoaderLogger>();
            var logLines = new List<string>();
            mockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });
            var loader = new AssemblyLoader(appDomain, _MockPluginLoaderSettings.Object, mockLogger.Object);
            var assemblyDictionary = AssemblyDictionary.GetInstance(appDomain, mockLogger.Object);
            var dll = "file2.dll";
            var oldversion = "1.0.0.0";
            var version = "1.0.0.1";
            var key = AssemblyLoader.GetKey(dll, oldversion);
            var testAssembly = appDomain.GetAssemblies().First();
            assemblyDictionary.Assemblies.Add(key, testAssembly);
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };
            var mockAssemblyReader = new Mock<IAssemblyNameReader>();
            mockAssemblyReader.Setup(m => m.GetAssemblyName(It.IsAny<string>())).Returns(assemblyName);
            loader.AssemblyNameReader = mockAssemblyReader.Object;

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
            var mockLogger = new Mock<IPluginLoaderLogger>();
            var logLines = new List<string>();
            mockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });
            var loader = new AssemblyLoader(appDomain, _MockPluginLoaderSettings.Object, mockLogger.Object);
            var assemblyDictionary = AssemblyDictionary.GetInstance(appDomain, mockLogger.Object);
            var dll = "file3.dll";
            var version = "1.0.0.1";
            var key = AssemblyLoader.GetKey(dll, version);
            var testAssembly = appDomain.GetAssemblies().First();
            assemblyDictionary.Assemblies.Add(key, testAssembly);
            var assemblyName = new AssemblyName { Version = new Version("1.0.0.1") };
            var mockAssemblyReader = new Mock<IAssemblyNameReader>();
            mockAssemblyReader.Setup(m => m.GetAssemblyName(It.IsAny<string>())).Returns(assemblyName);
            loader.AssemblyNameReader = mockAssemblyReader.Object;

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
