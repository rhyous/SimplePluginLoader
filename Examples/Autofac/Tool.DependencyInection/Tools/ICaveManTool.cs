namespace Tool.Tools
{
    public interface ICaveManTool<T> : ITool
        where T : ITool
    {
        T Tool { get; set; }
    }
}