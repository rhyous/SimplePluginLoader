using Ninject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.SimplePluginLoader.DependencyInjection;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection.Tests
{
    [TestClass]
    public class SimplePluginLoaderModuleTests
    {
        IKernel _RootScope;

        [TestInitialize]
        public void TestInitialize()
        {
            _RootScope = new StandardKernel();
            _RootScope.Load<SimplePluginLoaderModule>();
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoaderLogger_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginLoaderLogger>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AppDomain_Registered_Test()
        {
            var actual = _RootScope.Get<AppDomain>();
            Assert.AreEqual(AppDomain.CurrentDomain, actual);
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AppDomainWrapper_Registered_Test()
        {
            var actual = _RootScope.Get<IAppDomain>();
            Assert.IsNotNull(actual);
            Assert.AreEqual(typeof(AppDomainWrapper), actual.GetType());

        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoaderSettings_Registered_Test()
        {
            var actual = _RootScope.Get<IPluginLoaderSettings>();
            Assert.AreEqual(PluginLoaderSettings.Default, actual);
        }

        [TestMethod]
        public void SimplePluginLoaderModule_TypeLoader_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<ITypeLoader<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginPaths_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginPaths>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AppSettings_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IAppSettings>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AssemblyCache_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IAssemblyCache>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AssemblyNameReader_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IAssemblyNameReader>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_AssemblyLoader_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IAssemblyLoader>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_CacheFactory_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<ICacheFactory<string, IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyResolverObjectCreator_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginDependencyResolverObjectCreator>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyResolverCacheFactory_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginDependencyResolverCacheFactory>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginCacheFactory_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginCacheFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyResolver_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginDependencyResolver>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_NinjectObjectCreatorFactory_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IObjectCreatorFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_NinjectPluginObjectCreator_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginObjectCreator<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoaderFactory_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginLoaderFactory<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoader_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginLoader<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginFinder_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginFinder<IOrganization>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_RuntimePluginLoaderFactory_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IRuntimePluginLoaderFactory>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginDependencyRegistrar_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginDependencyRegistrar>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoader_IDependencyRegistrar_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginLoader<IDependencyRegistrar<IKernel>>>());
        }

        [TestMethod]
        public void SimplePluginLoaderModule_PluginLoader_IPluginObjectCreator_IDependencyRegistrar_ContainerBuilder_ContainerBuilder_Registered_Test()
        {
            Assert.IsNotNull(_RootScope.Get<IPluginObjectCreator<IDependencyRegistrar<IKernel>>>());
        }
    }
}
