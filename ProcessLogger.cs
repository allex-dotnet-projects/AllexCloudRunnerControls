using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace AllexCloudRunnerControls
{
    class FileLogger
    {
        #region Fields
        private string m_LogDirRootName="";
        private string m_JSFileName="";
        private string m_Buffer="";
        #endregion

        #region Properties
        public string LogDirRootName
        {
            get { return m_LogDirRootName; }
            set
            {
                m_LogDirRootName = value;
                if (!String.IsNullOrWhiteSpace(value))
                {
                    System.IO.Directory.CreateDirectory(m_LogDirRootName);
                }
            }
        }
        public string JSFileName
        {
            get { return m_JSFileName; }
            set { m_JSFileName = value; }
        }
        private string LogFilePath
        {
            get { return m_LogDirRootName + "\\" + System.IO.Path.GetFileNameWithoutExtension(m_JSFileName) + DateTime.Today.ToString("_MM_dd_yyyy") + ".txt"; }
        }
        private bool CannotLog
        {
            get { return null == m_LogDirRootName || m_LogDirRootName.Length < 1 || null == m_JSFileName || m_JSFileName.Length < 1; }
        }

        #endregion

        #region Public Methods
        public void Log(string text)
        {
            m_Buffer += text;
            DoLog();
        }
        #endregion

        #region Private Methods
        private void DoLog()
        {
            lock (this)
            {
                if (CannotLog)
                {
                    PostponeLog();
                    return;
                }
                string text = m_Buffer;
                m_Buffer = "";
                if (text.Length < 1)
                {
                    return;
                }
                try
                {
                    System.IO.File.AppendAllText(LogFilePath, text);
                }
                catch (IOException)
                {
                    m_Buffer = text + m_Buffer;
                    PostponeLog();
                }
                catch
                {

                }
            }
            //
        }        
        private void PostponeLog ()
        {
            new System.Threading.Timer(OnTimer, null, 0, 1000);
        }
        private void OnTimer (Object? o)
        {
            DoLog();
        }
        #endregion
    }
    public partial class ProcessLogger : UserControl
    {
        #region Fields
        private bool? m_LocalNode=null;
        private string m_JSFileName="";
        private FileLogger m_FileLogger = new FileLogger();
        private Process? m_Process=null;
        private string m_Buffer = "";
        private bool m_ProcessClosed = true;
        delegate void Callback();
        #endregion
        #region Ctor
        public ProcessLogger()
        {
            InitializeComponent();
        }
        #endregion
        #region Properties
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Process"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool? LocalNode { 
            get { return m_LocalNode; } 
            set { 
                m_LocalNode = value;
                Start();
            }
        }
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Process"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string JSFileName
        {
            get { return m_JSFileName; }
            set
            {
                m_JSFileName = value;
                m_FileLogger.JSFileName = value;
                Start();
            }
        }
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Category("Process"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string LogDirRootName
        {
            get { return m_FileLogger.LogDirRootName; }
            set {
                if (this.DesignMode)
                {
                    return;
                }
                m_FileLogger.LogDirRootName = value;
            }
        }
        public bool ProcessClosed
        {
            get { return m_ProcessClosed; }
        }
        #endregion
        #region Public Methods
        public void Stop ()
        {
            if (m_Process!=null && !m_Process.HasExited)
            {
                m_Process.Kill();
            }
        }
        #endregion
        #region Protected Methods
        protected void Start()
        {
            if (this.DesignMode)
            {
                return;
            }
            Stop();
            if (LocalNode == null)
            {
                return;
            }
            if (!String.IsNullOrWhiteSpace(m_JSFileName))
            {
                try
                {
                    m_Process = new Process();
                    if (LocalNode.HasValue && LocalNode.Value)
                    {
                        m_Process.StartInfo.EnvironmentVariables["Path"] = "nodejs;" + m_Process.StartInfo.EnvironmentVariables["Path"];
                    }
                    m_Process.StartInfo.UseShellExecute = false;
                    m_Process.StartInfo.RedirectStandardOutput = true;
                    m_Process.StartInfo.RedirectStandardError = true;
                    m_Process.StartInfo.FileName = Path.Combine(LocalNode.HasValue && LocalNode.Value ? "nodejs" : "", "node.exe");
                    m_Process.StartInfo.Arguments = m_JSFileName;
                    m_Process.StartInfo.CreateNoWindow = true;
                    stopButt.Visible = true;
                    startButt.Visible = false;
                    m_Process.Start();
                    m_Process.EnableRaisingEvents = true;
                    m_Process.Exited += OnProcessExited;
                    m_Process.BeginOutputReadLine();
                    m_Process.BeginErrorReadLine();
                    m_Process.OutputDataReceived += onProcessText;
                    m_Process.ErrorDataReceived += onProcessText;
                    m_ProcessClosed = false;
                }
                catch (Exception e)
                {
                    setText(e.Message);
                }
            }
        }

        protected void OnProcessExited(object? sender, System.EventArgs args)
        {
            doProcessExited();
        }
        #endregion
        #region Private Methods
        private void doProcessExited ()
        {
            if (InvokeRequired)
            {
                Invoke(new Callback(doProcessExited));
            }
            else
            {
                stopButt.Visible = false;
                startButt.Visible = true;
                m_ProcessClosed = true;
            }
        }
        private void onProcessText(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            setText(e.Data);
        }
        private void setText(string? text)
        {
            if (text==null)
            {
                return;
            }
            /*
            //npm suppresion hack
            if (text.StartsWith("'npm' is not recognized"))
            {
                return;
            }
            if (text.Equals("operable program or batch file."))
            {
                return;
            }
            */
            lock (this)
            {
                text += "\r\n";
                m_FileLogger.Log(text);
                bool bufferWasEmpty = m_Buffer.Length < 1;
                m_Buffer += text;
                if (logBox == null)
                    return;
                if (logBox.IsDisposed)
                    return;
                if (logBox.InvokeRequired)
                {
                    if (bufferWasEmpty)
                    {
                        try
                        {
                            logBox.Invoke(dumpBuffer);
                        }
                        catch { }
                    }
                }
                else
                {
                    dumpBuffer();
                }
            }
        }
        private void dumpBuffer()
        {
            string b = m_Buffer;
            m_Buffer = "";
            logBox.AppendText(b);
        }
        #endregion
        #region Event Handlers
        private void startButt_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void stopButt_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void clearButt_Click(object sender, EventArgs e)
        {
            logBox.Clear();
        }
        #endregion
    }
}
