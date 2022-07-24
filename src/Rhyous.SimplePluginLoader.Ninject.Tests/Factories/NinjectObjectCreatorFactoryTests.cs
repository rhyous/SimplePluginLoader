using Ninject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    [TestClass]
    public class NinjectObjectCreatorFactoryTests
    {

        [TestMethod]
        public void NinjectObjectCreatorFactory_Create_Test()
        {
            // Arrange

            var rootScope = new StandardKernel();
            rootScope.Bind(typeof(IObjectCreator<>))
                     .To(typeof(NinjectObjectCreator<>));
            var factory = new NinjectObjectCreatorFactory<Organization>(rootScope);

            // Act
            var result = factory.Create();

            // Assert
            Assert.AreEqual(typeof(NinjectObjectCreator<Organization>), result.GetType());
            Assert.IsNotNull(result);
        }
    }
}
