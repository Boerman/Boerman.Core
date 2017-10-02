using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Boerman.Core.Serialization
{
    public class ObjectSerializer
    {
        /// <summary>
        /// Serialize an object to a byte array.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A byte array. Null on exception.</returns>
        public static byte[] Serialize(object obj)
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
    }
}
