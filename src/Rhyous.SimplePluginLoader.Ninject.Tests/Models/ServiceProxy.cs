using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public class ServiceProxy<TEntity, TInterface, TId> : IService<TEntity, TInterface, TId>
        where TEntity : class, TInterface, new()
        where TInterface : IBaseEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        private readonly IServiceLoader<IService<TEntity, TInterface, TId>> _ServiceLoader;

        public ServiceProxy(IServiceLoader<IService<TEntity, TInterface, TId>> serviceLoader)
        {
            _ServiceLoader = serviceLoader;
        }

        internal IService<TEntity, TInterface, TId> Service => _Service ?? (_Service = _ServiceLoader.Load());
        private IService<TEntity, TInterface, TId> _Service;

        public TInterface Get(TId id)
        {
            return Service.Get(id);
        }
    }
}
