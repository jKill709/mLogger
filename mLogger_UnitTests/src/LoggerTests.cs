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
}