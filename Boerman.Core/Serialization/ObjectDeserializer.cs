using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Boerman.Core.Serialization
{
    public class ObjectDeserializer<T> where T : class
    {
        /// <summary>
        /// Deserialize an byte array to a specific object.
        /// </summary>
        /// <param name="buffer">The byte array to deserialize</param>
        /// <returns>The object T to serialize to. Null on exception</returns>
        public static T Deserialize(byte[] buffer)
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
