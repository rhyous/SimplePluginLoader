using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class SimplePluginLoaderModuleTests
    {
        IContainer Container;

        [TestInitialize]
        public void TestInitialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<SimplePluginLoaderModule>();
            Container = builder.Build();
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoaderLogger_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoaderLogger>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AppDomain_Registered_Test()
        {
            var actual = Container.Resolve<AppDomain>();
            Assert.AreEqual(AppDomain.CurrentDomain, actual);
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AppDomainWrapper_Registered_Test()
        {
            var actual = Container.Resolve<IAppDomain>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(typeof(AppDomainWrapper), actual.GetType());

        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoaderSettings_Registered_Test()
        {
            var actual = Container.Resolve<IPluginLoaderSettings>();
            Assert.AreEqual(PluginLoaderSettings.Default, actual);
        }

        [TestMethod]
        public void SimplePluginLoaderModule_TypeLoader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<ITypeLoader<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginPaths_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginPaths>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AppSettings_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAppSettings>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AssemblyCache_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAssemblyCache>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AssemblyNameReader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAssemblyNameReader>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AssemblyLoader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAssemblyLoader>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_CacheFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<ICacheFactory<string,IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyResolverObjectCreator_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyResolverObjectCreator>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyResolverCacheFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyResolverCacheFactory>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginCacheFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginCacheFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyResolver_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyResolver>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AutofacObjectCreatorFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IObjectCreatorFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AutofacPluginObjectCreator_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginObjectCreator<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoaderFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoaderFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoader<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginFinder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginFinder<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_RuntimePluginLoaderFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IRuntimePluginLoaderFactory>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyRegistrar_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyRegistrar>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoader_IDependencyRegistrar_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoader_IPluginObjectCreator_IDependencyRegistrar_ContainerBuilder_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>());
        }
    }
}
