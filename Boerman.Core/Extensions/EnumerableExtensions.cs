using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Boerman.Core.Extensions
{
    public static partial class Extensions
    {
        public static IEnumerable<T> All<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }

        public static IEnumerable<object> All(this IEnumerator enumerator)
        {
            while (enumerator.MoveNext()) yield return enumerator.Current;
        }
        
        public static bool MultiAny<T>(this IEnumerable<T> enumerable, params Func<T, bool>[] conditions)
        {
            return conditions.All(enumerable.Any);
        }

        public static IEnumerable<T> Find<T>(this IEnumerable<T> enumerable, object value)
        {
            var data = enumerable as T[] ?? enumerable.ToArray();

            return data.Where(
                y => ((object) y).Equals(value)
                     || data
                         .SelectMany(x => x.GetType().GetFields())
                         .Any(x => x.GetValue(y).Equals(value))
                     || data
                         .SelectMany(x => x.GetType().GetProperties())
                         .Where(x => x.GetIndexParameters().Length == 0)
                         .Any(x => x.GetValue(y).Equals(value))
            );
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null) throw new NullReferenceException(nameof(action));
            enumerable.ToList().ForEach(action);
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing(new Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, Random rand)
        {
            return enumerable.ElementAt(
                rand.Next(0, enumerable.Count()));
        }
    }
}
