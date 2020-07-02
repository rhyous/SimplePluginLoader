using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.Autofac.Tests;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class AutofacObjectCreatorTests
    {
        [TestMethod]
        public void AutofacObjectCreator_Create_Works()
        {
            // Arrange
            Type type = typeof(Organization);

            var builder = new ContainerBuilder();
            builder.RegisterType<Organization>().As<IOrganization>();
            var container = builder.Build();

            var autofacObjectCreator = new AutofacObjectCreator<IOrganization>(container);

            // Act
            var result = autofacObjectCreator.Create(type);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_Null_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var builder = new ContainerBuilder();
            var container = builder.Build();

            var autofacObjectCreator = new AutofacObjectCreator<IOrganization>(container);
            autofacObjectCreator.RegisterTypeMethod = (ContainerBuilder b, Type t1, Type t2) => { return null; };

            // Act
            var result = autofacObjectCreator.Create(type);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AutofacObjectCreator_Create_EmptyGeneric_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var builder = new ContainerBuilder();
            var container = builder.Build();

            var autofacObjectCreator = new AutofacObjectCreator<IOrganization>(container);
            autofacObjectCreator.RegisterTypeMethod = (ContainerBuilder b, Type t1, Type t2) => { return typeof(Service<,,>); };

            // Act
            var result = autofacObjectCreator.Create(type);

            // Assert
            Assert.IsNull(result);
        }
    }
}
