using System;
using System.Globalization;
using UnityEngine;

namespace BeatSaberMarkupLanguage
{
    /// <summary>
    /// String parsing utilities.
    /// </summary>
    public static class Parse
    {
        /// <summary>
        /// Parse a string as an <see cref="float"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="float"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static float Float(string s)
        {
            if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            {
                throw new ParseException($"Could not parse '{s}' as a float");
            }

            return result;
        }

        /// <summary>
        /// Parse a string as an <see cref="bool"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="bool"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static bool Bool(string s)
        {
            if (!bool.TryParse(s, out bool result))
            {
                throw new ParseException($"Could not parse '{s}' as a bool");
            }

            return result;
        }

        /// <summary>
        /// Parse a string as an <see cref="int"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="int"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static int Int(string s)
        {
            if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result))
            {
                throw new ParseException($"Could not parse '{s}' as an integer");
            }

            return result;
        }

        /// <summary>
        /// Parse a string as a <see cref="UnityEngine.Vector2"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="UnityEngine.Vector2"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static Vector2 Vector2(string s)
        {
            string[] parts = s.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            float x;
            float y;

            switch (parts.Length)
            {
                case 1:
                    x = y = Float(parts[0]);
                    break;
                case 2:
                    x = Float(parts[0]);
                    y = Float(parts[1]);
                    break;
                default:
                    throw new ParseException("Unexpected number of components");
            }

            return new Vector2(x, y);
        }

        /// <summary>
        /// Parse a string as a <see cref="UnityEngine.Vector3"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <param name="defaultZ">Z value used if the string only has two components.</param>
        /// <returns>A <see cref="UnityEngine.Vector3"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static Vector3 Vector3(string s, float defaultZ = 0)
        {
            string[] parts = s.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            float x;
            float y;
            float z;

            switch (parts.Length)
            {
                case 1:
                    x = y = z = Float(parts[0]);
                    break;
                case 2:
                    x = Float(parts[0]);
                    y = Float(parts[1]);
                    z = defaultZ;
                    break;
                case 3:
                    x = Float(parts[0]);
                    y = Float(parts[1]);
                    z = Float(parts[2]);
                    break;
                default:
                    throw new ParseException("Unexpected number of components");
            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Culture Invariant ToString for BSML values.
        /// </summary>
        /// <param name="obj">The object to stringify.</param>
        /// <returns>The string representation of the value.</returns>
        public static string InvariantToString(this object obj)
        {
            return obj switch
            {
                IConvertible convertible => convertible.ToString(CultureInfo.InvariantCulture),
                _ => obj?.ToString(),
            };
        }
    }
}
