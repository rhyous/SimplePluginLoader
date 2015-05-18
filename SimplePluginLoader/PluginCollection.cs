using System.Collections.Generic;

namespace SimplePluginLoader
{
    public class PluginCollection<T> : List<Plugin<T>> where T : class
    {
        public PluginCollection()
        {
        }

        public PluginCollection(int capacity)
            : base(capacity)
        {
        }

        public PluginCollection(IEnumerable<Plugin<T>> list)
            : base(list)
        {
        }

        public List<T> AllObjects
        {
            get
            {
                var list = new List<T>();
                foreach (var plugin in this)
                {
                    list.AddRange(plugin.PluginObjects);
                }
                return list;
            }
        }
    }
}
