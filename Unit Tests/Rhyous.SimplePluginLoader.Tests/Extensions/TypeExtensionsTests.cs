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

        [TestMethod]
        public void IsGenericInterfaceOf_EmptyConcreteGeneric()
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
        public void IsGenericInterfaceOf_ConcreteGeneric()
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
        public void IsGenericInterfaceOf_NonInterfaceNonGeneric()
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
        public void IsGenericInterfaceOf_ConcreteNonGeneric()
        {
            // Arrange
            var type1 = typeof(IA<>);
            var type2 = typeof(C);

            // Act
            var actual = type1.IsGenericInterfaceOf(type2);

            // Assert
            Assert.IsFalse(actual);
        }
    }
}
