using Example.DepencyOfDependency;
using Interfaces.Localization;
using System.Collections.Generic;

namespace Example.Dependency
{
    public class Localizer : ILocalizer
    {
        Dictionary<string, string> _Dictionary = new Dictionary<string, string>();

        public Localizer()
        {
            _Dictionary.Add("Rock", "Granite Rock");
            _Dictionary.Add("RockAction", "Kabang. Kabang. Kabang. {0} thing(s) smashed.");
            _Dictionary.Add("RockBroke", "Your rock broke.");
            _Dictionary.Add("RockUnusable", GetRockUnusableSentence());
            _Dictionary.Add("Wrench", "Wrench");
            _Dictionary.Add("Saw", "Saw");
        }


        public string Localize(string key)
        {
            if (_Dictionary.TryGetValue(key, out string localized))
                return localized;
            return "No localized string found";
        }

        internal string GetRockUnusableSentence() { return new Sentence().RockUnusable; }
    }
}
