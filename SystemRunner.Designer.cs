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
            LMLogger = new ProcessLogger();
            AMLogger = new ProcessLogger();
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
            SplitContainer.Panel1.Controls.Add(LMLogger);
            // 
            // SplitContainer.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(AMLogger);
            SplitContainer.Size = new Size(884, 613);
            SplitContainer.SplitterDistance = 884*m_PanelRatio/100;
            SplitContainer.TabIndex = 0;
            // 
            // LMLogger
            // 
            LMLogger.Dock = DockStyle.Fill;
            LMLogger.JSFileName = "";
            LMLogger.LocalNode = null;
            LMLogger.Location = new Point(0, 0);
            LMLogger.LogDirRootName = "";
            LMLogger.Margin = new Padding(4, 3, 4, 3);
            LMLogger.Name = "LMLogger";
            LMLogger.Size = new Size(294, 613);
            LMLogger.TabIndex = 0;
            // 
            // AMLogger
            // 
            AMLogger.Dock = DockStyle.Fill;
            AMLogger.JSFileName = "";
            AMLogger.LocalNode = null;
            AMLogger.Location = new Point(0, 0);
            AMLogger.LogDirRootName = "";
            AMLogger.Margin = new Padding(4, 3, 4, 3);
            AMLogger.Name = "AMLogger";
            AMLogger.Size = new Size(586, 613);
            AMLogger.TabIndex = 0;
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
        private ProcessLogger LMLogger;
        private ProcessLogger AMLogger;
    }
}
