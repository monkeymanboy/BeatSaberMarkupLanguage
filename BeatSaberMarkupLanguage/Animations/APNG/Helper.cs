using System;

namespace BeatSaberMarkupLanguage.Animations
{
    internal class Helper
    {
        /// <summary>
        /// Compare two byte arrays.
        /// </summary>
        /// <param name="byte1">First byte array.</param>
        /// <param name="byte2">Second byte array.</param>
        /// <returns>Value with endianness swapped.</returns>
        public static bool IsBytesEqual(byte[] byte1, byte[] byte2)
        {
            if (byte1.Length != byte2.Length)
            {
                return false;
            }

            for (int i = 0; i < byte1.Length; i++)
            {
                if (byte1[i] != byte2[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert big-endian to little-endian or reverse.
        /// </summary>
        /// <param name="i">Value to convert as an array of bytes.</param>
        /// <returns>Value with endianness swapped.</returns>
        internal static byte[] ConvertEndian(byte[] i)
        {
            if (i.Length % 2 != 0)
            {
                throw new ArgumentException("Byte array length must multiple of 2");
            }

            Array.Reverse(i);

            return i;
        }

        /// <summary>
        /// Convert big-endian to little-endian or reverse.
        /// </summary>
        /// <param name="i">Value to convert.</param>
        /// <returns>Value with endianness swapped.</returns>
        internal static int ConvertEndian(int i)
        {
            return BitConverter.ToInt32(ConvertEndian(BitConverter.GetBytes(i)), 0);
        }

        /// <summary>
        /// Convert big-endian to little-endian or reverse.
        /// </summary>
        /// <param name="i">Value to convert.</param>
        /// <returns>Value with endianness swapped.</returns>
        internal static uint ConvertEndian(uint i)
        {
            return BitConverter.ToUInt32(ConvertEndian(BitConverter.GetBytes(i)), 0);
        }

        /// <summary>
        /// Convert big-endian to little-endian or reverse.
        /// </summary>
        /// <param name="i">Value to convert.</param>
        /// <returns>Value with endianness swapped.</returns>
        internal static short ConvertEndian(short i)
        {
            return BitConverter.ToInt16(ConvertEndian(BitConverter.GetBytes(i)), 0);
        }

        /// <summary>
        ///     Convert big-endian to little-endian or reverse.
        /// </summary>
        /// <param name="i">Value to convert.</param>
        /// <returns>Value with endianness swapped.</returns>
        internal static ushort ConvertEndian(ushort i)
        {
            return BitConverter.ToUInt16(ConvertEndian(BitConverter.GetBytes(i)), 0);
        }
    }
}
