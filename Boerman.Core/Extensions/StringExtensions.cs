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
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
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
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
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

        [Obsolete("Yo fucker use the `GetBytes` extension method")]
        public static byte[] ToByteArray(this string str)
        {
            return str.GetBytes();
        }

        public static string ToText(this double degree, bool useFullText = false)
        {
            int val = (int)((degree / 22.5) + .5);

            var shortText = new[] { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
            var longText = new[] { "North", "North-northeast", "Northeast", "East-northeast", "East", "East-southeast", "Southeast", "South-southeast", "South", "South-southwest", "Southwest", "West-southwest", "West", "West-northwest", "Northwest", "North-northwest" };

            return useFullText ? longText[val%16] : shortText[val % 16];
        }
        
        public static string Join(this string[] arr, string separator)
        {
            return String.Join(separator, arr);
        }

        public static string Hash(this string str)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA256.Create();
            return algorithm.ComputeHash(str.GetBytes()).GetString();
        }
    }
}
