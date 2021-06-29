using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Rhyous.SimplePluginLoader.Tests.Extensions
{
    [TestClass]
    public class TypeExtensionsTests
    {
        public interface IA<T> { T Item { get; set; } }
        public interface IE<T, T2> : IA<T>{ }
        public class A<T> : IA<T> { public T Item { get; set; } }
        public class B { }
        public class C : A<B> { }
        public class D<T> : A<T> { }
        public class DD : D<B> { }
        public class E<T, T2> : A<T>, IE<T, T2> { }
        public class E1<T> : E<T, B> { }
        public class E2<T2> : E<B, T2> { }

        public interface IWith3<T1, T2, T3> { }
        public class With3 : IWith3<int, int, int> { }

        #region IsAssignableFrom 
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
        #endregion

        #region IsGenericInterfaceOf
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
        public void TypeExtensions_IsGenericInterfaceOf_LeftConcreteGenericInterface_RightConcreteGenericChild()
        {
            // Arrange
            var left = typeof(IA<B>);
            var right = typeof(D<>);

            // Act
            var actual = left.IsGenericInterfaceOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericInterfaceOf_LeftConcreteGenericInterface_RightConcreteGenericChild_DifferentGenericParams()
        {
            // Arrange
            var left = typeof(IA<B>);
            var right = typeof(E<,>);

            // Act
            var actual = left.IsGenericInterfaceOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericInterfaceOf_LeftConcreteGenericInterface_RightConcreteGenericChild_DifferentGenericParams_1()
        {
            // Arrange
            var left = typeof(IA<B>);
            var right = typeof(E1<>);

            // Act
            var actual = left.IsGenericInterfaceOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftConcreteGeneric_RightConcreteGenericChild_2()
        {
            // Arrange
            var left = typeof(IE<B, B>);
            var right = typeof(A<>);

            // Act
            var actual = right.IsPluginType<IE<B, B>>();

            // Assert
            Assert.IsFalse(actual);
        }
        #endregion

        #region IsTypeOf
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
        #endregion

        #region GetFixedDeclaringType()
        [TestMethod]
        public void TypeExtensions__Test()
        {
            // Arrange
            MyDelegate1 d = (int a) => { return true; };

            // Act
            var actual = d.GetType().GetFixedDeclaringType();

            // Assert
            Assert.AreEqual(this.GetType(), actual);
        } public delegate bool MyDelegate1(int a);

        #endregion

        #region IsGenericTypeDefinitionOf
        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftEmptyGeneric_RightConcreteGeneric()
        {
            // Arrange
            var left = typeof(A<>);
            var right = typeof(A<B>);

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftConcreteGeneric_RightEmptyGeneric()
        {
            // Arrange
            var left = typeof(A<B>);
            var right = typeof(A<>);

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftConcreteGenericChild_RightEmptyGeneric()
        {
            // Arrange
            var left = typeof(D<B>);
            var right = typeof(A<>);

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftEmptyGeneric_RightConcreteGenericChild()
        {
            // Arrange
            var left = typeof(A<>);
            var right = typeof(D<B>);            

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftEmptyGenericChild_RightConcreteGeneric()
        {
            // Arrange
            var left = typeof(D<>);
            var right = typeof(A<B>);

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsFalse(actual); // !!! NOTICE IT IS FALSE
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftConcreteGeneric_RightConcreteGenericChild()
        {
            // Arrange
            var left = typeof(A<B>);
            var right = typeof(D<>);

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsGenericTypeDefinitionOf_LeftConcreteGeneric_RightConcreteNotGenericChild()
        {
            // Arrange
            var left = typeof(A<B>);
            var right = typeof(DD);

            // Act
            var actual = left.IsGenericTypeDefinitionOf(right);

            // Assert
            Assert.IsTrue(actual);
        }
        #endregion

        #region IsPluginType
        [TestMethod]
        public void TypeExtensions_IsPluginType_()
        {
            // Arrange
            var right = typeof(E<,>);

            // Act
            var actual = right.IsPluginType<IA<B>>();

            // Assert
            Assert.IsFalse(actual); // NOTICE: It is false
        }

        [TestMethod]
        public void TypeExtensions_IsPluginType_E1()
        {
            // Arrange
            var right = typeof(E1<>);

            // Act
            var actual = right.IsPluginType<IA<B>>();

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TypeExtensions_IsPluginType_E2()
        {
            // Arrange
            var right = typeof(E2<>);

            // Act
            var actual = right.IsPluginType<IA<B>>();

            // Assert
            Assert.IsTrue(actual);
        }
        #endregion
    }
}
