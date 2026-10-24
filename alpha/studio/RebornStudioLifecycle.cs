// Reborn: attach close confirmation to EA Studio and contain each owned build in a Windows job, never enumerate or kill unrelated processes.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Reborn
{
    public static class StudioLifecycle
    {
        private static BuildSession current;
        private static Form studio;
        private static bool installed;
        // Reborn: injectable confirmation is internal to the assembly's offline harness; production always uses a real Yes/No dialog.
        internal static Func<DialogResult> Confirmation = null;
        // Reborn: EA Studio colors only its own step/build summary lines green; redirected backend lines with the same meaning are recolored here.
        private static readonly Regex GreenLine = new Regex(@"^(?:\[\d\d:\d\d:\d\d(?:\.\d+)?\] )?(?:Step time taken: |(?:Validation|Compilation) (?:started|completed): |Validation Completed \(no publication\)\.$)", RegexOptions.CultureInvariant);
        private static RichTextBox output;
        private static int colored;
        private static bool recoloring;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: defer main-form attachment until EA has completed its constructor and entered the message loop. */
        //-------------------------------------------------------------------------------------------------
        public static void Install()
        {
            if (installed) return;
            installed = true;
            Application.Idle += OnIdle;
            OnIdle(null, EventArgs.Empty);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: select the first top-level application form once; closing a documentation/error dialog must not cancel a build. */
        //-------------------------------------------------------------------------------------------------
        private static void OnIdle(object sender, EventArgs args)
        {
            if (Application.OpenForms.Count == 0) return;
            Attach(Application.OpenForms[0]);
            Application.Idle -= OnIdle;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: the same event attachment is exercised by hidden synthetic forms in the offline regression harness. */
        //-------------------------------------------------------------------------------------------------
        internal static void Attach(Form form)
        {
            if (studio != null) return;
            studio = form;
            studio.FormClosing += OnClosing;
            AttachOutput(form);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: find EA's build log box by its designer field; a missing or changed control leaves the log uncolored, never broken. */
        //-------------------------------------------------------------------------------------------------
        private static void AttachOutput(Form form)
        {
            try
            {
                FieldInfo field = form.GetType().GetField("rtbGUIOutput", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                output = field == null ? null : field.GetValue(form) as RichTextBox;
                if (output == null) return;
                colored = 0;
                output.TextChanged += OnOutputChanged;
            }
            catch (Exception) { output = null; }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: recolor only newly completed lines; restore caret and insertion color so EA's next AppendText keeps its own colors. */
        //-------------------------------------------------------------------------------------------------
        private static void OnOutputChanged(object sender, EventArgs args)
        {
            if (recoloring || output == null || output.IsDisposed) return;
            string text = output.Text;
            // Reborn: "Clear Build Log" shortens the text; restart from the beginning.
            if (text.Length < colored) colored = 0;
            int end = text.LastIndexOf('\n');
            if (end < colored) return;
            recoloring = true;
            int start = output.SelectionStart, length = output.SelectionLength; Color insertion = output.SelectionColor;
            try
            {
                for (int at = colored; at <= end; )
                {
                    int next = text.IndexOf('\n', at);
                    string line = text.Substring(at, next - at).TrimEnd('\r');
                    if (GreenLine.IsMatch(line)) { output.Select(at, line.Length); output.SelectionColor = Color.Green; }
                    at = next + 1;
                }
                colored = end + 1;
            }
            catch (Exception) { }
            finally
            {
                // Reborn: never repaint a user's selected range; an empty caret carries the insertion color for EA's next line.
                output.Select(start, length); if (length == 0) output.SelectionColor = insertion;
                recoloring = false;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: allocate a unique job and control objects before EA dispatches its child so even immediate close cannot orphan a late-starting worker. */
        //-------------------------------------------------------------------------------------------------
        public static string Begin()
        {
            if (current != null && current.Running) throw new InvalidOperationException("This Studio already owns an active build.");
            if (current != null) current.Dispose();
            current = new BuildSession();
            return current.Token;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: release a finished session only after the complete owned process tree has exited, independent of UI callback timing. */
        //-------------------------------------------------------------------------------------------------
        public static void Complete()
        {
            if (current == null) return;
            // Reborn: an exit callback arriving after window disposal must not queue work to a destroyed UI handle.
            if (studio != null && studio.IsDisposed) { current.Dispose(); current = null; return; }
            // Reborn: serialize session disposal with the main form's close handler, including callbacks raised on a process-exit thread.
            if (studio != null && studio.InvokeRequired) { studio.BeginInvoke(new MethodInvoker(Complete)); return; }
            if (current.ActiveProcesses != 0) return;
            current.Dispose(); current = null;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: No cancels window closure, Yes cancels only this job; any failure keeps the window open with its ownership handles intact. */
        //-------------------------------------------------------------------------------------------------
        private static void OnClosing(object sender, FormClosingEventArgs args)
        {
            // Reborn: respect any earlier EA close veto instead of overriding unrelated save/validation decisions.
            bool earlierVeto = args.Cancel;
            BuildSession session = current;
            if (session == null || !session.Running) { Complete(); return; }
            args.Cancel = true;
            DialogResult answer = Confirmation == null
                ? MessageBox.Show(studio, "A build is still running. Cancel the build and close the SDK?", "Uprising Mod SDK", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                : Confirmation();
            if (answer != DialogResult.Yes) return;
            // Reborn: a build can finish while the confirmation dialog is open; do not cancel an already disposed session.
            if (!Object.ReferenceEquals(current, session) || !session.Running) { args.Cancel = earlierVeto; Complete(); return; }
            try
            {
                session.Cancel();
                if (Object.ReferenceEquals(current, session)) { current = null; session.Dispose(); }
                args.Cancel = earlierVeto;
            }
            catch (Exception error)
            {
                if (Confirmation != null) throw;
                MessageBox.Show(studio, "The build could not be stopped safely; the SDK was left open.\n" + error.Message, "Uprising Mod SDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: execute only the EA-dispatched build host; the actual worker is suspended until job assignment succeeds. */
        //-------------------------------------------------------------------------------------------------
        [STAThread]
        private static int Main(string[] args)
        {
            try
            {
                if (args.Length < 3 || args[0] != "--run") throw new InvalidDataException("Expected an owned Studio build request.");
                Guid token; if (!Guid.TryParseExact(args[1], "N", out token)) throw new InvalidDataException("Invalid build session.");
                return RunHost(args);
            }
            catch (Exception error) { Console.WriteLine("Critical: ALPHA130 Studio build host failed: " + error.Message); return 1; }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: standard handles retain EA's redirected logging, while only the contained PowerShell/descendants receive cancellation. */
        //-------------------------------------------------------------------------------------------------
        internal static int RunHost(string[] args)
        {
            string token = args[1];
            using (EventWaitHandle cancel = EventWaitHandle.OpenExisting(BuildSession.Name(token, "Cancel")))
            using (EventWaitHandle done = EventWaitHandle.OpenExisting(BuildSession.Name(token, "Done")))
            using (EventWaitHandle started = EventWaitHandle.OpenExisting(BuildSession.Name(token, "Started")))
            {
                started.Set();
                Native.ProcessInfo process = new Native.ProcessInfo(); IntPtr job = IntPtr.Zero;
                try
                {
                    if (cancel.WaitOne(0)) return 1223;
                    job = Native.OpenJobObject(1, false, BuildSession.Name(token, "Job")); Native.Check(job != IntPtr.Zero);
                    string command = Quote(args[2]); for (int i = 3; i < args.Length; i++) command += " " + Quote(args[i]);
                    Native.StartupInfo startup = new Native.StartupInfo(); startup.Size = Marshal.SizeOf(typeof(Native.StartupInfo));
                    startup.Flags = 0x100; startup.Input = Native.GetStdHandle(-10); startup.Output = Native.GetStdHandle(-11); startup.Error = Native.GetStdHandle(-12);
                    // Reborn: make only the host's existing standard streams inheritable; job/control handles are never inherited.
                    foreach (IntPtr handle in new IntPtr[] { startup.Input, startup.Output, startup.Error })
                        if (handle != IntPtr.Zero && handle != new IntPtr(-1)) Native.Check(Native.SetHandleInformation(handle, 1, 1));
                    Native.Check(Native.CreateProcess(args[2], new StringBuilder(command), IntPtr.Zero, IntPtr.Zero, true, 0x08000004, IntPtr.Zero, null, ref startup, out process));
                    Native.Check(Native.AssignProcessToJobObject(job, process.Process));
                    // Reborn: leave the GUI as the sole job owner; abnormal GUI termination then also kills its contained workers.
                    Native.CloseHandle(job); job = IntPtr.Zero;
                    if (cancel.WaitOne(0)) Native.Check(Native.TerminateProcess(process.Process, 1223));
                    else Native.Check(Native.ResumeThread(process.Thread) != UInt32.MaxValue);
                    Native.Check(Native.WaitForSingleObject(process.Process, UInt32.MaxValue) == 0);
                    uint code; Native.Check(Native.GetExitCodeProcess(process.Process, out code));
                    if (cancel.WaitOne(0))
                    {
                        CleanupCancelledRun(token);
                        Console.WriteLine("Critical: ALPHA131 Build cancelled by Studio close; owned workers stopped and SDK lock released.");
                        return 1223;
                    }
                    return unchecked((int)code);
                }
                catch
                {
                    if (process.Process != IntPtr.Zero) { Native.TerminateProcess(process.Process, 1223); Native.WaitForSingleObject(process.Process, 5000); }
                    throw;
                }
                finally
                {
                    if (job != IntPtr.Zero) Native.CloseHandle(job);
                    if (process.Thread != IntPtr.Zero) Native.CloseHandle(process.Thread);
                    if (process.Process != IntPtr.Zero) Native.CloseHandle(process.Process);
                    done.Set();
                }
            }
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: remove only this GUID-owned marked private work tree after cancellation; sources and final outputs are never cleanup targets. */
        //-------------------------------------------------------------------------------------------------
        private static void CleanupCancelledRun(string token)
        {
            string sdk = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", ".."));
            string root = Path.Combine(sdk, ".work", token);
            if (!Directory.Exists(root)) return;
            string marker = Path.Combine(root, "studio-owner.txt");
            if (!File.Exists(marker) || File.ReadAllText(marker) != token) { Console.WriteLine("Warning: ALPHA132 Unmarked cancelled work retained."); return; }
            for (DirectoryInfo at = new DirectoryInfo(root); at != null; at = at.Parent)
                if ((at.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Redirected cancelled work retained.");
            foreach (string path in Directory.GetFileSystemEntries(root, "*", SearchOption.AllDirectories))
                if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) throw new IOException("Redirected cancelled work retained.");
            Directory.Delete(root, true);
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: quote every argument according to Windows process parsing, including embedded quotes and trailing backslashes. */
        //-------------------------------------------------------------------------------------------------
        internal static string Quote(string value)
        {
            StringBuilder result = new StringBuilder("\""); int slashes = 0;
            foreach (char c in value)
            {
                if (c == '\\') { slashes++; continue; }
                if (c == '"') { result.Append('\\', slashes * 2 + 1); result.Append(c); slashes = 0; continue; }
                result.Append('\\', slashes); slashes = 0; result.Append(c);
            }
            result.Append('\\', slashes * 2); return result.Append('"').ToString();
        }

        internal sealed class BuildSession : IDisposable
        {
            internal readonly string Token = Guid.NewGuid().ToString("N");
            internal readonly EventWaitHandle CancelEvent;
            internal readonly EventWaitHandle Done;
            internal readonly EventWaitHandle Started;
            internal readonly Mutex Publish;
            private IntPtr job;

            //-------------------------------------------------------------------------------------------------
            /** Reborn: no breakaway limits are permitted; ownership begins before any worker can execute user code. */
            //-------------------------------------------------------------------------------------------------
            internal BuildSession()
            {
                job = Native.CreateJobObject(IntPtr.Zero, Name(Token, "Job")); Native.Check(job != IntPtr.Zero);
                try
                {
                    Native.ExtendedLimits limits = new Native.ExtendedLimits(); limits.Basic.Flags = 0x2000;
                    Native.Check(Native.SetInformationJobObject(job, 9, ref limits, (uint)Marshal.SizeOf(typeof(Native.ExtendedLimits))));
                    CancelEvent = new EventWaitHandle(false, EventResetMode.ManualReset, Name(Token, "Cancel"));
                    Done = new EventWaitHandle(false, EventResetMode.ManualReset, Name(Token, "Done"));
                    Started = new EventWaitHandle(false, EventResetMode.ManualReset, Name(Token, "Started"));
                    Publish = new Mutex(false, Name(Token, "Publish"));
                }
                catch { Dispose(); throw; }
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: names are session-scoped, never a shared global PID or process-name selector. */
            //-------------------------------------------------------------------------------------------------
            internal static string Name(string token, string suffix) { return "Local\\RebornStudio-" + token + "-" + suffix; }

            // Reborn: include a queued host not yet started; completion and actual job accounting must agree.
            internal bool Running { get { return !Done.WaitOne(0) || ActiveProcesses != 0; } }
            internal uint ActiveProcesses
            {
                get { Native.Accounting info; Native.Check(Native.QueryInformationJobObject(job, 1, out info, (uint)Marshal.SizeOf(typeof(Native.Accounting)), IntPtr.Zero)); return info.Active; }
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: serialize cancellation against final output publication, kill the owned tree and wait until handles/lock can no longer be held. */
            //-------------------------------------------------------------------------------------------------
            internal void Cancel()
            {
                CancelEvent.Set(); bool held = false;
                try
                {
                    try { held = Publish.WaitOne(5000); } catch (AbandonedMutexException) { held = true; }
                    if (!held) throw new IOException("Output publication is still busy; retry closing after it finishes.");
                    Native.Check(Native.TerminateJobObject(job, 1223));
                    Stopwatch timer = Stopwatch.StartNew();
                    while (ActiveProcesses != 0 && timer.ElapsedMilliseconds < 5000) Thread.Sleep(25);
                    if (ActiveProcesses != 0) throw new IOException("Owned build workers have not exited.");
                }
                finally { if (held) Publish.ReleaseMutex(); }
                // Reborn: a queued host may not yet exist; closing all names makes any later dispatch fail before launching a worker.
                if (Started.WaitOne(0) && !Done.WaitOne(5000)) throw new IOException("Build host shutdown did not complete.");
            }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: the GUI-held job handle is the final process-tree safety net on normal close and application crashes. */
            //-------------------------------------------------------------------------------------------------
            public void Dispose()
            {
                if (job != IntPtr.Zero) { Native.CloseHandle(job); job = IntPtr.Zero; }
                if (CancelEvent != null) CancelEvent.Dispose(); if (Done != null) Done.Dispose(); if (Started != null) Started.Dispose(); if (Publish != null) Publish.Dispose();
            }
        }

        // Reborn: native declarations are restricted to owned process/job handles; no privilege escalation, process enumeration or system-wide termination.
        private static class Native
        {
            [StructLayout(LayoutKind.Sequential)] internal struct BasicLimits { internal long ProcessTime, JobTime; internal uint Flags; internal UIntPtr Min, Max; internal uint Count; internal UIntPtr Affinity; internal uint Priority, Scheduling; }
            [StructLayout(LayoutKind.Sequential)] internal struct IoCounters { internal ulong ReadOps, WriteOps, OtherOps, ReadBytes, WriteBytes, OtherBytes; }
            [StructLayout(LayoutKind.Sequential)] internal struct ExtendedLimits { internal BasicLimits Basic; internal IoCounters Io; internal UIntPtr ProcessMemory, JobMemory, PeakProcess, PeakJob; }
            [StructLayout(LayoutKind.Sequential)] internal struct Accounting { internal long User, Kernel, PeriodUser, PeriodKernel; internal uint PageFaults, Total, Active, Terminated; }
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)] internal struct StartupInfo { internal int Size; internal string Reserved, Desktop, Title; internal uint X, Y, Width, Height, XChars, YChars, Fill, Flags; internal short Show, ReservedSize; internal IntPtr ReservedBytes, Input, Output, Error; }
            [StructLayout(LayoutKind.Sequential)] internal struct ProcessInfo { internal IntPtr Process, Thread; internal uint ProcessId, ThreadId; }
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern IntPtr CreateJobObject(IntPtr security, string name);
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern IntPtr OpenJobObject(uint access, bool inherit, string name);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool SetInformationJobObject(IntPtr job, int kind, ref ExtendedLimits info, uint size);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool QueryInformationJobObject(IntPtr job, int kind, out Accounting info, uint size, IntPtr length);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool TerminateJobObject(IntPtr job, uint code);
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)] internal static extern bool CreateProcess(string app, StringBuilder command, IntPtr processSecurity, IntPtr threadSecurity, bool inherit, uint flags, IntPtr environment, string directory, ref StartupInfo startup, out ProcessInfo process);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern uint ResumeThread(IntPtr thread);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool TerminateProcess(IntPtr process, uint code);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool GetExitCodeProcess(IntPtr process, out uint code);
            [DllImport("kernel32.dll", SetLastError = true)] internal static extern bool SetHandleInformation(IntPtr handle, uint mask, uint flags);
            [DllImport("kernel32.dll")] internal static extern IntPtr GetStdHandle(int which);
            [DllImport("kernel32.dll")] internal static extern bool CloseHandle(IntPtr handle);

            //-------------------------------------------------------------------------------------------------
            /** Reborn: native failures abort a build instead of allowing an uncontained worker to proceed. */
            //-------------------------------------------------------------------------------------------------
            internal static void Check(bool success) { if (!success) throw new Win32Exception(Marshal.GetLastWin32Error()); }
        }
    }
}
