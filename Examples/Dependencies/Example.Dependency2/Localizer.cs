using Example.DepencyOfDependency;

namespace Example.Dependency
{
    public class Localizer
    {
        public string Rock = "Granite Rock";
        public string RockAction = "Kabang. Kabang. Kabang. {0} thing(s) smashed.";
        public string RockBroke = "Your rock broke.";
        public string GetRockUnusableSentence() { return new Sentence().RockUnusable; }
    }
}
