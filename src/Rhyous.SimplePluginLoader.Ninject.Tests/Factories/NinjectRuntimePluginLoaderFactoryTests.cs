using Ninject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;
using Ninject.Extensions.ChildKernel;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests.Factories
{
    [TestClass]
    public class NinjectRuntimePluginLoaderFactoryTests
    {
        private MockRepository _MockRepository;

        private IKernel _RootScope;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);
            _RootScope = new StandardKernel();
            _RootScope.Load<SimplePluginLoaderModule>();
        }

        private NinjectRuntimePluginLoaderFactory CreateFactory(IChildKernel scope)
        {
            return new NinjectRuntimePluginLoaderFactory(scope);
        }

        [TestMethod]
        public void NinjectRuntimePluginLoaderFactory_Create_Resolves()
        {
            // Arrange
            var childScope = new ChildKernel(_RootScope);
            var factory = CreateFactory(childScope);

            // Act
            var result = factory.Create<TestRuntimePluginLoader, IOrganization>();

            // Assert
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }
    }
}