namespace DebloaterTool
{
    partial class EULAForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EULAForm));
            this.acceptButton = new System.Windows.Forms.Button();
            this.eulaTextBox = new System.Windows.Forms.TextBox();
            this.titleLabel = new System.Windows.Forms.Label();
            this.updatedLabel = new System.Windows.Forms.Label();
            this.dontacceptButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // acceptButton
            // 
            this.acceptButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.acceptButton.FlatAppearance.BorderSize = 0;
            this.acceptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.acceptButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.acceptButton.ForeColor = System.Drawing.Color.White;
            this.acceptButton.Location = new System.Drawing.Point(513, 409);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(100, 36);
            this.acceptButton.TabIndex = 3;
            this.acceptButton.Text = "Accept";
            this.acceptButton.UseVisualStyleBackColor = false;
            this.acceptButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // eulaTextBox
            // 
            this.eulaTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.eulaTextBox.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.eulaTextBox.ForeColor = System.Drawing.Color.White;
            this.eulaTextBox.Location = new System.Drawing.Point(20, 80);
            this.eulaTextBox.Multiline = true;
            this.eulaTextBox.Name = "eulaTextBox";
            this.eulaTextBox.ReadOnly = true;
            this.eulaTextBox.Size = new System.Drawing.Size(593, 323);
            this.eulaTextBox.TabIndex = 2;
            this.eulaTextBox.Text = resources.GetString("eulaTextBox.Text");
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(20, 20);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(357, 25);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "End User License Agreement (EULA) 📜";
            // 
            // updatedLabel
            // 
            this.updatedLabel.AutoSize = true;
            this.updatedLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.updatedLabel.ForeColor = System.Drawing.Color.LightGray;
            this.updatedLabel.Location = new System.Drawing.Point(20, 50);
            this.updatedLabel.Name = "updatedLabel";
            this.updatedLabel.Size = new System.Drawing.Size(181, 15);
            this.updatedLabel.TabIndex = 1;
            this.updatedLabel.Text = "Last Updated: December 14, 2025";
            // 
            // dontacceptButton
            // 
            this.dontacceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dontacceptButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.dontacceptButton.FlatAppearance.BorderSize = 0;
            this.dontacceptButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dontacceptButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dontacceptButton.ForeColor = System.Drawing.Color.White;
            this.dontacceptButton.Location = new System.Drawing.Point(20, 409);
            this.dontacceptButton.Name = "dontacceptButton";
            this.dontacceptButton.Size = new System.Drawing.Size(120, 36);
            this.dontacceptButton.TabIndex = 5;
            this.dontacceptButton.Text = "Don\'t Accept";
            this.dontacceptButton.UseVisualStyleBackColor = false;
            this.dontacceptButton.Click += new System.EventHandler(this.dontacceptButton_Click);
            // 
            // EULAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(625, 457);
            this.Controls.Add(this.dontacceptButton);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.eulaTextBox);
            this.Controls.Add(this.updatedLabel);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EULAForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EULA";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EULAForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.TextBox eulaTextBox;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label updatedLabel;
        private System.Windows.Forms.Button dontacceptButton;
    }
}
