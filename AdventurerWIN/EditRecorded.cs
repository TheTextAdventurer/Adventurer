using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdventurerWIN
{
    public partial class EditRecorded : Form
    {
        public EditRecorded()
        {
            InitializeComponent();
        }

        RecordGame r = null;

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "rec files (*.rec)|*.rec";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    r = RecordGame.Load(ofd.FileName);
                    SetForm();
                }
            }
        }

        private void SetForm()
        {
            propertyGrid1.SelectedObject = r;
            textBox1.Lines = r.PlayerInput.Select(i => i.ToString()).ToArray();
        }

        //save code
        private void button2_Click(object sender, EventArgs e)
        {
            r.PlayerInput =
                textBox1.Lines.Select(l =>

                    new RecordGame.Input(
                        Convert.ToInt32(l.Substring(0, l.IndexOf(",",0)))
                        , l.Substring(l.IndexOf(",", 0)+1).Trim()
                        )

                ).ToList();

            RecordGame.Save(r);
            SetForm();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (r != null)
            {
                r = RecordGame.Load(r.File);
                SetForm();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure", "Are you sure", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Regex r = new Regex(textBox2.Text, RegexOptions.Multiline);
                textBox1.Text = r.Replace(textBox1.Text, textBox3.Text);
            }
        }
    }
}
