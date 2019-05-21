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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miDisplayTurnCounter = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.textColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsTurnCounter = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.txtView = new System.Windows.Forms.TextBox();
            this.txtMessages = new System.Windows.Forms.TextBox();
            this.miSaveGame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.miOutputXML = new System.Windows.Forms.ToolStripMenuItem();
            this.miOutputComment = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(423, 24);
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // MiLoadGame
            // 
            this.MiLoadGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MiLoadGame.Name = "MiLoadGame";
            this.MiLoadGame.Size = new System.Drawing.Size(180, 22);
            this.MiLoadGame.Text = "Load Game";
            this.MiLoadGame.Click += new System.EventHandler(this.MiLoadGame_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(177, 6);
            // 
            // miNew
            // 
            this.miNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miNew.Name = "miNew";
            this.miNew.Size = new System.Drawing.Size(180, 22);
            this.miNew.Text = "New";
            this.miNew.Click += new System.EventHandler(this.miNew_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // miLoadSaveGame
            // 
            this.miLoadSaveGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miLoadSaveGame.Name = "miLoadSaveGame";
            this.miLoadSaveGame.Size = new System.Drawing.Size(180, 22);
            this.miLoadSaveGame.Text = "Load Save Game";
            this.miLoadSaveGame.Click += new System.EventHandler(this.miLoadSaveGame_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
            // 
            // miExit
            // 
            this.miExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(180, 22);
            this.miExit.Text = "Exit";
            this.miExit.Click += new System.EventHandler(this.miExit_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisplayTurnCounter,
            this.toolStripSeparator4,
            this.textColourToolStripMenuItem,
            this.backgroundColourToolStripMenuItem,
            this.toolStripSeparator5,
            this.miOutputXML,
            this.miOutputComment});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // miDisplayTurnCounter
            // 
            this.miDisplayTurnCounter.CheckOnClick = true;
            this.miDisplayTurnCounter.Name = "miDisplayTurnCounter";
            this.miDisplayTurnCounter.Size = new System.Drawing.Size(214, 22);
            this.miDisplayTurnCounter.Text = "Display turn counter";
            this.miDisplayTurnCounter.Click += new System.EventHandler(this.miDisplayTurnCounter_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(211, 6);
            // 
            // textColourToolStripMenuItem
            // 
            this.textColourToolStripMenuItem.Name = "textColourToolStripMenuItem";
            this.textColourToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.textColourToolStripMenuItem.Text = "Text Colour";
            this.textColourToolStripMenuItem.Click += new System.EventHandler(this.textColourToolStripMenuItem_Click);
            // 
            // backgroundColourToolStripMenuItem
            // 
            this.backgroundColourToolStripMenuItem.Name = "backgroundColourToolStripMenuItem";
            this.backgroundColourToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.backgroundColourToolStripMenuItem.Text = "Background Colour";
            this.backgroundColourToolStripMenuItem.Click += new System.EventHandler(this.backgroundColourToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MiAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // MiAbout
            // 
            this.MiAbout.Name = "MiAbout";
            this.MiAbout.Size = new System.Drawing.Size(107, 22);
            this.MiAbout.Text = "About";
            this.MiAbout.Click += new System.EventHandler(this.MiAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsFileName,
            this.tsTurnCounter});
            this.statusStrip1.Location = new System.Drawing.Point(0, 309);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(423, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsFileName
            // 
            this.tsFileName.Name = "tsFileName";
            this.tsFileName.Size = new System.Drawing.Size(0, 17);
            // 
            // tsTurnCounter
            // 
            this.tsTurnCounter.Name = "tsTurnCounter";
            this.tsTurnCounter.Size = new System.Drawing.Size(0, 17);
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(7, 282);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(408, 20);
            this.txtInput.TabIndex = 2;
            this.txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtInput_KeyDown);
            // 
            // txtView
            // 
            this.txtView.Location = new System.Drawing.Point(7, 30);
            this.txtView.Multiline = true;
            this.txtView.Name = "txtView";
            this.txtView.ReadOnly = true;
            this.txtView.Size = new System.Drawing.Size(408, 120);
            this.txtView.TabIndex = 3;
            this.txtView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtView_KeyPress);
            // 
            // txtMessages
            // 
            this.txtMessages.Location = new System.Drawing.Point(7, 156);
            this.txtMessages.Multiline = true;
            this.txtMessages.Name = "txtMessages";
            this.txtMessages.ReadOnly = true;
            this.txtMessages.Size = new System.Drawing.Size(408, 120);
            this.txtMessages.TabIndex = 4;
            this.txtMessages.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessages_KeyPress);
            // 
            // miSaveGame
            // 
            this.miSaveGame.Name = "miSaveGame";
            this.miSaveGame.Size = new System.Drawing.Size(180, 22);
            this.miSaveGame.Text = "Save Game";
            this.miSaveGame.Click += new System.EventHandler(this.miSaveGame_Click_1);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(211, 6);
            // 
            // miOutputXML
            // 
            this.miOutputXML.Name = "miOutputXML";
            this.miOutputXML.Size = new System.Drawing.Size(214, 22);
            this.miOutputXML.Text = "Output as XML";
            this.miOutputXML.Click += new System.EventHandler(this.miOutputXML_Click);
            // 
            // miOutputComment
            // 
            this.miOutputComment.Name = "miOutputComment";
            this.miOutputComment.Size = new System.Drawing.Size(214, 22);
            this.miOutputComment.Text = "Output as commented dat";
            this.miOutputComment.Click += new System.EventHandler(this.miOutputComment_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 331);
            this.Controls.Add(this.txtMessages);
            this.Controls.Add(this.txtView);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AdventurerWIN";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsFileName;
        private System.Windows.Forms.ToolStripStatusLabel tsTurnCounter;
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
    }
}

