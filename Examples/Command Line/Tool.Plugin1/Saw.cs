using Interfaces.Localization;

namespace Tool.Plugin1
{
    public class Saw : ITool
    {
        private readonly ILocalizer _Localizer;
        private int _LogsCut;

        public Saw(ILocalizer localizer)
        {
            _Localizer = localizer;
        }

        public string Name
        {
            get { return _Localizer.Localize("Saw"); }
        }

        public string DoWork()
        {
            return "Cut. Cut. Cut. " + ++_LogsCut + " bolts turned.";
        }
    }
}