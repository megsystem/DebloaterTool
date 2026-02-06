using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static DebloaterTool.Program;

namespace DebloaterTool
{
    public partial class WebBrowser : Form
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd,
                                       DWMWINDOWATTRIBUTE attribute,
                                       ref int pvAttribute,
                                       uint cbAttribute);

        public enum DWMWINDOWATTRIBUTE : uint
        {
            DWMWA_NCRENDERING_ENABLED,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_PASSIVE_UPDATE_MODE,
            DWMWA_USE_HOSTBACKDROPBRUSH,
            DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19,
            DWMWA_USE_IMMERSIVE_DARK_MODE,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_BORDER_COLOR,
            DWMWA_CAPTION_COLOR,
            DWMWA_TEXT_COLOR,
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
            DWMWA_SYSTEMBACKDROP_TYPE,
            DWMWA_LAST
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var preference = Convert.ToInt32(true);
            if (DwmSetWindowAttribute(this.Handle,
                                      DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                                      ref preference,
                                      sizeof(uint)) != 0)
                Marshal.ThrowExceptionForHR(
                    DwmSetWindowAttribute(this.Handle,
                                          DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1,
                                          ref preference,
                                          sizeof(uint)));
        }

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
