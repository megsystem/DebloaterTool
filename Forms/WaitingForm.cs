using System;
using System.Drawing;
using System.Windows.Forms;
using static DebloaterTool.Program;

namespace DebloaterTool
{
    public partial class WaitingForm : Form
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        public bool AllowClose { get; set; } = false;
        private void WaitingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AllowClose)
            {
                e.Cancel = true;
                return;
            }
        }

        int posX = 0;
        int posY = 0;

        private void checkPercentage_Tick(object sender, System.EventArgs e)
        {
            progressBar1.Value = ApiResponse.percetage;
            this.Location = new Point(posX, posY);
            label1.Text = $"Work: {ApiResponse.percetage}%";
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = (this.ClientSize.Height - label1.Height) / 2;
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            posX = this.Location.X;
            posY = this.Location.Y;
        }
    }
}
