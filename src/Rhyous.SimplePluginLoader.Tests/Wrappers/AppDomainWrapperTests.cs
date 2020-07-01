using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using System;
using System.Reflection;

namespace Rhyous.SimplePluginLoader.Tests.Wrappers
{
    [TestClass]
    public class AppDomainWrapperTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private AppDomainWrapper CreateAppDomainWrapper()
        {
            return new AppDomainWrapper(
                AppDomain.CurrentDomain,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void AppDomainWrapper_AssemblyResolve_Registration_Test()
        {
            // Arrange
            var appDomainWrapper = CreateAppDomainWrapper();
            var expectedLogMsg1 = "AppDomainWrapperTests.AppDomainWrapper_AssemblyResolve subscribed to AppDomain.AssemblyResolve.";
            var expectedLogMsg2 = "Total subscriptions: 1";
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg1));
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), expectedLogMsg2));

            // Act
            appDomainWrapper.AssemblyResolve += AppDomainWrapper_AssemblyResolve;

            // Assert
            _MockRepository.VerifyAll();
        }

        private Assembly AppDomainWrapper_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}