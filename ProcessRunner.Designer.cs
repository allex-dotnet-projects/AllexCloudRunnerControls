using System.Windows.Forms.VisualStyles;

namespace AllexCloudRunnerControls
{
    partial class ProcessRunner
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new System.Windows.Forms.Panel();
            logBox = new System.Windows.Forms.TextBox();
            startButt = new System.Windows.Forms.Button();
            stopButt = new System.Windows.Forms.Button();
            clearButt = new System.Windows.Forms.Button();
            startToolTip = new ToolTip();
            stopToolTip = new ToolTip();
            clearToolTip = new ToolTip();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(this.clearButt);
            panel1.Controls.Add(this.stopButt);
            panel1.Controls.Add(this.startButt);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(569, 16);
            panel1.TabIndex = 0;
            // 
            // logBox
            // 
            logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            logBox.Location = new System.Drawing.Point(0, 16);
            logBox.Multiline = true;
            logBox.Name = "logBox";
            logBox.ReadOnly = true;
            logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            logBox.Size = new System.Drawing.Size(569, 351);
            logBox.BorderStyle = BorderStyle.None;
            logBox.TabIndex = 1;
            // 
            // startButt
            // 
            startButt.Location = new System.Drawing.Point(4, 0);
            startButt.Name = "startButt";
            startButt.BackgroundImage = Resources.Run;
            startButt.BackgroundImageLayout = ImageLayout.Stretch;
            startButt.Size = new System.Drawing.Size(14, 14);
            startButt.Padding = new Padding(0);
            startButt.FlatStyle = FlatStyle.Flat;
            startButt.FlatAppearance.BorderSize = 0;
            startButt.TabIndex = 0;
            //startButt.Text = "Start";
            startButt.UseVisualStyleBackColor = true;
            startButt.Click += new System.EventHandler(this.startButt_Click);
            // 
            // stopButt
            // 
            stopButt.Location = new System.Drawing.Point(4, 0);
            stopButt.Name = "stopButt";
            stopButt.BackgroundImage = Resources.Stop;
            stopButt.BackgroundImageLayout = ImageLayout.Stretch;
            stopButt.Size = new System.Drawing.Size(14, 14);
            stopButt.Padding = new Padding(0);
            stopButt.FlatStyle = FlatStyle.Flat;
            stopButt.FlatAppearance.BorderSize = 0;
            stopButt.TabIndex = 1;
            //stopButt.Text = "Stop";
            stopButt.UseVisualStyleBackColor = true;
            stopButt.Click += new System.EventHandler(this.stopButt_Click);
            // 
            // clearButt
            // 
            clearButt.Anchor = System.Windows.Forms.AnchorStyles.Right;
            clearButt.Location = new System.Drawing.Point(550, 2);
            clearButt.Name = "clearButt";
            clearButt.BackgroundImage = Resources.Clear;
            clearButt.BackgroundImageLayout = ImageLayout.Stretch;
            clearButt.Size = new System.Drawing.Size(12, 12);
            clearButt.FlatStyle = FlatStyle.Flat;
            clearButt.FlatAppearance.BorderSize = 0;
            clearButt.TabIndex = 2;
            //clearButt.Text = "Clear";
            clearButt.UseVisualStyleBackColor = true;
            clearButt.Click += new System.EventHandler(this.clearButt_Click);
            //
            // startToolTip
            //
            startToolTip.SetToolTip(startButt, "Start");
            //
            // stopToolTip
            //
            stopToolTip.SetToolTip(stopButt, "Stop");
            //
            // clearToolTip
            //
            clearToolTip.SetToolTip(clearButt, "Clear Log Display");
            // 
            // ProcessLogger
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(this.logBox);
            Controls.Add(this.panel1);
            Name = "ProcessLogger";
            Size = new System.Drawing.Size(569, 382);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button stopButt;
        private System.Windows.Forms.Button startButt;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Button clearButt;
        private System.Windows.Forms.ToolTip startToolTip;
        private System.Windows.Forms.ToolTip stopToolTip;
        private System.Windows.Forms.ToolTip clearToolTip;
    }
}
