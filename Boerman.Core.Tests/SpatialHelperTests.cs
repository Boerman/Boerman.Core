using Boerman.Core.Spatial;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Boerman.Core.Tests
{
    [TestClass]
    public class SpatialHelperTests
    {
        private void RunTest(TPVector vector, TurnDirection expected)
        {
            // Execute the test
            var result = SpatialHelpers.DetermineTurnDirection(vector);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DetermineDirection_1()
        {
            RunTest(new TPVector(
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 2)), TurnDirection.None);
        }

        [TestMethod]
        public void DetermineDirection_2()
        {
            RunTest(new TPVector(
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 3)), TurnDirection.OverLeft);
        }

        [TestMethod]
        public void DetermineDirection_3()
        {
            RunTest(new TPVector(
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 1)), TurnDirection.OverRight);
        }

        [TestMethod]
        public void DetermineDirection_4()
        {
            RunTest(new TPVector(
                new Point(0, 0),
                new Point(-1, -1),
                new Point(-2, -2)), TurnDirection.None);
        }

        [TestMethod]
        public void DetermineDirection_5()
        {
            RunTest(new TPVector(
                new Point(0, 0),
                new Point(-1, -1),
                new Point(-2, -3)), TurnDirection.OverLeft);
        }

        [TestMethod]
        public void DetermineDirection_6()
        {
            RunTest(new TPVector(
                new Point(0, 0),
                new Point(-1, -1),
                new Point(-2, -1)), TurnDirection.OverRight);
        }
    }
}
