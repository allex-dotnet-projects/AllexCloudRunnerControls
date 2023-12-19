using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace AllexCloudRunnerControls
{
    public partial class SystemRunner : UserControl
    {
        #region Static Methods
        private static bool StartProcess (string path, bool localnode)
        {
            if (!File.Exists (path))
            {
                return false;
            }
            Process proc = new Process();
            if (localnode)
            {
                proc.StartInfo.EnvironmentVariables["Path"] = "nodejs;" + proc.StartInfo.EnvironmentVariables["Path"];
            }
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.FileName = Path.Combine(localnode ? "nodejs" : "", "node.exe");
            proc.StartInfo.Arguments = path;
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.EnableRaisingEvents = false;
            return true;
        }
        private static void StartPortOffice(bool localnode)
        {
            if (!localnode)
            {
                string? appdatapath = System.Environment.GetEnvironmentVariable("APPDATA");
                if (String.IsNullOrWhiteSpace(appdatapath))
                {
                    return;
                }
                if (StartProcess(System.IO.Path.Combine(appdatapath, "npm", "node_modules", "allex_portofficeserverruntimelib", "portoffice.js"), false)) return;
                if (StartProcess(System.IO.Path.Combine(appdatapath, "npm", "node_modules", "allex", "node_modules", "allex_portofficeserverruntimelib", "portoffice.js"), false)) return;
                return;
            }
            if (StartProcess(System.IO.Path.Combine("node_modules", "allex_portofficeserverruntimelib", "portoffice.js"), false)) return;
            if (StartProcess(System.IO.Path.Combine("node_modules", "allex", "node_modules", "allex_portofficeserverruntimelib", "portoffice.js"), false)) return;
        }
        #endregion

        #region Classes
        private class FormCloser
        {
            #region Fields
            Form m_Form;
            SystemRunner m_Runner;
            string m_OriginalFormText="";
            int m_PulseCount = 0;
            private System.Windows.Forms.Timer? m_ClosingTimer = null;
            #endregion

            #region Ctor
            public FormCloser(Form form, SystemRunner runner)
            {
                m_Form = form;
                m_Runner = runner;
                m_OriginalFormText = m_Form.Text;
                m_Form.FormClosing += OnFormClosing;
            }

            private void OnFormClosing(object? sender, FormClosingEventArgs e)
            {
                if (m_Runner.CanClose)
                {
                    m_Form.FormClosing -= OnFormClosing;
                    return;
                }
                if (m_PulseCount % 10 == 0)
                {
                    m_Form.Text = "Closing " + m_OriginalFormText + PulseDots();
                }
                m_PulseCount++;
                e.Cancel = true;
                m_Runner.Stop(true);
                if (m_ClosingTimer == null)
                {
                    m_ClosingTimer = new System.Windows.Forms.Timer();
                    m_ClosingTimer.Interval = 100;
                    m_ClosingTimer.Tick += OnTimer; ;
                    m_ClosingTimer.Start();
                }
            }

            private void OnTimer(object? sender, EventArgs e)
            {
                if (m_ClosingTimer != null)
                {
                    m_ClosingTimer.Tick -= OnTimer;
                    m_ClosingTimer.Stop();
                    m_ClosingTimer.Dispose();
                }
                m_ClosingTimer = null;
                m_Form.Close();
            }

            private string PulseDots ()
            {
                string ret = " ";
                for (int i = 0; i< m_PulseCount%40; i+=10)
                {
                    ret += ".";
                }
                return ret;
            }
            #endregion
        }
        #endregion

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
            get { return LMRunner.LogDirRootName; }
            set
            {
                LMRunner.LogDirRootName = value;
                AMRunner.LogDirRootName = value;
            }
        }
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Process"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool? LocalNode
        {
            get { return LMRunner.LocalNode; }
            set
            {
                LMRunner.LocalNode = value;
                AMRunner.LocalNode = value;
                if (value==null)
                {
                    return;
                }
                StartPortOffice(value == true);
                var pidfilename = "allexmaster.pid";
                if (File.Exists(pidfilename))
                    File.Delete(pidfilename);
                if (value == false)
                {
                    string? appdatapath = System.Environment.GetEnvironmentVariable("APPDATA");
                    if (String.IsNullOrWhiteSpace(appdatapath))
                    {
                        return;
                    }
                    LMRunner.JSFileName = System.IO.Path.Combine(appdatapath, "npm", "node_modules", "allex", "lanmanager.js");
                    AMRunner.JSFileName = System.IO.Path.Combine(appdatapath, "npm", "node_modules", "allex", "master.js");
                    return;
                }
                LMRunner.JSFileName = System.IO.Path.Combine("node_modules", "allex", "lanmanager.js");
                AMRunner.JSFileName = System.IO.Path.Combine("node_modules", "allex", "master.js");
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
            get { return LMRunner.ProcessClosed && AMRunner.ProcessClosed; }
        }
        #endregion

        #region Public Methods
        public void Stop (bool forever = false)
        {
            LMRunner.Stop (forever);
            AMRunner.Stop (forever);
        }
        public void AttachForClose (Form f)
        {
            _ = new FormCloser(f, this);
        }
        #endregion
    }
}
