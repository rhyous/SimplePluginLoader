using Example.Dependency;
using Tool;

public class Rock : ITool
{
    private Localizer _localizer = new Localizer();
    public string Name => _localizer.Rock;

    public uint _ThingsHammered;

    public string DoWork()
    {
        if (_ThingsHammered < 3)
            return string.Format(_localizer.RockAction, ++_ThingsHammered);
        if (_ThingsHammered == 3)
            return string.Format(_localizer.RockAction, ++_ThingsHammered) + " " + _localizer.RockBroke;
        return _localizer.RockUnusable;
    }
}