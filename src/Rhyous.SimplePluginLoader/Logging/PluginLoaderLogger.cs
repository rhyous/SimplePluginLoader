using Rhyous.SimplePluginLoader.Extensions;
using System;
using System.Collections.Concurrent;
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
        public static IPluginLoaderLogger Factory(IAppSettings appSettings)
        {
            return Instance ?? (Instance = new PluginLoaderLogger(appSettings));
        }

        public string LogPathConfiguration { get { return _AppSettings.Settings["PluginLoaderLogPath"]; } }
        public string LogLevelConfiguration { get { return _AppSettings.Settings["PluginLoaderLogLevel"]; } }

        #region Singleton
        
        internal ConcurrentQueue<LogMessage> _LogMessages = new ConcurrentQueue<LogMessage>();

        internal static Locked<int> InstanceCount => new Locked<int>();
        internal int InstanceId;

        public static IPluginLoaderLogger Instance;

        private readonly IAppSettings _AppSettings;

        internal PluginLoaderLogger(IAppSettings appSettings)
        {
            InstanceId = ++InstanceCount.Value;
            _AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        #endregion

        public ITextWriter SafeSetWriter()
        {
            if (_TextWriter != null)
                return _TextWriter;
            lock (_Locker)
            {
                if (_TextWriter != null)
                    return _TextWriter;
                CreateDirectory();
                return _TextWriter = new TextWriterWrapper(new StreamWriter(LogFullPath, true));
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
        }
        private string _LogPath;

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
            if (string.IsNullOrEmpty(msg))
                return;
            if (level >= LogLevel)
            {
                _LogMessages.Enqueue(new LogMessage(level, msg));
                WriteQueue();
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
            if (e == null)
                return;
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

        internal ITextWriter Writer
        {
            get { return SafeSetWriter(); }
            set { _TextWriter = value; }
        } private ITextWriter _TextWriter;

        #endregion

        internal void WriteQueue()
        {
            if (_IsWritingToQueue)
                return;
            _IsWritingToQueue = true;
            while (_LogMessages.Any())
            {
                if (_LogMessages.TryDequeue(out LogMessage logMessage))
                {
                    try
                    {
                        Writer.Write($"{logMessage.Level}: {logMessage.Message}");
                        Writer.Flush();
                    }
                    catch { } // If we fail to log, don't crash. Failure to log shouldn't cause a crash.
                }
            }
            _IsWritingToQueue = false;
        }
        private bool _IsWritingToQueue;
    }

    internal struct LogMessage
    {
        public LogMessage(PluginLoaderLogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
        public readonly PluginLoaderLogLevel Level;
        public readonly string Message;
    }
}