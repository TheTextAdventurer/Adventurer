using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GameEngine;

namespace AdventurerWIN
{
    public partial class Form1 : Form
    {
        private string FormTitle = null;

        private string Recorder = null;
        private DateTime lastInput = DateTime.Now;

        public Form1()
        {
            InitializeComponent();

            Advent.RoomView += Advent_RoomView;
            Advent.GameMessages += Advent_GameMessages;
            Advent.GameOver += Advent_GameOver;
            Reset();
        }

        private void Advent_GameOver(object sender, EventArgs e)
        {
            MessageBox.Show("This game is now over", "Game over", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private Color ForeGroundColour { get; set; } = SystemColors.WindowText;
        private Color BackgroundColour { get; set; } = SystemColors.Window;

        private void Reset()
        {
            SetColours();
            miSaveGame.Enabled = false;
            miOutputXML.Enabled = false;
            miOutputComment.Enabled = false;
        }

        private void SetColours()
        {
            txtInput.BackColor = BackgroundColour;
            txtInput.ForeColor = ForeGroundColour;
            txtMessages.BackColor = BackgroundColour;
            txtMessages.ForeColor = ForeGroundColour;
            txtView.BackColor = BackgroundColour;
            txtView.ForeColor = ForeGroundColour;
        }

        private void LoadNewGame(string pGame)
        {
            Advent.LoadGame(pGame);

            FormTitle += " " + Advent.GameName;

            SetColours();
            miSaveGame.Enabled = true;
            txtMessages.DeselectAll();
            txtView.DeselectAll();
            txtInput.Focus();
            txtInput.ReadOnly = false;
            miOutputXML.Enabled = true;
            miOutputComment.Enabled = true;

        }

        #region advent events

        private void Advent_GameMessages(object sender, Advent.GameOuput e)
        {
            if (e.Refresh)
                txtMessages.Text = e.Message;
            else
                txtMessages.Text += e.Message;
        }

        private void Advent_RoomView(object sender, Advent.Roomview e)
        {
            txtView.Text = $"{e.View}{Environment.NewLine}{Environment.NewLine}{e.Items}";
        }

        #endregion


        #region menu item clicks

        private void MiLoadGame_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "dat files (*.dat)|*.dat";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    LoadNewGame(Path.GetFileName(ofd.FileName));
                }
            }
        }

        private void miNew_Click(object sender, EventArgs e)
        {

        }

        private void miLoadSaveGame_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "save files (*.sav)|*.sav";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Advent.RestoreGame(Advent.GameName, ofd.FileName);
                }
            }
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to exit this game","Exit game",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void MiAbout_Click(object sender, EventArgs e)
        {
            using (About a = new About())
            {
                a.ShowDialog();
            }
        }


        private void textColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog cfd = new ColorDialog())
            {
                if (cfd.ShowDialog() == DialogResult.OK)
                {
                    ForeGroundColour = cfd.Color;
                    SetColours();
                }
            }
        }

        private void backgroundColourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog cfd = new ColorDialog())
            {
                if (cfd.ShowDialog() == DialogResult.OK)
                {
                    BackgroundColour = cfd.Color;
                    SetColours();
                }
            }
        }

        private void miSaveGame_Click_1(object sender, EventArgs e)
        {
            Advent.SaveGame();
        }

        private void miOutputXML_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML files (*.XML)|*.XML";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var g = GameData.Load(Advent.GameName);
                    g.SaveAsCommentedXML();
                }
            }
        }

        private void miOutputComment_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "dat files (*.dat)|*.dat";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    GameData.SaveAsCommentedDat(Advent.GameName);
                }
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    txtInput.Font = fd.Font;
                    txtMessages.Font = fd.Font;
                    txtView.Font = fd.Font;
                    AutoSizeInput();
                }
            }
        }

        private void recordGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var r = sender as ToolStripMenuItem;

            if (r.Checked)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "rec files (*.rec)|*.rec";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        Recorder = sfd.FileName;
                        lastInput = DateTime.Now;
                    }
                    else
                        Recorder = null;
                }
            }
        }

        #endregion

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter)
            {
                var t = sender as TextBox;

                Advent.ProcessText(t.Text);
                

                if (miDisplayTurnCounter.Checked)
                    this.Text = this.FormTitle + $" Turns: {Advent.TurnCounter}";
                else
                    this.Text = this.FormTitle;          
                
                if (Recorder != null)
                {
                    DateTime n = DateTime.Now;
                    File.AppendAllText(Recorder, $"{(int)(n - lastInput).TotalMilliseconds}  \"{t.Text}\"{Environment.NewLine}");
                    lastInput = n;
                }

                t.Text = "";
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtInput.Focus();
        }

        private void txtMessages_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtInput.Focus();
        }

        private void txtView_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtInput.Focus();
        }

        /// <summary>
        /// Set the text box height,based on the font
        /// </summary>
        /// <param name="txt"></param>
        private void AutoSizeInput()
        {
            const int x_margin = 0;
            const int y_margin = 2;
            Size size = TextRenderer.MeasureText(txtInput.Text, txtInput.Font);
            txtInput.ClientSize =
                new Size(size.Width + x_margin, size.Height + y_margin);

            

            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
