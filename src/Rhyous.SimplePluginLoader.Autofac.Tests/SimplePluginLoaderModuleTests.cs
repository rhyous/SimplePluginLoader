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
        public void SimplePlugin_PluginLoaderLogger_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoaderLogger>());
        }

        [TestMethod]
        public void SimplePlugin_AppDomain_Registered_Test()
        {
            var actual = Container.Resolve<AppDomain>();
            Assert.AreEqual(AppDomain.CurrentDomain, actual);
        }

        [TestMethod]
        public void SimplePlugin_AppDomainWrapper_Registered_Test()
        {
            var actual = Container.Resolve<IAppDomain>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(typeof(AppDomainWrapper), actual.GetType());

        }

        [TestMethod]
        public void SimplePlugin_PluginLoaderSettings_Registered_Test()
        {
            var actual = Container.Resolve<IPluginLoaderSettings>();
            Assert.AreEqual(PluginLoaderSettings.Default, actual);
        }

        [TestMethod]
        public void SimplePlugin_TypeLoader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<ITypeLoader<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginPaths_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginPaths>());
        }

        [TestMethod]
        public void SimplePlugin_AppSettings_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAppSettings>());
        }

        [TestMethod]
        public void SimplePlugin_AssemblyCache_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAssemblyCache>());
        }

        [TestMethod]
        public void SimplePlugin_AssemblyNameReader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAssemblyNameReader>());
        }

        [TestMethod]
        public void SimplePlugin_AssemblyLoader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IAssemblyLoader>());
        }

        [TestMethod]
        public void SimplePlugin_CacheFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<ICacheFactory<string,IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginDependencyResolverObjectCreator_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyResolverObjectCreator>());
        }

        [TestMethod]
        public void SimplePlugin_PluginDependencyResolverCacheFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyResolverCacheFactory>());
        }

        [TestMethod]
        public void SimplePlugin_PluginCacheFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginCacheFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginDependencyResolver_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyResolver>());
        }

        [TestMethod]
        public void SimplePlugin_AutofacObjectCreatorFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IObjectCreatorFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_AutofacPluginObjectCreatorFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginObjectCreatorFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_AutofacPluginObjectCreator_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginObjectCreator<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginLoaderFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoaderFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginLoader_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoader<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginFinder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginFinder<IOrganization>>());
        }

        [TestMethod]
        public void SimplePlugin_RuntimePluginLoaderFactory_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IRuntimePluginLoaderFactory>());
        }

        [TestMethod]
        public void SimplePlugin_PluginDependencyRegistrar_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginDependencyRegistrar>());
        }

        [TestMethod]
        public void SimplePlugin_PluginLoader_IDependencyRegistrar_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginLoader<IDependencyRegistrar<ContainerBuilder>>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginLoader_IPluginObjectCreator_IDependencyRegistrar_ContainerBuilder_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginObjectCreator<IDependencyRegistrar<ContainerBuilder>>>());
        }

        [TestMethod]
        public void SimplePlugin_PluginLoader_IPluginObjectCreatorFactory_IDependencyRegistrar_ContainerBuilder_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(Container.Resolve<IPluginObjectCreatorFactory<IDependencyRegistrar<ContainerBuilder>>>());
        }
    }
}
