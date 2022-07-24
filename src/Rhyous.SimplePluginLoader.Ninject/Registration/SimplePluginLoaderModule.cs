using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Modules;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    /// <summary>
    /// An Ninject module for registering all SimplePluginLoader tools.
    /// </summary>
    public class SimplePluginLoaderModule : NinjectModule
    {
        /// <summary>
        /// This is used by Ninject when loading a module to load all SimplePluginLoader tools.
        /// </summary>
        public override void Load()
        {
            Kernel.Bind<IPluginLoaderLogger>()
                  .ToMethod((ctx) => PluginLoaderLogger.Instance = new PluginLoaderLogger(ctx.Kernel.Get<IAppSettings>()))
                  .InSingletonScope();
            Kernel.Bind<AppDomain>()
                  .ToConstant(AppDomain.CurrentDomain);
            Kernel.Bind<IAppDomain>()
                  .To<AppDomainWrapper>()
                  .InSingletonScope();
            Kernel.Bind<IPluginLoaderSettings>()
                  .ToConstant(PluginLoaderSettings.Default)
                  .InSingletonScope();
            Kernel.Bind(typeof(ITypeLoader<>))
                  .To(typeof(TypeLoader<>));
            Kernel.Bind<IPluginPaths>()
                  .ToMethod((ctx) =>
                   {
                       var settings = ctx.Kernel.Get<IPluginLoaderSettings>();
                       return new AppPluginPaths(settings.AppName,
                                                 settings.PluginFolder,
                                                 ctx.Kernel.Get<IAppDomain>(),
                                                 ctx.Kernel.Get<IPluginLoaderLogger>());
                   });
            Kernel.Bind<IAppSettings>()
                  .To<AppSettings>()
                  .InSingletonScope();
            Kernel.Bind<IAssemblyCache>()
                  .To<AssemblyCache>()
                  .InSingletonScope();
            Kernel.Bind<IWaiter>()
                  .To<Waiter>();
            Kernel.Bind<IAssemblyResolveCache>()
                  .To<AssemblyResolveCache>();
            Kernel.Bind<IAssemblyNameReader>()
                  .To<AssemblyNameReader>()
                  .InSingletonScope();
            Kernel.Bind<IAssemblyLoader>()
                  .To<AssemblyLoader>()
                  .InSingletonScope();
            Kernel.Bind(typeof(ICacheFactory<,>))
                  .To(typeof(CacheFactory<,>))
                  .InSingletonScope();
            Kernel.Bind<IPluginDependencyResolverObjectCreator>()
                  .To<PluginDependencyResolverObjectCreator>()
                  .InSingletonScope();
            Kernel.Bind<IPluginDependencyResolverCacheFactory>()
                .To<PluginDependencyResolverCacheFactory>()
                  .InSingletonScope();
            Kernel.Bind(typeof(IPluginCacheFactory<>))
                  .To(typeof(PluginCacheFactory<>));
            Kernel.Bind<IPluginDependencyResolver>()
                  .To<PluginDependencyResolver>();
            Kernel.Bind(typeof(IObjectCreatorFactory<>))
                  .To(typeof(NinjectObjectCreatorFactory<>));
            Kernel.Bind(typeof(IObjectCreator<>))
                  .To(typeof(NinjectObjectCreator<>));
            Kernel.Bind(typeof(IPluginObjectCreator<>))
                  .To(typeof(NinjectPluginObjectCreator<>));
            Kernel.Bind(typeof(IPluginLoaderFactory<>))
                  .To(typeof(PluginLoaderFactory<>))
                  .InSingletonScope();
            Kernel.Bind(typeof(IPluginLoader<>))
                  .To(typeof(PluginLoader<>));
            Kernel.Bind(typeof(IPluginFinder<>))
                  .To(typeof(PluginFinder<>));

            // Both set the default factory and return it.
            // To override this, simply register it again after registering this module.
            Kernel.Bind<IRuntimePluginLoaderFactory>()
                  .ToMethod((c) => { return new NinjectRuntimePluginLoaderFactory(new ChildKernel(c.Kernel)); });

            // Plugin Loader Registration for a plugin's own registration module
            // This allows for loading dependencies from a Plugin, registering them,
            // then using those dependencies to Resolve an object with Ninject
            Kernel.Bind<IPluginDependencyRegistrar>()
                  .To<PluginDependencyRegistrar>()
                  .InSingletonScope();
            Kernel.Bind<IPluginLoader<IDependencyRegistrar<IKernel>>>()
                  .To<PluginLoader<IDependencyRegistrar<IKernel>>>();
            Kernel.Bind<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>()
                  .To<PluginObjectCreator<IDependencyRegistrar<IKernel>>>(); //Instance per dependency
        }
    }
}