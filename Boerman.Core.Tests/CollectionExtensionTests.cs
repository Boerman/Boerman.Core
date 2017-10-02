using Boerman.Core.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Boerman.Core.Tests
{
    [TestClass]
    public class CollectionExtensionTests
    {
        [TestMethod]
        public void AddRangeTest_1()
        {
            var destination = new List<string>()
            {
                "s1",
                "s2",
                "s3"
            };

            destination.AddRange(new List<string>()
            {
                "s4",
                "s5",
                "s6"
            });

            Assert.AreEqual(destination.Count, 6);
        }

        [TestMethod]
        public void AddRangeTest_2()
        {
            Collection<string> destination = new Collection<string>()
            {
                "s1",
                "s2",
                "s3"
            };

            destination.AddRange(new[]
            {
                "s4",
                "s5",
                "s6"
            });

            Assert.AreEqual(destination.Count, 6);
        }

        [TestMethod]
        public void JoinCollectionTest()
        {
            var enumerable = new[]
            {
                "a",
                "b",
                "c"
            };

            var result = enumerable.Join(", ");

            Assert.AreEqual(result, "a, b, c");
        }

        [TestMethod]
        public void NameValueCollectionToDictionary()
        {
            var test = new NameValueCollection()
            {
                { "k1", "v1" },
                { "k2", "v2" },
                { "k3", "v3" }
            };

            test.ToDictionary<string, string>();

            Assert.AreEqual(test.Count, 3);
            Assert.AreEqual(test.Get("k1"), "v1");
            Assert.AreEqual(test.Get("k2"), "v2");
            Assert.AreEqual(test.Get("k3"), "v3");
        }
    }
}
