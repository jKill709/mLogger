using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using jColorProviders;

using static System.Windows.Forms.LinkLabel;

namespace mLogger
{
    public class RichTextBoxSink : LogSinkBase, IColoredLogSink
    {
        private readonly FixedHashColorProvider _hashColorProvider;
        private readonly RichTextBox _textBox;
        private readonly RegexColorProvider _regexColors;
        private readonly TextColorProvider _textColors;
        //private readonly List<(Regex Pattern, Color Color)> _colorPatterns = new();

        private readonly ConcurrentQueue<(string Text, Color? ForeColor, Color? BackColor, FontStyle? Style, float? FontSize)> _pending = new();
        
        public RichTextBoxSink(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

            _textBox.HandleCreated += (_, _) => FlushPending();

            _hashColorProvider = new FixedHashColorProvider();
            _regexColors = new RegexColorProvider();
            _textColors = new TextColorProvider();
        }
        public RichTextBoxSink(LogSinkBase other, RichTextBox textBox) : base(other)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

            _textBox.HandleCreated += (_, _) => FlushPending();

            // If it's really a RichTextBoxSink, copy the rest
            if (other is RichTextBoxSink cs)
            {
                _regexColors = new RegexColorProvider(cs._regexColors);
            }
            else
            {
                _regexColors = new RegexColorProvider();
            }
            _hashColorProvider = new FixedHashColorProvider();
            _textColors = new TextColorProvider();
        }

        public new void AddPattern(Regex pattern)
        {
            AddPattern(pattern, _hashColorProvider.GetColor(pattern.ToString()));
        }
        public void AddPattern(Regex pattern, Color color)
        {
            _regexColors.AddPattern(pattern, color);

            base.AddPattern(pattern);
        }
        public new void AddPattern(string pattern)
        {
            AddPattern(new Regex(pattern, RegexOptions.Compiled));
        }
        public void AddPattern(string pattern, Color color)
        {
            AddPattern(new Regex(pattern, RegexOptions.Compiled), color);
        }
        public new void RemovePattern(string pattern)
        {
            _regexColors.RemovePattern(new Regex(pattern, RegexOptions.Compiled));

            base.RemovePattern(pattern);
        }
        public new void AddSource(string source, bool andModules = true)
        {
            Color color = _hashColorProvider.GetColor(source);
            AddSource(source, color, andModules);
        }
        public void AddSource(string source, Color color, bool andModules = true)
        {
            Regex pattern = CreateSourceRegex(source, andModules);
            base.AddPattern(pattern);
            //if (color == default)
            //    color = _hashColorProvider.GetColor(source);
            _regexColors.AddPattern(pattern, color);
        }
        public new void RemoveSource(string source, bool andModules = true)
        {
            Regex pattern = CreateSourceRegex(source, andModules);
            base.RemovePattern(pattern);
            _regexColors.RemovePattern(pattern);
        }

        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            string Title = LogFormatter.FormatOneLineText(entry, true, true, true, false);
            Color TitleColor = _regexColors.GetColor(entry.Source);

            string Message = LogFormatter.FormatOneLineText(entry, false, false, false, true);
            Color MessageColor = entry.Level switch
            {
                LogLevel.DEBUG => Color.Blue,
                LogLevel.INFO => Color.Black,
                LogLevel.WARN => Color.Orange,
                LogLevel.ERROR => Color.Red,
                LogLevel.FATAL => Color.DarkRed,
                _ => Color.Green
            };

            if (!_textBox.IsHandleCreated)
            {
                _pending.Enqueue((Title, TitleColor, Color.Empty, FontStyle.Regular, (float?)null));
                _pending.Enqueue((Message, MessageColor, Color.Empty, FontStyle.Regular, (float?)null));
                _pending.Enqueue((Environment.NewLine, Color.Empty, Color.Empty, FontStyle.Regular, (float?)null));
                if (_textBox.IsHandleCreated)
                {
                    FlushPending();
                }
                return;
            }

            AppendText(Title, TitleColor, Color.Empty, FontStyle.Regular, (float?)null);
            AppendText(Message, MessageColor, Color.Empty, FontStyle.Regular, (float?)null);
            AppendText(Environment.NewLine, Color.Empty, Color.Empty, FontStyle.Regular, (float?)null);
        }
        public override void WriteHeading(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            string Title = LogFormatter.FormatOneLineText(entry, true, true, true, false);
            Color TitleColor = _regexColors.GetColor(entry.Source);

            string Message = LogFormatter.FormatOneLineText(entry, false, false, false, true);
            Color MessageColor = entry.Level switch
            {
                LogLevel.DEBUG => Color.Blue,
                LogLevel.INFO => Color.Black,
                LogLevel.WARN => Color.Orange,
                LogLevel.ERROR => Color.Red,
                LogLevel.FATAL => Color.DarkRed,
                _ => Color.Green
            };

            if (!_textBox.IsHandleCreated)
            {
                _pending.Enqueue((Title, _textColors.GetColor(TitleColor), TitleColor, FontStyle.Regular, (float?)null));
                _pending.Enqueue((Message, MessageColor, Color.Empty, FontStyle.Bold, (float?)null));
                _pending.Enqueue((Environment.NewLine, Color.Empty, Color.Empty, FontStyle.Bold, (float?)null));
                if (_textBox.IsHandleCreated)
                {
                    FlushPending();
                }
                return;
            }

            AppendText(Title, _textColors.GetColor(TitleColor), TitleColor, FontStyle.Regular, (float?)null);
            AppendText(Message, MessageColor, Color.Empty, FontStyle.Bold, (float?)null);
            AppendText(Environment.NewLine, Color.Empty, Color.Empty, FontStyle.Bold, (float?)null);
        }
        public override void WriteSeparator(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            AppendLine(new string('-', 120));
        }
        private void AppendText(string line, Color? foreColor = null, Color? backColor = null, FontStyle fontStyle = FontStyle.Regular, float? fontSize = null)
        {
            // Check if the RichTextBox is disposed before proceeding
            if (_textBox.IsDisposed)
                return;

            if (_textBox.InvokeRequired)
            {
                _textBox.BeginInvoke(() => AppendText(line, foreColor, backColor, fontStyle, fontSize));
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

            _textBox.AppendText(line);

            // Restore formatting
            _textBox.SelectionColor = originalForeColor;
            _textBox.SelectionBackColor = originalBackColor;
            _textBox.SelectionFont = originalFont;
        }
        public void AppendLine(string line)
        {
            AppendText(line + Environment.NewLine);

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
                if (line.ForeColor == null || line.ForeColor == Color.Empty)
                    line.ForeColor = _textBox.ForeColor;
                if (line.BackColor == null || line.BackColor == Color.Empty)
                    line.BackColor = _textBox.BackColor;
                if (line.Style == null)
                    line.Style = FontStyle.Regular;
                if (line.FontSize == null)
                    line.FontSize = _textBox.Font.Size; 

                AppendText(line.Text, line.ForeColor, line.BackColor, (FontStyle)line.Style, line.FontSize);
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

    public class RichTextBoxWindowSink : LogSinkBase
    {
        private readonly Form _form;
        private readonly RichTextBox _textBox;
        private readonly RichTextBoxSink _sink;

        public RichTextBoxWindowSink(string title = "Logging Output", int width = 800, int height = 600)
        {
            _form = new Form()
            {
                Text = title,
                Width = width,
                Height = height,
                StartPosition = FormStartPosition.CenterScreen
            };

            _textBox = new RichTextBox()
            {
                ReadOnly = true,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                Dock = DockStyle.Fill
            };

            _form.Controls.Add(_textBox);

            // Do not allow closing the logging window
            // to terminate the application.
            _form.FormClosing += (_, e) =>
            {
                e.Cancel = true;
                _form.Hide();
            };

            _sink = new RichTextBoxSink(_textBox);
        }
        public RichTextBoxWindowSink(RichTextBoxSink previousSink, string title = "Logging Output", int width = 800, int height = 600)
        {
            _form = new Form()
            {
                Text = title,
                Width = width,
                Height = height,
                StartPosition = FormStartPosition.CenterScreen
            };

            _textBox = new RichTextBox()
            {
                ReadOnly = true,
                Multiline = true,
                ScrollBars = RichTextBoxScrollBars.Both,
                WordWrap = false,
                Dock = DockStyle.Fill
            };

            _form.Controls.Add(_textBox);

            // Do not allow closing the logging window
            // to terminate the application.
            _form.FormClosing += (_, e) =>
            {
                e.Cancel = true;
                _form.Hide();
            };

            _sink = new RichTextBoxSink(previousSink, _textBox);
        }

        public void Show()
        {
            if (_form.InvokeRequired)
            {
                _form.BeginInvoke(Show);
                return;
            }

            _form.Show();
        }


        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            _sink.WriteLine(entry);
        }


        public override void WriteHeading(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            _sink.WriteHeading(entry);
        }


        public override void WriteSeparator(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            _sink.WriteSeparator(entry);
        }


        public override void ResetForTesting()
        {
            _sink.ResetForTesting();
        }


        public override void Shutdown()
        {
            if (_form.InvokeRequired)
            {
                _form.BeginInvoke(new Action(() =>
                {
                    _form.Dispose();
                }));

                return;
            }

            _form.Dispose();
        }
    }
}