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
            builder.RegisterType<AssemblyCache>()
                   .As<IAssemblyCache>()
                   .SingleInstance();
            builder.RegisterType<AssemblyNameReader>()
                   .As<IAssemblyNameReader>()
                   .SingleInstance();
            builder.RegisterType<AssemblyLoader>()
                   .As<IAssemblyLoader>()
                   .SingleInstance();
            builder.RegisterGeneric(typeof(CacheFactory<,>))
                   .As(typeof(ICacheFactory<,>))
                   .SingleInstance();
            builder.RegisterType<PluginDependencyResolverObjectCreator>()
                   .As<IPluginDependencyResolverObjectCreator>()
                   .SingleInstance();
            builder.RegisterType<PluginDependencyResolverCacheFactory>()
                   .As<IPluginDependencyResolverCacheFactory>()
                   .SingleInstance();
            builder.RegisterGeneric(typeof(PluginCacheFactory<>))
                   .As(typeof(IPluginCacheFactory<>));
            builder.RegisterType<PluginDependencyResolver>()
                   .As<IPluginDependencyResolver>();
            builder.RegisterGeneric(typeof(AutofacObjectCreatorFactory<>))
                   .As(typeof(IObjectCreatorFactory<>));
            builder.RegisterGeneric(typeof(AutofacPluginObjectCreatorFactory<>))
                   .As(typeof(IPluginObjectCreatorFactory<>));
            builder.RegisterGeneric(typeof(AutofacPluginObjectCreator<>))
                   .As(typeof(IPluginObjectCreator<>));

            // Plugin Loader Registration for a plugin's own registration module
            builder.RegisterType<PluginDependencyRegistrar>()
                   .As<IPluginDependencyRegistrar>()
                   .SingleInstance();
            builder.RegisterType<PluginLoader<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<PluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>(); //Instance per dependency
            builder.RegisterType<PluginObjectCreatorFactory<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IPluginObjectCreatorFactory<IDependencyRegistrar<ContainerBuilder>>>(); //Instance per dependency
        }
    }
}
