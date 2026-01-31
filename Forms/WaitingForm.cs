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

        private void checkPercentage_Tick(object sender, System.EventArgs e)
        {
            int targetPercentage = ApiResponse.percetage;
            if (progressBar1.Value < targetPercentage)
            {
                progressBar1.Value += 1; // increment
            }
            else if (progressBar1.Value > targetPercentage)
            {
                progressBar1.Value -= 1; // decrement if needed
            }
            label1.Text = $"Work: {progressBar1.Value}%";
        }

        private int posX = 0;
        private int posY = 0;
        private void WaitingForm_Load(object sender, EventArgs e)
        {
            posX = this.Location.X;
            posY = this.Location.Y;
        }

        private void checkLocation_Tick(object sender, EventArgs e)
        {
            this.Location = new Point(posX, posY);
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;
            label1.Top = (this.ClientSize.Height - label1.Height) / 2;
        }
    }
}
