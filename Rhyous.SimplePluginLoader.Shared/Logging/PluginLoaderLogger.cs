using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Rhyous.SimplePluginLoader
{
    /// <summary>
    /// A default logger.
    /// </summary>
    /// <remarks>It is expected that most consumers will implement their own loggers. This logger is quite simple.</remarks>
    public class PluginLoaderLogger : IPluginLoaderLogger
    {
        public static string LogPathConfiguration { get { return ConfigurationManager.AppSettings["PluginLoaderLogPath"]; } }
        public static string LogLevelConfiguration { get { return ConfigurationManager.AppSettings["PluginLoaderLogLevel"]; } }
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
                        CreateDirectory();
                        _Writer = writer ?? new StreamWriter(LogFullPath, true);

                    }
                }
            }
        } private object _Locker = new object();  

        public string LogPath
        {
            get
            {
                return _LogPath ?? (_LogPath = string.IsNullOrWhiteSpace(LogPathConfiguration)
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log")
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
                
        public void Write(PluginLoaderLogLevel level, string msg)
        {
            Writer.Write(msg);
            Writer.Flush();
        }

        public void WriteLine(PluginLoaderLogLevel level, string msg)
        {
            Write(level, msg + Environment.NewLine);
        }

        public void WriteLines(PluginLoaderLogLevel level, params string[] messages)
        {
            if (messages == null || !messages.Any())
                return;
            foreach (var msg in messages)
                WriteLine(level, msg);
        }

        private void CreateDirectory()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }
    }
}