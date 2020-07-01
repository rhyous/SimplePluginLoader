using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.SimplePluginLoader.Tests.TestClasses;
using System;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class ObjectCreatorTests
    {
        #region Non-generic

        [TestMethod]
        public void ObjectCreator_Create_Constructorless_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<Org>();

            // Act
            var result = objectCreator.Create();

            // Assert
            Assert.AreEqual(typeof(Org), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_EmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<Org2>();

            // Act
            var result = objectCreator.Create();

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_CreatorIsForParentEmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<Org>();
            var type = typeof(Org2);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_NoEmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<TestPluginLoader>();
            Type type = typeof(TestPluginLoader);

            // Act
            // Assert
            Assert.ThrowsException<MissingMethodException>(() => 
            {
                objectCreator.Create(type);
            });
        }
        #endregion

        #region Non-generic Interface parent

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_Constructorless_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<IOrg>();
            var type = typeof(Org);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_EmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<IOrg>();
            var type = typeof(Org2);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_CreatorIsForParentEmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<IOrg>();
            var type = typeof(Org2);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Org2), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_InterfaceParent_NoEmptyConstructor_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<IRuntimePluginLoader<Org>>();
            Type type = typeof(TestPluginLoader);

            // Act
            // Assert
            Assert.ThrowsException<MissingMethodException>(() =>
            {
                objectCreator.Create(type);
            });
        }
        #endregion

        #region Generic
        [TestMethod]
        public void ObjectCreator_Create_GenericWithParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<Entity<Org>>();
            var type = typeof(Entity<Org>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_GenericWithoutParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<Entity<Org>>();
            var type = typeof(Entity<>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_Interface_GenericWithParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<IEntity<Org>>();
            var type = typeof(Entity<Org>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ObjectCreator_Create_Interface_GenericWithoutParameter_Constructorless_Model()
        {
            // Arrange
            var objectCreator = new ObjectCreator<IEntity<Org>>();
            var type = typeof(Entity<>);

            // Act
            var result = objectCreator.Create(type);

            // Assert
            Assert.AreEqual(typeof(Entity<Org>), result.GetType());
            Assert.IsNotNull(result);
        }
        #endregion
    }
}
