namespace mLogger_WinForms_Demo
{
    partial class DemoMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            MainTBsink_Box = new RichTextBox();
            panel1 = new Panel();
            LogHeading_Button = new Button();
            LogEntry_Button = new Button();
            RemoveSource_Button = new Button();
            AddSource_Button = new Button();
            Message_Label = new Label();
            RandomSource_Button = new Button();
            RandomMessage_Button = new Button();
            Source_Label = new Label();
            LogLevel_Box = new GroupBox();
            FATAL_Option = new RadioButton();
            ERR_Option = new RadioButton();
            WARN_Option = new RadioButton();
            INFO_Option = new RadioButton();
            Debug_Option = new RadioButton();
            Message_Box = new TextBox();
            Source_Box = new ComboBox();
            TextFileSink_Button = new Button();
            ConsoleSink_Button = new Button();
            MemorySink_Button = new Button();
            Sources_ListBox = new ListView();
            Sources_Label = new Label();
            RichTextWindow_Button = new Button();
            Whitelist_Box = new CheckBox();
            Blacklist_Box = new CheckBox();
            panel1.SuspendLayout();
            LogLevel_Box.SuspendLayout();
            SuspendLayout();
            // 
            // MainTBsink_Box
            // 
            MainTBsink_Box.Location = new Point(12, 12);
            MainTBsink_Box.Name = "MainTBsink_Box";
            MainTBsink_Box.Size = new Size(776, 319);
            MainTBsink_Box.TabIndex = 0;
            /*MainTBsink_Box.Text = "";*/
            // 
            // panel1
            // 
            panel1.Controls.Add(LogHeading_Button);
            panel1.Controls.Add(LogEntry_Button);
            panel1.Controls.Add(RemoveSource_Button);
            panel1.Controls.Add(AddSource_Button);
            panel1.Controls.Add(Message_Label);
            panel1.Controls.Add(RandomSource_Button);
            panel1.Controls.Add(RandomMessage_Button);
            panel1.Controls.Add(Source_Label);
            panel1.Controls.Add(LogLevel_Box);
            panel1.Controls.Add(Message_Box);
            panel1.Controls.Add(Source_Box);
            panel1.Location = new Point(142, 337);
            panel1.Name = "panel1";
            panel1.Size = new Size(386, 148);
            panel1.TabIndex = 1;
            // 
            // LogHeading_Button
            // 
            LogHeading_Button.Font = new Font("Segoe UI", 7F);
            LogHeading_Button.Location = new Point(301, 109);
            LogHeading_Button.Name = "LogHeading_Button";
            LogHeading_Button.Size = new Size(82, 28);
            LogHeading_Button.TabIndex = 10;
            LogHeading_Button.Text = "Heading";
            LogHeading_Button.TextAlign = ContentAlignment.TopCenter;
            LogHeading_Button.UseVisualStyleBackColor = true;
            LogHeading_Button.Click += LogHeadingButton_Click;
            // 
            // LogEntry_Button
            // 
            LogEntry_Button.Font = new Font("Segoe UI", 8F);
            LogEntry_Button.Location = new Point(213, 109);
            LogEntry_Button.Name = "LogEntry_Button";
            LogEntry_Button.Size = new Size(82, 28);
            LogEntry_Button.TabIndex = 8;
            LogEntry_Button.Text = "Log Entry";
            LogEntry_Button.TextAlign = ContentAlignment.TopCenter;
            LogEntry_Button.UseVisualStyleBackColor = true;
            LogEntry_Button.Click += LogEntry_Button_Click;
            // 
            // RemoveSource_Button
            // 
            RemoveSource_Button.Font = new Font("Segoe UI", 7F);
            RemoveSource_Button.Location = new Point(301, 75);
            RemoveSource_Button.Name = "RemoveSource_Button";
            RemoveSource_Button.Size = new Size(82, 28);
            RemoveSource_Button.TabIndex = 10;
            RemoveSource_Button.Text = "Remove Source";
            RemoveSource_Button.UseVisualStyleBackColor = true;
            RemoveSource_Button.Click += RemoveSource_Button_Click;
            // 
            // AddSource_Button
            // 
            AddSource_Button.Font = new Font("Segoe UI", 7F);
            AddSource_Button.Location = new Point(213, 75);
            AddSource_Button.Name = "AddSource_Button";
            AddSource_Button.Size = new Size(82, 28);
            AddSource_Button.TabIndex = 9;
            AddSource_Button.Text = "Add Source";
            AddSource_Button.UseVisualStyleBackColor = true;
            AddSource_Button.Click += AddSource_Button_Click;
            // 
            // Message_Label
            // 
            Message_Label.AutoSize = true;
            Message_Label.Font = new Font("Segoe UI", 6F);
            Message_Label.Location = new Point(3, 0);
            Message_Label.Name = "Message_Label";
            Message_Label.Size = new Size(35, 11);
            Message_Label.TabIndex = 6;
            Message_Label.Text = "Message";
            // 
            // RandomSource_Button
            // 
            RandomSource_Button.Font = new Font("Segoe UI", 6F);
            RandomSource_Button.Location = new Point(353, 48);
            RandomSource_Button.Name = "RandomSource_Button";
            RandomSource_Button.Size = new Size(30, 23);
            RandomSource_Button.TabIndex = 5;
            RandomSource_Button.Text = "RND";
            RandomSource_Button.UseVisualStyleBackColor = true;
            RandomSource_Button.Click += RandomSource_Button_Click;
            // 
            // RandomMessage_Button
            // 
            RandomMessage_Button.Font = new Font("Segoe UI", 6F);
            RandomMessage_Button.Location = new Point(353, 12);
            RandomMessage_Button.Name = "RandomMessage_Button";
            RandomMessage_Button.Size = new Size(30, 23);
            RandomMessage_Button.TabIndex = 4;
            RandomMessage_Button.Text = "RND";
            RandomMessage_Button.UseVisualStyleBackColor = true;
            RandomMessage_Button.Click += RandomMessage_Button_Click;
            // 
            // Source_Label
            // 
            Source_Label.AutoSize = true;
            Source_Label.Font = new Font("Segoe UI", 6F);
            Source_Label.Location = new Point(213, 35);
            Source_Label.Name = "Source_Label";
            Source_Label.Size = new Size(30, 11);
            Source_Label.TabIndex = 3;
            Source_Label.Text = "Source";
            // 
            // LogLevel_Box
            // 
            LogLevel_Box.Controls.Add(FATAL_Option);
            LogLevel_Box.Controls.Add(ERR_Option);
            LogLevel_Box.Controls.Add(WARN_Option);
            LogLevel_Box.Controls.Add(INFO_Option);
            LogLevel_Box.Controls.Add(Debug_Option);
            LogLevel_Box.Location = new Point(3, 41);
            LogLevel_Box.Name = "LogLevel_Box";
            LogLevel_Box.Size = new Size(203, 97);
            LogLevel_Box.TabIndex = 1;
            LogLevel_Box.TabStop = false;
            LogLevel_Box.Text = "LogLevel";
            // 
            // FATAL_Option
            // 
            FATAL_Option.AutoSize = true;
            FATAL_Option.Location = new Point(106, 47);
            FATAL_Option.Name = "FATAL_Option";
            FATAL_Option.Size = new Size(57, 19);
            FATAL_Option.TabIndex = 4;
            FATAL_Option.TabStop = true;
            FATAL_Option.Text = "FATAL";
            FATAL_Option.UseVisualStyleBackColor = true;
            // 
            // ERR_Option
            // 
            ERR_Option.AutoSize = true;
            ERR_Option.Location = new Point(106, 22);
            ERR_Option.Name = "ERR_Option";
            ERR_Option.Size = new Size(45, 19);
            ERR_Option.TabIndex = 3;
            ERR_Option.TabStop = true;
            ERR_Option.Text = "ERR";
            ERR_Option.UseVisualStyleBackColor = true;
            // 
            // WARN_Option
            // 
            WARN_Option.AutoSize = true;
            WARN_Option.Location = new Point(6, 72);
            WARN_Option.Name = "WARN_Option";
            WARN_Option.Size = new Size(60, 19);
            WARN_Option.TabIndex = 2;
            WARN_Option.TabStop = true;
            WARN_Option.Text = "WARN";
            WARN_Option.UseVisualStyleBackColor = true;
            // 
            // INFO_Option
            // 
            INFO_Option.AutoSize = true;
            INFO_Option.Location = new Point(6, 47);
            INFO_Option.Name = "INFO_Option";
            INFO_Option.Size = new Size(52, 19);
            INFO_Option.TabIndex = 1;
            INFO_Option.TabStop = true;
            INFO_Option.Text = "INFO";
            INFO_Option.UseVisualStyleBackColor = true;
            // 
            // Debug_Option
            // 
            Debug_Option.AutoSize = true;
            Debug_Option.Location = new Point(6, 22);
            Debug_Option.Name = "Debug_Option";
            Debug_Option.Size = new Size(62, 19);
            Debug_Option.TabIndex = 0;
            Debug_Option.TabStop = true;
            Debug_Option.Text = "DEBUG";
            Debug_Option.UseVisualStyleBackColor = true;
            // 
            // Message_Box
            // 
            Message_Box.Location = new Point(3, 12);
            Message_Box.Name = "Message_Box";
            Message_Box.Size = new Size(344, 23);
            Message_Box.TabIndex = 0;
            // 
            // Source_Box
            // 
            Source_Box.AllowDrop = true;
            Source_Box.FormattingEnabled = true;
            Source_Box.Location = new Point(213, 46);
            Source_Box.Name = "Source_Box";
            Source_Box.Size = new Size(134, 23);
            Source_Box.TabIndex = 7;
            // 
            // TextFileSink_Button
            // 
            TextFileSink_Button.Location = new Point(12, 337);
            TextFileSink_Button.Name = "TextFileSink_Button";
            TextFileSink_Button.Size = new Size(124, 23);
            TextFileSink_Button.TabIndex = 2;
            TextFileSink_Button.Text = "Text File";
            TextFileSink_Button.UseVisualStyleBackColor = true;
            TextFileSink_Button.Click += TextFileSink_Button_Click;
            // 
            // ConsoleSink_Button
            // 
            ConsoleSink_Button.Location = new Point(12, 366);
            ConsoleSink_Button.Name = "ConsoleSink_Button";
            ConsoleSink_Button.Size = new Size(124, 23);
            ConsoleSink_Button.TabIndex = 3;
            ConsoleSink_Button.Text = "Console";
            ConsoleSink_Button.UseVisualStyleBackColor = true;
            ConsoleSink_Button.Click += ConsoleSink_Button_Click;
            // 
            // MemorySink_Button
            // 
            MemorySink_Button.Location = new Point(12, 395);
            MemorySink_Button.Name = "MemorySink_Button";
            MemorySink_Button.Size = new Size(124, 23);
            MemorySink_Button.TabIndex = 4;
            MemorySink_Button.Text = "Memory";
            MemorySink_Button.UseVisualStyleBackColor = true;
            MemorySink_Button.Click += MemorySink_Button_Click;
            // 
            // Sources_ListBox
            // 
            Sources_ListBox.Location = new Point(545, 372);
            Sources_ListBox.Name = "Sources_ListBox";
            Sources_ListBox.Size = new Size(254, 113);
            Sources_ListBox.TabIndex = 5;
            Sources_ListBox.UseCompatibleStateImageBehavior = false;
            Sources_ListBox.View = View.List;
            // 
            // Sources_Label
            // 
            Sources_Label.AutoSize = true;
            Sources_Label.Location = new Point(534, 337);
            Sources_Label.Name = "Sources_Label";
            Sources_Label.Size = new Size(48, 15);
            Sources_Label.TabIndex = 6;
            Sources_Label.Text = "Sources";
            // 
            // RichTextWindow_Button
            // 
            RichTextWindow_Button.Location = new Point(12, 425);
            RichTextWindow_Button.Name = "RichTextWindow_Button";
            RichTextWindow_Button.Size = new Size(124, 23);
            RichTextWindow_Button.TabIndex = 7;
            RichTextWindow_Button.Text = "RichText Window";
            RichTextWindow_Button.UseVisualStyleBackColor = true;
            RichTextWindow_Button.Click += RichTextWindow_Button_Click;
            // 
            // Whitelist_Box
            // 
            Whitelist_Box.AutoSize = true;
            Whitelist_Box.Location = new Point(545, 351);
            Whitelist_Box.Name = "Whitelist_Box";
            Whitelist_Box.Size = new Size(72, 19);
            Whitelist_Box.TabIndex = 8;
            Whitelist_Box.Text = "Whitelist";
            Whitelist_Box.UseVisualStyleBackColor = true;
            Whitelist_Box.CheckedChanged += Whitelist_Box_CheckedChanged;
            // 
            // Blacklist_Box
            // 
            Blacklist_Box.AutoSize = true;
            Blacklist_Box.Location = new Point(633, 351);
            Blacklist_Box.Name = "Blacklist_Box";
            Blacklist_Box.Size = new Size(69, 19);
            Blacklist_Box.TabIndex = 9;
            Blacklist_Box.Text = "Blacklist";
            Blacklist_Box.UseVisualStyleBackColor = true;
            Blacklist_Box.CheckedChanged += Blacklist_Box_CheckedChanged;
            // 
            // DemoMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 497);
            Controls.Add(Blacklist_Box);
            Controls.Add(Whitelist_Box);
            Controls.Add(RichTextWindow_Button);
            Controls.Add(Sources_Label);
            Controls.Add(Sources_ListBox);
            Controls.Add(MemorySink_Button);
            Controls.Add(ConsoleSink_Button);
            Controls.Add(TextFileSink_Button);
            Controls.Add(panel1);
            Controls.Add(MainTBsink_Box);
            Name = "DemoMain";
            Text = "mLogger Demo";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            LogLevel_Box.ResumeLayout(false);
            LogLevel_Box.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox MainTBsink_Box;
        private Panel panel1;
        private TextBox Message_Box;
        private Label ManualEntry_Label;
        private GroupBox LogLevel_Box;
        private RadioButton Debug_Option;
        private RadioButton FATAL_Option;
        private RadioButton ERR_Option;
        private RadioButton WARN_Option;
        private RadioButton INFO_Option;
        private Label Message_Label;
        private Button RandomSource_Button;
        private Button RandomMessage_Button;
        private Label Source_Label;
        private Button AddSource_Button;
        private Button LogEntry_Button;
        private ComboBox Source_Box;
        private Button RemoveSource_Button;
        private Button TextFileSink_Button;
        private Button ConsoleSink_Button;
        private Button MemorySink_Button;
        private ListView Sources_ListBox;
        private Label Sources_Label;
        private Button RichTextWindow_Button;
        private CheckBox Whitelist_Box;
        private CheckBox Blacklist_Box;
        private Button LogHeading_Button;
    }
}
