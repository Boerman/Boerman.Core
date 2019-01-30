using System;
using Boerman.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boerman.Core.Tests
{
    [TestClass]
    public class DateTimeExtensionTests
    {
        [TestMethod]
        public void UnixTime()
        {
            var now = DateTime.Parse("Tue, 21 Mar 2017 14:00:00 GMT");
            var result = now.ToUnixTime();

            Assert.AreEqual(result, 1490108400);
        }

        [TestMethod]
        public void UnixTimeInMilliseconds()
        {
            var now = DateTime.Parse("Tue, 21 Mar 2017 14:00:00 GMT");
            var result = now.ToUnixMilliseconds();

            Assert.AreEqual(result, 1490108400000);
        }

        [TestMethod]
        public void DateTimeFromUnixTime()
        {
            var result = Extensions.Extensions.FromUnixTime(1490108400);

            Assert.AreEqual(DateTime.Parse("Tue, 21 Mar 2017 14:00:00 GMT"), result);
        }

        [TestMethod]
        public void FirstDayOfWeek_1()
        {
            var test = DateTime.Parse("Tue, 21 Mar 2017 14:00:00 GMT");
            var result = test.FirstDayOfWeek();

            Assert.IsTrue(result.Date == DateTime.Parse("Mon, 20 Mar 2017"));
        }

        [TestMethod]
        public void FirstDayOfWeek_2()
        {
            var test = DateTime.Parse("Wed, 1 Mar 2017 14:00:00 GMT");
            var result = test.FirstDayOfWeek();

            Assert.IsTrue(result.Date == DateTime.Parse("Mon, 27 Feb 2017"));
        }

        [TestMethod]
        public void LastDayOfWeek_1()
        {
            var test = DateTime.Parse("Tue, 21 Mar 2017 14:00:00 GMT");
            var result = test.LastDayOfWeek();

            Assert.IsTrue(result.Date == DateTime.Parse("Sun, 26 Mar 2017"));
        }

        [TestMethod]
        public void LastDayOfWeek_2()
        {
            var test = DateTime.Parse("Tue, 28 Mar 2017 14:00:00 GMT");
            var result = test.LastDayOfWeek();

            Assert.IsTrue(result.Date == DateTime.Parse("Sun, 2 Apr 2017"));
        }

        [TestMethod]
        public void NextQuarter_1()
        {
            var test = DateTime.Parse("Tue, 28 Mar 2017 13:59:59 GMT").NextQuarter();
            var result = DateTime.Parse("Tue, 28 Mar 2017 14:00:00 GMT");

            Assert.AreEqual(result, test);
        }

        /// <summary>
        /// Please note that we are exactly at this point in time, which I think, qualifies as legit quarter.
        /// </summary>
        [TestMethod]
        public void NextQuarter_2()
        {
            var test = DateTime.Parse("Tue, 28 Mar 2017 14:00:00 GMT").NextQuarter();
            var result = DateTime.Parse("Tue, 28 Mar 2017 14:00:00 GMT");

            Assert.AreEqual(result, test);
        }

        [TestMethod]
        public void NextQuarter_3()
        {
            var test = DateTime.Parse("Tue, 28 Mar 2017 14:00:01 GMT").NextQuarter();
            var result = DateTime.Parse("Tue, 28 Mar 2017 14:15:00 GMT");

            Assert.AreEqual(result, test);
        }
    }
}
