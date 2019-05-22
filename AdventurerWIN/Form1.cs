using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using GameEngine;

namespace AdventurerWIN
{

    public partial class Form1 : Form
    {
        private string FormTitle = null;

        private RecordGame Recorder = null;


        public Form1()
        {
            InitializeComponent();

            Advent.RoomView += Advent_RoomView;
            Advent.GameMessages += Advent_GameMessages;
            Advent.GameOver += Advent_GameOver;

            Reset();

            LoadNewGame("Adv01.dat");
            //BeginRecording("E:\\temp\\Adventurer\\AdventurerWIN\\bin\\Debug\\foo.rec");
            //BeginPlayback("E:\\temp\\Adventurer\\AdventurerWIN\\bin\\Debug\\test.rec");
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
                        BeginRecording(sfd.FileName);
                    }
                    else
                    {
                        Recorder = null;
                        r.Checked = false;
                    }
                }
            }
        }


        private void playRecordedGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "rec files (*.rec)|*.rec";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    BeginPlayback(ofd.FileName);
                }
            }            
        }

        private void stopRecordingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Recorder != null)
            {
                RecordGame.Save(Recorder);
            }
        }

        private void editRecordedGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (EditRecorded er = new EditRecorded())
            {
                er.ShowDialog();
            }
        }

        #endregion

        #region textbox code

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

                if (Recorder != null && !Recorder.Playback)
                    Recorder.AddInput(t.Text);

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
        /// Set the text box height,based on the font size
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




        #endregion

        #region recorder code

        private void BeginRecording(string pPath)
        {
            Recorder = new RecordGame(Advent.GameData, pPath);
        }


        private void BeginPlayback(string pPath)
        {
            if (Recorder != null)
            {
                Recorder.eKeystroke -= Recorder_eKeystroke;
                Recorder.ReplayFinished -= Recorder_ReplayFinished;
            }

            Recorder = RecordGame.Load(pPath);

            Recorder.eKeystroke += Recorder_eKeystroke;
            Recorder.ReplayFinished += Recorder_ReplayFinished;
            Recorder.eCarriageReturn += Recorder_eCarriageReturn;
            Advent.RestoreGame(Recorder.Data);
            Recorder.StartReplay();
        }

        private void Recorder_eCarriageReturn(object sender, EventArgs e)
        {
            txtInput.Focus();
            SendKeys.Send(Environment.NewLine);
        }


        private void Recorder_ReplayFinished(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void Recorder_eKeystroke(object sender, RecordGame.Keystroke e)
        {
            txtInput.Focus();
            txtInput.Text += e.Key;
            txtInput.SelectionStart = txtInput.Text.Length;
            txtInput.SelectionLength = 0;
        }


        #endregion

        private void txtView_DoubleClick(object sender, EventArgs e)
        {
            Recorder.StopReplay();
        }

        private void txtMessages_DoubleClick(object sender, EventArgs e)
        {
            if (Recorder != null)
            {
                txtInput.Text = Recorder.GetNextInput();
                Recorder_eCarriageReturn(null, null);
            }
        }
    }
    /*
     500	,	go east
    500	,	go east
    500	,	take axe
    500	,	go north
    500	,	take ox
    500	,	say bunyon
    500	,	swim
    500	,	go south
    500	,	go hole
    500	,	take flint
    500	,	go up
    500	,	go west
    500	,	go west
    500	,	take axe
    500	,	take ox
    500	,	take fruit
    500	,	go east
    500	,	take chiggers
    500	,	climb tree
    500	,	drop chiggers
    500	,	take keys
    500	,	read web
    500	,	climb down
    500	,	chop tree
    500	,	drop axe
    500	,	take mud
    500	,	go stump
    500	,	drop mud
    500	,	drop ox
    500	,	drop fruit
    500	,	go down
    500	,	take rubies
    500	,	go up
    500	,	drop rubies
    500, score
    500	,	take lamp
    500	,	rub lamp
    500	,	rub lamp
    500	,	score
    500	,	go down
    500	,	go hole
    500	,	open door
    500	,	drop keys
    500	,	light lamp
    500	,	go hall
    500	,	go down
    500	,	go south
    500	,	take bladder
    500	,	go north
    500	,	go up
    500	,	go up
    500	,	go up
    500	,	unlight lamp
    500	,	go up
    500	,	go up
    500	,	take gas
    1000, inv
    500	,	go stump
    500	,	go down

    500	,	light lamp
    500	,	go hole
    500	,	go hall
    500	,	go down
    500	,	go south
    500	,	go up
    500	,	drop bladder
    500	,	ignite gas
    500	,	go hole
    500	,	jump
    500	,	take mirror
    500	,	scream bear
    500	,	take mirror
    500	,	go throne
    500	,	take crown
    500	,	go west
    500	,	jump
    500	,	go west
    500	,	take bricks
    500	,	go down
    500	,	go north
    500	,	go up
    500	,	go up
    500	,	go up
    500	,	unlight lamp
    500	,	go up
    500	,	drop crown
    500, score
    500	,	take bottle
    500	,	go down
    500	,	light lamp
    500	,	drop flint
    500	,	go hole
    500	,	go hall
    500	,	go down
    500	,	go down
    500	,	go down
    500	,	go west
    500	,	go down
    500	,	take rug
    500	,	go down
    500	,	build dam
    500	,	drop bricks
    500	,	look lava
    500	,	pour water
    500	,	take firestone
    500	,	take net
    500	,	say away
    500	,	say away
    500	,	unlight lamp
    500	,	go south
    500	,	go stump
    500	,	drop rug
    500	,	drop mirror
    500	,	drop firestone
    500, score
    500	,	go up
    500	,	go east
    500	,	go north
    500	,	take water
    500	,	take fish
    500	,	go south
    500	,	go west
    500	,	go stump
    500	,	drop fish
    500	,	drop net
    500,score
    500	,	take mud
    500	,	go down
    500	,	take flint
    500	,	go hole
    500	,	light lamp
    500	,	drop flint
    500	,	go hole
    500	,	go hall
    500	,	go down
    500	,	go north
    500	,	save game
    500	,	go north
    500	,	take honey
    500	,	pour water
    500	,	take bees
    500	,	go south
    500	,	go south
    500	,	go up
    500	,	go up
    500	,	unlight lamp
    500	,	go up
    500	,	drop mud
    500	,	go up
    500	,	go north
    500	,	drop bees
    500	,	take eggs
    500	,	go south
    500	,	go stump
    500	,	drop eggs
    500	,	drop honey
    500	,	Score

    */

}
