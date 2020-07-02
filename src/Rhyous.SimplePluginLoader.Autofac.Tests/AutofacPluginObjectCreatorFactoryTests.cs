using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class AutofacPluginObjectCreatorFactoryTests
    {
        [TestMethod]
        public void AutofacPluginObjectCreatorFactory_Create_Test()
        {
            // Arrange

            var builder = new ContainerBuilder();
            builder.RegisterType<PluginDependencyRegistrar>()
                   .As<IPluginDependencyRegistrar>()
                   .SingleInstance();
            builder.RegisterGeneric(typeof(AutofacPluginObjectCreator<>))
                   .As(typeof(IPluginObjectCreator<>));
            var container = builder.Build();
            var factory = new AutofacPluginObjectCreatorFactory<Organization>(container);

            // Act
            var result = factory.Create();

            // Assert
            Assert.AreEqual(typeof(AutofacPluginObjectCreator<Organization>), result.GetType());
            Assert.IsNotNull(result);
        }
    }
}
