using Ninject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    [TestClass]
    public class NinjectPluginObjectCreatorTests
    {
        private MockRepository _MockRepository;

        private Mock<IPluginLoader<IDependencyRegistrar<IKernel>>> _MockPluginLoader;
        private Mock<IPluginDependencyRegistrar> _MockPluginDependencyRegistrar;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockPluginLoader = _MockRepository.Create<IPluginLoader<IDependencyRegistrar<IKernel>>>();
            _MockPluginDependencyRegistrar = _MockRepository.Create<IPluginDependencyRegistrar>();
        }

        private NinjectPluginObjectCreator<T> CreateNinjectPluginObjectCreator<T>(IKernel context)
        {
            return new NinjectPluginObjectCreator<T>(context, _MockPluginDependencyRegistrar.Object);
        }

        [TestMethod]
        public void NinjectObjectCreator_Create_T_IsSimpleClass()
        {
            // Arrange
            var rootScope = new StandardKernel();
            rootScope.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                     .ToConstant(_MockPluginLoader.Object);
            rootScope.Bind<IUser>()
                     .To<User>();

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<IKernel>(),
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((IKernel inrootScope, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(rootScope).RegisterPluginDependencies(inrootScope, plugin, inType);
                });
            var NinjectPluginObjectCreator = CreateNinjectPluginObjectCreator<User>(rootScope);
            Type type = typeof(User);

            var mockPlugin = _MockRepository.Create<IPlugin<User>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var result = NinjectPluginObjectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.AreEqual(result.GetType(), typeof(User));
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void NinjectObjectCreator_Create_T_IsInterface()
        {
            // Arrange
            var rootScope = new StandardKernel();
            rootScope.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                     .ToConstant(_MockPluginLoader.Object);
            rootScope.Bind<IUser>()
                     .To<User>();

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<IKernel>(),
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((IKernel scope, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(rootScope).RegisterPluginDependencies(scope, plugin, inType);
                });
            var creator = CreateNinjectPluginObjectCreator<IUser>(rootScope);
            var type = typeof(User);

            var mockPlugin = _MockRepository.Create<IPlugin<IUser>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var actual = creator.Create(mockPlugin.Object, type);

            // Assert
            Assert.AreEqual(type, actual.GetType());
        }

        [TestMethod]
        public void NinjectObjectCreator_Create_T_IsGeneric()
        {
            // Arrange
            var rootScope = new StandardKernel();
            rootScope.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                     .ToConstant(_MockPluginLoader.Object);
            rootScope.Bind(typeof(IService<,,>)).To(typeof(Service<,,>));


            _MockPluginLoader.Setup(m => m.LoadPlugin(It.IsAny<string>()))
                             .Returns((IPlugin<IDependencyRegistrar<IKernel>>)null);
            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                                It.IsAny<IKernel>(),
                                                It.IsAny<IPlugin>(),
                                                It.IsAny<Type>()))
                .Callback((IKernel scope, IPlugin plugin, Type type) =>
                {
                    new PluginDependencyRegistrar(rootScope).RegisterPluginDependencies(scope, plugin, type);
                });

            var mockPlugin = _MockRepository.Create<IPlugin<IService<Organization, IOrganization, int>>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            var creator = CreateNinjectPluginObjectCreator<IService<Organization, IOrganization, int>>(rootScope);

            // Act
            var actual = creator.Create(mockPlugin.Object, typeof(Service<Organization, IOrganization, int>));

            // Assert
            Assert.AreEqual(actual.GetType(), typeof(Service<Organization, IOrganization, int>));
        }

        [TestMethod]
        public void NinjectPluginObjectCreator_Create_Null_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var rootScope = new StandardKernel();
            rootScope.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                     .ToConstant(_MockPluginLoader.Object);

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<IKernel>(),
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((IKernel scope, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(rootScope).RegisterPluginDependencies(scope, plugin, inType);
                });
            var NinjectPluginObjectCreator = new NinjectPluginObjectCreator<IOrganization>(rootScope, _MockPluginDependencyRegistrar.Object);
            NinjectPluginObjectCreator.RegisterTypeMethod = (IKernel b, Type t1, Type t2) => { return null; };

            var mockPlugin = _MockRepository.Create<IPlugin<IOrganization>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var result = NinjectPluginObjectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NinjectPluginObjectCreator_Create_EmptyGeneric_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var rootScope = new StandardKernel();
            rootScope.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                     .ToConstant(_MockPluginLoader.Object);

            _MockPluginDependencyRegistrar.Setup(m => m.RegisterPluginDependencies(
                                    It.IsAny<IKernel>(),
                                    It.IsAny<IPlugin>(),
                                    It.IsAny<Type>()))
                .Callback((IKernel scope, IPlugin plugin, Type inType) =>
                {
                    new PluginDependencyRegistrar(rootScope).RegisterPluginDependencies(scope, plugin, inType);
                });

            var NinjectPluginObjectCreator = new NinjectPluginObjectCreator<IOrganization>(rootScope, _MockPluginDependencyRegistrar.Object);
            NinjectPluginObjectCreator.RegisterTypeMethod = (IKernel b, Type t1, Type t2) => { return typeof(Service<,,>); };

            var mockPlugin = _MockRepository.Create<IPlugin<IOrganization>>();
            mockPlugin.Setup(m => m.FullPath).Returns((string)null);

            // Act
            var result = NinjectPluginObjectCreator.Create(mockPlugin.Object, type);

            // Assert
            Assert.IsNull(result);
        }
    }
}
