using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace mLogger
{
    public class LogPattern
    {
        public Regex Regex { get; }
        public Color? Color { get; }
    }
    public class RichTextBoxSink : LogSinkBase
    {
        private readonly RichTextBox _textBox; 
        private readonly List<(Regex Pattern, Color Color)> _colorPatterns = new();

        private readonly ConcurrentQueue<(string Text, Color? ForeColor, Color? BackColor, FontStyle? Style, float? FontSize)> _pending = new();

        public RichTextBoxSink(RichTextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));

            _textBox.HandleCreated += (_, _) => FlushPending();
        }

        public void AddPattern(string pattern, Color color)
        {
            _colorPatterns.Add((new Regex(pattern, RegexOptions.Compiled), color));

            base.AddPattern(pattern);
        }
        public void AddSource(string source, bool andModules = true, Color color = default)
        {
            Regex pattern = CreateSourceRegex(source, andModules);
            base.AddPattern(pattern);
            _colorPatterns.Add((pattern, color));
        }
        private Color GetColor(string source)
        {
            foreach (var entry in _colorPatterns)
            {
                if (entry.Pattern.IsMatch(source))
                    return entry.Color;
            }

            return Color.Black; // _textBox.ForeColor;
        }
        public override void WriteLine(LogEntry entry)
        {
            if (!ShouldWrite(entry.Source))
                return;

            string Title = LogFormatter.FormatOneLineText(entry);
            Color TitleColor = GetColor(entry.Source);
            
            string Message = LogFormatter.FormatOneLineText(entry) + Environment.NewLine;
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
                if (_textBox.IsHandleCreated)
                {
                    FlushPending();
                }
                return;
            }

            AppendText(Title, TitleColor, Color.Empty, FontStyle.Regular, (float?)null);
            AppendText(Message, MessageColor, Color.Empty, FontStyle.Regular, (float?)null);
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
        private void AppendText(string line, Color? foreColor = null, Color? backColor = null, FontStyle fontStyle = FontStyle.Regular, float? fontSize = null)
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
}