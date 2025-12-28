using System.Windows.Forms;

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
    }
}
