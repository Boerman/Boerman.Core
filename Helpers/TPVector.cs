namespace Boerman.Core.Helpers
{
    /// <summary>
    /// ThreePointVector
    /// </summary>
    public struct TPVector
    {
        public Point A;
        public Point B;
        public Point C;

        public TPVector(Point a, Point b, Point c)
        {
            A = a;
            B = b;
            C = c;
        }
    }

    public struct Point
    {
        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X;
        public double Y;
    }

    public struct Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X;
        public double Y;
        public double Z;
    }
}