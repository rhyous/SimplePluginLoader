﻿using System;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Extensions
{
    internal static class StringExtensions
    {
        public static T ToEnum<T>(this string str, T defaultValue, bool ignoreCase = true, bool allowNumeric = true)
            where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("", "str");
            T ret;
            var isNumeric = str.All(c => Char.IsDigit(c));
            if ((allowNumeric || !isNumeric) && Enum.TryParse(str, ignoreCase, out ret))
                return ret;
            return defaultValue;
        }
    }
}