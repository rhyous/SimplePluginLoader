using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class ObjectCreatorFactoryTests
    {
        private MockRepository mockRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            mockRepository = new MockRepository(MockBehavior.Strict);
        }

        private ObjectCreatorFactory<ITestPlugin> CreateFactory()
        {
            return new ObjectCreatorFactory<ITestPlugin>();
        }

        [TestMethod]
        public void ObjectCreatorFactory_Create_Test()
        {
            // Arrange
            var factory = CreateFactory();

            // Act
            var result = factory.Create();

            // Assert
            Assert.AreEqual(typeof(ObjectCreator<ITestPlugin>), result.GetType());
            mockRepository.VerifyAll();
        }
    }
}
