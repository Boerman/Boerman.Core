using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boerman.Core.Underscore
{
    public partial class _
    {
        public static class Math
        {
            public static double Perimeter(double x, double y)
            {
                return x * 2 + y * 2;
            }

            public static double Area(double x, double y)
            {
                return x * y;
            }

            public static double Circumference(double radius)
            {
                return 2 * System.Math.PI * radius;
            }

            public static double CircleArea(double radius)
            {
                return System.Math.PI * System.Math.Pow(radius, 2);
            }

            public static double PythagoreanTheorem(double a, double b)
            {
                return System.Math.Sqrt(System.Math.Pow(a, 2) + System.Math.Pow(b, 2));
            }
        }
    }
}
