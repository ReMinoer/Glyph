using System;
using System.Globalization;

namespace Glyph.Xml
{
    static public class XmlTools
    {
        static public string ToString<T>(this T value)
        {
            string text;

            try
            {
                text = (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);
            }
            catch
            {
                text = "";
            }

            return text;
        }

        static public T Parse<T>(this string text)
        {
            T value;

            try
            {
                value = (T)Convert.ChangeType(text, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                value = default(T);
            }

            return value;
        }

        static public T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}