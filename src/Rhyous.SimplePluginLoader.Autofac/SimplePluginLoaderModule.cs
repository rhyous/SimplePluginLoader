using Autofac;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public class SimplePluginLoaderModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PluginLoaderLogger>()
                   .As<IPluginLoaderLogger>()
                   .SingleInstance();
            builder.RegisterInstance(AppDomain.CurrentDomain)
                   .SingleInstance();
            builder.RegisterType<AppDomainWrapper>()
                   .As<IAppDomain>().SingleInstance();
            builder.RegisterInstance(PluginLoaderSettings.Default)
                   .As<IPluginLoaderSettings>()
                   .SingleInstance();
            builder.RegisterGeneric(typeof(TypeLoader<>))
                   .As(typeof(ITypeLoader<>));
            builder.RegisterType<PluginPaths>()
                   .WithParameter("appName", "sectionName")
                   .WithParameter("PluginSubFolder", null);
            builder.RegisterType<AppSettings>()
                   .As<IAppSettings>()
                   .SingleInstance();
            builder.RegisterType<AssemblyLoader>()
                   .As<IAssemblyLoader>()
                   .SingleInstance();
            builder.RegisterGeneric(typeof(PluginDependencyResolver<>))
                   .As(typeof(IPluginDependencyResolver<>));
            builder.RegisterGeneric(typeof(AutofacObjectCreatorFactory<>))
                   .As(typeof(IObjectCreatorFactory<>));
            builder.RegisterGeneric(typeof(AutofacObjectCreator<>))
                   .As(typeof(IObjectCreator<>));
            builder.RegisterGeneric(typeof(InstanceLoaderFactory<>))
                   .As(typeof(IInstanceLoaderFactory<>));

            // Plugin Loader Registration for a plugin's own registration module
            builder.RegisterType<PluginLoader<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<ObjectCreator<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IObjectCreator<IDependencyRegistrar<ContainerBuilder>>>(); //Instance per dependency
            builder.RegisterType<ObjectCreatorFactory<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IObjectCreatorFactory<IDependencyRegistrar<ContainerBuilder>>>(); //Instance per dependency
        }
    }
}
