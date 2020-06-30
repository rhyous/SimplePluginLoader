namespace Rhyous.SimplePluginLoader.Tests
{

    public class Org : IOrg
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Org2 : Org
    {
        public Org2() { }
    }

}