namespace Rhyous.SimplePluginLoader
{
    public class ObjectCreatorFactory<T> : IObjectCreatorFactory<T>
        where T : class
    {
        public IObjectCreator<T> Create()
        {
            return new ObjectCreator<T>();
        }
    }
}