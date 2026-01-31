using System;
using System.Windows.Forms;
using static DebloaterTool.Program;

namespace DebloaterTool
{
    public partial class WebBrowser : Form
    {
        public WebBrowser(string url)
        {
            InitializeComponent();
            webBrowser1.Url = new Uri(url);
        }

        private void Local_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApiResponse.percetage > 0 && ApiResponse.percetage < 100) 
                { e.Cancel = true; }
            else 
                { Environment.Exit(0); }
        }

        private WaitingForm _waitingForm;
        private void checkInfo_Tick(object sender, EventArgs e)
        {
            if (ApiResponse.percetage > 0 && ApiResponse.percetage < 100)
            {
                Opacity = 0.5;

                if (_waitingForm == null || _waitingForm.IsDisposed)
                {
                    _waitingForm = new WaitingForm();
                    _waitingForm.AllowClose = false;
                    _waitingForm.ShowDialog(this);
                    _waitingForm = null;
                }
            }
            else
            {
                Opacity = 1;

                if (_waitingForm != null && !_waitingForm.IsDisposed)
                {
                    _waitingForm.AllowClose = true;
                    _waitingForm.Close();
                }
            }
        }
    }
}
