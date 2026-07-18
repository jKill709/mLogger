// SPDX-License-Identifier: MIT
// Copyright (c) 2026 Jeremy Killinger

using System;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace mLogger
{
    #region Enums and Helpers
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
    public static class LogFormatter
    {
        public static string FormatTimestamp(LogEntry entry)
        {
            return FormatTimestamp(entry.Timestamp);
        }
        public static string FormatTimestamp(DateTime timestamp)
        {
            return timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }
        public static string FormatLogLevel(LogEntry entry)
        {
            return FormatLogLevel(entry.Level);
        }
        public static string FormatLogLevel(LogLevel level) {
            return level switch
            {
                LogLevel.DEBUG => "DBG",
                LogLevel.INFO => "INF",
                LogLevel.WARN => "WRN",
                LogLevel.ERROR => "ERR",
                LogLevel.FATAL => "FTL",
                _ => "UNK"
            };
        }
        public static string FormatOneLineText(LogEntry entry, bool showTimestamp = true, bool showLevel = true, bool showSource = true, bool showMessage = true)
        {
            string timestamp = FormatTimestamp(entry);
            string levelName = FormatLogLevel(entry);

            string returnValue = string.Empty;

            if (showTimestamp)
            {
                returnValue += $"[{timestamp}]";
            }
            if (showLevel)
            {
                if (returnValue.Length > 0)
                    returnValue += " ";
                returnValue += $"[{levelName}]";
            }
            if (showSource)
            {
                if (returnValue.Length > 0)
                    returnValue += " ";
                returnValue += $" [{entry.Source}]";
            }
            if (showMessage)
            {
                if (returnValue.Length > 0)
                    returnValue += " ";
                returnValue += $" {entry.Message ?? "null"}";
            }
            return returnValue;
        }
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
        private readonly List<LogSinkBase> _sinks = new();

        private Logger() { }

        public void Initialize(string appName)
        {
            _appName = appName;
            _isInitialized = true;
        }

        public void AddSink(LogSinkBase sink)
        {
            ArgumentNullException.ThrowIfNull(sink);
            _sinks.Add(sink);
        }
        public void RemoveSink(LogSinkBase sink)
        {
            ArgumentNullException.ThrowIfNull(sink);

            lock (_lock)
            {
                if (_sinks.Remove(sink))
                {
                    sink.Shutdown();
                }
            }
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
                sink.WriteLine(entry);
            }
        }
        public void LogHeading(LogLevel level, string source, string message)
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
                sink.WriteHeading(entry);
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
    #endregion
}