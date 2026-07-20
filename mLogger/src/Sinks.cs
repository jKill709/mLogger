using System.Text;
using System.Text.RegularExpressions;

namespace mLogger
{
    #region Sinks and Interface
    public interface ILogSink // todo - add ': IDisposable'
    {
        void WriteLine(LogEntry entry);
        void WriteHeading(LogEntry entry);
        void WriteSeparator(LogEntry entry);
        void ResetForTesting();
        void Shutdown();
    }
    public abstract class LogSinkBase : ILogSink
    {
        protected readonly List<Regex> _patterns = new();

        public bool isBlacklist { get; set; } = true;
        public bool useList { get; set; } = false;

        public LogSinkBase() { }
        protected LogSinkBase(LogSinkBase other)
        {
            isBlacklist = other.isBlacklist;
            useList = other.useList;
            _patterns.AddRange(other._patterns);
        }
        public void AddPattern(Regex pattern)
        {
            _patterns.Add(pattern);
        }
        public void AddPattern(string pattern)
        {
            _patterns.Add(new Regex(pattern, RegexOptions.Compiled));
        }
        public void AddSource(string source, bool andModules = true)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Source cannot be null or empty.", nameof(source));

            AddPattern(CreateSourceRegex(source, andModules));
        }
        public void RemovePattern(Regex pattern)
        {
            _patterns.Remove(pattern);
        }
        public void RemovePattern(string pattern)
        {
            _patterns.RemoveAll(r => r.ToString() == pattern);
        }
        public void RemoveSource(string source, bool andModules = true)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Source cannot be null or empty.", nameof(source));
            Regex patternToRemove = CreateSourceRegex(source, andModules);
            RemovePattern(patternToRemove);
        }
        protected static Regex CreateSourceRegex(string source, bool andModules)
        {
            string pattern = andModules
                ? $"^{Regex.Escape(source)}(?:_.*)?$"
                : $"^{Regex.Escape(source)}$";

            return new Regex(pattern, RegexOptions.Compiled);
        }
        protected bool IsListed(string source)
        {
            return _patterns.Any(r => r.IsMatch(source));
        }
        protected bool ShouldWrite(string source)
        {
            if (useList)
            {
                return isBlacklist ? !IsListed(source) : IsListed(source);
            }
            else
            {
                return true; // no filtering
            }
        }

        public abstract void WriteLine(LogEntry entry);
        public abstract void WriteHeading(LogEntry entry);
        public abstract void WriteSeparator(LogEntry entry);

        //public void CopyTo(LogSinkBase destination)
        //{
        //    CopyBase(destination);

        //    if (destination.GetType() == GetType())
        //        CopyDerived(destination);
        //}
        //protected virtual void CopyBase(LogSinkBase destination)
        //{
        //    destination.isBlacklist = isBlacklist;
        //    destination.useList = useList;
        //}
        //protected virtual void CopyDerived(LogSinkBase destination)
        //{
        //}
        public abstract void ResetForTesting();
        public abstract void Shutdown();
    }
    public class TextFileSink : LogSinkBase
    {
        private readonly object _lock = new();

        private string _appName;
        private string _fileExtension;
        private string _fileName;
        private string _fileDirectory;
        private StreamWriter _writer;
        private string _currentDate = "0000-00-00";
        public string FilePath { get { return Path.Combine(_fileDirectory, _fileName); } }

        public TextFileSink(LogSinkBase other) : base(other)
        {
            // If it's really a TextFileSink, copy the App Name
            if (other is TextFileSink tfs)
                _appName = tfs._appName;
        }
        public TextFileSink(string fileDirectory, string appName, string fileExtension = ".txt")
        {
            ArgumentNullException.ThrowIfNull(fileDirectory);
            ArgumentNullException.ThrowIfNull(appName);
            ArgumentNullException.ThrowIfNull(fileExtension);

            _fileDirectory = fileDirectory;
            _appName = appName;
            _fileExtension = fileExtension;
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
                Char separatorChar = '-';
                string line = LogFormatter.FormatOneLineText(entry);
                string innerPadding = new string(' ', 2);
                string partialSeparator = new string(separatorChar, 6);
                string leadingPadding = new string(' ', line.Length - entry.Message.Length);
                string fullSeparator = new string(separatorChar, entry.Message.Length + (innerPadding.Length * 2) + (partialSeparator.Length * 2));
                LogEntry firstLineEntry = new LogEntry { Timestamp = entry.Timestamp,
                                                         Level = entry.Level,
                                                         Source = entry.Source,
                                                         Message = fullSeparator };

                NewLogFileIfNeeded();
                _writer.WriteLine(LogFormatter.FormatOneLineText(firstLineEntry));
                _writer.WriteLine(leadingPadding + partialSeparator + innerPadding + entry.Message + innerPadding + partialSeparator);
                _writer.WriteLine(leadingPadding + fullSeparator);
                _writer.Flush();
            }
        }
        public override void WriteSeparator(LogEntry entry)
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
                    _fileName = _appName + "_" + today + _fileExtension;
                    _writer = new StreamWriter(Path.Combine(_fileDirectory, _fileName), true, Encoding.UTF8);
                }

                return _currentDate;
            }
        }
    }
    public class ConsoleSink : LogSinkBase
    {
        public ConsoleSink() { }
        public ConsoleSink(LogSinkBase other) : base(other) { }
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

            Char separatorChar = '-';
            string line = LogFormatter.FormatOneLineText(entry);
            string innerPadding = new string(' ', 2);
            string partialSeparator = new string(separatorChar, 6);
            string leadingPadding = new string(' ', line.Length - entry.Message.Length);
            string fullSeparator = new string(separatorChar, entry.Message.Length + (innerPadding.Length * 2) + (partialSeparator.Length * 2));
            LogEntry firstLineEntry = new LogEntry
            {
                Timestamp = entry.Timestamp,
                Level = entry.Level,
                Source = entry.Source,
                Message = fullSeparator
            };

            Console.WriteLine(LogFormatter.FormatOneLineText(firstLineEntry));
            Console.WriteLine(leadingPadding + partialSeparator + innerPadding + entry.Message + innerPadding + partialSeparator);
            Console.WriteLine(leadingPadding + fullSeparator);
        }
        public override void WriteSeparator(LogEntry entry)
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

        public InMemorySink() { }
        public InMemorySink(LogSinkBase other) : base(other) { }
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
        public override void WriteSeparator(LogEntry entry)
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