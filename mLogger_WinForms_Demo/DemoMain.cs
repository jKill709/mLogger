using mLogger;

namespace mLogger_WinForms_Demo
{
    public partial class DemoMain : Form
    {
        Logger logger = null;

        RichTextBoxSink mainTBsink;

        public DemoMain()
        {
            InitializeComponent();

            logger = Logger.Instance;

            mainTBsink = new RichTextBoxSink(MainTBsink_Box);
        }

        private async void AddSource()
        {
            if (string.IsNullOrWhiteSpace(Source_Box.Text))
            {
                Color previousTB = Source_Box.BackColor;
                Color previousButton = AddSource_Button.BackColor;

                Source_Box.BackColor = Color.Red;
                AddSource_Button.BackColor = Color.Red;
                await Task.Delay(200);

                Source_Box.BackColor = Color.Pink;
                Source_Box.ForeColor = previousTB;
                await Task.Delay(50);

                Source_Box.BackColor = previousTB;
                Source_Box.ForeColor = previousButton;
                return;
            }

            using (ColorDialog dialog = new ColorDialog())
            {
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                mainTBsink.AddSource(Source_Box.Text, false, dialog.Color);

                Source_Box.Items.Add(Source_Box.Text);

                ListViewItem item = new ListViewItem(Source_Box.Text)
                {
                    BackColor = dialog.Color
                };
                Sources_ListBox.Items.Add(item);
            }
        }
        private async void RemoveSource()
        {
            if (string.IsNullOrWhiteSpace(Source_Box.Text))
            {
                Color previousTB = Source_Box.BackColor;
                Color previousButton = RemoveSource_Button.BackColor;

                Source_Box.BackColor = Color.Red;
                RemoveSource_Button.BackColor = Color.Red;
                await Task.Delay(200);

                Source_Box.BackColor = Color.Pink;
                RemoveSource_Button.BackColor = Color.Pink;
                await Task.Delay(50);

                Source_Box.BackColor = previousTB;
                RemoveSource_Button.BackColor = previousButton;
                return;
            }

            // Remove from the sink
            mainTBsink.RemoveSource(Source_Box.Text, false);

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
        }
        private void AddSource_Button_Click(object sender, EventArgs e)
        {
            AddSource();
        }

        private void RemoveSource_Button_Click(object sender, EventArgs e)
        {
            RemoveSource();  // Did not remove the first item, only the second.  Needs debug.
        }
    }
}
