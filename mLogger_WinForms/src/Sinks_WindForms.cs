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

        public void Write(LogEntry entry)
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