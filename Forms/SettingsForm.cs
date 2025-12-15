using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace DebloaterTool
{
    public partial class SettingsForm : Form
    {
        public string local = null;

        public SettingsForm(string data)
        {
            InitializeComponent();
            local = data;
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;
            flowLayoutPanel1.HorizontalScroll.Enabled = false;
            flowLayoutPanel1.HorizontalScroll.Visible = false;
            LoadModules();

            using (var eulaform = new EULAForm())
            {
                eulaform.ShowDialog();
            }
        }

        public class ModuleItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public bool Default { get; set; }
        }

        // Add a property to hold selected modules
        public List<string> SelectedModules { get; private set; } = new List<string>();

        private void LoadModules()
        {
            var serializer = new JavaScriptSerializer();
            var modules = serializer.Deserialize<List<ModuleItem>>(local);

            foreach (var module in modules)
            {
                // Container panel for each row
                var panel = new Panel
                {
                    Width = flowLayoutPanel1.ClientSize.Width - 25,
                    Height = 45,
                    Margin = new Padding(5)
                };

                // Checkbox
                var checkBox = new CheckBox
                {
                    Checked = module.Default,
                    AutoSize = true,
                    Location = new Point(5, 12),
                    Tag = module.Name // useful later
                };

                // Title label (Name)
                var nameLabel = new Label
                {
                    Text = module.Name,
                    AutoSize = true,
                    Font = new Font(Font, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(30, 5)
                };

                // Description label
                var descLabel = new Label
                {
                    Text = module.Description,
                    AutoSize = true,
                    ForeColor = Color.DimGray,
                    Location = new Point(30, 22)
                };

                panel.Controls.Add(checkBox);
                panel.Controls.Add(nameLabel);
                panel.Controls.Add(descLabel);

                flowLayoutPanel1.Controls.Add(panel);
            }

            // Handle resizing to prevent horizontal scroll
            flowLayoutPanel1.SizeChanged += (s, e) =>
            {
                foreach (Panel p in flowLayoutPanel1.Controls.OfType<Panel>())
                {
                    p.Width = flowLayoutPanel1.ClientSize.Width - SystemInformation.VerticalScrollBarWidth;
                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Collect all checked modules
            var selectedModules = flowLayoutPanel1.Controls
                .OfType<Panel>()                          // each module panel
                .Select(p => p.Controls.OfType<CheckBox>().First()) // get checkbox
                .Where(cb => cb.Checked)                   // only checked
                .Select(cb => cb.Tag.ToString())           // get module name
                .ToList();

            if (selectedModules.Count == 0)
            {
                MessageBox.Show("No modules selected.");
                return;
            }

            SelectedModules = selectedModules;
            allowClose = false;
            Enabled = false;
        }

        private bool allowClose = true;
        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (allowClose)
            {
                Environment.Exit(0);
            } 
            else
            {
                e.Cancel = true;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (CheckBox cb in flowLayoutPanel1.Controls
                .OfType<Control>()
                .SelectMany(c => c.Controls.OfType<CheckBox>()))
            {
                cb.Checked = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (CheckBox cb in flowLayoutPanel1.Controls
                .OfType<Control>()
                .SelectMany(c => c.Controls.OfType<CheckBox>()))
            {
                cb.Checked = true;
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                byte[] bytes = wc.DownloadData("https://raw.githubusercontent.com/megsystem/megsystem/refs/heads/main/banner.png");
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    pictureBox1.Image = Image.FromStream(ms);
                }
            }
        }
    }
}
