/*
 * ToDo: Add support for comparing complex properties.
 */

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Boerman.Core.Extensions
{
    public static class ObjectExtensions
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

        public static object IfThen(this object target, object ifThis, object thenThat)
        {
            return target == ifThis ? thenThat : target;
        }
        
        public static T IfThen<T>(this T target, Func<T, bool> predicate, T thenThis)
        {
            return predicate.Invoke(target) ? thenThis : target;
        }

        public static object IfThen(this object target, Func<object, bool> predicate, object thenThis)
        {
            return predicate.Invoke(target) ? thenThis : target;
        }

        /// <summary>
        /// Serialize an object to a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A byte array. Null on exception.</returns>
        public static byte[] Serialize(this object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                stream.Close();
            }
        }

        /// <summary>
        /// Deserialize an byte array to a specific object.
        /// </summary>
        /// <param name="buffer">The byte array to deserialize</param>
        /// <returns>The object T to serialize to. Null on exception</returns>
        public static T Deserialize<T>(this byte[] buffer) where T : class
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(buffer)
            {
                Position = 0
            };

            try
            {
                return formatter.Deserialize(stream) as T;
            }
            catch
            {
                return null;
            }
            finally
            {
                stream.Close();
            }
        }
    }
}
