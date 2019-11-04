using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class AutofacObjectCreatorTests
    {
        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsSimpleClass()
        {
            // Arrange
            var mockPluginLoader = new Mock<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            mockPluginLoader.Setup(m => m.LoadPlugin(It.IsAny<string>()))
                            .Returns((IPlugin<IDependencyRegistrar<ContainerBuilder>>)null);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(mockPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<User>().As(typeof(IUser));
            var container = builder.Build();

            var creator = new AutofacObjectCreator<IUser>(container);

            // Act
            var actual = creator.Create(typeof(User));

            // Assert
            Assert.AreEqual(actual.GetType(), typeof(User));
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsInterface()
        {
            // Arrange
            var mockPluginLoader = new Mock<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            mockPluginLoader.Setup(m => m.LoadPlugin(It.IsAny<string>()))
                            .Returns((IPlugin<IDependencyRegistrar<ContainerBuilder>>)null);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(mockPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<User>().As(typeof(IUser));
            var container = builder.Build();

            var creator = new AutofacObjectCreator<IUser>(container);

            // Act
            var actual = creator.Create(typeof(IUser));

            // Assert
            Assert.AreEqual(actual.GetType(), typeof(User));
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_T_IsGeneric()
        {
            // Arrange
            var mockPluginLoader = new Mock<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            mockPluginLoader.Setup(m => m.LoadPlugin(It.IsAny<string>()))
                            .Returns((IPlugin<IDependencyRegistrar<ContainerBuilder>>)null);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(mockPluginLoader.Object).As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterGeneric(typeof(Service<,,>)).As(typeof(IService<,,>));
            var container = builder.Build();

            var creator = new AutofacObjectCreator<IService<Organization, IOrganization, int>>(container);

            // Act
            var actual = creator.Create(typeof(Service<Organization, IOrganization, int>));

            // Assert
            Assert.AreEqual(actual.GetType(), typeof(Service<Organization, IOrganization, int>));
        }


    }
}
