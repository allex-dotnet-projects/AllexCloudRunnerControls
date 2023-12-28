using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

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
    public partial class ProcessRunner : UserControl
    {
        #region WinAPI stuff
        internal const int CTRL_C_EVENT = 0;
        [DllImport("kernel32.dll")]
        internal static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate? HandlerRoutine, bool Add);
        // Delegate type to be used as the Handler Routine for SCCH
        delegate Boolean ConsoleCtrlDelegate(uint CtrlType);
        #endregion

        #region Fields
        private bool? m_LocalNode=null;
        private string? m_PathToAllexSystem=null;
        private string m_JSFileName="";
        private FileLogger m_FileLogger = new FileLogger();
        private Process? m_Process=null;
        private string m_Buffer = "";
        private bool m_ProcessClosed = true;
        private Thread? m_StoppingThread;
        private bool m_TimeToStop = false;
        private AutoResetEvent m_ProcessStopped = new AutoResetEvent(false);
        delegate void Callback();
        #endregion

        #region Ctor
        public ProcessRunner()
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
        public string? PathToAllexSystem
        {
            get { return m_PathToAllexSystem; }
            set
            {
                m_PathToAllexSystem = value;
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
        public void Stop (bool forever = false)
        {
            m_TimeToStop = m_TimeToStop||forever;
            if (m_StoppingThread != null)
            {
                return;
            }
            stopButt.Enabled = m_Process == null || m_Process.HasExited;
            startButt.Enabled = m_Process == null || m_Process.HasExited;
            m_StoppingThread = new Thread(StoppingThreadProc);
            m_StoppingThread.Start(this);
        }
        #endregion

        #region Protected Methods
        protected void Start()
        {
            if (this.DesignMode)
            {
                return;
            }
            StopSync();
            if (LocalNode == null)
            {
                return;
            }
            if (PathToAllexSystem == null)
            {
                return;
            }
            if (!String.IsNullOrWhiteSpace(m_JSFileName))
            {
                System.Diagnostics.Debug.WriteLine($"Starting {m_JSFileName}");
                try
                {
                    m_Process = new Process();
                    if (LocalNode.HasValue && LocalNode.Value)
                    {
                        m_Process.StartInfo.EnvironmentVariables["Path"] = "nodejs;" + m_Process.StartInfo.EnvironmentVariables["Path"];
                        m_Process.StartInfo.FileName = Path.Combine(PathToAllexSystem, "nodejs", "node.exe");
                    }
                    else
                    {
                        m_Process.StartInfo.FileName = "node.exe";
                    }
                    m_Process.StartInfo.UseShellExecute = false;
                    m_Process.StartInfo.WorkingDirectory = PathToAllexSystem;
                    m_Process.StartInfo.RedirectStandardOutput = true;
                    m_Process.StartInfo.RedirectStandardError = true;
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
        protected void StopSync()
        {
            if (m_StoppingThread == null)
            {
                Stop();
            }
            m_StoppingThread?.Join();
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
                stopButt.Enabled = !m_TimeToStop;
                startButt.Enabled = !m_TimeToStop;
                stopButt.Visible = false;
                startButt.Visible = !m_TimeToStop;
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
                try
                {
                    logBox.Invoke(dumpBuffer);
                }
                catch { }
                /*
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
                */
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
        protected void OnProcessExited(object? sender, System.EventArgs args)
        {
            m_ProcessStopped.Set();
            doProcessExited();
        }
        #endregion

        #region Thread Proc
        private static object _StoppingLock = new object();
        private void StoppingThreadProc (object? runnerobj)
        {
            ProcessRunner myrunner;
            Process myprocess;
            if (runnerobj == null || runnerobj is not ProcessRunner)
            {
                return;
            }
            myrunner = (ProcessRunner)runnerobj;
            if (myrunner.m_Process == null)
            {
                myrunner.m_StoppingThread = null;
                return;
            }
            myprocess = myrunner.m_Process;
            lock (_StoppingLock)
            {
                if (myprocess != null && !myprocess.HasExited)
                {
                    if (AttachConsole((uint)myprocess.Id))
                    {
                        SetConsoleCtrlHandler(null, true);
                        try
                        {
                            if (!GenerateConsoleCtrlEvent(CTRL_C_EVENT, 0))
                                return;// false;
                            myrunner.m_ProcessStopped.WaitOne();
                            SetConsoleCtrlHandler(null, false);
                            FreeConsole();
                        }
                        catch { }
                    }
                }
                myrunner.m_StoppingThread = null;
            }
        }
        #endregion
    }
}
