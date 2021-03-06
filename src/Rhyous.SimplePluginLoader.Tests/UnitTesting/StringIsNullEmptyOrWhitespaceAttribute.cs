﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rhyous.UnitTesting
{
    public class StringIsNullEmptyOrWhitespaceAttribute : Attribute, ITestDataSource
    {

        private readonly Dictionary<string, string> NameMap = new Dictionary<string, string>(3)
        {
            { "null", "Null" },
            { "" , "Empty" },
            { " ", "Whitespace"}
        };

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            return new[]
            {
                    new string[] { null}, // null
                    new string[] { ""},  // Empty
                    new string[] { " "}, // Whitespace
            };
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            return NameMap[data[0]?.ToString() ?? "null"];
        }
    }
}
