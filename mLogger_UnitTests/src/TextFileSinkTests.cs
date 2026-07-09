using mLogger;
using Xunit;

public class TextFileSinkTests
{
    private string CreateTempFile()
    {
        string dir =
            Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());

        Directory.CreateDirectory(dir);

        return Path.Combine(dir, "test.log");
    }


    [Fact]
    public void Write_ShouldCreateFile()
    {
        string file = CreateTempFile();

        var sink = new TextFileSink(file);

        sink.Write(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.INFO,
            Source = "Test",
            Message = "Hello"
        });

        sink.Shutdown();

        Assert.True(File.Exists(file));
    }


    [Fact]
    public void Write_ShouldWriteMessage()
    {
        string file = CreateTempFile();

        var sink = new TextFileSink(file);

        sink.Write(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.ERROR,
            Source = "Camera",
            Message = "Failure"
        });

        sink.Shutdown();

        string text = File.ReadAllText(file);

        Assert.Contains("Failure", text);
        Assert.Contains("Camera", text);
        Assert.Contains("ERR", text);
    }


    [Fact]
    public void Reset_ShouldClearFile()
    {
        string file = CreateTempFile();

        var sink = new TextFileSink(file);

        sink.Write(new LogEntry
        {
            Level = LogLevel.INFO,
            Message = "Old"
        });

        sink.ResetForTesting();

        sink.Shutdown();

        string text = File.ReadAllText(file);

        Assert.DoesNotContain("Old", text);
    }
} 