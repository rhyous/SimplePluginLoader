namespace Rhyous.SimplePluginLoader
{
    public interface IPluginObjectCreatorFactory<T>
    {
        IPluginObjectCreator<T> Create();
    }
}