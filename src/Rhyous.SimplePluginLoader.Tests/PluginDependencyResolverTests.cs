using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rhyous.SimplePluginLoader.Tests
{
    [TestClass]
    public class PluginDependencyResolverTests
    {
        private MockRepository _MockRepository;

        private Mock<IAppDomain> _MockAppDomain;
        private Mock<IPluginLoaderSettings> _MockIPluginLoaderSettings;
        private Mock<IAssemblyLoader> _MockAssemblyLoader;
        private Mock<IPlugin> _MockPlugin;
        private Mock<IPluginLoaderLogger> _MockPluginLoaderLogger;
        private Mock<IDirectory> _MockDirectory;
        private IWaiter _Waiter;
        private IAssemblyResolveCache _AssemblyResolveCache;

        [TestInitialize]
        public void TestInitialize()
        {
            _MockRepository = new MockRepository(MockBehavior.Strict);

            _MockAppDomain = _MockRepository.Create<IAppDomain>();
            _MockIPluginLoaderSettings = _MockRepository.Create<IPluginLoaderSettings>();
            _MockAssemblyLoader = _MockRepository.Create<IAssemblyLoader>();
            _MockPlugin = _MockRepository.Create<IPlugin>();
            _MockPluginLoaderLogger = _MockRepository.Create<IPluginLoaderLogger>();
            _MockDirectory = _MockRepository.Create<IDirectory>();
            _Waiter = new Waiter(_MockPluginLoaderLogger.Object);
            _AssemblyResolveCache = new AssemblyResolveCache();
        }

        private PluginDependencyResolver CreatePluginDependencyResolver()
        {
            return new PluginDependencyResolver(_MockAppDomain.Object,
                                                _MockIPluginLoaderSettings.Object,
                                                _MockAssemblyLoader.Object,
                                                _Waiter,
                                                _AssemblyResolveCache,
                                                _MockPluginLoaderLogger.Object)
            {
                Plugin = _MockPlugin.Object,
                Directory = _MockDirectory.Object
            };
        }

        #region Constructor

        [TestMethod]
        public void PluginDependencyResolver_Constructor_NullAppDomain_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new PluginDependencyResolver(
                    null,
                    _MockIPluginLoaderSettings.Object,
                    _MockAssemblyLoader.Object,
                    new Waiter(_MockPluginLoaderLogger.Object),
                    new AssemblyResolveCache(),
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void PluginDependencyResolver_Constructor_NullIPluginLoaderSettings_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new PluginDependencyResolver(
                    _MockAppDomain.Object,
                    null,
                    _MockAssemblyLoader.Object,
                    new Waiter(_MockPluginLoaderLogger.Object),
                    new AssemblyResolveCache(),
                    _MockPluginLoaderLogger.Object);
            });
        }

        [TestMethod]
        public void PluginDependencyResolver_Constructor_NullAssemblyLoader_Test()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new PluginDependencyResolver(
                    _MockAppDomain.Object,
                    _MockIPluginLoaderSettings.Object,
                    null,
                    new Waiter(_MockPluginLoaderLogger.Object),
                    new AssemblyResolveCache(),
                    _MockPluginLoaderLogger.Object);
            });
        }
        #endregion

        [TestMethod]
        public void PluginDependencyResolver_GetPaths_BasicTest()
        {
            // Arrange
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m => m.SharedPaths).Returns(new[] { @"c:\bin", @"c:\sharedbin\", @"c:\Libs" });
            var expectedPaths = new List<string> {
                "",
                @"c:\my\plugins",
                @"c:\my\plugins\bin",
                @"c:\my\plugins\MyPlugin",
                @"c:\my\plugins\MyPlugin\bin",
                @"c:\bin",
                @"c:\sharedbin\",
                @"c:\Libs" };

            var resolver = CreatePluginDependencyResolver();

            // Act
            var actualpaths = resolver.Paths;

            // Assert
            CollectionAssert.AreEqual(expectedPaths, actualpaths);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_Dispose_Test()
        {
            // Arrange
            var pluginDependencyResolver = CreatePluginDependencyResolver();

            // Act
            pluginDependencyResolver.Dispose();

            // Assert
            _MockRepository.VerifyAll();
        }

        #region AssemblyResolveHandler
        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_NullPlugin()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            var pluginDependencyResolver = new PluginDependencyResolver(_MockAppDomain.Object,
                                                                        _MockIPluginLoaderSettings.Object,
                                                                        _MockAssemblyLoader.Object,
                                                                        new Waiter(_MockPluginLoaderLogger.Object),
                                                                        new AssemblyResolveCache(),
                                                                        _MockPluginLoaderLogger.Object);
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), "Removed AssemblyResolver for plugin: unknown."));

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_NullSharedPaths()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m => m.SharedPaths).Returns((IEnumerable<string>)null);
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>()));
                var pathList = _MockPlugin.Object.GetPaths(_MockIPluginLoaderSettings.Object);
            foreach (var path in pathList)
            {
                _MockDirectory.Setup(m => m.Exists(path)).Returns(true);
            }

            var mockAssembly = _MockRepository.Create<IAssembly>();
            mockAssembly.Setup(m => m.Instance).Returns(Assembly.GetExecutingAssembly());
            _MockAssemblyLoader.Setup(m => m.TryLoad(@"name.dll", @"name.pdb", null))
                   .Returns(mockAssembly.Object);

            var pluginDependencyResolver = CreatePluginDependencyResolver();

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNotNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_EmptyPaths()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            var pluginDependencyResolver = CreatePluginDependencyResolver();
            pluginDependencyResolver.Paths = new List<string>();
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>()));
            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_AllPathsAlreadyTried()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m => m.SharedPaths).Returns((IEnumerable<string>)null);
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>()));
            var pathList = _MockPlugin.Object.GetPaths(_MockIPluginLoaderSettings.Object);
            foreach (var path in pathList)
            {
                _MockDirectory.Setup(m => m.Exists(path)).Returns(true);
            }

            var pluginDependencyResolver = CreatePluginDependencyResolver();

            pluginDependencyResolver._AttemptedPaths.TryAdd("name", pathList);

            // Act
            var result = pluginDependencyResolver.AssemblyResolveHandler(sender, args);

            // Assert
            Assert.IsNull(result);
            _MockRepository.VerifyAll();
        }

        [TestMethod]
        public void PluginDependencyResolver_AssemblyResolveHandler_MultipleHitsAtTheSameTime_Test()
        {
            // Arrange
            object sender = new { };
            ResolveEventArgs args = new ResolveEventArgs("name");
            _MockPlugin.Setup(m => m.FullPath).Returns(@"c:\my\plugins\MyPlugin.dll");
            _MockPlugin.Setup(m => m.Directory).Returns(@"c:\my\plugins");
            _MockPlugin.Setup(m => m.Name).Returns(@"MyPlugin");
            _MockIPluginLoaderSettings.Setup(m => m.SharedPaths).Returns((IEnumerable<string>)null);
            var pluginDependencyResolver = CreatePluginDependencyResolver();
            var pathList = _MockPlugin.Object.GetPaths(_MockIPluginLoaderSettings.Object);
            foreach (var path in pathList)
            {
                _MockDirectory.Setup(m => m.Exists(path)).Returns(true);
            }
            var mockAssembly = _MockRepository.Create<IAssembly>();
            mockAssembly.Setup(m => m.Instance).Returns(Assembly.GetExecutingAssembly());
            _MockAssemblyLoader.Setup(m => m.TryLoad(@"name.dll", @"name.pdb", null))
                               .Returns(mockAssembly.Object);

            var listLog = new ConcurrentBag<string>();
            _MockPluginLoaderLogger.Setup(m => m.WriteLine(It.IsAny<PluginLoaderLogLevel>(), It.IsAny<string>()))
                                   .Callback((PluginLoaderLogLevel level, string msg) => listLog.Add($"{level} - {msg}{Environment.NewLine}"));

            // Act
            Parallel.Invoke(() => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 01
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 02
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 03
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 04
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 05
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 06
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 07
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 08
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 09
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 10
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 11
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 12
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 13
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 14
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 15
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 16
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 17
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 18
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 19
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 20
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 21
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 22
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 23
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 24
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 25
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 26
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 27
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args), // 28
                            () => pluginDependencyResolver.AssemblyResolveHandler(sender, args));// 29

            // Assert
            Assert.AreEqual(1, listLog.Count(l=> Regex.IsMatch(l,"Debug - Thread [0-9]+ is searching for name_.\r\n")));
            Assert.AreEqual(28, listLog.Count(l => Regex.IsMatch(l, "Debug - Thread [0-9]+ is waiting name_.\r\n")));
            Assert.IsFalse(_Waiter.InProgress.Values.All(a => a));
            _MockRepository.VerifyAll();
        }
        #endregion

        #region EventTests - This is usefull for now how events behave.
        public class Event1Args
        {
            public Event1Args(string text) { Text = text; }
            public string Text { get; }
        }

        public delegate void Event1Handler(object sender, Event1Args e);
        public interface IEventHolder
        {
            event PluginDependencyResolverTests.Event1Handler Event1;

            void TriggerEvent1();
        }

        public class EventHolder : IEventHolder
        {
            public event Event1Handler Event1;
            public void TriggerEvent1()
            {
                if (Event1 != null)
                {
                    Event1.Invoke(this, new Event1Args($"Total subscribers: ${Event1.GetInvocationList().Length}"));
                }
            }
        }

        public class Event1Subscriber
        {
            private readonly IEventHolder _EventHolder;

            public Event1Subscriber(IEventHolder eventHolder)
            {
                _EventHolder = eventHolder;
            }

            public void Subscribe()
            {
                _EventHolder.Event1 += Handler;
            }
            public void Unsubscribe()
            {
                _EventHolder.Event1 -= Handler;
            }
            public int ExecutionCount { get; set; }

            private void Handler(object sender, Event1Args e)
            {
                ExecutionCount++;
            }
        }

        [TestMethod]
        public void EventFiresTwiceIfRegisteredTwice()
        {
            // Arrange
            var eventHolder = new EventHolder();
            var subscriber = new Event1Subscriber(eventHolder);
            subscriber.Subscribe();
            subscriber.Subscribe();

            // Act
            eventHolder.TriggerEvent1();

            // Assert
            Assert.AreEqual(2, subscriber.ExecutionCount);
        }

        [TestMethod]
        public void EventFiresIfRegisteredTwiceButOnlyRemovedOnce()
        {
            // Arrange
            var eventHolder = new EventHolder();
            var subscriber = new Event1Subscriber(eventHolder);
            subscriber.Subscribe();
            subscriber.Subscribe();
            subscriber.Unsubscribe();

            // Act
            eventHolder.TriggerEvent1();

            // Assert
            Assert.AreEqual(1, subscriber.ExecutionCount);
        }

        #endregion
    }
}
