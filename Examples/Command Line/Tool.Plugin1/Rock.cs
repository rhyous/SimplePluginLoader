using Tool;

public class Rock : ITool
{
    public string Name => "Rock";

    public uint _ThingsHammered;

    public string DoWork()
    {
        if (_ThingsHammered < 3)
            return "Whack. Whack. Whack. " + ++_ThingsHammered + " thing(s) smashed.";
        if (_ThingsHammered == 3)
            return "Whack. Whack. Whack. " + ++_ThingsHammered + " thing(s) smashed. Your rock broke.";
        return "You can't use this rock. It is broken.";
    }
}