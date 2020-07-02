using Autofac;
using System;

namespace Rhyous.SimplePluginLoader.DependencyInjection
{
    public interface IPluginDependencyRegistrar
    {
        void RegisterPluginDependencies(ContainerBuilder builder, IPlugin plugin, Type type);
    }
}