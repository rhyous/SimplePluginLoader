using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.Autofac.Tests.Factories
{
    [TestClass]
    public class AutofacRuntimePluginLoaderFactoryTests
    {
        private MockRepository _MockRepository;

        private ILifetimeScope _LifetimeScope;
        private IContainer _Container;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);
            var builder = new ContainerBuilder();
            builder.RegisterModule<SimplePluginLoaderModule>();
            _Container = builder.Build();
            
        }

        private AutofacRuntimePluginLoaderFactory CreateFactory(ILifetimeScope scope)
        {
            return new AutofacRuntimePluginLoaderFactory(scope);
        }

        [TestMethod]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var lifetimeScope = _Container.BeginLifetimeScope();
            var factory = CreateFactory(lifetimeScope);

            // Act
            var result = factory.Create<TestRuntimePluginLoader, IOrganization>();

            // Assert
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }
    }
}
