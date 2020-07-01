using Example.Dependency;

namespace Tool.Plugin2
{
    public class GraniteRock : ITool
    {
        private Localizer _localizer = new Localizer();
        public string Name => _localizer.Localize("Granite Rock");

        public uint _ThingsHammered;

        public string DoWork()
        {
            if (_ThingsHammered < 5)
                return string.Format(_localizer.Localize("RockAction"), ++_ThingsHammered);
            if (_ThingsHammered == 5)
                return string.Format(_localizer.Localize("RockAction"), ++_ThingsHammered) + " " + _localizer.Localize("RockBroke");
            return _localizer.Localize("RockUnusable");
        }
    }
}