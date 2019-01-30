using System;
using System.Threading;

namespace Boerman.Core.Extensions
{
    public static partial class Extensions
    {
        public static bool WaitCancellationRequested(
            this CancellationToken token,
            TimeSpan timeout)
        {
            return token.WaitHandle.WaitOne(timeout);
        }
    }
}
