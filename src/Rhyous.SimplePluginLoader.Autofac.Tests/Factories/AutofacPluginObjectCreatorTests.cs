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
        private Mock<IPluginDependencyRegistrar> _MockPluginDependencyRegistrar;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockContainerBuilderPluginLoader = _MockRepository.Create<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            _MockPluginDependencyRegistrar = _MockRepository.Create<IPluginDependencyRegistrar>();
        }

        private AutofacPluginObjectCreator<T> CreateAutofacPluginObjectCreator<T>(IComponentContext context)
        {
            return new AutofacPluginObjectCreator<T>(context, _MockPluginDependencyRegistrar.Object);
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

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<ContainerBuilder>(),
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((ContainerBuilder inBuilder, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(container).RegisterPluginDependencies(inBuilder, plugin, inType);
                });
            var autofacPluginObjectCreator = CreateAutofacPluginObjectCreator<User>(container);
            Type type = typeof(User);

            var mockPlugin = _MockRepository.Create<IPlugin<User>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var result = autofacPluginObjectCreator.Create(mockPlugin.Object, type);

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

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<ContainerBuilder>(), 
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((ContainerBuilder inBuilder, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(container).RegisterPluginDependencies(inBuilder, plugin, inType);
                });
            var creator = CreateAutofacPluginObjectCreator<IUser>(container);
            var type = typeof(User);

            var mockPlugin = _MockRepository.Create<IPlugin<IUser>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var actual = creator.Create(mockPlugin.Object, type);

            // Assert
            Assert.AreEqual(type, actual.GetType());
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsGeneric()
        {
            // Arrange
            var builder = new ContainerBuilder();
            builder.RegisterInstance(_MockContainerBuilderPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterGeneric(typeof(Service<,,>)).As(typeof(IService<,,>));
            var container = builder.Build();

            _MockContainerBuilderPluginLoader.Setup(m => m.LoadPlugin(It.IsAny<string>()))
                            .Returns((IPlugin<IDependencyRegistrar<ContainerBuilder>>)null);
            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                                It.IsAny<ContainerBuilder>(),
                                                It.IsAny<IPlugin>(),
                                                It.IsAny<Type>()))
                .Callback((ContainerBuilder inBuilder, IPlugin plugin, Type type) =>
                {
                    new PluginDependencyRegistrar(container).RegisterPluginDependencies(inBuilder, plugin, type);
                });

            var mockPlugin = _MockRepository.Create<IPlugin<IService<Organization, IOrganization, int>>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            var creator = CreateAutofacPluginObjectCreator<IService<Organization, IOrganization, int>>(container);

            // Act
            var actual = creator.Create(mockPlugin.Object, typeof(Service<Organization, IOrganization, int>));

            // Assert
            Assert.AreEqual(actual.GetType(), typeof(Service<Organization, IOrganization, int>));
        }

        [TestMethod]
        public void AutofacPluginObjectCreator_Create_Null_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_MockContainerBuilderPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            var container = builder.Build();

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<ContainerBuilder>(), 
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((ContainerBuilder inBuilder, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(container).RegisterPluginDependencies(inBuilder, plugin, inType);
                });
            var autofacPluginObjectCreator = new AutofacPluginObjectCreator<IOrganization>(container, _MockPluginDependencyRegistrar.Object);
            autofacPluginObjectCreator.RegisterTypeMethod = (ContainerBuilder b, Type t1, Type t2) => { return null; };

            var mockPlugin = _MockRepository.Create<IPlugin<IOrganization>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var result = autofacPluginObjectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AutofacPluginObjectCreator_Create_EmptyGeneric_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var builder = new ContainerBuilder(); 
            builder.RegisterInstance(_MockContainerBuilderPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            var container = builder.Build();
            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<ContainerBuilder>(),
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((ContainerBuilder inBbuilder, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(container).RegisterPluginDependencies(inBbuilder, plugin, inType);
                });

            var autofacPluginObjectCreator = new AutofacPluginObjectCreator<IOrganization>(container, _MockPluginDependencyRegistrar.Object);
            autofacPluginObjectCreator.RegisterTypeMethod = (ContainerBuilder b, Type t1, Type t2) => { return typeof(Service<,,>); };

            var mockPlugin = _MockRepository.Create<IPlugin<IOrganization>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var result = autofacPluginObjectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.IsNull(result);
        }
    }
}
