using System;

namespace Boerman.Core.Spatial
{
    public static class SpatialHelpers
    {
        // For info check out cool answer here: https://stackoverflow.com/a/5024675/1720761
        private static double GetHeadingError(double initial, double final)
        {
            if (initial > 360 || initial < 0)
                throw new ArgumentOutOfRangeException(nameof(initial));

            if (final > 360 || final < 0)
                throw new ArgumentOutOfRangeException(nameof(final));

            var diff = final - initial;
            var absDiff = Math.Abs(diff);

            if (absDiff <= 180)
                return absDiff == 180 ? absDiff : diff;
            if (final > initial)
                return absDiff - 360;

            return 360 - absDiff;
        }

        public static double DegreeLongitudeToKilometers(double longitude)
        {
            return 2 * Math.PI / 360 * 6378 * Math.Cos(longitude);
        }

        public static double DegreeLatitudeToKilometers(double latitude)
        {
            // This is a fairly constant constant (110.567km at the equator to 111.699km at the poles.)
            // return (2*Math.PI/360)*6378;
            return 111;
        }

        public static bool ArePointsInLine(double margin, TPVector vector)
        {
            return ArePointsInLine(margin, vector.A, vector.B, vector.C);
        }

        public static bool ArePointsInLine(double margin, Point a, Point b, Point c)
        {
            return GetLineDeviation(a, b, c) <= margin;
        }

        public static double GetLineDeviation(TPVector vector)
        {
            return GetLineDeviation(vector.A, vector.B, vector.C);
        }
        public static double GetLineDeviation(Point a, Point b, Point c)
        {
            // ToDo: Create a function which checks whether an unspecified amount of points is on the same line
            return (a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y)) / 2;
        }

        public static TurnDirection DetermineTurnDirection(TPVector vector)
        {

            var result = GetLineDeviation(vector);

            if (result == 0) return TurnDirection.None;

            return result < 0 ? TurnDirection.OverRight : TurnDirection.OverLeft;
        }

    }
}
