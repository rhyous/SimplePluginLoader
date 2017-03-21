using Example.DepencyOfDependency;

namespace Example.Dependency
{
    public class Localizer
    {
        public string Rock = "Rock";
        public string RockAction = "Whack. Whack. Whack. {0} thing(s) smashed.";
        public string RockBroke = "Your rock broke.";
        public string GetRockUnusableSentence() { return new Sentence().RockUnusable;  }
    }
}
