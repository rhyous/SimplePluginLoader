using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public class ServiceExtended<TEntity, TInterface, TId> : Service<TEntity, TInterface, TId>
    where TEntity : class, TInterface, new()
    where TInterface : IBaseEntity<TId>
    where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        public virtual TInterface Set(TEntity entity)
        {
            return entity;
        }
    }
}
