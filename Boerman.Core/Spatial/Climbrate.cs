namespace Boerman.Core.Spatial
{
    public class Climbrate
    {
        private Climbrate()
        {
            
        }

        public static Climbrate FromFpm(double climbrate)
        {
            return new Climbrate {FeetPerMinute = climbrate};
        }

        public double FeetPerMinute { get; private set; }
    }
}
