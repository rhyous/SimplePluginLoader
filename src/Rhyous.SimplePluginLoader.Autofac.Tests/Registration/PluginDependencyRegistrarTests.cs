using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Autofac.Tests.Registration
{
    [TestClass]
    public class PluginDependencyRegistrarTests
    {
        private MockRepository _MockRepository;
        Mock<IPlugin> _MockPlugin;
        Mock<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>> _MockPluginLoader;
        Mock<IPlugin<IDependencyRegistrar<ContainerBuilder>>> _MockPluginDependencyRegistrar;
        Mock<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>> _MockPluginObjectCreator;

        IContainer _Container;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);
            _MockPlugin = _MockRepository.Create<IPlugin>();
            _MockPluginLoader = _MockRepository.Create<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            _MockPluginDependencyRegistrar = _MockRepository.Create<IPlugin<IDependencyRegistrar<ContainerBuilder>>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>();

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_MockPluginLoader.Object)
                   .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterInstance(_MockPluginObjectCreator.Object)
                   .As<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>();
            _Container = builder.Build();
        }

        private PluginDependencyRegistrar CreatePluginDependencyRegistrar(IComponentContext context)
        {
            return new PluginDependencyRegistrar(context);
        }

        public class FakeDependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
        {
            public bool RegisterWasCalled = false;
            public void Register(ContainerBuilder containerBuilder)
            {
                RegisterWasCalled = true;
            }
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_NoPluginReturned()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_Container);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns((IPlugin<IDependencyRegistrar<ContainerBuilder>>)null);

            // Act
            _Container.BeginLifetimeScope(builder => 
            {
                pluginDependencyRegistrar.RegisterPluginDependencies(builder, _MockPlugin.Object, type);
            });

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_PluginTypesEmpty()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_Container);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> {  };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);

            // Act
            _Container.BeginLifetimeScope(builder =>
            {
                pluginDependencyRegistrar.RegisterPluginDependencies(builder, _MockPlugin.Object, type);
            });

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_PluginTypesNull()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_Container);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns((List<Type>)null);

            // Act
            _Container.BeginLifetimeScope(builder =>
            {
                pluginDependencyRegistrar.RegisterPluginDependencies(builder, _MockPlugin.Object, type);
            });

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_PluginCreatesNullObjects()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_Container);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { typeof(FakeDependencyRegistrar) };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            _MockPluginDependencyRegistrar.Setup(m => m.CreatePluginObjects(It.IsAny< IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>()))
                                          .Returns((List<IDependencyRegistrar<ContainerBuilder>>)null);

            // Act
            _Container.BeginLifetimeScope(builder =>
            {
                pluginDependencyRegistrar.RegisterPluginDependencies(builder, _MockPlugin.Object, type);
            });

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_Works()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_Container);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m=>m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { typeof(FakeDependencyRegistrar) };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            var fakeDependencyRegistrar = new FakeDependencyRegistrar();
            var list = new List<IDependencyRegistrar<ContainerBuilder>> { fakeDependencyRegistrar };
            _MockPluginDependencyRegistrar.Setup(m => m.CreatePluginObjects(It.IsAny<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>()))
                                          .Returns(list);

            // Act
            _Container.BeginLifetimeScope(builder =>
            {
                pluginDependencyRegistrar.RegisterPluginDependencies(builder, _MockPlugin.Object, type);
            });

            // Assert
            Assert.IsTrue(fakeDependencyRegistrar.RegisterWasCalled);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_TypeFromDifferentAssembly_Works()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_Container);
            Type type = typeof(string);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { typeof(FakeDependencyRegistrar) };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            var fakeDependencyRegistrar = new FakeDependencyRegistrar();
            var list = new List<IDependencyRegistrar<ContainerBuilder>> { fakeDependencyRegistrar };
            _MockPluginDependencyRegistrar.Setup(m => m.CreatePluginObjects(It.IsAny<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>())).Returns(list);

            // Act
            _Container.BeginLifetimeScope(builder =>
            {
                pluginDependencyRegistrar.RegisterPluginDependencies(builder, _MockPlugin.Object, type);
            });

            // Assert
            Assert.IsTrue(fakeDependencyRegistrar.RegisterWasCalled);
            _MockRepository.VerifyAll();
        }
    }
}
