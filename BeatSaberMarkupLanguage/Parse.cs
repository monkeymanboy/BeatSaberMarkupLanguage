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
        /// <param name="defaultZ">Z value used if the string only has two components. Defaults to the same behaviour as when casting a <see cref="UnityEngine.Vector2"/> to a <see cref="UnityEngine.Vector3"/> (Z = 0).</param>
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
        /// Parse a string as a <see cref="UnityEngine.Vector4"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="UnityEngine.Vector4"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static Vector4 Vector4(string s)
        {
            string[] parts = s.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            float x;
            float y;
            float z;
            float w;

            switch (parts.Length)
            {
                case 1:
                    x = y = z = w = Float(parts[0]);
                    break;
                case 2:
                    x = z = Float(parts[0]);
                    y = w = Float(parts[1]);
                    break;
                case 3:
                    x = Float(parts[0]);
                    y = w = Float(parts[1]);
                    z = Float(parts[2]);
                    break;
                case 4:
                    x = Float(parts[0]);
                    y = Float(parts[1]);
                    z = Float(parts[2]);
                    w = Float(parts[3]);
                    break;
                default:
                    throw new ParseException("Unexpected number of components");
            }

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Parse a string as a <see cref="UnityEngine.RectOffset"/>.
        /// </summary>
        /// <remarks>The order follows <see href="https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.UI.RectMask2D.html#UnityEngine_UI_RectMask2D_padding">the apparent convention of Unity UI for Vector4 representation of padding</see> (X = Left, Y = Bottom, Z = Right, W = Top).</remarks>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="UnityEngine.RectOffset"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static RectOffset RectOffset(string s)
        {
            string[] parts = s.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            int left;
            int bottom;
            int right;
            int top;

            switch (parts.Length)
            {
                case 1:
                    left = bottom = right = top = Int(parts[0]);
                    break;
                case 2:
                    left = right = Int(parts[0]);
                    bottom = top = Int(parts[1]);
                    break;
                case 3:
                    left = Int(parts[0]);
                    bottom = top = Int(parts[1]);
                    right = Int(parts[2]);
                    break;
                case 4:
                    left = Int(parts[0]);
                    bottom = Int(parts[1]);
                    right = Int(parts[2]);
                    top = Int(parts[3]);
                    break;
                default:
                    throw new ParseException("Unexpected number of components");
            }

            return new RectOffset(top, bottom, left, right);
        }

        /// <summary>
        /// Parse a string as a <see cref="UnityEngine.Color"/>.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>A <see cref="UnityEngine.Color"/> representation of the string.</returns>
        /// <exception cref="ParseException">Thrown if the string cannot be parsed.</exception>
        public static Color Color(string s)
        {
            if (ColorUtility.TryParseHtmlString(s, out Color color))
            {
                return color;
            }
            else
            {
                throw new ParseException($"Invalid color '{s}'");
            }
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
