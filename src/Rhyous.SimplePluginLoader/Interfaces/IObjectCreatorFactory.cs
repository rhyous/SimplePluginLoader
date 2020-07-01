namespace Rhyous.SimplePluginLoader
{
    public interface IObjectCreatorFactory<T>
    {
        IObjectCreator<T> Create();
    }
}