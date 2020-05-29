using System;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    internal static class ExceptionExtensions
    {
        public static void LogReflectionTypeLoadExceptions(this Exception e, IPluginLoaderLogger logger)
        {
            if (!(e is ReflectionTypeLoadException reflectionTypeLoadException))
            {
                reflectionTypeLoadException = e.InnerException as ReflectionTypeLoadException;
            }
            if (reflectionTypeLoadException != null)
            {
                if (reflectionTypeLoadException.LoaderExceptions != null && reflectionTypeLoadException.LoaderExceptions.Any())
                {
                    foreach (var loaderException in reflectionTypeLoadException.LoaderExceptions)
                    {
                        logger.Log(loaderException);
                    }
                }
            }
        }
    }
}
