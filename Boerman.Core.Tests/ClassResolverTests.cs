using System.Linq;
using Boerman.Core.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boerman.Core.Tests
{
    [TestClass]
    public class ClassResolverTests
    {
        /// <summary>
        /// Checks whether the resolver can resolve a public class (defined in this scope)
        /// </summary>
        [TestMethod]
        public void ResolvePublicClass()
        {
            var result = ClassResolver.Resolve("ClassToResolve");

            Assert.AreEqual(result.Count(), 1);
            Assert.IsTrue(result.First().IsAssignableFrom(typeof(ClassResolverTarget)));
        }

        /// <summary>
        /// Checks whether the resolver can resolve a class by a name which does not exist. (Spoiler alert, it can't)
        /// </summary>
        [TestMethod]
        public void ResolveClassWhichDoesNotExist()
        {
            var result = ClassResolver.Resolve("de625b81-7659-4d69-89d0-2c1089b0be39");

            Assert.IsTrue(!result.Any());
        }

        /// <summary>
        /// Checks whether the resolver can resolve a private class which is defined in this scope.
        /// </summary>
        [TestMethod]
        public void ResolvePrivateClass()
        {
            var result = ClassResolver.Resolve("PrivateClassToResolve");

            Assert.AreEqual(result.Count(), 1);
            Assert.IsTrue(result.First().IsAssignableFrom(typeof(PrivateClassResolverTarget)));
        }

        /// <summary>
        /// Checks whether the resolver can resolve multiple classes which have the same name.
        /// </summary>
        [TestMethod]
        public void ResolveMultipleClasses()
        {
            var result = ClassResolver.Resolve("OneOfMultiple");

            Assert.AreEqual(result.Count(), 2);
            Assert.IsTrue(result.Any(q => q.IsAssignableFrom(typeof(OneOfMultipleClassResolverTargets))));
            Assert.IsTrue(result.Any(q => q.IsAssignableFrom(typeof(TwoOfMultipleClassResolverTargets))));
        }

        /// <summary>
        /// This test summarises the workings of the resolver which looks at the type. Most importantly the types returned must be of a specific type.
        /// </summary>
        [TestMethod]
        public void ResolveMultipleWithType()
        {
            var result = ClassResolver.Resolve<OneOfMultipleClassResolverTargets>("OneOfMultiple");

            Assert.AreEqual(result.Count(), 1);
            Assert.IsTrue(result.First().GetType().IsAssignableFrom(typeof(OneOfMultipleClassResolverTargets)));
        }

        #region classes
        [ClassResolver(Name = "ClassToResolve")]
        public class ClassResolverTarget
        {
            public string Name = "Class";
        }

        [ClassResolver(Name = "PrivateClassToResolve")]
        private class PrivateClassResolverTarget
        {
            public string Name = "Private Class";
        }

        [ClassResolver(Name = "OneOfMultiple")]
        private class OneOfMultipleClassResolverTargets
        {

        }

        [ClassResolver(Name = "OneOfMultiple")]
        private class TwoOfMultipleClassResolverTargets
        {

        }
        #endregion
    }
}
