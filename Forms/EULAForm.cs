using System;
using System.Windows.Forms;

namespace DebloaterTool
{
    public partial class EULAForm : Form
    {
        public EULAForm()
        {
            InitializeComponent();
        }

        // Block closing by default
        private void EULAForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!allowClose) // only allow close when the button is clicked
            {
                e.Cancel = true;
            }
        }

        // A field to track whether the form can close
        private bool allowClose = false;

        // Button click allows the form to close
        private void button1_Click(object sender, EventArgs e)
        {
            allowClose = true; // mark that closing is allowed
            this.Close();      // triggers FormClosing event
        }

        private void dontacceptButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
