using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class AssemblyNameReaderTests
    {
        private MockRepository _MockRepository;

        private Mock<IFile> _MockFile;



        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockFile = _MockRepository.Create<IFile>();
        }

        private AssemblyNameReader CreateAssemblyNameReader()
        {
            return new AssemblyNameReader
            {
                File = _MockFile.Object
            };
        }

        [TestMethod]
        public void AssemblyNameReader_GetAssemblyName_FileDoesNotExist_Test()
        {
            // Arrange
            var assemblyNameReader = CreateAssemblyNameReader();
            string dll = @"c:\Path\To\MyAssembly.dll";
            _MockFile.Setup(m => m.Exists(dll)).Returns(false);

            // Act
            var result = assemblyNameReader.GetAssemblyName(dll);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AssemblyNameReader_GetAssemblyName_FileExists_ButItDoesntReallySoExpectionIsCaught_Test()
        {
            // Arrange
            var assemblyNameReader = CreateAssemblyNameReader();
            string dll = @"c:\Path\To\MyAssembly.dll";
            _MockFile.Setup(m => m.Exists(dll)).Returns(true);

            // Act
            var result = assemblyNameReader.GetAssemblyName(dll);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [TestCategory("LocalOnly")] // Some build agents don't like loading dlls
        public void AssemblyNameReader_GetAssemblyName_FileExists_Test()
        {
            // Arrange
            var assemblyNameReader = CreateAssemblyNameReader();
            var name = "Example.Dependency";
            string dll = $@"PluginDlls\{name}.dll";
            _MockFile.Setup(m => m.Exists(dll)).Returns(true);

            // Act
            var result = assemblyNameReader.GetAssemblyName(dll); // This loads a dll

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(name, result.Name);
            _MockRepository.VerifyAll();
        }
    }
}
