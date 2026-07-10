// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Jeremy Killinger

using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace mLogger
{
    #region Sink Interfaces and Implementations
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL
    }
    public class LogEntry
    {
        public DateTime Timestamp { get; init; }

        public LogLevel Level { get; init; }

        public string Source { get; init; } = "";

        public string Message { get; init; } = "";
    }
    internal static class LogFormatter
    {
        public static string FormatOneLineText(LogEntry entry)
        {
            string timestamp = entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

            string levelName = entry.Level switch
            {
                LogLevel.DEBUG => "DBG",
                LogLevel.INFO => "INF",
                LogLevel.WARN => "WRN",
                LogLevel.ERROR => "ERR",
                LogLevel.FATAL => "FTL",
                _ => "UNK"
            };

            return $"[{timestamp}] [{levelName}] [{entry.Source}] {entry.Message ?? "null"}";
        }

    }
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
        {   }
    }
    #endregion

    #region Logger Class
    public class Logger
    {
        private static readonly Lazy<Logger> _instance = new(() => new Logger());
        public static Logger Instance => _instance.Value;

        private readonly object _lock = new();
        private string _appName;
        private bool _isInitialized;
        private readonly List<ILogSink> _sinks = new();

        private Logger() { }

        public void Initialize(string appName)
        {
            _appName = appName;
            _isInitialized = true;
        }

        public void AddSink(ILogSink sink)
        {
            ArgumentNullException.ThrowIfNull(sink);
            _sinks.Add(sink);
        }

        public void Log(LogLevel level, string source, string message)
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Logger must be initialized before use.");
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Source = source,
                Message = message
            };

            foreach (var sink in _sinks)
            {
                sink.Write(entry);
            }
        }

        public void Debug(string source, string message) => Log(LogLevel.DEBUG, source, message);
        public void Info(string source, string message) => Log(LogLevel.INFO, source, message);
        public void Warn(string source, string message) => Log(LogLevel.WARN, source, message);
        public void Error(string source, string message) => Log(LogLevel.ERROR, source, message);
        public void Fatal(string source, string message) => Log(LogLevel.FATAL, source, message);

        public void ResetForTests()
        {
            foreach(var sink in _sinks)
            {
                sink.ResetForTesting();
            }
            _sinks.Clear();
        }

        public void Shutdown()
        {
            foreach (var sink in _sinks)
                sink.Shutdown();
            _sinks.Clear();
        }
    }

    /*Start of new version/
    /// <summary>
    /// Thread-safe singleton logger that writes messages to daily rotating log files.
    /// </summary>
    public class Logger
    {
        private static readonly Lazy<Logger> _instance = new(() => new Logger());
        public static Logger Instance => _instance.Value;

        private readonly object _lock = new();

        private string _appName;
        private string _logDirectory;
        private string _currentDate;
        private StreamWriter _writer;

        private Logger() { }

        /// <summary>
        /// Initializes the logger and creates the log directory if needed.
        /// Log files are named:
        ///
        ///     {AppName}_YYYY-MM-DD.log
        /// </summary>
        public void Initialize(string appName, string logDirectory)
        {
            _appName = appName;
            _logDirectory = logDirectory;
            Directory.CreateDirectory(_logDirectory);
            _currentDate = "0000-00-00";
            CurrentLogFileName();
        }

        /// <summary>
        /// Opens a new log file if the calendar date has changed.
        /// </summary>
        private string CurrentLogFileName()
        {
            // If the date has changed since the last write, close the old file and open a new one
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            if (today != _currentDate)
            {
                _writer?.Dispose();
                _currentDate = today;
                string filename = Path.Combine(_logDirectory, $"{_appName}_{_currentDate}.log");
                _writer = new StreamWriter(filename, true, Encoding.UTF8);
            }
            return _currentDate;
        }

        /// <summary>
        /// Formats a log entry as:
        ///
        /// [timestamp] [level] [source] message
        /// </summary>
        private string FormatEntry(LogLevel level, string source, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string levelName = level switch
            {
                LogLevel.DEBUG => "DBG",
                LogLevel.INFO => "INF",
                LogLevel.WARN => "WRN",
                LogLevel.ERROR => "ERR",
                LogLevel.FATAL => "FTL",
                _ => "UNK"
            };
            return $"[{timestamp}] [{levelName}] [{source}] {message}";
        }

        /// <summary>
        /// Writes a log entry.
        /// Thread-safe.
        /// </summary>
        public void Log(LogLevel level, string source, string message)
        {
            lock (_lock)
            {
                // Ensure writes go to the correct daily log file.
                CurrentLogFileName();

                _writer.WriteLine(FormatEntry(level, source, message));

                // Flush immediately to reduce risk of data loss.
                _writer.Flush();
            }
        }

        public void Debug(string source, string message) =>
            Log(LogLevel.DEBUG, source, message);

        public void Info(string source, string message) =>
            Log(LogLevel.INFO, source, message);

        public void Warn(string source, string message) =>
            Log(LogLevel.WARN, source, message);

        public void Error(string source, string message) =>
            Log(LogLevel.ERROR, source, message);

        public void Fatal(string source, string message) =>
            Log(LogLevel.FATAL, source, message);

        /// <summary>
        /// Releases the current log file.
        /// Should be called during application shutdown.
        /// </summary>
        public void Shutdown()
        {
            lock (_lock)
            {
                _writer?.Dispose();
            }
        }
    }/*End of new version*/
    #endregion
}