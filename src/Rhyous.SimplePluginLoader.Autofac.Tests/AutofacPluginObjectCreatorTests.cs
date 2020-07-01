using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class AutofacPluginObjectCreatorTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>> _MockContainerBuilderPluginLoader;
        private Mock<IPlugin> _MockPlugin;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPlugin = _MockRepository.Create<IPlugin>();
            _MockContainerBuilderPluginLoader = _MockRepository.Create<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
        }

        private AutofacPluginObjectCreator<T> CreateAutofacPluginObjectCreator<T>(IComponentContext context)
        {
            return new AutofacPluginObjectCreator<T>(context)
            {
                Plugin = _MockPlugin.Object
            };
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsSimpleClass()
        {
            // Arrange
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_MockContainerBuilderPluginLoader.Object)
                   .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<User>()
                   .As(typeof(IUser));
            var container = builder.Build();

            _MockPlugin.Setup(m => m.FullPath).Returns((string)null);

            var autofacPluginObjectCreator = CreateAutofacPluginObjectCreator<User>(container);
            Type type = typeof(User);

            // Act
            var result = autofacPluginObjectCreator.Create(type);

            // Assert
            Assert.AreEqual(result.GetType(), typeof(User));
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsInterface()
        {
            // Arrange
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_MockContainerBuilderPluginLoader.Object)
                   .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<User>()
                   .As(typeof(IUser));
            var container = builder.Build();

            _MockPlugin.Setup(m => m.FullPath).Returns((string)null);

            var creator = CreateAutofacPluginObjectCreator<IUser>(container);
            var type = typeof(User);

            // Act
            var actual = creator.Create(type);

            // Assert
            Assert.AreEqual(type, actual.GetType());
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsGeneric()
        {
            // Arrange
            _MockContainerBuilderPluginLoader.Setup(m => m.LoadPlugin(It.IsAny<string>()))
                            .Returns((IPlugin<IDependencyRegistrar<ContainerBuilder>>)null);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_MockContainerBuilderPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterGeneric(typeof(Service<,,>)).As(typeof(IService<,,>));
            var container = builder.Build();

            _MockPlugin.Setup(m => m.FullPath).Returns((string)null);

            var creator = CreateAutofacPluginObjectCreator<IService<Organization, IOrganization, int>>(container);

            // Act
            var actual = creator.Create(typeof(Service<Organization, IOrganization, int>));

            // Assert
            Assert.AreEqual(actual.GetType(), typeof(Service<Organization, IOrganization, int>));
        }
    }
}
