using Ninject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader.DependencyInjection.Tests;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    [TestClass]
    public class NinjectObjectCreatorTests
    {
        [TestMethod]
        public void NinjectObjectCreator_Create_Works()
        {
            // Arrange
            Type type = typeof(Organization);

            var rootScope = new StandardKernel();
            rootScope.Bind<IOrganization>().To<Organization>();
            var NinjectObjectCreator = new NinjectObjectCreator<IOrganization>(rootScope);

            // Act
            var result = NinjectObjectCreator.Create(type);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void NinjectObjectCreator_Create_Null_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var rootScope = new StandardKernel();

            var NinjectObjectCreator = new NinjectObjectCreator<IOrganization>(rootScope);
            NinjectObjectCreator.RegisterTypeMethod = (IKernel b, Type t1, Type t2) => { return null; };

            // Act
            var result = NinjectObjectCreator.Create(type);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NinjectObjectCreator_Create_EmptyGeneric_TypeToLoad()
        {
            // Arrange
            Type type = typeof(Organization);

            var rootScope = new StandardKernel();

            var NinjectObjectCreator = new NinjectObjectCreator<IOrganization>(rootScope);
            NinjectObjectCreator.RegisterTypeMethod = (IKernel b, Type t1, Type t2) => { return typeof(Service<,,>); };

            // Act
            var result = NinjectObjectCreator.Create(type);

            // Assert
            Assert.IsNull(result);
        }
    }
}
