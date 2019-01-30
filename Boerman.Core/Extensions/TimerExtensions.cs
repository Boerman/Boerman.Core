using System.Timers;

namespace Boerman.Core.Extensions
{
    public static partial class Extensions
    {
        public static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }
}
