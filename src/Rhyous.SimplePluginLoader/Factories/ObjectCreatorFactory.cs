namespace Rhyous.SimplePluginLoader
{
    public class ObjectCreatorFactory<T> : IObjectCreatorFactory<T>
    {
        public IObjectCreator<T> Create()
        {
            return new ObjectCreator<T>();
        }
    }
}