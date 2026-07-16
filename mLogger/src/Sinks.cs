using System.Text;
using System.Text.RegularExpressions;

namespace mLogger
{
    #region Sinks and Interface
    public interface ILogSink // todo - add ': IDisposable'
    {

        void WriteLine(LogEntry entry);
        void WriteHeading(LogEntry entry);
        void WriteSeperator(LogEntry entry);
        void ResetForTesting();
        void Shutdown();
    }
    public abstract class LogSinkBase : ILogSink
    {
        private readonly List<Regex> _patterns = new();

        public bool IsBlacklist { get; set; } = true;

        public void AddPattern(string pattern)
        {
            _patterns.Add(new Regex(pattern, RegexOptions.Compiled));
        }

        protected bool IsListed(string source)
        {
            return _patterns.Any(r => r.IsMatch(source));
        }

        protected bool ShouldWrite(string source)
        {
            bool listed = IsListed(source);

            return IsBlacklist
                ? !listed      // blacklist
                : listed;      // whitelist
        }

        public abstract void WriteLine(LogEntry entry);
        public abstract void WriteHeading(LogEntry entry);
        public abstract void WriteSeperator(LogEntry entry);
        public abstract void ResetForTesting();
        public abstract void Shutdown();
    }
    public class TextFileSink : LogSinkBase
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
        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            lock (_lock)
            {
                NewLogFileIfNeeded();
                _writer.WriteLine(LogFormatter.FormatOneLineText(entry));
                _writer.Flush();
            }
        }
        public override void WriteHeading(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

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
        public override void WriteSeperator(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            lock (_lock)
            {
                _writer.WriteLine(new string('-', 120));
                _writer.Flush();
            }
        }
        public override void ResetForTesting()
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
        public override void Shutdown()
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
    public class ConsoleSink : LogSinkBase
    {
        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return; 
            
            Console.WriteLine(LogFormatter.FormatOneLineText(entry));
        }
        public override void WriteHeading(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

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
        public override void WriteSeperator(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            Console.WriteLine(new string('-', 120));
        }
        public override void ResetForTesting()
        {
            Console.WriteLine("--- ResetForTesting called on ConsoleSink. No action taken. ---");
        }
        public override void Shutdown()
        {
            Console.WriteLine("--- Shutdown called on ConsoleSink. No action taken. ---");
        }
    }
    public class InMemorySink : LogSinkBase
    {
        private readonly object _lock = new();

        private readonly List<string> _inMemoryLogs = new();
        public IReadOnlyList<string> Logs => _inMemoryLogs;

        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            lock (_lock)
            {   
                _inMemoryLogs.Add(LogFormatter.FormatOneLineText(entry));
            }
        }
        public override void WriteHeading(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            LogEntry headeredEntry = new LogEntry
            {
                Timestamp = entry.Timestamp,
                Level = entry.Level,
                Source = entry.Source,
                Message = $"--- {entry.Message} ---"
            };

            lock (_lock)
            {
                _inMemoryLogs.Add(LogFormatter.FormatOneLineText(headeredEntry)); 
            }
        }
        public override void WriteSeperator(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            lock (_lock)
            {
                _inMemoryLogs.Add(new string('-', 120));
            }
        }
        public override void ResetForTesting()
        {
            lock (_lock)
            {
                _inMemoryLogs.Clear();
            }
        }
        public override void Shutdown()
        { }
    }
    #endregion
}