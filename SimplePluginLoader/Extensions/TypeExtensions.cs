using System;

namespace SimplePluginLoader
{
    public static class TypeExtensions
    {
        public static bool IsSameOrSubclassAs(this Type currentType, Type type)
        {
            return currentType.IsSubclassOf(type)
                   || currentType == type;
        }
    }
}
