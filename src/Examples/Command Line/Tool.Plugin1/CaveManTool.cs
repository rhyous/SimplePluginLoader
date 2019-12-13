namespace Tool.Plugin1
{
    using Tools;

    public class CaveManTool<T> : ICaveManTool<T>
        where T : class, ITool
    {
        public CaveManTool(T tool) { Tool = tool; }

        public T Tool { get; set; }

        public string Name => $"CaveMan {Tool.Name}";


        public string DoWork()
        {
            return Tool.DoWork();
        }
    }
}
