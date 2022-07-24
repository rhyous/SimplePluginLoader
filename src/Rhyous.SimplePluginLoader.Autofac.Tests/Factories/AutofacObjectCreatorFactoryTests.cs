using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class AutofacObjectCreatorFactoryTests
    {

        [TestMethod]
        public void AutofacObjectCreatorFactory_Create_Test()
        {
            // Arrange

            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(AutofacObjectCreator<>))
                   .As(typeof(IObjectCreator<>));
            var container = builder.Build();
            var factory = new AutofacObjectCreatorFactory<Organization>(container);

            // Act
            var result = factory.Create();

            // Assert
            Assert.AreEqual(typeof(AutofacObjectCreator<Organization>), result.GetType());
            Assert.IsNotNull(result);
        }
    }
}
