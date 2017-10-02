/*
 * This code originates from http://www.sanity-free.com/134/standard_crc_16_in_csharp.html
 */

using System;
using System.Linq;

namespace Boerman.Core.Helpers
{
    public static class Crc16
    {
        private const ushort Polynomial = 0xA001;
        private static readonly ushort[] Table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0;
            for (var i = 0; i < bytes.Length; ++i)
            {
                var index = (byte) (crc ^ i);
                crc = (ushort) ((crc >> 8) ^ Table[index]);
            }
            return crc;
        }

        public static byte[] ComputeChecksumBytes(byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        static Crc16()
        {
            for (ushort i = 0; i < Table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;

                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort) ((value >> 1) ^ Polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                Table[i] = value;
            }
        }
    }

    public static class Crc8
    {
        private static readonly byte[] Table = new byte[256];
        // x8 + x7 + x6 + x4 + x2 + 1
        private const byte Poly = 0xd5;

        public static byte ComputeChecksum(params byte[] bytes)
        {
            byte crc = 0;
            if (bytes != null && bytes.Length > 0)
                crc = bytes.Aggregate(crc, (current, b) => Table[current ^ b]);

            return crc;
        }

        static Crc8()
        {
            for (int i = 0; i < 256; ++i)
            {
                int temp = i;
                for (int j = 0; j < 8; ++j)
                {
                    if ((temp & 0x80) != 0)
                    {
                        temp = (temp << 1) ^ Poly;
                    }
                    else
                    {
                        temp <<= 1;
                    }
                }
                Table[i] = (byte)temp;
            }
        }
    }
}
