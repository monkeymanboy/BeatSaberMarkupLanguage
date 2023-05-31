using System;
using System.Globalization;

namespace BeatSaberMarkupLanguage
{
    public static class Parse
    {
        public static float Float(string s)
        {
            try
            {
                return float.Parse(s, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception("Could not parse float: " + s);
            }
        }

        public static bool Bool(string s)
        {
            try
            {
                return bool.Parse(s);
            }
            catch
            {
                throw new Exception("Could not parse bool: " + s);
            }
        }

        public static int Int(string s)
        {
            try
            {
                return int.Parse(s, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception("Could not parse int: " + s);
            }
        }

        /// <summary>
        /// Culture Invariant ToString for BSML values.
        /// </summary>
        /// <param name="obj">The object to stringify.</param>
        /// <returns>The string representation of the value.</returns>
        public static string InvariantToString(this object obj)
        {
            if (obj is float floatValue)
            {
                return floatValue.ToString(CultureInfo.InvariantCulture);
            }
            else if (obj is double doubleValue)
            {
                return doubleValue.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                return obj?.ToString();
            }
        }
    }
}
