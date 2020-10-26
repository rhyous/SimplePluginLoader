using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhyous.SimplePluginLoader;
using Rhyous.UnitTesting;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Rhyous.SimplePluginLoader.Tests.Logging
{
    [TestClass]
    public class PluginLoaderLoggerTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppSettings> _MockAppSettings;
        private Mock<IFile> _MockFile;
        private Mock<IDirectory> _MockDirectory;
        private Mock<ITextWriter> _MockTextWriter;


        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppSettings = _MockRepository.Create<IAppSettings>();
            _MockFile = _MockRepository.Create<IFile>();
            _MockDirectory = _MockRepository.Create<IDirectory>();
            _MockTextWriter = _MockRepository.Create<ITextWriter>();
        }

        private PluginLoaderLogger CreatePluginLoaderLogger()
        {
            return new PluginLoaderLogger(_MockAppSettings.Object)
            {
                File = _MockFile.Object,
                Directory = _MockDirectory.Object,
                Writer = _MockTextWriter.Object
            };
        }

        [TestMethod]
        public void PluginLoaderLogger_SafeSetWriter_Test()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();

            // Act
            ITextWriter[] textWriterArray = new ITextWriter[15];
            Parallel.Invoke(
                () => textWriterArray[0] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[1] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[2] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[3] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[4] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[5] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[6] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[7] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[8] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[9] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[10] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[11] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[12] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[13] = pluginLoaderLogger.SafeSetWriter(),
                () => textWriterArray[14] = pluginLoaderLogger.SafeSetWriter()
                );
            var expected = pluginLoaderLogger.Writer;


            // Assert
            foreach (var textWriter in textWriterArray)
            {
                Assert.AreEqual(expected, textWriter);
            }
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        [StringIsNullEmpty]
        public void PluginLoaderLogger_Write_NullEmptyMsg_Ignored(string msg)
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();
            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;

            // Act
            pluginLoaderLogger.Write(level, msg);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_Write_LogLevel_Off()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();
            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;
            var msg = "msg1";
            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Info" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            // Act
            pluginLoaderLogger.Write(level, msg);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_Write_WhitespaceMsg_Written()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();

            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Debug" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;
            var msg = Environment.NewLine;
            _MockTextWriter.Setup(m=>m.Write($"Debug: {Environment.NewLine}"));
            _MockTextWriter.Setup(m=>m.Flush());

            // Act
            pluginLoaderLogger.Write(level, msg);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_WriteLine_EmptyMsg_WritesNewLine()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();

            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Debug" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;
            string msg = null;
            _MockTextWriter.Setup(m => m.Write($"Debug: {Environment.NewLine}"));
            _MockTextWriter.Setup(m => m.Flush());

            // Act
            pluginLoaderLogger.WriteLine(level, msg);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_WriteLine_MessageAlreadyInQueue_WritesQueuedLineFirst()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();
            var logMessage = new LogMessage(PluginLoaderLogLevel.Debug, $"prior message.{Environment.NewLine}");
            pluginLoaderLogger._LogMessages.Enqueue(logMessage);

            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Debug" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            var priorMsg = $"Debug: prior message.{Environment.NewLine}";
            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;
            _MockTextWriter.Setup(m => m.Write(priorMsg));
            _MockTextWriter.Setup(m => m.Flush());
            string msg = "msg1";
            _MockTextWriter.Setup(m => m.Write($"Debug: {msg}{Environment.NewLine}"));
            _MockTextWriter.Setup(m => m.Flush());

            // Act
            pluginLoaderLogger.WriteLine(level, msg);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_WriteLines_Written()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();

            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Debug" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;
            string[] messages = new[] {"msg1", "msg2" };
            _MockTextWriter.Setup(m => m.Write($"Debug: msg1{Environment.NewLine}"));
            _MockTextWriter.Setup(m => m.Flush());
            _MockTextWriter.Setup(m => m.Write($"Debug: msg2{Environment.NewLine}"));

            // Act
            pluginLoaderLogger.WriteLines(level, messages);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_Log_Exception_Null()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();
            Exception e = null;

            // Act
            pluginLoaderLogger.Log(e);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_Log_ExceptionMessageLogged()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();
            
            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Debug" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            var msg = "Count cannot be less than zero.";
            Exception e = new ArgumentOutOfRangeException("count", msg);
            _MockTextWriter.Setup(m => m.Write($"Debug: System.ArgumentOutOfRangeException: Count cannot be less than zero.{Environment.NewLine}Parameter name: count{Environment.NewLine}"));
            _MockTextWriter.Setup(m => m.Flush());

            // Act
            pluginLoaderLogger.Log(e);

            // Assert
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginLoaderLogger_Log_ExceptionMessage_TryingToLog_Ignored()
        {
            // Arrange
            var pluginLoaderLogger = CreatePluginLoaderLogger();

            var nvc = new NameValueCollection { { "PluginLoaderLogLevel", "Debug" } };
            _MockAppSettings.Setup(m => m.Settings).Returns(nvc);

            PluginLoaderLogLevel level = PluginLoaderLogLevel.Debug;

            var msg = "msg1";
            Exception e = new ArgumentOutOfRangeException("count", msg);
            _MockTextWriter.Setup(m => m.Write($"{level}: {msg}"))
                           .Throws(e);

            // Act
            pluginLoaderLogger.Write(level, msg);

            // Assert
            _MockRepository.VerifyAll();
        }
    }
}
