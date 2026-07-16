using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace mLogger
{
    public class RichTextBoxSink : LogSinkBase
    {
        private readonly RichTextBox _textBox;
        private readonly ConcurrentQueue<string> _pending = new();

        public RichTextBoxSink(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

            _textBox.HandleCreated += (_, _) => FlushPending();
        }

        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
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
            LogEntry firstLineEntry = new LogEntry { Timestamp = entry.Timestamp,
                                                      Level = entry.Level,
                                                      Source = entry.Source,
                                                      Message = fullSeperator };

            AppendLine(LogFormatter.FormatOneLineText(firstLineEntry));
            AppendLine(leadingPadding + partialSeperator + innerPadding + entry.Message + innerPadding + partialSeperator);
            AppendLine(leadingPadding + fullSeperator);
        }
        public override void WriteSeperator(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            AppendLine(new string('-', 120));
        }
        private void AppendLine(string line, Color? foreColor = null, Color? backColor = null, FontStyle fontStyle = FontStyle.Regular, float? fontSize = null)
        {
            // Check if the RichTextBox is disposed before proceeding
            if (_textBox.IsDisposed)
                return;

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(() => AppendLine(line));
                return;
            }

            // Save current formatting
            Color originalForeColor = _textBox.SelectionColor;
            Color originalBackColor = _textBox.SelectionBackColor;
            Font originalFont = _textBox.SelectionFont ?? _textBox.Font;

            // Apply requested formatting
            _textBox.SelectionColor = foreColor ?? _textBox.ForeColor;
            _textBox.SelectionBackColor = backColor ?? _textBox.BackColor;

            float size = fontSize ?? originalFont.Size;
            _textBox.SelectionFont = new Font(
                originalFont.FontFamily,
                size,
                fontStyle);

            _textBox.AppendText(line + Environment.NewLine);

            // Restore formatting
            _textBox.SelectionColor = originalForeColor;
            _textBox.SelectionBackColor = originalBackColor;
            _textBox.SelectionFont = originalFont;


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

        public override void ResetForTesting()
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

        public override void Shutdown()
        {
            // No resources to release for RichTextBoxSink
        }
    }
}