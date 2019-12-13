using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests
{
    public interface IRepository<T, Tinterface, Tid>
        where T : Tinterface
    {
        List<Tinterface> Get();        
    }

}
