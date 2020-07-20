using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Rhyous.SimplePluginLoader.Tests.Extensions
{
    [TestClass]
    public class TypeExtensionsTests
    {
        public interface IA<T> { T Item { get; set; } };
        public class A<T> : IA<T> { public T Item { get; set; } };
        public class B { }
        public class C : A<B> { }

        public interface IWith3<T1, T2, T3> { }
        public class With3 : IWith3<int, int, int> { }

        [TestMethod]
        public void TypeExtensions_IsAssignableFrom_With3Generics()
        {
            // Arrange
            var type1 = typeof(IWith3<int, int, int>);
            var type2 = typeof(With3);

            // Act
            var actual = type1.IsAssignableFrom(type2);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsAssignableFrom_With3GenericsOrg()
        {
            // Arrange
            var type1 = typeof(IRepository<Org, IOrg, int>);
            var type2 = typeof(OrgRepository);

            // Act
            var actual = type1.IsAssignableFrom(type2);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericInterfaceOf_EmptyConcreteGeneric()
        {
            // Arrange
            var type1 = typeof(IA<>);
            var type2 = typeof(A<>);

            // Act
            var actual = type1.IsGenericInterfaceOf(type2);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericInterfaceOf_ConcreteGeneric()
        {
            // Arrange
            var type1 = typeof(IA<>);
            var type2 = typeof(A<B>);

            // Act
            var actual = type1.IsGenericInterfaceOf(type2);

            // Assert
            Assert.IsTrue(actual);
        }
        
        [TestMethod]
        public void TypeExtensions_IsGenericInterfaceOf_NonInterfaceNonGeneric()
        {
            // Arrange
            var type1 = typeof(IA<>);
            var type2 = typeof(B);

            // Act
            var actual = type1.IsGenericInterfaceOf(type2);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericInterfaceOf_ConcreteNonGeneric()
        {
            // Arrange
            var type1 = typeof(IA<>);
            var type2 = typeof(C);

            // Act
            var actual = type1.IsGenericInterfaceOf(type2);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsPluginType_Test()
        {
            // Arrange
            var type = typeof(With3);

            // Act
            var actual = type.IsTypeOf(typeof(IWith3<int, int, int>));

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions__Test()
        {
            // Arrange
            MyDelegate1 d = (int a) => { return true; };

            // Act
            var actual = d.GetType().GetFixedDeclaringType();

            // Assert
            Assert.AreEqual(this.GetType(), actual);
        }

        public delegate bool MyDelegate1(int a);

    }
}
