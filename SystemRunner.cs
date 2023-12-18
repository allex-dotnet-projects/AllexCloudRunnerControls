using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllexCloudRunnerControls
{
    public partial class SystemRunner : UserControl
    {
        #region Fields
        private int m_PanelRatio=33;
        #endregion

        #region Ctor
        public SystemRunner()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Properties
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Process"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string LogDirRootName
        {
            get { return LMLogger.LogDirRootName; }
            set
            {
                LMLogger.LogDirRootName = value;
                AMLogger.LogDirRootName = value;
            }
        }
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Process"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool? LocalNode
        {
            get { return LMLogger.LocalNode; }
            set
            {
                LMLogger.LocalNode = value;
                AMLogger.LocalNode = value;
                if (value==null)
                {
                    return;
                }
                if (value == false)
                {
                    string? appdatapath = System.Environment.GetEnvironmentVariable("APPDATA");
                    if (String.IsNullOrWhiteSpace(appdatapath))
                    {
                        return;
                    }
                    LMLogger.JSFileName = System.IO.Path.Combine(appdatapath, "npm", "node_modules", "allex", "lanmanager.js");
                    AMLogger.JSFileName = System.IO.Path.Combine(appdatapath, "npm", "node_modules", "allex", "master.js");
                    return;
                }
                LMLogger.JSFileName = System.IO.Path.Combine("node_modules", "allex", "lanmanager.js");
                AMLogger.JSFileName = System.IO.Path.Combine("node_modules", "allex", "master.js");
            }
        }
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Design"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int PanelRatio
        {
            get { return m_PanelRatio; }
            set
            {
                m_PanelRatio = value;
                SplitContainer.SplitterDistance = SplitContainer.Width*value/100;
            }
        }
        public bool CanClose
        {
            get { return LMLogger.ProcessClosed && AMLogger.ProcessClosed; }
        }
        #endregion

        #region Public Methods
        public void Stop ()
        {
            LMLogger.Stop ();
            AMLogger.Stop ();
        }
        #endregion
    }
}
