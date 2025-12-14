using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DebloaterTool
{
    public partial class WebBrowser : Form
    {
        public WebBrowser(string url)
        {
            InitializeComponent();
        }

        private void Local_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
