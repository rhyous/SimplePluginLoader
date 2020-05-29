namespace Rhyous.SimplePluginLoader
{
    public interface IObjectCreatorFactory<T> where T : class
    {
        IObjectCreator<T> Create();
    }
}