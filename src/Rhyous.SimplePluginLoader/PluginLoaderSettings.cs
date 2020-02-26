namespace Rhyous.SimplePluginLoader
{
    public class PluginLoaderSettings : IPluginLoaderSettings
    {
        public bool ThrowExceptionsOnLoad { get; set; }

        public static PluginLoaderSettings Default = _Default ?? (_Default = new PluginLoaderSettings { ThrowExceptionsOnLoad = false });
        private static PluginLoaderSettings _Default;
    }
}
