using System;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// If you want only one logger, use this static singleton.
    /// </summary>
    public static class AppDomainLocator
    {
        public static IAppDomain Current
        {
            get { if (_AppDomain == null)
                    SafeSet(ref _AppDomain, () => { return new AppDomainWrapper(AppDomain.CurrentDomain); });
                return _AppDomain;
            }
        } private static IAppDomain _AppDomain;
        
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