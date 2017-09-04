using System;

namespace Boerman.Core.Extensions
{
    public static class ValidationExtensions
    {
        public static void ThrowWhenFalse(this bool b)
        {
            if (!b) throw new Exception();
        }

        public static void ThrowWhenFalse(this bool b, Exception ex)
        {
            if (!b) throw ex;
        }

        public static void ThrowWhenTrue(this bool b)
        {
            if (b) throw new Exception();
        }

        public static void ThrowWhenTrue(this bool b, Exception ex)
        {
            if (b) throw ex;
        }
    }
}
