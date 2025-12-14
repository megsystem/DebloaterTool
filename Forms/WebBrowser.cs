using System;
using System.Diagnostics;
using System.Reflection;
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

        private void winFormButton_click(object sender, EventArgs e)
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string args = "--winform";
            Process.Start(exePath, args);
            Environment.Exit(0);
        }
    }
}
