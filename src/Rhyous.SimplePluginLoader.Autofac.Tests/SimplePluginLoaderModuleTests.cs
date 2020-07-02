using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhyous.SimplePluginLoader.DependencyInjection;

namespace Rhyous.SimplePluginLoader.Autofac.Tests
{
    [TestClass]
    public class SimplePluginLoaderModuleTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<SimplePluginLoaderModule>();
            builder.Build(); // As long as we don't crash, this test works.
        }
    }
}
