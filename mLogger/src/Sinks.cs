using System.Text;

namespace mLogger
{
    #region Sinks and Interface
    public interface ILogSink
    {
        void Write(LogEntry entry);
        void ResetForTesting();
        void Shutdown();
    }
    public class TextFileSink : ILogSink
    {
        private readonly object _lock = new();

        private string _appName;
        private string _fileExtention;
        private string _fileName;
        private string _fileDirectory;
        private StreamWriter _writer;
        private string _currentDate = "0000-00-00";

        public TextFileSink(string fileDirectory, string appName, string fileExtension = ".txt")
        {
            ArgumentNullException.ThrowIfNull(fileDirectory);
            ArgumentNullException.ThrowIfNull(appName);
            ArgumentNullException.ThrowIfNull(fileExtension);

            _fileDirectory = fileDirectory;
            _appName = appName;
            _fileExtention = fileExtension;
            NewLogFileIfNeeded();
        }
        ~TextFileSink()
        {
            Shutdown();
        }
        public void Write(LogEntry entry)
        {
            lock (_lock)
            {
                NewLogFileIfNeeded();
                _writer.WriteLine(LogFormatter.FormatOneLineText(entry));
                _writer.Flush();
            }
        }
        public void ResetForTesting()
        {
            lock (_lock)
            {
                // Close the current file.
                _writer?.Dispose();

                string filePath = Path.Combine(_fileDirectory, _fileName);

                // Delete the existing log file if it exists.
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Resest fileName and _writer state
                _currentDate = "0000-00-00";
                NewLogFileIfNeeded();
            }
        }
        public void Shutdown()
        {
            lock (_lock)
            {
                _writer?.Dispose();
                _writer = null;
            }
        }
        private string NewLogFileIfNeeded()
        {
            // If the date has changed since the last write, close the old file and open a new one

            lock (_lock)
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                if (today != _currentDate)
                {
                    _writer?.Dispose();
                    _currentDate = today;
                    _fileName = _appName + "_" + today + _fileExtention;
                    _writer = new StreamWriter(Path.Combine(_fileDirectory, _fileName), true, Encoding.UTF8);
                }

                return _currentDate;
            }
        }
    }
    public class ConsoleSink : ILogSink
    {
        public void Write(LogEntry entry)
        {
            Console.WriteLine(LogFormatter.FormatOneLineText(entry));
        }
        public void ResetForTesting()
        {
            Console.WriteLine("--- ResetForTesting called on ConsoleSink. No action taken. ---");
        }
        public void Shutdown()
        {
            Console.WriteLine("--- Shutdown called on ConsoleSink. No action taken. ---");
        }
    }
    public class InMemorySink : ILogSink
    {
        private readonly object _lock = new();

        private readonly List<string> _inMemoryLogs = new();
        public IReadOnlyList<string> Logs => _inMemoryLogs;

        public void Write(LogEntry entry)
        {
            lock (_lock)
            {
                _inMemoryLogs.Add(LogFormatter.FormatOneLineText(entry));
            }
        }
        public void ResetForTesting()
        {
            lock (_lock)
            {
                _inMemoryLogs.Clear();
            }
        }
        public void Shutdown()
        { }
    }
    #endregion
}