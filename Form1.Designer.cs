namespace QTTest2
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ClearQTButton = new System.Windows.Forms.Button();
            this.DeepQuarterQTButton = new System.Windows.Forms.Button();
            this.NodeToQuarterTextbox = new System.Windows.Forms.TextBox();
            this.TestButton = new System.Windows.Forms.Button();
            this.RefreshQTButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.SetupAStarNodesButton = new System.Windows.Forms.Button();
            this.ShowAStarCheckbox = new System.Windows.Forms.CheckBox();
            this.ShowSmoothingCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.HModTextbox = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.LoadButton = new System.Windows.Forms.Button();
            this.WalkButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(4, 5);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(133, 94);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // ClearQTButton
            // 
            this.ClearQTButton.Enabled = false;
            this.ClearQTButton.Location = new System.Drawing.Point(16, 10);
            this.ClearQTButton.Margin = new System.Windows.Forms.Padding(4);
            this.ClearQTButton.Name = "ClearQTButton";
            this.ClearQTButton.Size = new System.Drawing.Size(159, 28);
            this.ClearQTButton.TabIndex = 1;
            this.ClearQTButton.Text = "Clear QT";
            this.ClearQTButton.UseVisualStyleBackColor = true;
            this.ClearQTButton.Click += new System.EventHandler(this.ClearQTButton_Click);
            // 
            // DeepQuarterQTButton
            // 
            this.DeepQuarterQTButton.Enabled = false;
            this.DeepQuarterQTButton.Location = new System.Drawing.Point(16, 46);
            this.DeepQuarterQTButton.Margin = new System.Windows.Forms.Padding(4);
            this.DeepQuarterQTButton.Name = "DeepQuarterQTButton";
            this.DeepQuarterQTButton.Size = new System.Drawing.Size(159, 28);
            this.DeepQuarterQTButton.TabIndex = 2;
            this.DeepQuarterQTButton.Text = "Deep Quarter QT";
            this.DeepQuarterQTButton.UseVisualStyleBackColor = true;
            this.DeepQuarterQTButton.Click += new System.EventHandler(this.DeepQuarterQTButton_Click);
            // 
            // NodeToQuarterTextbox
            // 
            this.NodeToQuarterTextbox.Enabled = false;
            this.NodeToQuarterTextbox.Location = new System.Drawing.Point(180, 81);
            this.NodeToQuarterTextbox.Margin = new System.Windows.Forms.Padding(4);
            this.NodeToQuarterTextbox.Name = "NodeToQuarterTextbox";
            this.NodeToQuarterTextbox.Size = new System.Drawing.Size(155, 22);
            this.NodeToQuarterTextbox.TabIndex = 4;
            this.NodeToQuarterTextbox.Text = "0";
            // 
            // TestButton
            // 
            this.TestButton.Location = new System.Drawing.Point(180, 10);
            this.TestButton.Margin = new System.Windows.Forms.Padding(4);
            this.TestButton.Name = "TestButton";
            this.TestButton.Size = new System.Drawing.Size(159, 28);
            this.TestButton.TabIndex = 6;
            this.TestButton.Text = "Find Path";
            this.TestButton.UseVisualStyleBackColor = true;
            this.TestButton.Click += new System.EventHandler(this.TestButton_Click);
            // 
            // RefreshQTButton
            // 
            this.RefreshQTButton.Location = new System.Drawing.Point(180, 46);
            this.RefreshQTButton.Margin = new System.Windows.Forms.Padding(4);
            this.RefreshQTButton.Name = "RefreshQTButton";
            this.RefreshQTButton.Size = new System.Drawing.Size(159, 28);
            this.RefreshQTButton.TabIndex = 7;
            this.RefreshQTButton.Text = "Refresh Image";
            this.RefreshQTButton.UseVisualStyleBackColor = true;
            this.RefreshQTButton.Click += new System.EventHandler(this.RefreshQTButton_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(347, 10);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(552, 423);
            this.panel1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Enabled = false;
            this.label1.Location = new System.Drawing.Point(63, 85);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "New Node Cost";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 442);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(915, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // SetupAStarNodesButton
            // 
            this.SetupAStarNodesButton.Location = new System.Drawing.Point(16, 113);
            this.SetupAStarNodesButton.Margin = new System.Windows.Forms.Padding(4);
            this.SetupAStarNodesButton.Name = "SetupAStarNodesButton";
            this.SetupAStarNodesButton.Size = new System.Drawing.Size(159, 28);
            this.SetupAStarNodesButton.TabIndex = 13;
            this.SetupAStarNodesButton.Text = "Setup A* Nodes";
            this.SetupAStarNodesButton.UseVisualStyleBackColor = true;
            this.SetupAStarNodesButton.Click += new System.EventHandler(this.SetupAStarNodesButton_Click);
            // 
            // ShowAStarCheckbox
            // 
            this.ShowAStarCheckbox.AutoSize = true;
            this.ShowAStarCheckbox.Location = new System.Drawing.Point(16, 149);
            this.ShowAStarCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.ShowAStarCheckbox.Name = "ShowAStarCheckbox";
            this.ShowAStarCheckbox.Size = new System.Drawing.Size(137, 21);
            this.ShowAStarCheckbox.TabIndex = 14;
            this.ShowAStarCheckbox.Text = "Show A* Process";
            this.ShowAStarCheckbox.UseVisualStyleBackColor = true;
            // 
            // ShowSmoothingCheckbox
            // 
            this.ShowSmoothingCheckbox.AutoSize = true;
            this.ShowSmoothingCheckbox.Enabled = false;
            this.ShowSmoothingCheckbox.Location = new System.Drawing.Point(16, 177);
            this.ShowSmoothingCheckbox.Margin = new System.Windows.Forms.Padding(4);
            this.ShowSmoothingCheckbox.Name = "ShowSmoothingCheckbox";
            this.ShowSmoothingCheckbox.Size = new System.Drawing.Size(190, 21);
            this.ShowSmoothingCheckbox.TabIndex = 15;
            this.ShowSmoothingCheckbox.Text = "Show Smoothing Process";
            this.ShowSmoothingCheckbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 202);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 17);
            this.label2.TabIndex = 17;
            this.label2.Text = "Heuristic Mod";
            // 
            // HModTextbox
            // 
            this.HModTextbox.Location = new System.Drawing.Point(129, 198);
            this.HModTextbox.Margin = new System.Windows.Forms.Padding(4);
            this.HModTextbox.Name = "HModTextbox";
            this.HModTextbox.Size = new System.Drawing.Size(155, 22);
            this.HModTextbox.TabIndex = 16;
            this.HModTextbox.Text = "1";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(180, 113);
            this.SaveButton.Margin = new System.Windows.Forms.Padding(4);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(156, 28);
            this.SaveButton.TabIndex = 1;
            this.SaveButton.Text = "Save QT";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(180, 148);
            this.LoadButton.Margin = new System.Windows.Forms.Padding(4);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(156, 28);
            this.LoadButton.TabIndex = 18;
            this.LoadButton.Text = "Load QT";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // WalkButton
            // 
            this.WalkButton.Location = new System.Drawing.Point(16, 230);
            this.WalkButton.Margin = new System.Windows.Forms.Padding(4);
            this.WalkButton.Name = "WalkButton";
            this.WalkButton.Size = new System.Drawing.Size(320, 28);
            this.WalkButton.TabIndex = 19;
            this.WalkButton.Text = "Walk This Path";
            this.WalkButton.UseVisualStyleBackColor = true;
            this.WalkButton.Click += new System.EventHandler(this.WalkButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 464);
            this.Controls.Add(this.WalkButton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.HModTextbox);
            this.Controls.Add(this.ShowSmoothingCheckbox);
            this.Controls.Add(this.ShowAStarCheckbox);
            this.Controls.Add(this.SetupAStarNodesButton);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.RefreshQTButton);
            this.Controls.Add(this.TestButton);
            this.Controls.Add(this.NodeToQuarterTextbox);
            this.Controls.Add(this.DeepQuarterQTButton);
            this.Controls.Add(this.ClearQTButton);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button ClearQTButton;
        private System.Windows.Forms.Button DeepQuarterQTButton;
        private System.Windows.Forms.TextBox NodeToQuarterTextbox;
        private System.Windows.Forms.Button TestButton;
        private System.Windows.Forms.Button RefreshQTButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.Button SetupAStarNodesButton;
        private System.Windows.Forms.CheckBox ShowAStarCheckbox;
        private System.Windows.Forms.CheckBox ShowSmoothingCheckbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox HModTextbox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button WalkButton;
    }
}

