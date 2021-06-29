using Autofac;
using System.Linq;

namespace Rhyous.SimplePluginLoader.Autofac.Tests.Models
{
    public class ServiceLoader<T> : IServiceLoader<T>
    {
        private readonly ILifetimeScope _Container;

        public ServiceLoader(ILifetimeScope container)
        {
            _Container = container;
        }

        public T Load()
        {
            using (var scope = _Container.BeginLifetimeScope(builder =>
            {
                var types = new[] { typeof(UserService), typeof(Service<Organization, IOrganization, int>) };
                foreach (var type in types)
                {

                    if (!type.IsGenericTypeDefinition)
                        builder.RegisterType(type).As(type.GetInterfaces().FirstOrDefault());
                    else
                        builder.RegisterGeneric(type.GetGenericTypeDefinition()).As(type.GetInterfaces().FirstOrDefault());

                }
            }))
            {
                if (typeof(T).IsGenericTypeDefinition)
                    return default(T);
                return scope.Resolve<T>(); ;
            }
        }
    }
}
