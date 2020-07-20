using Rhyous.SimplePluginLoader;

namespace Tool
{
    public class MyPluginLoaderSettings : PluginLoaderSettings
    {
        public MyPluginLoaderSettings(IAppSettings appSettings) : base(appSettings) { }

        public override string Company => "ToolFactory";
        public override string AppName => "Tool.CommandLine";
        public override string PluginFolder => null;
    }
}
