/*
 * ToDo: Add support for comparing complex properties.
 */

using System;

namespace Boerman.Core.Extensions
{
    public static partial class Extensions
    {
        /// <returns>1 for equal. 0 when unable to determine equality. -1 with an inequality.</returns>
        public static int CompareWith<T>(this T o1, T o2) where T : class
        {
            var properties = o1.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (!Object.Equals(property.GetValue(o1), property.GetValue(o2)))
                    return -1;
            }

            return 1;
        }
    }
}
