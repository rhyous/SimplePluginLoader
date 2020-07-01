namespace Rhyous.SimplePluginLoader
{
    public interface IPluginObjectCreator<T> : IObjectCreator<T>, IPluginContainer
    {
    }
}