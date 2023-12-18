using System.Windows.Forms.VisualStyles;

namespace AllexCloudRunnerControls
{
    partial class ProcessLogger
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.logBox = new System.Windows.Forms.TextBox();
            this.startButt = new System.Windows.Forms.Button();
            this.stopButt = new System.Windows.Forms.Button();
            this.clearButt = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clearButt);
            this.panel1.Controls.Add(this.stopButt);
            this.panel1.Controls.Add(this.startButt);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(569, 16);
            this.panel1.TabIndex = 0;
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Location = new System.Drawing.Point(0, 16);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ReadOnly = true;
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logBox.Size = new System.Drawing.Size(569, 351);
            this.logBox.BorderStyle = BorderStyle.None;
            this.logBox.TabIndex = 1;
            // 
            // startButt
            // 
            this.startButt.Location = new System.Drawing.Point(4, 0);
            this.startButt.Name = "startButt";
            this.startButt.BackgroundImage = Resources.Run;
            this.startButt.Size = new System.Drawing.Size(10, 10);
            this.startButt.FlatStyle = FlatStyle.Flat;
            this.startButt.FlatAppearance.BorderSize = 0;
            this.startButt.TabIndex = 0;
            this.startButt.Text = "Start";
            this.startButt.UseVisualStyleBackColor = true;
            this.startButt.Click += new System.EventHandler(this.startButt_Click);
            // 
            // stopButt
            // 
            this.stopButt.Location = new System.Drawing.Point(4, 0);
            this.stopButt.Name = "stopButt";
            this.stopButt.BackgroundImage = Resources.Stop;
            this.stopButt.Size = new System.Drawing.Size(10, 10);
            this.stopButt.FlatStyle = FlatStyle.Flat;
            this.stopButt.FlatAppearance.BorderSize = 0;
            this.stopButt.TabIndex = 1;
            //this.stopButt.Text = "Stop";
            this.stopButt.UseVisualStyleBackColor = true;
            this.stopButt.Click += new System.EventHandler(this.stopButt_Click);
            // 
            // clearButt
            // 
            this.clearButt.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.clearButt.Location = new System.Drawing.Point(550, 2);
            this.clearButt.Name = "clearButt";
            this.clearButt.BackgroundImage = Resources.Clear;
            this.clearButt.Size = new System.Drawing.Size(10, 10);
            this.clearButt.FlatStyle = FlatStyle.Flat;
            this.clearButt.FlatAppearance.BorderSize = 0;
            this.clearButt.TabIndex = 2;
            this.clearButt.Text = "Clear";
            this.clearButt.UseVisualStyleBackColor = true;
            this.clearButt.Click += new System.EventHandler(this.clearButt_Click);
            // 
            // ProcessLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logBox);
            this.Controls.Add(this.panel1);
            this.Name = "ProcessLogger";
            this.Size = new System.Drawing.Size(569, 382);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button stopButt;
        private System.Windows.Forms.Button startButt;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Button clearButt;
    }
}
