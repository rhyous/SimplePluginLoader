using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SimplePluginLoader
{
    public class Plugin<T> where T : class
    {
        public string Directory { get; set; }

        public string File { get; set; }

        public string FullPath
        {
            get { return Path.Combine(Directory, File); }
        }

        public Assembly Assembly { get; set; }

        public List<T> PluginObjects
        {
            get { return _PluginObjects ?? (_PluginObjects = Loader.LoadInstances(Assembly)); }
            set { _PluginObjects = value; }
        } private List<T> _PluginObjects;

        public ILoadInstancesOfType<T> Loader
        {
            get { return _Loader ??(_Loader = new InstancesLoader<T>()); }
            set { _Loader = value; }
        } private ILoadInstancesOfType<T> _Loader;
    }
}
