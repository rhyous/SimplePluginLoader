using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Extensions
{
    internal static class StringExtensions
    {
        public static T ToEnum<T>(this string str, T defaultValue, bool ignoreCase = true, bool allowNumeric = true)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum", "T");
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            var isNumeric = str.All(c => Char.IsDigit(c));
            if ((allowNumeric || !isNumeric) && Enum.TryParse(str, ignoreCase, out T ret))
                return ret;
            return defaultValue;
        }
    }
}