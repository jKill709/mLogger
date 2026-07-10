using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace mLogger
{
    public class RichTextBoxSink : ILogSink
    {
        private readonly RichTextBox _textBox;

        public RichTextBoxSink(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public void Write(LogEntry entry)
        {
            string line = LogFormatter.FormatOneLineText(entry); // $"[{entry.Timestamp:HH:mm:ss}] {entry.Message}";

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(new Action(() => _textBox.AppendText(line + Environment.NewLine)));
                _textBox.BeginInvoke(new Action(() => _textBox.ScrollToCaret()));
            }
            else
            {
                _textBox.AppendText(line + Environment.NewLine);
                _textBox.ScrollToCaret();
            }
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