namespace Tool
{
    public interface ITool
    {
        string Name { get; }
        string DoWork();
    }
}
