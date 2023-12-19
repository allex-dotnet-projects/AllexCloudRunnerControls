namespace AllexCloudRunnerControls
{
    partial class SystemRunner
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
            SplitContainer = new SplitContainer();
            LMRunner = new ProcessRunner();
            AMRunner = new ProcessRunner();
            ((System.ComponentModel.ISupportInitialize)SplitContainer).BeginInit();
            SplitContainer.Panel1.SuspendLayout();
            SplitContainer.Panel2.SuspendLayout();
            SplitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // SplitContainer
            // 
            SplitContainer.Dock = DockStyle.Fill;
            SplitContainer.Location = new Point(0, 0);
            SplitContainer.Name = "SplitContainer";
            // 
            // SplitContainer.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(LMRunner);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(AMRunner);
            SplitContainer.Size = new Size(884, 613);
            SplitContainer.SplitterDistance = 884*m_PanelRatio/100;
            SplitContainer.TabIndex = 0;
            // 
            // LMLogger
            // 
            LMRunner.Dock = DockStyle.Fill;
            LMRunner.JSFileName = "";
            LMRunner.LocalNode = null;
            LMRunner.Location = new Point(0, 0);
            LMRunner.LogDirRootName = "";
            LMRunner.Margin = new Padding(4, 3, 4, 3);
            LMRunner.Name = "LMLogger";
            LMRunner.Size = new Size(294, 613);
            LMRunner.TabIndex = 0;
            // 
            // AMLogger
            // 
            AMRunner.Dock = DockStyle.Fill;
            AMRunner.JSFileName = "";
            AMRunner.LocalNode = null;
            AMRunner.Location = new Point(0, 0);
            AMRunner.LogDirRootName = "";
            AMRunner.Margin = new Padding(4, 3, 4, 3);
            AMRunner.Name = "AMLogger";
            AMRunner.Size = new Size(586, 613);
            AMRunner.TabIndex = 0;
            // 
            // SystemRunner
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(SplitContainer);
            Name = "SystemRunner";
            Size = new Size(884, 613);
            SplitContainer.Panel1.ResumeLayout(false);
            SplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SplitContainer).EndInit();
            SplitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer SplitContainer;
        private ProcessRunner LMRunner;
        private ProcessRunner AMRunner;
    }
}
