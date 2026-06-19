using mLogger;
using Xunit;

public class LoggerTests
{
    private static string GetExpectedLogFile(string appName, string logDir)
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        return Path.Combine(logDir, $"{appName}_{today}.log");
    }
    private static void ResetLogger(string appName, string logDir)
    {
        Logger.Instance.Shutdown();
        Logger.Instance.ResetForTests();

        Logger.Instance.Initialize(appName, logDir);
    }
    private static void ResetLogger()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);
    }

    [Theory]
    [InlineData(LogLevel.DEBUG, "DBG")]
    [InlineData(LogLevel.INFO, "INF")]
    [InlineData(LogLevel.WARN, "WRN")]
    [InlineData(LogLevel.ERROR, "ERR")]
    [InlineData(LogLevel.FATAL, "FTL")]
    public void LogLevel_ShouldWriteCorrectLabel(LogLevel level, string expectedTag)
    {
        // Arrange
        Logger.Instance.ResetForTests();

        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Logger.Instance.Initialize(appName, logDir);

        // Act
        Logger.Instance.Log(level, "UnitTest", "Hello");

        Logger.Instance.Shutdown();

        string file = Path.Combine(
            logDir,
            $"{appName}_{DateTime.Now:yyyy-MM-dd}.log"
        );

        string contents = File.ReadAllText(file);

        // Assert
        Assert.Contains($"[{expectedTag}]", contents);
    }

    [Fact]
    public void Info_ShouldCreateLogFile()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        Logger.Instance.Info("UnitTest", "Hello");

        Logger.Instance.Shutdown();

        string logFile = GetExpectedLogFile(appName, logDir);

        Assert.True(File.Exists(logFile));
    }

    [Fact]
    public void Info_ShouldWriteSource()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        string source = "MyComponent";

        Logger.Instance.Info(source, "Hello");

        Logger.Instance.Shutdown();

        string contents =
            File.ReadAllText(GetExpectedLogFile(appName, logDir));

        Assert.Contains("[MyComponent]", contents);
    }

    [Fact]
    public void Info_ShouldWriteInfoLevel()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        Logger.Instance.Info("UnitTest", "Hello");

        Logger.Instance.Shutdown();

        string contents =
            File.ReadAllText(GetExpectedLogFile(appName, logDir));

        Assert.Contains("[INF]", contents);
    }

    [Fact]
    public void Info_ShouldWriteTimestamp()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        Logger.Instance.Info("UnitTest", "Hello");

        Logger.Instance.Shutdown();

        string contents =
            File.ReadAllText(GetExpectedLogFile(appName, logDir));

        string pattern =
            @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3}\]";

        Assert.Matches(pattern, contents);
    }

    [Fact]
    public void Info_ShouldWriteMessage()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        string expectedMessage = "Hello World";

        Logger.Instance.Info("UnitTest", expectedMessage);

        Logger.Instance.Shutdown();

        string logFile = GetExpectedLogFile(appName, logDir);

        string contents = File.ReadAllText(logFile);

        Assert.Contains(expectedMessage, contents);
    }

    [Fact]
    public void LogInfo_ShouldStoreMessage()
    {
        ResetLogger();

        string message = "Hello world";

        Logger.Instance.Info("TestSource", message);

        Assert.Contains(Logger.Instance.Logs, log => log.Contains(message));
    }

    [Fact]
    public void Info_EmptyMessage_ShouldWriteEmptyMessage()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        Exception ex = Record.Exception(() => Logger.Instance.Info("UnitTest", ""));

        Logger.Instance.Shutdown();

        string contents = File.ReadAllText(GetExpectedLogFile(appName, logDir));

        Assert.Contains("[UnitTest]", contents);
        Assert.DoesNotContain("null", contents);
    }

    [Fact]
    public void Info_NullMessage_ShouldNotThrow()
    {
        ResetLogger();

        Exception ex =
            Record.Exception(() =>
                Logger.Instance.Info("UnitTest", null));

        Assert.Null(ex);
    }

    [Fact]
    public void Info_NullMessage_ShouldWriteNull()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        Exception ex = Record.Exception(() => Logger.Instance.Info("UnitTest", null));

        Logger.Instance.Shutdown();

        string contents = File.ReadAllText(GetExpectedLogFile(appName, logDir));

        Assert.Contains("null", contents);
    }

    [Fact]
    public void Info_EmptySource_ShouldNotThrow()
    {
        ResetLogger();

        Exception ex =
            Record.Exception(() =>
                Logger.Instance.Info("", "Hello"));

        Assert.Null(ex);
    }

    [Fact]
    public void Info_LongMessage_ShouldBeWritten()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        string msg = new string('X', 10000);

        Logger.Instance.Info("UnitTest", msg);

        Logger.Instance.Shutdown();

        string contents =
            File.ReadAllText(GetExpectedLogFile(appName, logDir));

        Assert.Contains(msg, contents);
    }

    [Fact]
    public void Info_UnicodeMessage_ShouldBeWritten()
    {
        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        ResetLogger(appName, logDir);

        string msg = "Ω你好🚀";

        Logger.Instance.Info("UnitTest", msg);

        Logger.Instance.Shutdown();

        string contents =
            File.ReadAllText(GetExpectedLogFile(appName, logDir));

        Assert.Contains(msg, contents);
    }

    [Fact]
    public void Info_BeforeInitialize_ShouldThrowInvalidOperationException()
    {
        Logger.Instance.Shutdown();
        Logger.Instance.ResetForTests();

        Assert.Throws<InvalidOperationException>(() => Logger.Instance.Info("UnitTest", "Hello"));
    }

    [Fact]
    public void Initialize_ShouldAllowLogging()
    {
        string appName = "TestApp";
        string logDir =
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        ResetLogger(appName, logDir);

        Exception ex =
            Record.Exception(() =>
                Logger.Instance.Info("UnitTest", "Hello"));

        Assert.Null(ex);
    }

    [Fact]
    public void Info_AfterShutdown_ShouldThrow()
    {
        string appName = "TestApp";
        string logDir =
            Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        ResetLogger(appName, logDir);

        Logger.Instance.Shutdown();

        Exception ex =
            Record.Exception(() =>
                Logger.Instance.Info("UnitTest", "Hello"));

        Assert.NotNull(ex);
    }

    [Fact]
    public void Info_ParallelWrites_ShouldNotLoseMessages()
    {
        // Arrange
        Logger.Instance.Shutdown();
        Logger.Instance.ResetForTests();

        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Logger.Instance.Initialize(appName, logDir);

        int messageCount = 1000;

        // Act
        Parallel.For(0, messageCount, i =>
        {
            Logger.Instance.Info("ThreadTest", $"Message {i}");
        });

        Logger.Instance.Shutdown();

        // Assert (in-memory check is fastest and most reliable)
        Assert.Equal(messageCount, Logger.Instance.Logs.Count);
    }

    [Fact]
    public void Info_ParallelWrites_ShouldPreserveAllMessages()
    {
        Logger.Instance.Shutdown();
        Logger.Instance.ResetForTests();

        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Logger.Instance.Initialize(appName, logDir);

        int messageCount = 500;

        Parallel.For(0, messageCount, i =>
        {
            Logger.Instance.Info("ThreadTest", $"Message-{i}");
        });

        Logger.Instance.Shutdown();

        var logs = Logger.Instance.Logs;

        // 1. Correct count
        Assert.Equal(messageCount, logs.Count);

        // 2. Every message exists exactly once
        for (int i = 0; i < messageCount; i++)
        {
            string expected = $"Message-{i}";
            Assert.Contains(logs, l => l.Contains(expected));
        }
    }

    [Fact]
    public void Logger_MultipleThreads_ShouldRemainConsistent()
    {
        Logger.Instance.Shutdown();
        Logger.Instance.ResetForTests();

        string appName = "TestApp";
        string logDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Logger.Instance.Initialize(appName, logDir);

        int threads = 10;
        int perThread = 200;

        Parallel.For(0, threads, t =>
        {
            for (int i = 0; i < perThread; i++)
            {
                Logger.Instance.Info($"Thread-{t}", $"Msg-{i}");
            }
        });

        Logger.Instance.Shutdown();

        int expected = threads * perThread;

        Assert.Equal(expected, Logger.Instance.Logs.Count);

        // Ensure no corrupted null/empty entries
        Assert.All(Logger.Instance.Logs, log =>
        {
            Assert.False(string.IsNullOrWhiteSpace(log));
            Assert.Contains("INF", log);
        });
    }
}