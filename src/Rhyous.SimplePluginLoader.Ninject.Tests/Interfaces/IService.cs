using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    public interface IService<TEntity, TInterface, TId>
        where TEntity : class, TInterface, new()
        where TInterface : IBaseEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        TInterface Get(TId id);
    }
}
