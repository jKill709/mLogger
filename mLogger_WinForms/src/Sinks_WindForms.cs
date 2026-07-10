using System;
using System.Windows.Forms;

namespace mLogger
{
    public class TextBoxSink : ILogSink
    {
        private readonly TextBox _textBox;

        public TextBoxSink(TextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public void Write(LogEntry entry)
        {
            string line =
                $"[{entry.Timestamp:HH:mm:ss}] {entry.Message}";

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(new Action(() =>
                    _textBox.AppendText(line + Environment.NewLine)));
            }
            else
            {
                _textBox.AppendText(line + Environment.NewLine);
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
            // No resources to release for TextBoxSink
        }
    }
}