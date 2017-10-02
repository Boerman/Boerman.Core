using System;

namespace Boerman.Core.Spatial
{
    public class Speed
    {
        private Speed()
        {
            
        }

        public static Speed FromKnots(double speed)
        {
            return new Speed {Knots = speed};
        }

        public static Speed FromKmh(double speed)
        {
            return new Speed {Knots = speed / 1.85200};
        }

        public static Speed FromMph(double speed)
        {
            return new Speed {Knots = speed / 1.15077945};
        }

        public double Knots { get; private set; }

        public double MilesPerHour => Knots * 1.15077945;
        public double KilometersPerHour => Knots * 1.85200;

        public override string ToString()
        {
            return $"{Math.Floor(Knots)}KTS";
        }
    }
}