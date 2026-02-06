using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static DebloaterTool.Program;

namespace DebloaterTool
{
    public partial class WaitingForm : Form
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
