using Interfaces.Localization;
using Tool;

public class Rock : ITool
{
    private readonly ILocalizer _Localizer;

    public Rock(ILocalizer localizer)
    {
        _Localizer = localizer;
    }

    public string Name => _Localizer.Localize("Rock");

    public uint _ThingsHammered;

    public string DoWork()
    {
        if (_ThingsHammered < 3)
            return string.Format(_Localizer.Localize("RockAction"), ++_ThingsHammered);
        if (_ThingsHammered == 3)
            return string.Format(_Localizer.Localize("RockAction"), ++_ThingsHammered) + " " + _Localizer.Localize("RockBroke");
        return _Localizer.Localize("RockUnusable");
    }
}