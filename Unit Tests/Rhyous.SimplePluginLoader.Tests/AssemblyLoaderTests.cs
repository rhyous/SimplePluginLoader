using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.Tests.TestClasses;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class AssemblyLoaderTests
    {
        [TestMethod]
        public void FindAlreadyLoadedAssembly_NothingLoaded()
        {
            // Arrange
            var appDomain = new AppDomainWrapper(AppDomain.CurrentDomain);
            var mockLogger = new Mock<IPluginLoaderLogger>();
            var logLines = new List<string>();
            mockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>())).Callback((PluginLoaderLogLevel level, string msg) =>
            {
                logLines.Add(msg);
            });
            var loader = new AssemblyLoader<IOrg>(appDomain, mockLogger.Object);
            var assemblyDictionary = AssemblyDictionary.GetInstance(appDomain, mockLogger.Object);
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
            var loader = new AssemblyLoader<IOrg>(appDomain, mockLogger.Object);
            var assemblyDictionary = AssemblyDictionary.GetInstance(appDomain, mockLogger.Object);
            var dll = "file.dll";
            var oldversion = "1.0.0.0";
            var version = "1.0.0.1";
            var key = AssemblyLoader<IOrg>.GetKey(dll, oldversion);
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
            var loader = new AssemblyLoader<IOrg>(appDomain, mockLogger.Object);
            var assemblyDictionary = AssemblyDictionary.GetInstance(appDomain, mockLogger.Object);
            var dll = "file.dll";
            var version = "1.0.0.1";
            var key = AssemblyLoader<IOrg>.GetKey(dll, version);
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


        [TestMethod]
        public void FindAlreadyLoadedAssembly_AlreadyLoadedByAppDomainNotPluginLoader()
        {
            // Arrange
            
            // Act

            // Assert
        }
    }
}
