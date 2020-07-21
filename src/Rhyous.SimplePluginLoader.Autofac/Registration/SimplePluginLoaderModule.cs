using Autofac;
using Autofac.Core;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An Autofac module for registering all SimplePluginLoader tools.
    /// </summary>
    public class SimplePluginLoaderModule : Module
    {
        /// <summary>
        /// This is used by Autofac when loading a module to load all SimplePluginLoader tools.
        /// </summary>
        /// <param name="builder">The Autofac ContainerBuilder.</param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(PluginLoaderLogger.Instance)
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
            builder.RegisterType<AppPluginPaths>()
                   .WithParameter(new ResolvedParameter((pi, ctx) => pi.Name == "appName",
                                                        (pi, ctx) => ctx.Resolve<IPluginLoaderSettings>().AppName))
                   .WithParameter(new ResolvedParameter((pi, ctx) => pi.Name == "pluginSubFolder",
                                                        (pi, ctx) => ctx.Resolve<IPluginLoaderSettings>().PluginFolder))
                   .As<IPluginPaths>();
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
            builder.RegisterGeneric(typeof(AutofacObjectCreator<>))
                   .As(typeof(IObjectCreator<>));
            builder.RegisterGeneric(typeof(AutofacPluginObjectCreator<>))
                   .As(typeof(IPluginObjectCreator<>));
            builder.RegisterGeneric(typeof(PluginLoaderFactory<>))
                   .As(typeof(IPluginLoaderFactory<>))
                   .SingleInstance();
            builder.RegisterGeneric(typeof(PluginLoader<>))
                   .As(typeof(IPluginLoader<>));
            builder.RegisterGeneric(typeof(PluginFinder<>))
                   .As(typeof(IPluginFinder<>));

            // Both set the default factory and return it.
            // To override this, simply register it again after registering this module.
            builder.Register((c) => { return new AutofacRuntimePluginLoaderFactory(c.Resolve<ILifetimeScope>()); })
                   .As<IRuntimePluginLoaderFactory>();

            // Plugin Loader Registration for a plugin's own registration module
            // This allows for loading dependencies from a Plugin, registering them,
            // then using those dependencies to Resolve an object with Autofac
            builder.RegisterType<PluginDependencyRegistrar>()
                   .As<IPluginDependencyRegistrar>()
                   .SingleInstance();
            builder.RegisterType<PluginLoader<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>();
            builder.RegisterType<PluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>()
                   .As<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>(); //Instance per dependency
        }
    }
}