using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
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

        #region Singleton

        private static readonly Lazy<PluginLoaderLogger> Lazy = new Lazy<PluginLoaderLogger>(() => new PluginLoaderLogger());

        internal static Locked<int> InstanceCount => new Locked<int>();
        internal int InstanceId;

        public static PluginLoaderLogger Instance { get { return Lazy.Value; } }

        internal PluginLoaderLogger() { InstanceId = ++InstanceCount.Value; }

        #endregion



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
        } private PluginLoaderLogLevel? _LogLevel; // Always get from config unless overridden

        public string LogFileName { get; set; } = $"PluginLoader.{DateTime.Now.ToFileTime()}.log";

        public string LogFileExtension { get; set; }

        public string LogFile { get { return LogFileName + LogFileExtension; } }

        public string LogFullPath { get { return Path.Combine(LogPath, LogFile); } }

        public bool LogExists { get { return File.Exists(LogFullPath); } }
                
        public void Write(PluginLoaderLogLevel level, string msg)
        {
            if (level >= LogLevel)
            {
                Writer.Write(msg);
                Writer.Flush();
            }
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

        public void Log(Exception e)
        {
            if (LogLevel == PluginLoaderLogLevel.Debug)
                WriteLines(PluginLoaderLogLevel.Debug, e.ToString());
            if (LogLevel == PluginLoaderLogLevel.Info)
                WriteLines(PluginLoaderLogLevel.Info, e.Message);
        }

        private void CreateDirectory()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
        }

        #region Wrappers -  While this could come from the constructor, these are only used in Unit Tests, so we hide them internally
        [ExcludeFromCodeCoverage]
        internal IDirectory Directory
        {
            get { return _Directory ?? (_Directory = DirectoryWrapper.Instance); }
            set { _Directory = value; }
        } private IDirectory _Directory;

        [ExcludeFromCodeCoverage]
        internal IFile File
        {
            get { return _File ?? (_File = FileWrapper.Instance); }
            set { _File = value; }
        } private IFile _File;
        #endregion
    }
}