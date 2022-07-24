using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Ninject.Extensions.ChildKernel;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    [TestClass]
    public class KernelExtensionsTests
    {
        [TestMethod]
        public void KernelExtensions_RegisterType_InterfaceToInterface_Test()
        {
            // Arrange
            Type type = typeof(IOrganization);
            Type type2 = typeof(IOrganization);
            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            Type typeToLoad = pluginScope.RegisterType(type, type2);

            // Assert
            Assert.ThrowsException<ActivationException>(() =>
            {
                pluginScope.Get(typeToLoad);
            });
            Assert.AreEqual(typeof(IOrganization), typeToLoad);
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_InterfaceToInterface_PreRegistered_Test()
        {
            // Arrange
            Type type = typeof(IOrganization);
            Type type2 = typeof(IOrganization);
            var rootScope = new StandardKernel();
            rootScope.Bind<IOrganization>().To<Organization>();

            var pluginScope = new ChildKernel(rootScope);
            // Act
            Type typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(typeof(Organization), actual.GetType());
            Assert.AreEqual(typeof(IOrganization), typeToLoad);
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_SimpleModelToInterface_Test()
        {
            // Arrange
            Type type = typeof(Organization);
            Type type2 = typeof(IOrganization);

            var rootScope = new StandardKernel();


            var pluginScope = new ChildKernel(rootScope);
            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(type, actual.GetType());
            Assert.AreEqual(type, typeToLoad);
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_InterfaceToSimpleModel_InvalidUseCase_Test()
        {
            // Arrange
            Type type = typeof(IOrganization);
            Type type2 = typeof(Organization);
            var rootScope = new StandardKernel();


            var pluginScope = new ChildKernel(rootScope);
            {
                // Act
                // Assert
                Assert.ThrowsException<TypeMismatchException>(() =>
                {
                    pluginScope.RegisterType(type, type2);
                });
            }
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_SimpleModelToSameModel_Test()
        {
            // Arrange
            Type type = typeof(Organization);
            Type type2 = typeof(Organization);

            var rootScope = new StandardKernel();

            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(type, actual.GetType());
            Assert.AreEqual(typeToLoad, actual.GetType());
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_EmptyGenericToTypedGenericInterface_Test()
        {
            // Arrange
            Type type = typeof(Service<,,>);
            Type type2 = typeof(IService<User, IUser, int>);

            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(typeof(Service<User, IUser, int>), actual.GetType());
            Assert.AreEqual(typeof(Service<User, IUser, int>), typeToLoad);
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_EmptyGenericToTypedGeneric_Test()
        {
            // Arrange            
            Type type = typeof(Service<,,>);
            Type type2 = typeof(Service<User, IUser, int>);

            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(type2, actual.GetType());
            Assert.AreEqual(typeToLoad, actual.GetType());
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_TypedGenericToEmptyGeneric_Test()
        {
            // Arrange
            Type type = typeof(Service<User, IUser, int>);
            Type type2 = typeof(Service<,,>);

            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(type, actual.GetType());
            Assert.AreEqual(typeToLoad, actual.GetType());
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_TypedGenericToEmptyGenericInterface_Test()
        {
            // Arrange
            Type type = typeof(Service<User, IUser, int>);
            Type type2 = typeof(IService<,,>);

            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(type, actual.GetType());
            Assert.AreEqual(typeToLoad, actual.GetType());
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_EmptyGenericToEmptyGenericInterface_InvalidUseCase_ReturnNull_Test()
        {
            // Arrange
            Type type = typeof(Service<,,>);
            Type type2 = typeof(IService<,,>);

            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);

            // Assert
            Assert.IsNull(typeToLoad);
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_NonGenericToEmptyGenericInterface_Test()
        {
            // Arrange
            Type type = typeof(UserService);
            Type type2 = typeof(IService<,,>);

            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            var typeToLoad = pluginScope.RegisterType(type, type2);
            var actual = pluginScope.Get(typeToLoad);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(type, actual.GetType());
            Assert.AreEqual(typeToLoad, actual.GetType());
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_EmptyGenericInterfaceToNonGeneric_InvalidUseCase_Test()
        {
            // Arrange
            Type type = typeof(IService<,,>);
            Type type2 = typeof(UserService);
            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            // Assert
            Assert.ThrowsException<TypeMismatchException>(() =>
            {
                pluginScope.RegisterType(type, type2);
            });
        }

        [TestMethod]
        public void KernelExtensions_RegisterType_EmptyGenericToInterface_Test()
        {
            // Arrange
            Type type = typeof(Service<,,>);
            Type type2 = typeof(IUserService);
            var rootScope = new StandardKernel();
            var pluginScope = new ChildKernel(rootScope);

            // Act
            // Assert
            Assert.ThrowsException<TypeMismatchException>(() =>
            {
                pluginScope.RegisterType(type, type2);
            });
        }
    }
}
