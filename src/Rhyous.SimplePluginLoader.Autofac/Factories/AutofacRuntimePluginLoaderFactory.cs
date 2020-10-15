using Autofac;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for easily creating RuntimePluginLoader instances.
    /// </summary>
    public class AutofacRuntimePluginLoaderFactory : IRuntimePluginLoaderFactory
    {
        #region Singleton

        /// <summary>
        /// A static factory, that can be used in objects that may not have access
        /// to Dependency Injection.
        /// </summary>
        public static IRuntimePluginLoaderFactory Instance { get; set; }

        private readonly ILifetimeScope _Scope;

        /// <summary>
        /// The AutofacRuntimePluginLoaderFactory constructor
        /// </summary>
        /// <param name="scope">An Autofac ComponentContext object.</param>
        public AutofacRuntimePluginLoaderFactory(ILifetimeScope scope)
        {
            _Scope = scope;
        }

        #endregion


        /// <summary>
        /// Create an instance of IRuntimePluginLoader{T} using the Autofac 
        /// ComponentContext to resolve.
        /// </summary>
        /// <typeparam name="TRuntimePluginLoader">A type of IRuntimePluginLoader{T} to create.</typeparam>
        /// <typeparam name="T">The Plugin type. This is usually an interface.</typeparam>
        /// <param name="dependencies">Additional dependencies. These must be passed in in order.</param>
        /// <returns>An instantiated instance of IPluginObjectCreator{T}.</returns>
        public IRuntimePluginLoader<T> Create<TRuntimePluginLoader, T>(params object[] dependencies)
            where TRuntimePluginLoader : class, IRuntimePluginLoader<T>
            where T : class
        {
            var t = _Scope.ResolveOptional<TRuntimePluginLoader>();
            if (t == null)
            {
                var subscope = _Scope.BeginLifetimeScope(b => { b.RegisterType<TRuntimePluginLoader>(); });
                t = subscope.Resolve<TRuntimePluginLoader>();
            }
            return t;
        }
    }
}