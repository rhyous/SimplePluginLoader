using System;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// If you want only one logger, use this static singleton.
    /// </summary>
    public static class PluginLoaderLoggerCommon
    {
        public static IPluginLoaderLogger SharedLogger
        {
            get { if (_SharedLogger == null)
                    SafeSet(ref _SharedLogger, () => { return new PluginLoaderLogger(); });
                return _SharedLogger;
            }
        } private static IPluginLoaderLogger _SharedLogger;

        public static void SafeSet<T>(ref T backingField, Func<T> ValueCreator)
            where T : class
        {
            if (backingField == null)
            {
                lock (_Locker)
                {
                    if (backingField == null)
                    {
                        backingField = ValueCreator?.Invoke();
                    }
                }
            }
        } private static object _Locker = new object();
    }
}