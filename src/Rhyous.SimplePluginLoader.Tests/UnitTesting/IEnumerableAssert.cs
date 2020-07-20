using System.Collections.Generic;
using System.Linq;

namespace Rhyous.UnitTesting
{
    public class IEnumerableAssert
    {
        public const string Null = "Null";
        public const string Empty = "Empty";

        public static void IsNullOrEmpty<T>(IEnumerable<T> enumerable, string testTitle)
        {
            if (testTitle == Null)
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNull(enumerable, testTitle);
            if (testTitle == Empty)
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsFalse(enumerable.Any());
        }
    }
}
