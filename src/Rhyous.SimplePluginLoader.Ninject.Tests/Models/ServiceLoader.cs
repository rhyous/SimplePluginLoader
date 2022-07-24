using Ninject;
using Ninject.Extensions.ChildKernel;
using System.Linq;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests.Models
{
    public class ServiceLoader<T> : IServiceLoader<T>
    {
        private readonly IKernel _RootScope;

        public ServiceLoader(IKernel container)
        {
            _RootScope = container;
        }

        public T Load()
        {
            var childScope = new ChildKernel(_RootScope);
            var types = new[] { typeof(UserService), typeof(Service<Organization, IOrganization, int>) };
            foreach (var type in types)
            {
                if (!type.IsGenericTypeDefinition)
                    childScope.Bind(type.GetInterfaces().FirstOrDefault())
                              .To(type);
                else
                    childScope.Bind(type.GetInterfaces().FirstOrDefault())
                              .To(type.GetGenericTypeDefinition());
            }
            if (typeof(T).IsGenericTypeDefinition)
                return default(T);
            return childScope.Get<T>();
        }
    }
}