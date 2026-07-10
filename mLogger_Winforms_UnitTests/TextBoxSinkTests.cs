using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using mLogger;
using Mono.Cecil;
using static Microsoft.TestPlatform.AdapterUtilities.HierarchyConstants;

namespace mLogger_Winforms_UnitTests
{
    [TestClass]
    public sealed class mLogger_Winforms_UnitTests
    {
        [TestMethod]
        public void Constructor_StoresTextBox()
        {
            var tb = new TextBox();

            var sink = new TextBoxSink(tb);

            Assert.IsNotNull(sink);
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenTextBoxIsNull()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new TextBoxSink(null));
        }

        [TestMethod]
        public void Write_AppendsOneLine()
        {
            TextBox tb = new TextBox();
            TextBoxSink sink = new TextBoxSink(tb);
            LogEntry entry = new LogEntry { Timestamp = DateTime.Now,
                                            Level = LogLevel.DEBUG,
                                            Source = "Unit Tests",
                                            Message = "Test message" };

            sink.Write(entry);

            Assert.IsNotNull(tb.Text);
        }

        [TestMethod]
        public void Write_AppendsSource()
        {
            TextBox tb = new TextBox();
            TextBoxSink sink = new TextBoxSink(tb);
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Test message"
            };

            sink.Write(entry);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry.Source, tb.Text);
        }

        [TestMethod]
        public void Write_AppendsMessage()
        {
            TextBox tb = new TextBox();
            TextBoxSink sink = new TextBoxSink(tb);
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Test message"
            };

            sink.Write(entry);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry.Message, tb.Text);
        }

        [TestMethod]
        public void Write_AppendsMultipleLines()
        {
            TextBox tb = new TextBox();
            TextBoxSink sink = new TextBoxSink(tb);
            LogEntry entry1 = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "This Message"
            };
            LogEntry entry2 = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "That Text"
            }; LogEntry entry3 = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Another Thing"
            };

            sink.Write(entry1);
            sink.Write(entry2);
            sink.Write(entry3);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry1.Message, tb.Text);
            Assert.Contains(entry2.Message, tb.Text);
            Assert.Contains(entry3.Message, tb.Text);
        }

        [TestMethod]
        public void ResetForTesting_ClearsText()
        {
            TextBox tb = new TextBox();
            TextBoxSink sink = new TextBoxSink(tb);
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Test message"
            };

            sink.Write(entry);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry.Message, tb.Text);

            sink.ResetForTesting();

            Assert.IsNotNull(tb.Text);
            Assert.IsEmpty(tb.Text);
        }
    }
}
