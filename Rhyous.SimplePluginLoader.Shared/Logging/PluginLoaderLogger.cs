using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.Configuration;
using System.IO;

namespace Rhyous.SimplePluginLoader
{
    public class PluginLoaderLogger : IPluginLoaderLogger
    {
        internal static int InstanceCount = 0;
        internal int Instance;
        public PluginLoaderLogger() { Instance = ++InstanceCount; }

        public StreamWriter Writer
        {
            get {
                if (_Writer == null)
                    SafeSetWriter();
                return _Writer;
            }             
            set { _Writer = value; }
        } private StreamWriter _Writer;

        public void SafeSetWriter(StreamWriter writer = null)
        {
            if (_Writer == null)
            {
                lock (_Locker)
                {
                    if (_Writer == null)
                    {
                        _Writer = writer ?? new StreamWriter(LogFullPath, true);
                    }
                }
            }
        } private object _Locker = new object();

        public static string LogPathConfiguration { get { return ConfigurationManager.AppSettings["PluginLoaderLogPath"]; } }
        public static string LogLevelConfiguration { get { return ConfigurationManager.AppSettings["PluginLoaderLogLevel"]; } }
        public string LogPath
        {
            get
            {
                return _LogPath ?? (_LogPath = string.IsNullOrWhiteSpace(LogPathConfiguration)
                    ? AppDomain.CurrentDomain.BaseDirectory
                    : LogPathConfiguration);
            }
            set { _LogPath = value; }
        } private string _LogPath;

        public PluginLoaderLogLevel LogLevel
        {
            get { return _LogLevel ?? LogLevelConfiguration.ToEnum(PluginLoaderLogLevel.Debug); }
            internal set { _LogLevel = value; }
        } private PluginLoaderLogLevel? _LogLevel; // Alwasy get from config unless overridden

        public string LogFileName { get; set; } = $"PluginLoader.{DateTime.Now.ToFileTime()}.log";

        public string LogFileExtension { get; set; }

        public string LogFile { get { return LogFileName + LogFileExtension; } }

        public string LogFullPath { get { return Path.Combine(LogPath, LogFile); } }

        public bool LogExists { get { return File.Exists(LogFullPath); } }
               

        public void WriteLine(PluginLoaderLogLevel level, string inLogMessage)
        {
            Write(level, inLogMessage + Environment.NewLine);
        }
        
        public void Write(PluginLoaderLogLevel level, string msg)
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            Writer.Write(msg);
            Writer.Flush();
        }
    }
}
