namespace Tool.Plugin1
{
    using Tools;

    public class CaveManTool<T> : ICaveManTool<T>
        where T: class, ITool, new()
    {
        public T Tool
        {
            get { return _Tool ?? (_Tool = new T()); }
            set { _Tool = value; }
        } private T _Tool;
    }
}
