using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AllexCloudRunnerControls
{
    class FileLogger : IDisposable
    {
        #region Constants
        // How often the background thread drains the in-memory buffer to disk.
        private const int FlushIntervalMs = 250;
        #endregion

        #region Fields
        private string m_LogDirRootName="";
        private string m_JSFileName="";
        private readonly StringBuilder m_Buffer = new StringBuilder();
        private readonly object m_Lock = new object();      // guards m_Buffer (touched by reader thread)
        private readonly object m_WriteLock = new object(); // serializes file writes (timer callbacks only)
        private StreamWriter? m_Writer;
        private string m_OpenPath = "";
        private readonly System.Threading.Timer m_FlushTimer;
        #endregion

        #region Ctor
        public FileLogger()
        {
            // A single reusable timer drains the buffer on a background thread.
            // (The previous implementation spawned a brand new repeating Timer on every
            //  failed write and never disposed it, leaking timers under file contention.)
            m_FlushTimer = new System.Threading.Timer(OnTimer, null, FlushIntervalMs, FlushIntervalMs);
        }
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
            get { return m_LogDirRootName + "\\" + System.IO.Path.GetFileNameWithoutExtension(m_JSFileName) + DateTime.Today.ToString("_yyyy_MM_dd") + ".txt"; }
        }
        private bool CannotLog
        {
            get { return null == m_LogDirRootName || m_LogDirRootName.Length < 1 || null == m_JSFileName || m_JSFileName.Length < 1; }
        }

        #endregion

        #region Public Methods
        // Called on the process stdout/stderr reader thread. Must be cheap and must never
        // block: it only appends to an in-memory buffer. The actual file write happens on
        // the flush timer, so a slow disk can never apply backpressure to the node process.
        public void Log(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            lock (m_Lock)
            {
                m_Buffer.Append(text);
            }
        }

        public void Dispose()
        {
            m_FlushTimer.Dispose();
            DoLog();
            lock (m_WriteLock)
            {
                m_Writer?.Dispose();
                m_Writer = null;
            }
        }
        #endregion

        #region Private Methods
        private void OnTimer (Object? o)
        {
            DoLog();
        }
        private void DoLog()
        {
            string text;
            lock (m_Lock)
            {
                if (CannotLog || m_Buffer.Length < 1)
                {
                    return;
                }
                text = m_Buffer.ToString();
                m_Buffer.Clear();
            }
            // Serialize writes so re-entrant timer callbacks can't corrupt the StreamWriter.
            lock (m_WriteLock)
            {
                try
                {
                    EnsureWriter();
                    m_Writer!.Write(text);
                    m_Writer.Flush();
                }
                catch (IOException)
                {
                    // Transient lock (e.g. AV scan). Put the text back; the next tick retries.
                    lock (m_Lock)
                    {
                        m_Buffer.Insert(0, text);
                    }
                }
                catch
                {

                }
            }
        }
        // Keeps a single file handle open and reopens only when the date-stamped
        // path rolls over, instead of opening/closing the file on every line.
        private void EnsureWriter()
        {
            string path = LogFilePath;
            if (m_Writer != null && path == m_OpenPath)
            {
                return;
            }
            m_Writer?.Dispose();
            m_Writer = new StreamWriter(path, append: true) { AutoFlush = false };
            m_OpenPath = path;
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

        #region Constants
        // Cap the on-screen log so the WinForms TextBox can never grow without bound.
        // An ever-growing multiline TextBox is what turned high message volume into
        // O(n^2) UI work and pegged the CPU; we keep only a rolling tail.
        private const int MaxLogBoxChars = 200_000;
        // How often the UI thread drains the display buffer.
        private const int UiFlushIntervalMs = 200;
        #endregion

        #region Fields
        private bool? m_LocalNode=null;
        private string? m_PathToAllexSystem=null;
        private string m_JSFileName="";
        private FileLogger m_FileLogger = new FileLogger();
        private Process? m_Process=null;
        private readonly StringBuilder m_UiBuffer = new StringBuilder();
        private readonly object m_UiLock = new object();
        private System.Windows.Forms.Timer? m_UiTimer;
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
            // Drain the display buffer to the TextBox on a coalescing UI timer instead of
            // marshalling (and repainting) once per received line. The reader thread no
            // longer waits on the UI thread, so it can keep node's stdout pipe drained.
            m_UiTimer = new System.Windows.Forms.Timer { Interval = UiFlushIntervalMs };
            m_UiTimer.Tick += (s, e) => FlushUi();
            m_UiTimer.Start();
            Disposed += (s, e) =>
            {
                m_UiTimer?.Dispose();
                m_FileLogger.Dispose();
            };
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
        // Runs on the process stdout/stderr reader thread, once per received line.
        // Keep it cheap and non-blocking: hand the text to the (async) file logger and
        // append it to the display buffer. No file I/O and no synchronous UI marshalling
        // happen here, so the reader thread always keeps draining node's stdout pipe.
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
            text += "\r\n";
            m_FileLogger.Log(text);
            lock (m_UiLock)
            {
                m_UiBuffer.Append(text);
            }
        }
        // Runs on the UI thread from the flush timer. Drains whatever accumulated since the
        // last tick in one AppendText, and enforces the rolling size cap on the TextBox.
        private void FlushUi()
        {
            if (logBox == null || logBox.IsDisposed)
            {
                return;
            }
            string b;
            lock (m_UiLock)
            {
                if (m_UiBuffer.Length < 1)
                {
                    return;
                }
                b = m_UiBuffer.ToString();
                m_UiBuffer.Clear();
            }
            if (b.Length >= MaxLogBoxChars)
            {
                // This batch alone overflows the cap: keep only its tail.
                logBox.Clear();
                logBox.AppendText(b.Substring(b.Length - MaxLogBoxChars));
                return;
            }
            if (logBox.TextLength + b.Length > MaxLogBoxChars)
            {
                // Trim the oldest text so existing + new fits inside the cap.
                int keep = MaxLogBoxChars - b.Length;
                string existing = logBox.Text;
                if (keep < existing.Length)
                {
                    logBox.Text = existing.Substring(existing.Length - keep);
                }
            }
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
