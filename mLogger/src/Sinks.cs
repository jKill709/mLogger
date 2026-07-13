using System.Text;

namespace mLogger
{
    #region Sinks and Interface
    public interface ILogSink
    {
        void WriteLine(LogEntry entry);
        void WriteHeading(LogEntry message);
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
        public void WriteLine(LogEntry entry)
        {
            lock (_lock)
            {
                NewLogFileIfNeeded();
                _writer.WriteLine(LogFormatter.FormatOneLineText(entry));
                _writer.Flush();
            }
        }
        public void WriteHeading(LogEntry entry)
        {
            lock (_lock)
            {
                Char seperatorChar = '-';
                string line = LogFormatter.FormatOneLineText(entry);
                string innerPadding = new string(' ', 2);
                string partialSeperator = new string(seperatorChar, 6);
                string leadingPadding = new string(' ', line.Length - entry.Message.Length);
                string fullSeperator = new string(seperatorChar, entry.Message.Length + (innerPadding.Length * 2) + (partialSeperator.Length * 2));
                LogEntry firstLineEntry = new LogEntry { Timestamp = entry.Timestamp,
                                                         Level = entry.Level,
                                                         Source = entry.Source,
                                                         Message = fullSeperator };

                NewLogFileIfNeeded();
                _writer.WriteLine(LogFormatter.FormatOneLineText(firstLineEntry));
                _writer.WriteLine(leadingPadding + partialSeperator + innerPadding + entry.Message + innerPadding + partialSeperator);
                _writer.WriteLine(leadingPadding + fullSeperator);
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
        public void WriteLine(LogEntry entry)
        {
            Console.WriteLine(LogFormatter.FormatOneLineText(entry));
        }
        public void WriteHeading(LogEntry entry)
        {        
            Char seperatorChar = '-';
            string line = LogFormatter.FormatOneLineText(entry);
            string innerPadding = new string(' ', 2);
            string partialSeperator = new string(seperatorChar, 6);
            string leadingPadding = new string(' ', line.Length - entry.Message.Length);
            string fullSeperator = new string(seperatorChar, entry.Message.Length + (innerPadding.Length * 2) + (partialSeperator.Length * 2));
            LogEntry firstLineEntry = new LogEntry
            {
                Timestamp = entry.Timestamp,
                Level = entry.Level,
                Source = entry.Source,
                Message = fullSeperator
            };

            Console.WriteLine(LogFormatter.FormatOneLineText(firstLineEntry));
            Console.WriteLine(leadingPadding + partialSeperator + innerPadding + entry.Message + innerPadding + partialSeperator);
            Console.WriteLine(leadingPadding + fullSeperator);
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

        public void WriteLine(LogEntry entry)
        {
            lock (_lock)
            {
                _inMemoryLogs.Add(LogFormatter.FormatOneLineText(entry));
            }
        }
        public void WriteHeading(LogEntry entry)
        {
            _inMemoryLogs.Add(LogFormatter.FormatOneLineText(entry));
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