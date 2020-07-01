namespace Rhyous.SimplePluginLoader
{
    public class Locked<T>
    {
        public T Value
        {
            get { lock (Locker) return _Value; }
            set { lock (Locker) _Value = value; }
        }
        private T _Value;
        private object Locker = new object();
    }
}
