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
        public void Constructor_StoresRichTextBox()
        {
            var tb = new RichTextBox();

            var sink = new RichTextBoxSink(tb);

            Assert.IsNotNull(sink);
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WhenRichTextBoxIsNull()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new RichTextBoxSink(null));
        }

        [TestMethod]
        public void Write_AppendsOneLine()
        {
            RichTextBox tb = new RichTextBox();
            RichTextBoxSink sink = new RichTextBoxSink(tb);
            LogEntry entry = new LogEntry { Timestamp = DateTime.Now,
                                            Level = LogLevel.DEBUG,
                                            Source = "Unit Tests",
                                            Message = "Test message" };

            sink.WriteLine(entry);

            Assert.IsNotNull(tb.Text);
        }

        [TestMethod]
        public void Write_AppendsSource()
        {
            RichTextBox tb = new RichTextBox();
            RichTextBoxSink sink = new RichTextBoxSink(tb);
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Test message"
            };

            // Force the handle to be created
            using Form form = new();
            form.Controls.Add(tb);
            var handle = tb.Handle;

            sink.WriteLine(entry);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry.Source, tb.Text);
        }

        [TestMethod]
        public void Write_AppendsMessage()
        {
            RichTextBox tb = new RichTextBox();
            RichTextBoxSink sink = new RichTextBoxSink(tb);
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Test message"
            };

            // Force the handle to be created
            using Form form = new();
            form.Controls.Add(tb);
            var handle = tb.Handle;

            sink.WriteLine(entry);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry.Message, tb.Text);
        }

        [TestMethod]
        public void Write_AppendsMultipleLines()
        {
            RichTextBox tb = new RichTextBox();
            RichTextBoxSink sink = new RichTextBoxSink(tb);
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


            // Force the handle to be created
            using Form form = new();
            form.Controls.Add(tb);
            var handle = tb.Handle;

            sink.WriteLine(entry1);
            sink.WriteLine(entry2);
            sink.WriteLine(entry3);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry1.Message, tb.Text);
            Assert.Contains(entry2.Message, tb.Text);
            Assert.Contains(entry3.Message, tb.Text);
        }

        [TestMethod]
        public void ResetForTesting_ClearsText()
        {
            RichTextBox tb = new RichTextBox();
            RichTextBoxSink sink = new RichTextBoxSink(tb);
            LogEntry entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = LogLevel.DEBUG,
                Source = "Unit Tests",
                Message = "Test message"
            };

            // Force the handle to be created
            using Form form = new();
            form.Controls.Add(tb);
            var handle = tb.Handle;

            sink.WriteLine(entry);

            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry.Message, tb.Text);

            sink.ResetForTesting();

            Assert.IsNotNull(tb.Text);
            Assert.IsEmpty(tb.Text);
        }

        [TestMethod]
        public void PendingQueue_DequeuesPoroperly()
        {
            RichTextBox tb = new RichTextBox();
            RichTextBoxSink sink = new RichTextBoxSink(tb);
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

            sink.WriteLine(entry1);
            sink.WriteLine(entry2);
            sink.WriteLine(entry3);

            // At this point, the RichTextBox handle has not been created, so the messages should not be in the RichTextBox yet
            Assert.DoesNotContain(entry1.Message, tb.Text);
            Assert.DoesNotContain(entry2.Message, tb.Text);
            Assert.DoesNotContain(entry3.Message, tb.Text);

            // Force the handle to be created
            using Form form = new();
            form.Controls.Add(tb);
            var handle = tb.Handle;

            // Now the messages should be in the RichTextBox
            Assert.IsNotNull(tb.Text);
            Assert.Contains(entry1.Message, tb.Text);
            Assert.Contains(entry2.Message, tb.Text);
            Assert.Contains(entry3.Message, tb.Text);
        }

    }
}
