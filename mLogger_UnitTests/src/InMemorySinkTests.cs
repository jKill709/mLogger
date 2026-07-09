using mLogger;
using Xunit;

public class InMemorySinkTests
{
    [Fact]
    public void Write_ShouldStoreFormattedMessage()
    {
        var sink = new InMemorySink();

        sink.Write(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.INFO,
            Source = "Test",
            Message = "Hello"
        });

        Assert.Single(sink.Logs);
        Assert.Contains("Hello", sink.Logs[0]);
    }


    [Fact]
    public void Reset_ShouldClearLogs()
    {
        var sink = new InMemorySink();

        sink.Write(new LogEntry
        {
            Level = LogLevel.INFO,
            Message = "Hello"
        });

        sink.ResetForTesting();

        Assert.Empty(sink.Logs);
    }
}