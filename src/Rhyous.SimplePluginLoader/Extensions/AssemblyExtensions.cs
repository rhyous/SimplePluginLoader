using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rhyous.SimplePluginLoader
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> TryGetTypes(this IAssembly assembly, bool throwException, IPluginLoaderLogger logger)
        {
            // I hate using a catch but assembly doesn't have a safe way to get Types
            // that don't crash.
            try { return assembly.GetTypes(); }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    logger.Log(loaderException);
                }
                if (throwException)
                {
                    // Unwrap the exception if there is only one
                    if (ex.LoaderExceptions.Length == 1)
                        throw ex.LoaderExceptions[0];
                    throw ex;
                }
                return ex.Types.Where(t => t != null);
            }
        }
    }
}
