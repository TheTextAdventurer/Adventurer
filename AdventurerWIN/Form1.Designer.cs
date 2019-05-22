namespace AdventurerWIN
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MiLoadGame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miLoadSaveGame = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveGame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miDisplayTurnCounter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.fontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.miOutputXML = new System.Windows.Forms.ToolStripMenuItem();
            this.miOutputComment = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.recordGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playRecordedGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopRecordingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editRecordedGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtView = new System.Windows.Forms.TextBox();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(880, 35);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MiLoadGame,
            this.toolStripSeparator3,
            this.miNew,
            this.toolStripSeparator1,
            this.miLoadSaveGame,
            this.miSaveGame,
            this.toolStripSeparator2,
            this.miExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShowShortcutKeys = false;
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // MiLoadGame
            // 
            this.MiLoadGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MiLoadGame.Name = "MiLoadGame";
            this.MiLoadGame.Size = new System.Drawing.Size(228, 30);
            this.MiLoadGame.Text = "Load Game";
            this.MiLoadGame.Click += new System.EventHandler(this.MiLoadGame_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(225, 6);
            // 
            // miNew
            // 
            this.miNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miNew.Name = "miNew";
            this.miNew.Size = new System.Drawing.Size(228, 30);
            this.miNew.Text = "New";
            this.miNew.Click += new System.EventHandler(this.miNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(225, 6);
            // 
            // miLoadSaveGame
            // 
            this.miLoadSaveGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miLoadSaveGame.Name = "miLoadSaveGame";
            this.miLoadSaveGame.Size = new System.Drawing.Size(228, 30);
            this.miLoadSaveGame.Text = "Load Save Game";
            this.miLoadSaveGame.Click += new System.EventHandler(this.miLoadSaveGame_Click);
            // 
            // miSaveGame
            // 
            this.miSaveGame.Name = "miSaveGame";
            this.miSaveGame.Size = new System.Drawing.Size(228, 30);
            this.miSaveGame.Text = "Save Game";
            this.miSaveGame.Click += new System.EventHandler(this.miSaveGame_Click_1);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(225, 6);
            // 
            // miExit
            // 
            this.miExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(228, 30);
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisplayTurnCounter,
            this.toolStripSeparator4,
            this.fontToolStripMenuItem,
            this.textColourToolStripMenuItem,
            this.backgroundColourToolStripMenuItem,
            this.toolStripSeparator5,
            this.miOutputXML,
            this.miOutputComment,
            this.toolStripSeparator6,
            this.recordGameToolStripMenuItem,
            this.playRecordedGameToolStripMenuItem,
            this.stopRecordingToolStripMenuItem,
            this.editRecordedGameToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(88, 29);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // miDisplayTurnCounter
            // 
            this.miDisplayTurnCounter.CheckOnClick = true;
            this.miDisplayTurnCounter.Name = "miDisplayTurnCounter";
            this.miDisplayTurnCounter.Size = new System.Drawing.Size(307, 30);
            this.miDisplayTurnCounter.Text = "Display turn counter";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(304, 6);
            // 
            // fontToolStripMenuItem
            // 
            this.fontToolStripMenuItem.Name = "fontToolStripMenuItem";
            this.fontToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.fontToolStripMenuItem.Text = "Font";
            this.fontToolStripMenuItem.Click += new System.EventHandler(this.fontToolStripMenuItem_Click);
            // 
            // textColourToolStripMenuItem
            // 
            this.textColourToolStripMenuItem.Name = "textColourToolStripMenuItem";
            this.textColourToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.textColourToolStripMenuItem.Text = "Text Colour";
            this.textColourToolStripMenuItem.Click += new System.EventHandler(this.textColourToolStripMenuItem_Click);
            // 
            // backgroundColourToolStripMenuItem
            // 
            this.backgroundColourToolStripMenuItem.Name = "backgroundColourToolStripMenuItem";
            this.backgroundColourToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.backgroundColourToolStripMenuItem.Text = "Background Colour";
            this.backgroundColourToolStripMenuItem.Click += new System.EventHandler(this.backgroundColourToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(304, 6);
            // 
            // miOutputXML
            // 
            this.miOutputXML.Name = "miOutputXML";
            this.miOutputXML.Size = new System.Drawing.Size(307, 30);
            this.miOutputXML.Text = "Output as XML";
            this.miOutputXML.Click += new System.EventHandler(this.miOutputXML_Click);
            // 
            // miOutputComment
            // 
            this.miOutputComment.Name = "miOutputComment";
            this.miOutputComment.Size = new System.Drawing.Size(307, 30);
            this.miOutputComment.Text = "Output as commented dat";
            this.miOutputComment.Click += new System.EventHandler(this.miOutputComment_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(304, 6);
            // 
            // recordGameToolStripMenuItem
            // 
            this.recordGameToolStripMenuItem.CheckOnClick = true;
            this.recordGameToolStripMenuItem.Name = "recordGameToolStripMenuItem";
            this.recordGameToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.recordGameToolStripMenuItem.Text = "Record Game";
            this.recordGameToolStripMenuItem.Click += new System.EventHandler(this.recordGameToolStripMenuItem_Click);
            // 
            // playRecordedGameToolStripMenuItem
            // 
            this.playRecordedGameToolStripMenuItem.Name = "playRecordedGameToolStripMenuItem";
            this.playRecordedGameToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.playRecordedGameToolStripMenuItem.Text = "Play recorded game";
            this.playRecordedGameToolStripMenuItem.Click += new System.EventHandler(this.playRecordedGameToolStripMenuItem_Click);
            // 
            // stopRecordingToolStripMenuItem
            // 
            this.stopRecordingToolStripMenuItem.Name = "stopRecordingToolStripMenuItem";
            this.stopRecordingToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.stopRecordingToolStripMenuItem.Text = "Stop Recording";
            this.stopRecordingToolStripMenuItem.Click += new System.EventHandler(this.stopRecordingToolStripMenuItem_Click);
            // 
            // editRecordedGameToolStripMenuItem
            // 
            this.editRecordedGameToolStripMenuItem.Name = "editRecordedGameToolStripMenuItem";
            this.editRecordedGameToolStripMenuItem.Size = new System.Drawing.Size(307, 30);
            this.editRecordedGameToolStripMenuItem.Text = "Edit Recorded Game";
            this.editRecordedGameToolStripMenuItem.Click += new System.EventHandler(this.editRecordedGameToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MiAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // MiAbout
            // 
            this.MiAbout.Name = "MiAbout";
            this.MiAbout.Size = new System.Drawing.Size(146, 30);
            this.MiAbout.Text = "About";
            this.MiAbout.Click += new System.EventHandler(this.MiAbout_Click);
            // 
            // txtInput
            // 
            this.txtInput.BackColor = global::AdventurerWIN.Properties.Settings.Default.BackGroundColour;
            this.txtInput.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AdventurerWIN.Properties.Settings.Default, "BackGroundColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtInput.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AdventurerWIN.Properties.Settings.Default, "ForeGroundColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtInput.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AdventurerWIN.Properties.Settings.Default, "TextFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Font = global::AdventurerWIN.Properties.Settings.Default.TextFont;
            this.txtInput.ForeColor = global::AdventurerWIN.Properties.Settings.Default.ForeGroundColour;
            this.txtInput.Location = new System.Drawing.Point(4, 683);
            this.txtInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(872, 26);
            this.txtInput.TabIndex = 2;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // txtView
            // 
            this.txtView.BackColor = global::AdventurerWIN.Properties.Settings.Default.BackGroundColour;
            this.txtView.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AdventurerWIN.Properties.Settings.Default, "BackGroundColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtView.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AdventurerWIN.Properties.Settings.Default, "ForeGroundColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtView.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AdventurerWIN.Properties.Settings.Default, "TextFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtView.Font = global::AdventurerWIN.Properties.Settings.Default.TextFont;
            this.txtView.ForeColor = global::AdventurerWIN.Properties.Settings.Default.ForeGroundColour;
            this.txtView.Location = new System.Drawing.Point(4, 5);
            this.txtView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtView.Multiline = true;
            this.txtView.Name = "txtView";
            this.txtView.ReadOnly = true;
            this.txtView.Size = new System.Drawing.Size(872, 329);
            this.txtView.TabIndex = 3;
            this.txtView.DoubleClick += new System.EventHandler(this.txtView_DoubleClick);
            this.txtView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtView_KeyPress);
            // 
            // txtMessages
            // 
            this.txtMessages.BackColor = global::AdventurerWIN.Properties.Settings.Default.BackGroundColour;
            this.txtMessages.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::AdventurerWIN.Properties.Settings.Default, "BackGroundColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtMessages.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::AdventurerWIN.Properties.Settings.Default, "ForeGroundColour", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtMessages.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::AdventurerWIN.Properties.Settings.Default, "TextFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessages.Font = global::AdventurerWIN.Properties.Settings.Default.TextFont;
            this.txtMessages.ForeColor = global::AdventurerWIN.Properties.Settings.Default.ForeGroundColour;
            this.txtMessages.Location = new System.Drawing.Point(4, 344);
            this.txtMessages.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.Size = new System.Drawing.Size(872, 329);
            this.txtMessages.TabIndex = 4;
            this.txtMessages.DoubleClick += new System.EventHandler(this.txtMessages_DoubleClick);
            this.txtMessages.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessages_KeyPress);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.txtInput, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtView, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtMessages, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 35);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(880, 715);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = global::AdventurerWIN.Properties.Settings.Default.FormSize;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("ClientSize", global::AdventurerWIN.Properties.Settings.Default, "FormSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(648, 539);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AdventurerWIN";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miLoadSaveGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miDisplayTurnCounter;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MiAbout;
        private System.Windows.Forms.ToolStripMenuItem MiLoadGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.TextBox txtView;
        private System.Windows.Forms.TextBox txtMessages;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem textColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem backgroundColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miSaveGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem miOutputXML;
        private System.Windows.Forms.ToolStripMenuItem miOutputComment;
        private System.Windows.Forms.ToolStripMenuItem fontToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem recordGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playRecordedGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopRecordingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editRecordedGameToolStripMenuItem;
    }
}

