using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace mLogger
{
    public class RichTextBoxSink : ILogSink
    {
        private readonly RichTextBox _textBox;
        private readonly ConcurrentQueue<string> _pending = new();

        public RichTextBoxSink(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

            _textBox.HandleCreated += (_, _) => FlushPending();
        }

        public void WriteLine(LogEntry entry)
        {
            if (_textBox.IsDisposed)
                return;

            string line = LogFormatter.FormatOneLineText(entry);

            if (!_textBox.IsHandleCreated)
            {
                _pending.Enqueue(line);
                if (_textBox.IsHandleCreated)
                {
                    FlushPending();
                }
                return;
            }

            AppendLine(line);
        }
        public void WriteHeading(LogEntry entry)
        {
            if (_textBox.IsDisposed)
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

            AppendLine(LogFormatter.FormatOneLineText(firstLineEntry));
            AppendLine(leadingPadding + partialSeperator + innerPadding + entry.Message + innerPadding + partialSeperator);
            AppendLine(leadingPadding + fullSeperator);
        }
        private void AppendLine(string line)
        {
            if (_textBox.IsDisposed)
                return;

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(() => AppendLine(line));
                return;
            }

            _textBox.AppendText(line + Environment.NewLine);
            _textBox.ScrollToCaret();
        }
        private void FlushPending()
        {
            if (_textBox.IsDisposed)
                return;

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(FlushPending);
                return;
            }

            while (_pending.TryDequeue(out var line))
            {
                _textBox.AppendText(line + Environment.NewLine);
            }

            _textBox.SelectionStart = _textBox.TextLength;
            _textBox.ScrollToCaret();
        }

        public void ResetForTesting()
        {
            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(new Action(() =>
                    _textBox.Clear()));
            }
            else
            {
                _textBox.Clear();
            }
        }

        public void Shutdown()
        {
            // No resources to release for RichTextBoxSink
        }
    }
}