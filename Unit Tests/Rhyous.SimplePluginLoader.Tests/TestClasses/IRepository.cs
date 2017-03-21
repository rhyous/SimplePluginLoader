using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests.TestClasses
{
    public interface IRepository<T, Tinterface, Tid>
        where T : Tinterface
    {
        List<Tinterface> Get();        
    }

}
