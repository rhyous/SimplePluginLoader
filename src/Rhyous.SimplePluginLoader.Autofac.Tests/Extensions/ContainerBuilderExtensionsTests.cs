using Autofac;
using Autofac.Core.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class ContainerBuilderExtensionsTests
    {
        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_InterfaceToInterface_Test()
        {
            // Arrange
            Type type = typeof(IOrganization);
            Type type2 = typeof(IOrganization);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                // Assert
                Assert.ThrowsException<ComponentNotRegisteredException>(() =>
                {
                    pluginScope.Resolve(typeToLoad);
                });
                Assert.AreEqual(typeof(IOrganization), typeToLoad);
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_InterfaceToInterface_PreRegistered_Test()
        {
            // Arrange
            Type type = typeof(IOrganization);
            Type type2 = typeof(IOrganization);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<Organization>().As<IOrganization>();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(typeof(Organization), actual.GetType());
                Assert.AreEqual(typeof(IOrganization), typeToLoad);
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_SimpleModelToInterface_Test()
        {
            // Arrange
            Type type = typeof(Organization);
            Type type2 = typeof(IOrganization);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(type, actual.GetType());
                Assert.AreEqual(type, typeToLoad);
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_InterfaceToSimpleModel_InvalidUseCase_Test()
        {
            // Arrange
            Type type = typeof(IOrganization);
            Type type2 = typeof(Organization);
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                // Assert
                Assert.ThrowsException<TypeMismatchException>(() =>
                {
                    builder.RegisterType(type, type2);
                });
            }))
            {
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_SimpleModelToSameModel_Test()
        {
            // Arrange
            Type type = typeof(Organization);
            Type type2 = typeof(Organization);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(type, actual.GetType());
                Assert.AreEqual(typeToLoad, actual.GetType());
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_EmptyGenericToTypedGenericInterface_Test()
        {
            // Arrange
            Type type = typeof(Service<,,>);
            Type type2 = typeof(IService<User, IUser, int>);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(typeof(Service<User, IUser, int>), actual.GetType());
                Assert.AreEqual(typeof(Service<User, IUser, int>), typeToLoad);
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_EmptyGenericToTypedGeneric_Test()
        {
            // Arrange            
            Type type = typeof(Service<,,>);
            Type type2 = typeof(Service<User, IUser, int>);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(type2, actual.GetType());
                Assert.AreEqual(typeToLoad, actual.GetType());
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_TypedGenericToEmptyGeneric_Test()
        {
            // Arrange
            Type type = typeof(Service<User, IUser, int>);
            Type type2 = typeof(Service<,,>);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(type, actual.GetType());
                Assert.AreEqual(typeToLoad, actual.GetType());
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_TypedGenericToEmptyGenericInterface_Test()
        {
            // Arrange
            Type type = typeof(Service<User, IUser, int>);
            Type type2 = typeof(IService<,,>);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(type, actual.GetType());
                Assert.AreEqual(typeToLoad, actual.GetType());
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_EmptyGenericToEmptyGenericInterface_InvalidUseCase_ReturnNull_Test()
        {
            // Arrange
            Type type = typeof(Service<,,>);
            Type type2 = typeof(IService<,,>);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                // Assert
                Assert.IsNull(typeToLoad);
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_NonGenericToEmptyGenericInterface_Test()
        {
            // Arrange
            Type type = typeof(UserService);
            Type type2 = typeof(IService<,,>);
            Type typeToLoad = null;
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                typeToLoad = builder.RegisterType(type, type2);
            }))
            {
                var actual = pluginScope.Resolve(typeToLoad);

                // Assert
                Assert.IsNotNull(actual);
                Assert.AreEqual(type, actual.GetType());
                Assert.AreEqual(typeToLoad, actual.GetType());
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_EmptyGenericInterfaceToNonGeneric_InvalidUseCase_Test()
        {
            // Arrange
            Type type = typeof(IService<,,>);
            Type type2 = typeof(UserService);
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                // Assert
                Assert.ThrowsException<TypeMismatchException>(() =>
                {
                    builder.RegisterType(type, type2);
                });
            }))
            {
            };
        }

        [TestMethod]
        public void ContainerBuilderExtensions_RegisterType_EmptyGenericToInterface_Test()
        {
            // Arrange
            Type type = typeof(Service<,,>);
            Type type2 = typeof(IUserService);
            var containerBuilder = new ContainerBuilder();
            var container = containerBuilder.Build();

            using (var pluginScope = container.BeginLifetimeScope((builder) =>
            {
                // Act
                // Assert
                Assert.ThrowsException<TypeMismatchException>(() =>
                {
                    builder.RegisterType(type, type2);
                });
            }))
            {
            };
        }
    }
}
