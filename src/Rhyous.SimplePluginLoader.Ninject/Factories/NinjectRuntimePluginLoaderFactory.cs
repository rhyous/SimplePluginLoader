using Ninject;
using Ninject.Extensions.ChildKernel;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// A factory for easily creating RuntimePluginLoader instances.
    /// </summary>
    public class NinjectRuntimePluginLoaderFactory : IRuntimePluginLoaderFactory
    {
        #region Singleton

        private readonly IKernel _Scope;

        /// <summary>
        /// The NinjectRuntimePluginLoaderFactory constructor
        /// </summary>
        /// <param name="scope">An Ninject ComponentContext object.</param>
        public NinjectRuntimePluginLoaderFactory(IKernel scope)
        {
            _Scope = scope;
        }

        #endregion


        /// <summary>
        /// Create an instance of IRuntimePluginLoader{T} using the Ninject 
        /// ComponentContext to resolve.
        /// </summary>
        /// <typeparam name="TRuntimePluginLoader">A type of IRuntimePluginLoader{T} to create.</typeparam>
        /// <typeparam name="T">The Plugin type. This is usually an interface.</typeparam>
        /// <param name="dependencies">Additional dependencies. These must be passed in in order.</param>
        /// <returns>An instantiated instance of IPluginObjectCreator{T}.</returns>
        public IRuntimePluginLoader<T> Create<TRuntimePluginLoader, T>(params object[] dependencies)
            where TRuntimePluginLoader : class, IRuntimePluginLoader<T>
        {
            var t = _Scope.TryGet<TRuntimePluginLoader>();
            if (t == null)
            {
                var subscope = new ChildKernel(_Scope);
                subscope.Bind<TRuntimePluginLoader>().ToSelf();
                t = subscope.Get<TRuntimePluginLoader>();
            }
            return t;
        }
    }
}