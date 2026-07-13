using mLogger;
using Xunit;

public class TextFileSinkTests
{
    private (string Directory, string BaseFileName, string ExpectedFilePath) CreateTempFile()
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Directory.CreateDirectory(directory);

        string baseFileName = "test";
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        string expectedFilePath = Path.Combine(directory, $"{baseFileName}_{today}.txt");

        return (directory, baseFileName, expectedFilePath);
    }

    [Fact]
    public void Write_ShouldCreateFile()
    {
        var file = CreateTempFile();

        var sink = new TextFileSink(file.Directory, file.BaseFileName);

        sink.WriteLine(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.INFO,
            Source = "Test",
            Message = "Hello"
        });

        sink.Shutdown();

        Assert.True(File.Exists(file.ExpectedFilePath));
    }


    [Fact]
    public void Write_ShouldWriteMessage()
    {
        var file = CreateTempFile();

        var sink = new TextFileSink(file.Directory, file.BaseFileName);

        sink.WriteLine(new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.ERROR,
            Source = "Camera",
            Message = "Failure"
        });

        sink.Shutdown();

        string text = File.ReadAllText(file.ExpectedFilePath);

        Assert.Contains("Failure", text);
        Assert.Contains("Camera", text);
        Assert.Contains("ERR", text);
    }


    [Fact]
    public void Reset_ShouldClearFile()
    {
        var file = CreateTempFile();

        var sink = new TextFileSink(file.Directory, file.BaseFileName);

        sink.WriteLine(new LogEntry
        {
            Level = LogLevel.INFO,
            Message = "Old"
        });

        sink.ResetForTesting();

        Assert.True(File.Exists(file.ExpectedFilePath));
        sink.Shutdown();

        string text = File.ReadAllText(file.ExpectedFilePath);

        Assert.DoesNotContain("Old", text);
    }
} 