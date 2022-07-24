using Ninject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;
using System.Collections.Generic;
using Ninject.Extensions.ChildKernel;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests.Registration
{
    [TestClass]
    public class PluginDependencyRegistrarTests
    {
        private MockRepository _MockRepository;
        Mock<IPlugin> _MockPlugin;
        Mock<IPluginLoader<IDependencyRegistrar<IKernel>>> _MockPluginLoader;
        Mock<IPlugin<IDependencyRegistrar<IKernel>>> _MockPluginDependencyRegistrar;
        Mock<IPluginObjectCreator<IDependencyRegistrar<IKernel>>> _MockPluginObjectCreator;

        IKernel _RootScope;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);
            _MockPlugin = _MockRepository.Create<IPlugin>();
            _MockPluginLoader = _MockRepository.Create<IPluginLoader<IDependencyRegistrar<IKernel>>>();
            _MockPluginDependencyRegistrar = _MockRepository.Create<IPlugin<IDependencyRegistrar<IKernel>>>();
            _MockPluginObjectCreator = _MockRepository.Create<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>();

            _RootScope = new StandardKernel();
            _RootScope.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                      .ToConstant(_MockPluginLoader.Object);
            _RootScope.Bind<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>()
                      .ToConstant(_MockPluginObjectCreator.Object);

        }

        private PluginDependencyRegistrar CreatePluginDependencyRegistrar(IKernel scope)
        {
            return new PluginDependencyRegistrar(scope);
        }

        public class FakeDependencyRegistrar : IDependencyRegistrar<IKernel>
        {
            public bool RegisterWasCalled = false;
            public void Register(IKernel kernel)
            {
                RegisterWasCalled = true;
            }
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_NoPluginReturned()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_RootScope);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns((IPlugin<IDependencyRegistrar<IKernel>>)null);
            var childScope = new ChildKernel(_RootScope);

            // Act
            pluginDependencyRegistrar.RegisterPluginDependencies(childScope, _MockPlugin.Object, type);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_PluginTypesEmpty()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_RootScope);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            var childScope = new ChildKernel(_RootScope);

            // Act
            pluginDependencyRegistrar.RegisterPluginDependencies(childScope, _MockPlugin.Object, type);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_PluginTypesNull()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_RootScope);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns((List<Type>)null);
            var childScope = new ChildKernel(_RootScope);

            // Act
            pluginDependencyRegistrar.RegisterPluginDependencies(childScope, _MockPlugin.Object, type);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_PluginCreatesNullObjects()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_RootScope);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { typeof(FakeDependencyRegistrar) };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            _MockPluginDependencyRegistrar.Setup(m => m.CreatePluginObjects(It.IsAny<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>()))
                                          .Returns((List<IDependencyRegistrar<IKernel>>)null);
            var childScope = new ChildKernel(_RootScope);

            // Act
            pluginDependencyRegistrar.RegisterPluginDependencies(childScope, _MockPlugin.Object, type);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_Works()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_RootScope);
            Type type = typeof(Organization);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { typeof(FakeDependencyRegistrar) };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            var fakeDependencyRegistrar = new FakeDependencyRegistrar();
            var list = new List<IDependencyRegistrar<IKernel>> { fakeDependencyRegistrar };
            _MockPluginDependencyRegistrar.Setup(m => m.CreatePluginObjects(It.IsAny<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>()))
                                          .Returns(list);
            var childScope = new ChildKernel(_RootScope);

            // Act
            pluginDependencyRegistrar.RegisterPluginDependencies(childScope, _MockPlugin.Object, type);

            // Assert
            Assert.IsTrue(fakeDependencyRegistrar.RegisterWasCalled);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyRegistrar_RegisterPluginDependencies_TypeFromDifferentAssembly_Works()
        {
            // Arrange
            var pluginDependencyRegistrar = CreatePluginDependencyRegistrar(_RootScope);
            Type type = typeof(string);

            var pluginFullPath = @"c:\Plugins\MyPlugin\my.dll";
            _MockPlugin.Setup(m => m.FullPath).Returns(pluginFullPath);
            _MockPluginLoader.Setup(m => m.LoadPlugin(pluginFullPath)).Returns(_MockPluginDependencyRegistrar.Object);
            var types = new List<Type> { typeof(FakeDependencyRegistrar) };
            _MockPluginDependencyRegistrar.Setup(m => m.PluginTypes).Returns(types);
            var fakeDependencyRegistrar = new FakeDependencyRegistrar();
            var list = new List<IDependencyRegistrar<IKernel>> { fakeDependencyRegistrar };
            _MockPluginDependencyRegistrar.Setup(m => m.CreatePluginObjects(It.IsAny<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>())).Returns(list);
            var childScope = new ChildKernel(_RootScope);

            // Act
            pluginDependencyRegistrar.RegisterPluginDependencies(childScope, _MockPlugin.Object, type);

            // Assert
            Assert.IsTrue(fakeDependencyRegistrar.RegisterWasCalled);
            _MockRepository.VerifyAll();
        }
    }
}
