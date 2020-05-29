using System;
using System.Collections.Generic;

namespace Rhyous.SimplePluginLoader.Tests
{
    public class OrgRepository : IRepository<Org, IOrg, int>
    {
        public List<IOrg> Get()
        {
            throw new NotImplementedException();
        }
    }
}
