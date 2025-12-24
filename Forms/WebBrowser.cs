using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static DebloaterTool.Program;

namespace DebloaterTool
{
    public partial class WebBrowser : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect, // x-coordinate of upper-left corner
            int nTopRect, // y-coordinate of upper-left corner
            int nRightRect, // x-coordinate of lower-right corner
            int nBottomRect, // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public WebBrowser(string url)
        {
            InitializeComponent();
            webBrowser1.Url = new Uri(url);
            winFormButton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, winFormButton.Width, winFormButton.Height, radiusButton, radiusButton));
        }

        private void Local_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationWebServer.runningDebloating) 
                { e.Cancel = true; }
            else 
                { Environment.Exit(0); }
        }

        private void winFormButton_click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Do you want to switch to WinForm mode?",
                "Switch Mode",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                string exePath = Assembly.GetExecutingAssembly().Location;
                string args = "--winform";
                Process.Start(exePath, args);
                Environment.Exit(0);
            }
        }

        private int collapsedWidth = 30;
        private int expandedWidth = 160;
        private int animationStep = 5;
        private int radiusButton = 15;
        private bool expanding = false;

        private void HelpButton_MouseEnter(object sender, EventArgs e)
        {
            expanding = true;
            animationTimer.Start();
        }

        private void HelpButton_MouseLeave(object sender, EventArgs e)
        {
            expanding = false;
            animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (expanding)
            {
                if (winFormButton.Width < expandedWidth)
                {
                    winFormButton.Text = "Page doesn't load?";
                    winFormButton.Width += animationStep;
                    winFormButton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, winFormButton.Width, winFormButton.Height, radiusButton, radiusButton));
                    if (winFormButton.Width >= expandedWidth)
                    {
                        winFormButton.Width = expandedWidth;
                        animationTimer.Stop();
                    }
                }
            }
            else
            {
                if (winFormButton.Width > collapsedWidth)
                {
                    winFormButton.Width -= animationStep;
                    winFormButton.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, winFormButton.Width, winFormButton.Height, radiusButton, radiusButton));
                    if (winFormButton.Width <= collapsedWidth)
                    {
                        winFormButton.Width = collapsedWidth;
                        winFormButton.Text = "?";
                        animationTimer.Stop();
                    }
                }
            }
        }
    }
}
