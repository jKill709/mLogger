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
            if (_textBox.IsDisposed || !_textBox.IsHandleCreated)
                return;

            string line = LogFormatter.FormatOneLineText(entry);

            _textBox.BeginInvoke(new Action(() =>
            {
                if (_textBox.IsDisposed)
                    return;

                _textBox.AppendText(line + Environment.NewLine);
                _textBox.ScrollToCaret();
            }));
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