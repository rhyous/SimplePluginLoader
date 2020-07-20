using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.Collections;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Tests.Wrappers
{
    [TestClass]
    public class AppDomainWrapperTests
    {
        private MockRepository _MockRepository;

        private Mock<IDirectory> _MockDirectory;
        private Mock<IFile> _MockFile;
        private Mock<IPluginLoaderLogger> _MockLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);
            _MockDirectory = _MockRepository.Create<IDirectory>();
            _MockFile = _MockRepository.Create<IFile>();
            _MockLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private AppDomainWrapper CreateAppDomainWrapper()
        {
            return new AppDomainWrapper(AppDomain.CurrentDomain, _MockLogger.Object)
            {
                Directory = _MockDirectory.Object,
                File = _MockFile.Object
            };
        }

        [TestMethod]
        public void AppDomainWrapper_GetAssemblies_Returns_AsemblyWrappers_As_IAssembly()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();

            // Act
            var result = appDomainWrapper.GetAssemblies();

            // Assert
            Assert.IsTrue(result.All(a=>a.GetType() == typeof(AssemblyWrapper)));
            _MockRepository.VerifyAll();
        }

        #region TryLoad
        [TestMethod]
        [TestCategory("LocalOnly")] // Some build agents don't like loading dlls
        public void AppDomainWrapper_TryLoad_RawAssembly()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            byte[] rawAssembly = File.ReadAllBytes(@"PluginDlls\Example.Dependency.dll");

            // Act
            var result = appDomainWrapper.TryLoad(rawAssembly);

            // Assert
            Assert.AreEqual("Example.Dependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", result.FullName);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_TryLoad_RawAssembly_EmptyByteArray()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            byte[] rawAssembly = new byte[0];
           
            _MockLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, It.IsAny<string>()));

            // Act
            var result = appDomainWrapper.TryLoad(rawAssembly);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_TryLoad_RawAssembly_EmptyByteArray_NullLogger()
        {
            // Arrange
            var appDomainWrapper = new AppDomainWrapper(AppDomain.CurrentDomain, null)
            {
                Directory = _MockDirectory.Object
            };
            byte[] rawAssembly = new byte[0];

            // Act
            var result = appDomainWrapper.TryLoad(rawAssembly);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_TryLoad_RawAssembly_and_Pdb()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            byte[] rawAssembly = File.ReadAllBytes(@"PluginDlls\Example.Dependency.dll");
            byte[] rawSymbolStore = File.ReadAllBytes(@"PluginDlls\Example.Dependency.pdb");

            // Act
            var result = appDomainWrapper.TryLoad(rawAssembly, rawSymbolStore);

            // Assert
            Assert.AreEqual("Example.Dependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", result.FullName);
            _MockRepository.VerifyAll();
        }


        [TestMethod]
        public void AppDomainWrapper_TryLoad_RawAssembly_and_Pdb_EmptyByteArray()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            byte[] rawAssembly = new byte[0];
            byte[] rawSymbolStore = new byte[0];

            _MockLogger.Setup(m => m.WriteLine(PluginLoaderLogLevel.Debug, It.IsAny<string>()));

            // Act
            var result = appDomainWrapper.TryLoad(rawAssembly, rawSymbolStore);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_TryLoad_RawAssembly_and_Pdb_EmptyByteArray_NullLogger()
        {
            // Arrange
            var appDomainWrapper = new AppDomainWrapper(AppDomain.CurrentDomain, null)
            {
                Directory = _MockDirectory.Object
            };
            byte[] rawAssembly = new byte[0];
            byte[] rawSymbolStore = new byte[0];

            // Act
            var result = appDomainWrapper.TryLoad(rawAssembly, rawSymbolStore);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]

        [TestCategory("LocalOnly")] // Some build agents don't like loading dlls
        public void AppDomainWrapper_TryLoad_Assembly_and_Pdb_FilePaths()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            string dll = @"PluginDlls\Example.Dependency.dll";
            string pdb = @"PluginDlls\Example.Dependency.pdb";
            _MockFile.Setup(m => m.Exists(dll)).Returns(true);
            _MockFile.Setup(m => m.Exists(pdb)).Returns(true);
            _MockFile.Setup(m => m.ReadAllBytes(dll)).Returns(File.ReadAllBytes(@"PluginDlls\Example.Dependency.dll"));
            _MockFile.Setup(m => m.ReadAllBytes(pdb)).Returns(File.ReadAllBytes(@"PluginDlls\Example.Dependency.pdb"));

            // Act
            var result = appDomainWrapper.TryLoad(dll, pdb);

            // Assert
            Assert.AreEqual("Example.Dependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", result.FullName);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_TryLoad_Assembly_and_Pdb_FilePaths_Pdb_NotFound()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            string dll = @"PluginDlls\Example.Dependency.dll";
            string pdb = @"PluginDlls\Example.Dependency.pdb";
            _MockFile.Setup(m => m.Exists(dll)).Returns(true);
            _MockFile.Setup(m => m.Exists(pdb)).Returns(false);
            _MockFile.Setup(m => m.ReadAllBytes(dll)).Returns(File.ReadAllBytes(@"PluginDlls\Example.Dependency.dll"));

            // Act
            var result = appDomainWrapper.TryLoad(dll, pdb);

            // Assert
            Assert.AreEqual("Example.Dependency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", result.FullName);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_TryLoad_Assembly_and_Pdb_FilePaths_dll_NotFound()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            string dll = @"PluginDlls\Example.Dependency.dll";
            string pdb = @"PluginDlls\Example.Dependency.pdb";
            _MockFile.Setup(m => m.Exists(dll)).Returns(false);

            // Act
            var result = appDomainWrapper.TryLoad(dll, pdb);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }
        #endregion

        #region AssemblyResolve
        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_Registration_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expectedLogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expectedLogMsg2 = "Total subscriptions: 1";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg2));

            // Act
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_Registration_NullLogger_Test()
        {
            // Arrange
            var appDomainWrapper = new AppDomainWrapper(AppDomain.CurrentDomain, null)
            {
                Directory = _MockDirectory.Object
            };

            // Act
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_Registration_Twice_SecondRegistrationIgnored_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expectedLogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expectedLogMsg2 = "Total subscriptions: 1";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg2));
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;
            var expectedLogMsg3 = $"{nameof(AppDomainWrapperTests)} tried to subscribed to AppDomain.AssemblyResolve, but was already subscribed.";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg3));

            // Act
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_Registration_Twice_SecondRegistrationIgnored_NullLoggerTest()
        {
            // Arrange
            var appDomainWrapper = new AppDomainWrapper(AppDomain.CurrentDomain, null)
            {
                Directory = _MockDirectory.Object
            };
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;
            
            // Act
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_OnAssemblyResolve_NoRegistrations_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();

            // Act
            var result = appDomainWrapper.OnAssemblyResolve(new object(), new ResolveEventArgs("TestResolve"));

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_OnAssemblyResolve_AllRegistrationsReturnNull_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expected1LogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expected1LogMsg2 = "Total subscriptions: 1";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected1LogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected1LogMsg2));
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve_Null;

            // Act
            var result = appDomainWrapper.OnAssemblyResolve(new object(), new ResolveEventArgs("TestResolve"));

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        public class DisposableAssemblyResolve : IDisposable
        {
            public Assembly MyAssembly => Assembly.GetExecutingAssembly();
            public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
            {
                return MyAssembly;
            }

            public void Dispose()
            {
            }
        }

        /// <summary>
        /// This test proves that a disposed object will not be garbage collected via the GC
        /// while it is still resolved, so we shouldn't have to worry about our event trying
        /// call a disposed object.
        /// </summary>
        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_OnAssemblyResolve_DisposedRegistration_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expected1LogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expected1LogMsg2 = "Total subscriptions: 1";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected1LogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected1LogMsg2));
            using (var resolver = new DisposableAssemblyResolve())
            {
                appDomainWrapper.AssemblyResolve += resolver.AssemblyResolve;
            }
            GC.Collect();

            // Act
            var result = appDomainWrapper.OnAssemblyResolve(new object(), new ResolveEventArgs("TestResolve"));

            // Assert
            Assert.AreEqual(Assembly.GetExecutingAssembly(), result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_2Registrations_FirstReturnsSecondDoesNotRun_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expected1LogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expected1LogMsg2 = "Total subscriptions: 1";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected1LogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected1LogMsg2));

            var expected2LogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expected2LogMsg2 = "Total subscriptions: 2";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected2LogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expected2LogMsg2));

            bool resolve1WasCalled = false;
            object actual1Sender = null;
            ResolveEventArgs actual1ResolveEventArgs = null;
            appDomainWrapper.AssemblyResolve += (object sender1, ResolveEventArgs args1) =>
            {
                actual1Sender = sender1;
                actual1ResolveEventArgs = args1;
                resolve1WasCalled = true;
                return Assembly.GetExecutingAssembly();
            };
            bool resolve2WasCalled = false;
            object actual2Sender = null;
            ResolveEventArgs actual2ResolveEventArgs = null;
            appDomainWrapper.AssemblyResolve += (object sender2, ResolveEventArgs args2) =>
            {
                actual2Sender = sender2;
                actual2ResolveEventArgs = args2;
                resolve2WasCalled = true;
                return Assembly.GetExecutingAssembly();
            };
            var sender = new object();
            var args = new ResolveEventArgs("TestName", Assembly.GetCallingAssembly());

            // Act
            appDomainWrapper.OnAssemblyResolve(sender, args);

            // Assert
            Assert.IsTrue(resolve1WasCalled);
            Assert.AreEqual(sender, actual1Sender);
            Assert.AreEqual(args, actual1ResolveEventArgs);

            Assert.IsFalse(resolve2WasCalled);
            Assert.IsNull(actual2Sender);
            Assert.IsNull(actual2ResolveEventArgs);
            _MockRepository.VerifyAll();
        }

        private Assembly AppDomainWrapper_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.GetExecutingAssembly();
        }

        private Assembly AppDomainWrapper_AssemblyResolve_Null(object sender, ResolveEventArgs args)
        {
            return null;
        }

        #endregion

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_RemoveRegistration_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expectedSubscribeLogMsg1 = $"{nameof(AppDomainWrapperTests)} subscribed to AppDomain.AssemblyResolve.";
            var expectedSubscribeLogMsg2 = "Total subscriptions: 1";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedSubscribeLogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedSubscribeLogMsg2));
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            var expectedUnsubscribeLogMsg1 = $"{nameof(AppDomainWrapperTests)} unsubscribed to AppDomain.AssemblyResolve.";
            var expectedUnsubscribeLogMsg2 = "Total subscriptions: 0";
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedUnsubscribeLogMsg1));
            _MockLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedUnsubscribeLogMsg2));

            // Act
            appDomainWrapper.AssemblyResolve -= AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AppDomainWrapper_AppDomainWrapper_AssemblyResolve_RemoveRegistration_NulLogger_Test()
        {
            // Arrange
            var appDomainWrapper = new AppDomainWrapper(AppDomain.CurrentDomain, null)
            {
                Directory = _MockDirectory.Object
            };            
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            // Act
            appDomainWrapper.AssemblyResolve -= AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }
    }
}
