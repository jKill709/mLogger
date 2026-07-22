using jColorProviders;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.Logging;
using mLogger;
using System.Configuration;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace mLogger_WinForms_Demo
{
    public partial class DemoMain : Form
    {
        Logger logger = null;

        RichTextBoxSink _mainTBsink;
        TextFileSink? _textFileSink;
        ConsoleSink? _consoleSink;
        InMemorySink? _memorySink;
        RichTextBoxWindowSink? _richTextBoxWindowSink;
        private bool _textFileAllocated = false;
        private bool _memoryAllocated = false;
        private bool _consoleAllocated = false;
        private bool _richTextBoxWindowAllocated = false;

        List<LogSinkBase> _activeSinks;

        FixedHashColorProvider sourceColors = new FixedHashColorProvider();
        TextColorProvider textColors = new TextColorProvider();

        RandomStringProvider rspMessages = new RandomStringProvider(new[] { "Application started successfully.",
                                                                            "User authentication completed for user 'admin'.",
                                                                            "Unable to connect to remote server. Retrying connection attempt 3 of 5.",
                                                                            "Database connection failed. Exception: Timeout expired after 30000ms.",
                                                                            "Processing packet ID=0x4A2F, Length=128 bytes, Source=192.168.1.42",
                                                                            "System integrity check failed. Shutting down immediately.",
                                                                            "The quick brown fox jumps over the lazy dog.",
                                                                            "Unicode test: Café résumé naïve — 中文测试 — 日本語テスト — 🚀🔥✅",
                                                                            "Empty-like test with spaces:      ",
                                                                            "Very long message test: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum vulputate, nisl at tincidunt tincidunt, justo sapien facilisis erat, sed malesuada neque odio non libero. Integer faucibus consequat turpis,a feugiat metus viverra at",
                                                                            "Threading test: Message generated from worker thread ID 17 while UI thread is active.",
                                                                            "Shutdown test: Application shutdown requested.Flushing remaining log messages." });
        RandomStringProvider rspSourcves = new RandomStringProvider(new[] { "Camera",
                                                                            "CameraManager",
                                                                            "Camera.Capture",
                                                                            "Camera.Calibration",
                                                                            "Camera.Calibration.Charuco",
                                                                            "Logger",
                                                                            "Logger.FileSink",
                                                                            "Logger.ConsoleSink",
                                                                            "Network",
                                                                            "Network.MQTT",
                                                                            "Network.MQTT.Connection",
                                                                            "Database",
                                                                            "Database.SQL.QueryProcessor",
                                                                            "UI.MainWindow",
                                                                            "UI.SettingsDialog",
                                                                            "WorkerThread",
                                                                            "BackgroundWorker",
                                                                            "SystemMonitor",
                                                                            "Hardware.GPIO",
                                                                            "Hardware.GPIO.PiCamera",
                                                                            "Test.Source",
                                                                            "UnitTest.LoggerTests",
                                                                            "DEBUG",
                                                                            "ERROR_HANDLER",
                                                                            "FatalExceptionHandler",
                                                                            "Startup",
                                                                            "Shutdown",
                                                                            "Module_01",
                                                                            "Module_02.Sensor",
                                                                            "User.Authentication",
                                                                            "User.Authentication.TokenValidator" });

        public DemoMain()
        {
            InitializeComponent();

            logger = Logger.Instance;
            logger.Initialize("Demo");

            _activeSinks = new List<LogSinkBase>();

            _mainTBsink = new RichTextBoxSink(MainTBsink_Box);
            _mainTBsink.AddSource("Demo");

            logger.AddSink(_mainTBsink);
            _activeSinks.Add(_mainTBsink);

            logger.LogHeading(LogLevel.INFO, "Demo", "Welcome to the mLogger Demo");
            logger.Log(LogLevel.INFO, "Demo", "You may enter log entries, add sources or experiement with various sinks.");
            logger.Log(LogLevel.INFO, "Demo", "All the sinks in this demo are wired together for simplicity, but each sink has it's own regex list, whitelist/blacklist config color associations and can be configure independently.");
        }

        private async void AddSource()
        {
            List<Control> controls = new List<Control>();

            if (string.IsNullOrWhiteSpace(Source_Box.Text))
            {
                controls.Add(Source_Box);
                controls.Add(AddSource_Button);

                FlashControls(controls);
                return;
            }

            AddSource(Source_Box.Text);
        }
        private async void AddSource(string source)
        {
            // If the new source is already in the list (Sources_ListBox.Items) then return
            if (Sources_ListBox.Items.Cast<ListViewItem>().Any(item => item.Text.Equals(source, StringComparison.Ordinal)))
                return;

            Color color = sourceColors.GetColor(source);
            foreach (LogSinkBase sink in _activeSinks)
            {
                if (sink is RichTextBoxSink cs)
                {
                    cs.AddSource(source, false);
                }
                else
                {
                    sink.AddSource(source, false);
                }
            }

            if (source != "Demo")
            {
                Source_Box.Items.Add(source);

                ListViewItem item = new ListViewItem(source)
                {
                    ForeColor = textColors.GetColor(color),
                    BackColor = color
                };
                Sources_ListBox.Items.Add(item);

                logger.Log(LogLevel.INFO, "Demo", $"Add new source: {Source_Box.Text}");
            }
        }
        private void RemoveSource()
        {
            List<Control> controls = new List<Control>();

            if (string.IsNullOrWhiteSpace(Source_Box.Text))
            {
                controls.Add(Source_Box);
                controls.Add(RemoveSource_Button);

                FlashControls(controls);
                return;
            }

            // Remove from the sink
            _mainTBsink.RemoveSource(Source_Box.Text, false);

            // Remove from the ComboBox
            Source_Box.Items.Remove(Source_Box.Text);

            // Remove from the ListView
            foreach (ListViewItem item in Sources_ListBox.Items)
            {
                if (item.Text.Equals(Source_Box.Text, StringComparison.Ordinal))
                {
                    Sources_ListBox.Items.Remove(item);
                    break;
                }
            }

            logger.Log(LogLevel.INFO, "Demo", $"Removed source: {Source_Box.Text}");

            Source_Box.Text = "";
        }
        private async void FlashControls(List<Control> controls)
        {

            ChangeControlColor(controls, Color.Pink);
            await Task.Delay(100);

            ChangeControlColor(controls, Color.Red);
            await Task.Delay(200);

            ChangeControlColor(controls, Color.Pink);
            await Task.Delay(100);

            ChangeControlColor(controls, null);
            return;
        }
        private void ChangeControlColor(List<Control> controls, Color? color)
        {
            foreach (Control control in controls)
            {
                if (color == null)
                {
                    if (control is System.Windows.Forms.TextBox || control is System.Windows.Forms.ComboBox)
                        control.BackColor = SystemColors.Window;
                    else
                        control.BackColor = SystemColors.Control;
                }
                else
                {
                    control.BackColor = (Color)color;
                }
            }
        }

        private void AddSource_Button_Click(object sender, EventArgs e)
        {
            AddSource();
        }
        private void RemoveSource_Button_Click(object sender, EventArgs e)
        {
            RemoveSource();  // Did not remove the first item, only the second.  Needs debug.
        }
        private void RandomMessage_Button_Click(object sender, EventArgs e)
        {
            Message_Box.Text = rspMessages.GetString();
        }
        private void RandomSource_Button_Click(object sender, EventArgs e)
        {
            Source_Box.Text = rspSourcves.GetString();
        }
        private void LogEntry_Button_Click(object sender, EventArgs e)
        {
            List<Control> controls = new List<Control>();

            if (string.IsNullOrWhiteSpace(Source_Box.Text))
                controls.Add(Source_Box);
            if (string.IsNullOrWhiteSpace(Message_Box.Text))
                controls.Add(Message_Box);
            if (!FATAL_Option.Checked && !ERR_Option.Checked && !WARN_Option.Checked && !INFO_Option.Checked && !Debug_Option.Checked)
                controls.AddRange(new Control[] { FATAL_Option,
                                                  ERR_Option,
                                                  WARN_Option,
                                                  INFO_Option,
                                                  Debug_Option });

            if (controls.Count > 0)
            {
                controls.Add(LogEntry_Button);
                FlashControls(controls);
                return;
            }


            LogLevel level;
            if (FATAL_Option.Checked)
                level = LogLevel.FATAL;
            else if (ERR_Option.Checked)
                level = LogLevel.ERROR;
            else if (WARN_Option.Checked)
                level = LogLevel.WARN;
            else if (INFO_Option.Checked)
                level = LogLevel.INFO;
            else if (Debug_Option.Checked)
                level = LogLevel.DEBUG;
            else
                throw new ArgumentNullException(nameof(LogLevel_Box));


            logger.Log(level, Source_Box.Text, Message_Box.Text);
        }
        private void LogHeadingButton_Click(object sender, EventArgs e)
        {
            List<Control> controls = new List<Control>();

            if (string.IsNullOrWhiteSpace(Source_Box.Text))
                controls.Add(Source_Box);
            if (string.IsNullOrWhiteSpace(Message_Box.Text))
                controls.Add(Message_Box);
            if (!FATAL_Option.Checked && !ERR_Option.Checked && !WARN_Option.Checked && !INFO_Option.Checked && !Debug_Option.Checked)
                controls.AddRange(new Control[] { FATAL_Option,
                                                  ERR_Option,
                                                  WARN_Option,
                                                  INFO_Option,
                                                  Debug_Option });

            if (controls.Count > 0)
            {
                controls.Add(LogHeading_Button);
                FlashControls(controls);
                return;
            }


            LogLevel level;
            if (FATAL_Option.Checked)
                level = LogLevel.FATAL;
            else if (ERR_Option.Checked)
                level = LogLevel.ERROR;
            else if (WARN_Option.Checked)
                level = LogLevel.WARN;
            else if (INFO_Option.Checked)
                level = LogLevel.INFO;
            else if (Debug_Option.Checked)
                level = LogLevel.DEBUG;
            else
                throw new ArgumentNullException(nameof(LogLevel_Box));


            logger.LogHeading(level, Source_Box.Text, Message_Box.Text);
        }
        private void ConsoleSink_Button_Click(object sender, EventArgs e)
        {
            if (!_consoleAllocated)
            {
                ConsoleManager.AllocConsole();
                _consoleAllocated = true;
            }

            if (_consoleSink == null)
            {
                _consoleSink = new ConsoleSink();
                logger.AddSink(_consoleSink);
                _activeSinks.Add(_mainTBsink);
            }

            logger.Log(LogLevel.INFO, "Demo", "ConsoleSink Opened.  Closing the console will exit the application");
        }

        internal static class ConsoleManager
        {
            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeConsole();
        }

        private void MemorySink_Button_Click(object sender, EventArgs e)
        {
            if (!_memoryAllocated)
            {
                _memorySink = new InMemorySink();

                logger.AddSink(_memorySink);
                _activeSinks.Add(_mainTBsink);
            }
            logger.Log(LogLevel.INFO, "Demo", "InMemborySink is running.  Use debug features to inspect contents.");
        }
        private void TextFileSink_Button_Click(object sender, EventArgs e)
        {
            if (!_textFileAllocated)
            {
                _textFileSink = new TextFileSink(Path.GetTempPath(), "Demo");

                logger.AddSink(_textFileSink);
                _activeSinks.Add(_mainTBsink);
                _textFileAllocated = true;
            }

            logger.Log(LogLevel.INFO, "Demo", "TextFileSink is active.  Saving log to file: " + _textFileSink.FilePath);
        }
        private void RichTextWindow_Button_Click(object sender, EventArgs e)
        {
            if (!_richTextBoxWindowAllocated)
            {
                _richTextBoxWindowSink = new RichTextBoxWindowSink();
                logger.AddSink(_richTextBoxWindowSink);
                _activeSinks.Add(_mainTBsink);
                _richTextBoxWindowSink.Show();

                _richTextBoxWindowAllocated = true;
            }

            logger.Log(LogLevel.INFO, "Demo", "RichTextBoxWindowSink opened");
        }

        private void Whitelist_Box_CheckedChanged(object sender, EventArgs e)
        {
            if (Whitelist_Box.Checked)
            {
                Blacklist_Box.Checked = false;
                logger.Log(LogLevel.INFO, "Demo", "Whitelist is active.  Only sources in the list will be sent to sinks.");

                foreach (LogSinkBase sink in _activeSinks)
                {
                    sink.useList = true;
                    sink.isBlacklist = false;
                }
            }
            else
            {
                logger.Log(LogLevel.INFO, "Demo", "No list is active.  All sources will be sent to sinks.");

                foreach (LogSinkBase sink in _activeSinks)
                {
                    sink.useList = false;
                }
            }
        }
        private void Blacklist_Box_CheckedChanged(object sender, EventArgs e)
        {
            if (Blacklist_Box.Checked)
            {
                Whitelist_Box.Checked = false;
                logger.Log(LogLevel.INFO, "Demo", "Blacklist is active.  Sources in the list will not be sent to sinks.");

                foreach (LogSinkBase sink in _activeSinks)
                {
                    sink.RemoveSource("Demo");  // So demo messages still get through
                    
                    sink.useList = true;
                    sink.isBlacklist = true;
                }
            }
            else
            {
                logger.Log(LogLevel.INFO, "Demo", "No list is  again.  All sources will be sent to sinks.");
             
                foreach (LogSinkBase sink in _activeSinks)
                {
                    sink.useList = false;
                    sink.AddSource("Demo");  // So demo messages get through again
                }
            }
        }
    }
}
