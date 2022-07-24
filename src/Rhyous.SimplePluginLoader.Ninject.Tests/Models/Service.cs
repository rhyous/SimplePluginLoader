using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public class Service<TEntity, TInterface, TId> : IService<TEntity, TInterface, TId>
        where TEntity : class, TInterface, new()
        where TInterface : IBaseEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        public virtual TInterface Get(TId id)
        {
            return new TEntity { Id = id };
        }
    }
}
