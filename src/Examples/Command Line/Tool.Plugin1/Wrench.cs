using Tool.Tools;

namespace Tool.Plugin1
{
    public class Wrench : ITool
    {
        private int _BoltsTurned;

        public string Name
        {
            get { return "Wrench"; }
        }

        public string DoWork()
        {
            return "Turn. Turn. Turn. " + ++_BoltsTurned + " bolts turned.";
        }
    }
}
