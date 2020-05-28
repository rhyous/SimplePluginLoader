using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class InstanceLoaderFactoryTests
    {
        private MockRepository _MockRepository;

        private Mock<IObjectCreator<ITestPlugin>> _MockObjectCreator;
        private Mock<IObjectCreatorFactory<ITestPlugin>> _MockObjectCreatorFactory;
        private Mock<ITypeLoader<ITestPlugin>> _MockTypeLoader;
        private Mock<IPluginLoaderSettings> _MockPluginLoaderSettings;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockObjectCreator = _MockRepository.Create<IObjectCreator<ITestPlugin>>();
            _MockObjectCreatorFactory = _MockRepository.Create<IObjectCreatorFactory<ITestPlugin>> ();
            _MockTypeLoader = _MockRepository.Create<ITypeLoader<ITestPlugin>>();
            _MockPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
        }

        private InstanceLoaderFactory<ITestPlugin> CreateFactory()
        {
            return new InstanceLoaderFactory<ITestPlugin>(
                _MockObjectCreatorFactory.Object,
                _MockTypeLoader.Object,
                _MockPluginLoaderSettings.Object,
                _MockPluginLoaderLogger.Object);
        }

        [TestMethod]
        public void Create_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var factory = CreateFactory();

            _MockObjectCreatorFactory.Setup(m => m.Create())
                                     .Returns(_MockObjectCreator.Object);
            // Act
            var result = factory.Create();

            // Assert
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }
    }
}
