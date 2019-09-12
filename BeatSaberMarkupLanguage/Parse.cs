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
    }
}
