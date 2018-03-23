using Tool.Tools;

namespace Tool.Plugin1
{
    public class Screwdriver : ITool
    {
        private int _ScrewsTurned;

        public string Name
        {
            get { return "Screwdriver"; }
        }

        public string DoWork()
        {
            return "Twist. Twist. Twist. " + ++_ScrewsTurned + " screws turned.";
        }
    }
}
