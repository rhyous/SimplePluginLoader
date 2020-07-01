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
    }
}
