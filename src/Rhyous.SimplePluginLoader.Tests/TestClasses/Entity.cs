using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhyous.SimplePluginLoader.Tests.TestClasses
{
    public interface IEntity<T>
    {
        int Id { get; set; }
        string Name { get; set; }
    }

    public class Entity<T> : IEntity<T>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
