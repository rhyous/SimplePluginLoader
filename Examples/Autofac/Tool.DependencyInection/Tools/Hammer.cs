namespace Tool.Tools
{
    public class Hammer : ITool
    {
        private int _NailsHammered;

        public virtual string Name
        {
            get { return "Hammer"; }
        }

        public virtual string DoWork()
        {
            return "Whack. Whack. Whack. " + ++_NailsHammered + " nails hammered.";
        }
    }
}
