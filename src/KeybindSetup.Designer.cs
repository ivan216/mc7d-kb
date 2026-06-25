namespace _3dedit
{
    partial class KeybindSetup
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
            this.keybindSetsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.addNewLayout = new System.Windows.Forms.Button();
            this.keybindsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.addKeybind = new System.Windows.Forms.Button();
            this.DeleteLayout = new System.Windows.Forms.Button();
            this.keybindSetsPanel.SuspendLayout();
            this.keybindsPanel.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // keybindSetsPanel
            // 
            this.keybindSetsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.keybindSetsPanel.AutoScroll = true;
            this.keybindSetsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.keybindSetsPanel.Controls.Add(this.addNewLayout);
            this.keybindSetsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.keybindSetsPanel.Location = new System.Drawing.Point(16, 16);
            this.keybindSetsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.keybindSetsPanel.Name = "keybindSetsPanel";
            this.keybindSetsPanel.Size = new System.Drawing.Size(241, 565);
            this.keybindSetsPanel.TabIndex = 0;
            this.keybindSetsPanel.WrapContents = false;
            // 
            // addNewLayout
            // 
            this.addNewLayout.Location = new System.Drawing.Point(4, 4);
            this.addNewLayout.Margin = new System.Windows.Forms.Padding(4);
            this.addNewLayout.Name = "addNewLayout";
            this.addNewLayout.Size = new System.Drawing.Size(224, 47);
            this.addNewLayout.TabIndex = 0;
            this.addNewLayout.Text = "Add New Layout";
            this.addNewLayout.UseVisualStyleBackColor = true;
            this.addNewLayout.Click += new System.EventHandler(this.AddNewLayout_Click);
            // 
            // keybindsPanel
            // 
            this.keybindsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.keybindsPanel.AutoScroll = true;
            this.keybindsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.keybindsPanel.Controls.Add(this.flowLayoutPanel1);
            this.keybindsPanel.Controls.Add(this.flowLayoutPanel2);
            this.keybindsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.keybindsPanel.Location = new System.Drawing.Point(265, 16);
            this.keybindsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.keybindsPanel.Name = "keybindsPanel";
            this.keybindsPanel.Size = new System.Drawing.Size(875, 565);
            this.keybindsPanel.TabIndex = 1;
            this.keybindsPanel.WrapContents = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(4, 4);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(0, 0);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.WrapContents = false;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.addKeybind);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(4, 12);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(184, 39);
            this.flowLayoutPanel2.TabIndex = 3;
            // 
            // addKeybind
            // 
            this.addKeybind.Location = new System.Drawing.Point(4, 4);
            this.addKeybind.Margin = new System.Windows.Forms.Padding(4);
            this.addKeybind.Name = "addKeybind";
            this.addKeybind.Size = new System.Drawing.Size(176, 31);
            this.addKeybind.TabIndex = 2;
            this.addKeybind.Text = "Add Keybind";
            this.addKeybind.UseVisualStyleBackColor = true;
            this.addKeybind.Click += new System.EventHandler(this.AddKeybind_Click);
            // 
            // DeleteLayout
            // 
            this.DeleteLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteLayout.Location = new System.Drawing.Point(957, 600);
            this.DeleteLayout.Margin = new System.Windows.Forms.Padding(4);
            this.DeleteLayout.Name = "DeleteLayout";
            this.DeleteLayout.Size = new System.Drawing.Size(183, 40);
            this.DeleteLayout.TabIndex = 3;
            this.DeleteLayout.Text = "Delete Layout";
            this.DeleteLayout.UseVisualStyleBackColor = true;
            this.DeleteLayout.Click += new System.EventHandler(this.DeleteLayout_Click);
            // 
            // KeybindSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1153, 656);
            this.Controls.Add(this.DeleteLayout);
            this.Controls.Add(this.keybindsPanel);
            this.Controls.Add(this.keybindSetsPanel);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "KeybindSetup";
            this.Text = "Keybind Setup";
            this.Load += new System.EventHandler(this.KeybindSetup_Load);
            this.keybindSetsPanel.ResumeLayout(false);
            this.keybindsPanel.ResumeLayout(false);
            this.keybindsPanel.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel keybindSetsPanel;
        private System.Windows.Forms.Button addNewLayout;
        private System.Windows.Forms.FlowLayoutPanel keybindsPanel;
        private System.Windows.Forms.Button DeleteLayout;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button addKeybind;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}