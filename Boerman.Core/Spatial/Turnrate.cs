namespace Boerman.Core.Spatial
{
    public class Turnrate
    {
        private Turnrate()
        {
            
        }

        public static Turnrate FromTpm(double turnrate)
        {
            return new Turnrate {TurnsPerMinute = turnrate};
        }

        public double TurnsPerMinute { get; private set; }
    }
}
