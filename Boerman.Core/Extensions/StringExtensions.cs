using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Boerman.Core.Extensions
{
    public static class StringExtensions
    {
        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte[] HexStringToByteArray(this string hex)
        {
            if (hex.StartsWith("0x", true, CultureInfo.InvariantCulture))
                hex = hex.Remove(0, 2);

            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static string OrDefault(this string str, string def)
        {
            return String.IsNullOrEmpty(str) ? def : str;
        }

        public static object OrDefault(this object obj, object def)
        {
            return obj ?? def;
        }

        public static byte[] HexStringToBytes(string hexString)
        {
            if (hexString == null)
                throw new ArgumentNullException("hexString");
            if (hexString.Length % 2 != 0)
                throw new ArgumentException("hexString must have an even length", "hexString");
            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                string currentHex = hexString.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(currentHex, 16);
            }
            return bytes;
        }

        public static string AddQueryParameters(this string str, IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            return queryParams == null
                ? str
                : $"{str}?{String.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
        }

        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new BinaryWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string ToText(this double degree, bool useFullText = false)
        {
            int val = (int)((degree / 22.5) + .5);

            var shortText = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            var longText = new[] { "North", "North-northeast", "Northeast", "East-northeast", "East", "East-southeast", "Southeast", "South-southeast", "South", "South-southwest", "Southwest", "West-southwest", "West", "West-northwest", "Northwest", "North-northwest" };

            return useFullText ? longText[val%16] : shortText[val % 16];
        }

        /// <summary>
        /// Extension method that wraps the <see cref="string.Join(string, IEnumerable{string})"/> method.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> values, string separator)
        {
            return String.Join(separator, values);
        }

        /// <summary>
        /// Creates the a cryptographic hash of a given string.
        /// </summary>
        /// <param name="str">The string to hash</param>
        /// <param name="algorithm">The hashing algorithm to use. By default MD5 hashing is being used.</param>
        /// <returns></returns>
        public static string Hash(this string str, HashAlgorithm algorithm = null)
        {
            if (algorithm == null) algorithm = MD5.Create();

            return algorithm.ComputeHash(str.GetBytes()).GetString();
        }

        /// <summary>
        /// Extension methods that return an alternate value if the first predicate is true.
        /// Usefull for ETL processes.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="ifThis"></param>
        /// <param name="thenThat"></param>
        /// <returns></returns>
        public static string IfThen(this string target, string ifThis, string thenThat)
        {
            return target == ifThis ? thenThat : target;
        }
    }
}
