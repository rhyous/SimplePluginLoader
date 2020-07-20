using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Extensions
{
    /// <summary>
    /// These come from Rhyous.StringLibrary and are unit tested in that project.
    /// We copied the code so as not to have any dependency on another NuGet package.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class StringExtensions
    {
        internal static T ToEnum<T>(this string str, T defaultValue, bool ignoreCase = true, bool allowNumeric = true)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be an enum", "T");
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            var isNumeric = str.All(c => char.IsDigit(c));
            if ((allowNumeric || !isNumeric) && Enum.TryParse(str, ignoreCase, out T ret))
                return ret;
            return defaultValue;
        }

        internal static bool ToBool(this string str, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(str))
                return defaultValue;
            if (!bool.TryParse(str, out bool b))
                return defaultValue;
            return b;
        }
    }
}