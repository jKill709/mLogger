using mLogger;
using Xunit;

public class LoggerTests
{
    private InMemorySink _memorySink;

    private void ResetLogger()
    {
        Logger.Instance.Shutdown();
        Logger.Instance.ResetForTests();

        _memorySink = new InMemorySink();

        Logger.Instance.Initialize("TestApp");
        Logger.Instance.AddSink(_memorySink);
    }


    [Fact]
    public void Info_ShouldSendEntryToSink()
    {
        ResetLogger();

        Logger.Instance.Info("UnitTest", "Hello");

        Assert.Single(_memorySink.Logs);
        Assert.Contains("Hello", _memorySink.Logs[0]);
    }
    [Fact]
    public void WriteLine_ShouldStoreFormattedMessage()
    {
        var sink = new InMemorySink();

        sink.WriteLine(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.INFO,
            Source = "Test",
            Message = "Hello"
        });

        Assert.Single(sink.Logs);
        Assert.Contains("Test", sink.Logs[0]);
        Assert.Contains("Hello", sink.Logs[0]);
    }
    [Fact]
    public void WriteHeading_ShouldStoreFormattedMessage()
    {
        var sink = new InMemorySink();

        sink.WriteHeading(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.INFO,
            Source = "Test",
            Message = "Hello"
        });

        Assert.Single(sink.Logs);
        Assert.Contains("--- Hello ---", sink.Logs[0]);
    }
    [Fact]
    public void WriteSeperator_ShouldStoreSeperator()
    {
        var sink = new InMemorySink();
        sink.WriteSeperator(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.INFO,
            Source = "Test",
            Message = "Hello"
        });
        Assert.Single(sink.Logs);
        Assert.Equal(new string('-', 120), sink.Logs[0]);
    }


    [Theory]
    [InlineData(LogLevel.DEBUG, "DBG")]
    [InlineData(LogLevel.INFO, "INF")]
    [InlineData(LogLevel.WARN, "WRN")]
    [InlineData(LogLevel.ERROR, "ERR")]
    [InlineData(LogLevel.FATAL, "FTL")]
    public void Log_ShouldUseCorrectLevel(
        LogLevel level,
        string expected)
    {
        ResetLogger();

        Logger.Instance.Log(level, "Test", "Message");

        Assert.Contains(expected, _memorySink.Logs[0]);
    }


    [Fact]
    public void MultipleSinks_ShouldReceiveMessage()
    {
        ResetLogger();

        var secondSink = new InMemorySink();

        Logger.Instance.AddSink(secondSink);

        Logger.Instance.Info("Test", "Hello");

        Assert.Single(_memorySink.Logs);
        Assert.Single(secondSink.Logs);
    }

    [Fact]
    public void RemoveSink_ShouldStopReceivingMessages()
    {
        ResetLogger();

        var secondSink = new InMemorySink();
        Logger.Instance.AddSink(secondSink);
        Logger.Instance.Info("Test", "Hello");

        Assert.Single(_memorySink.Logs);
        Assert.Single(secondSink.Logs);

        Logger.Instance.RemoveSink(secondSink);
        Logger.Instance.Info("Test", "World");
        Assert.Equal(2, _memorySink.Logs.Count);
        Assert.Single(secondSink.Logs); // Should still be 1
    }

    [Fact]
    public void BlacklistPattern_ShouldHaveNoFalsePositives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddPattern("^Blocked$");

        Logger.Instance.Info("Allowed", "Hello");

        Assert.Single(_memorySink.Logs);
    }

    [Fact]
    public void BlacklistPattern_ShouldHaveNoFalseNegatives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddPattern("^Blocked$");

        Logger.Instance.Info("Blocked", "Hello");

        Assert.Empty(_memorySink.Logs);
    }

    [Fact]
    public void WhitelistPattern_ShouldHaveNoFalsePositives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddPattern("^Allowed$");

        Logger.Instance.Info("Blocked", "Hello");

        Assert.Empty(_memorySink.Logs);
    }

    [Fact]
    public void WhitelistPattern_ShouldHaveNoFalseNegatives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddPattern("^Allowed$");

        Logger.Instance.Info("Allowed", "Hello");

        Assert.Single(_memorySink.Logs);
    }

    [Fact]
    public void BlacklistSourceWithoutModules_ShouldHaveNoFalsePositives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddSource("Blocked", false);

        Logger.Instance.Info("Allowed", "Hello");

        Assert.Single(_memorySink.Logs);
    }

    [Fact]
    public void BlacklistSourceWithoutModules_ShouldHaveNoFalseNegatives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddSource("Blocked", false);

        Logger.Instance.Info("Blocked", "Hello");

        Assert.Empty(_memorySink.Logs);
    }

    [Fact]
    public void WhitelistSourceWithoutModules_ShouldHaveNoFalsePositives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddSource("Allowed", false);

        Logger.Instance.Info("Blocked", "Hello");

        Assert.Empty(_memorySink.Logs);
    }

    [Fact]
    public void WhitelistSourceWithoutModules_ShouldHaveNoFalseNegatives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddSource("Allowed", false);

        Logger.Instance.Info("Allowed", "Hello");

        Assert.Single(_memorySink.Logs);
    }

    [Fact]
    public void BlacklistSourceWithModules_ShouldHaveNoFalsePositives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddSource("Blocked", true);

        Logger.Instance.Info("Allowed_Module", "Hello");

        Assert.Single(_memorySink.Logs);
    }

    [Fact]
    public void BlacklistSourceWithModules_ShouldHaveNoFalseNegatives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddSource("Blocked", true);

        Logger.Instance.Info("Blocked_Module", "Hello");

        Assert.Empty(_memorySink.Logs);
    }

    [Fact]
    public void WhitelistSourceWithModules_ShouldHaveNoFalsePositives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddSource("Allowed", true);

        Logger.Instance.Info("Blocked_Module", "Hello");

        Assert.Empty(_memorySink.Logs);
    }

    [Fact]
    public void WhitelistSourceWithModules_ShouldHaveNoFalseNegatives()
    {
        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddSource("Allowed", true);

        Logger.Instance.Info("Allowed_Module", "Hello");

        Assert.Single(_memorySink.Logs);
    }

    [Fact]
    public void useList_ShouldEnableBlackListWhenTrue()
    {
        string allowed = "Allowed";
        string blocked = "Blocked";

        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = true;
        _memorySink.AddSource(blocked, true);

        Logger.Instance.Info(allowed, "Hello");
        Logger.Instance.Info(blocked, "Hello");

        Assert.Contains(_memorySink.Logs, log => log.Contains(allowed));
        Assert.DoesNotContain(_memorySink.Logs, log => log.Contains(blocked));
    }

    [Fact]
    public void useList_ShouldDisableBlackListWhenFalse()
    {
        string allowed = "Allowed";
        string blocked = "Blocked";

        ResetLogger();

        _memorySink.useList = false;
        _memorySink.isBlacklist = true;
        _memorySink.AddSource(blocked, true);

        Logger.Instance.Info(allowed, "Hello");
        Logger.Instance.Info(blocked, "Hello");

        Assert.Contains(_memorySink.Logs, log => log.Contains(allowed));
        Assert.Contains(_memorySink.Logs, log => log.Contains(blocked));
    }

    [Fact]
    public void useList_ShouldEnableWhitelistWhenTrue()
    {
        string allowed = "Allowed";
        string blocked = "Blocked";

        ResetLogger();

        _memorySink.useList = true;
        _memorySink.isBlacklist = false;
        _memorySink.AddSource(allowed, true);

        Logger.Instance.Info(allowed, "Hello");
        Logger.Instance.Info(blocked, "Hello");

        Assert.Contains(_memorySink.Logs, log => log.Contains(allowed));
        Assert.DoesNotContain(_memorySink.Logs, log => log.Contains(blocked));
    }

    [Fact]
    public void useList_ShouldDisableWhitelistWhenFalse()
    {
        string allowed = "Allowed";
        string blocked = "Blocked";

        ResetLogger();

        _memorySink.useList = false;
        _memorySink.isBlacklist = false;
        _memorySink.AddSource(allowed, true);

        Logger.Instance.Info(allowed, "Hello");
        Logger.Instance.Info(blocked, "Hello");

        Assert.Contains(_memorySink.Logs, log => log.Contains(allowed));
        Assert.Contains(_memorySink.Logs, log => log.Contains(blocked));
    }

}