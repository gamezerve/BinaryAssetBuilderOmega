// Reborn: hidden synthetic-form and owned-process tests for close confirmation; never open a Studio/game or stop existing user processes.
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Windows.Forms;
using Reborn;

internal static class RebornStudioLifecycleTest
{
    private static string host, self, root, sdkBase;
    private static Form form;
    private static int passed;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: helper worker modes are spawned only by this harness; the primary mode drives actual FormClosing handlers without showing a window. */
    //-------------------------------------------------------------------------------------------------
    [STAThread]
    private static int Main(string[] args)
    {
        try
        {
            self = Assembly.GetExecutingAssembly().Location;
            if (args.Length > 0 && args[0] == "--success") { Console.WriteLine("Reborn: stdout forwarding PASS"); Console.Error.WriteLine("Reborn: stderr forwarding PASS"); return 0; }
            if (args.Length > 0 && (args[0] == "--child" || args[0] == "--sentinel")) { File.WriteAllText(args[1], Process.GetCurrentProcess().Id.ToString()); Thread.Sleep(Timeout.Infinite); return 0; }
            if (args.Length > 0 && args[0] == "--worker")
            {
                using (FileStream locked = File.Open(args[1] + ".lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                using (Process child = Start(self, "--child", args[1] + ".child"))
                { File.WriteAllText(args[1] + ".ready", Process.GetCurrentProcess().Id.ToString()); Thread.Sleep(Timeout.Infinite); }
                return 0;
            }
            if (args.Length > 0 && args[0] == "--owner-crash")
            {
                host = args[1]; using (StudioLifecycle.BuildSession session = new StudioLifecycle.BuildSession())
                using (Process child = Spawn(session, "--worker", args[2])) { Wait(() => File.Exists(args[2] + ".child")); Environment.Exit(99); }
                return 0;
            }
            if (args.Length != 3) throw new Exception("Expected SDK root, PowerShell7 and driver path.");
            string sdk = Path.GetFullPath(args[0]); sdkBase = sdk; host = Path.Combine(sdk, "tools", "studio", "RebornStudioLifecycle.exe");
            root = Path.Combine(sdk, ".work", "lifecycle-test-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
            form = new Form(); StudioLifecycle.Attach(form);
            int prompts = 0; StudioLifecycle.Confirmation = delegate { prompts++; return DialogResult.Yes; };
            Check(!CloseAttempt() && prompts == 0, "idle close without prompt");
            StudioLifecycle.Begin(); Check(CloseAttempt(true), "earlier EA close veto preserved after cancellation");
            string token = StudioLifecycle.Begin(); StudioLifecycle.BuildSession own = Current();
            StudioLifecycle.Confirmation = delegate { return DialogResult.No; };
            Check(CloseAttempt() && own.Running, "No keeps Studio/session alive");
            StudioLifecycle.Confirmation = delegate { return DialogResult.Yes; };
            Check(!CloseAttempt(), "Yes cancels queued build");
            using (Process late = Start(host, "--run", token, self, "--worker", Path.Combine(root, "late")))
            { WaitExit(late); Check(late.ExitCode != 0 && !File.Exists(Path.Combine(root, "late.ready")), "late dispatch cannot start an orphan worker"); }
            StudioLifecycle.Begin(); own = Current();
            using (Process normal = Spawn(own, "--success"))
            { WaitExit(normal); Check(normal.ExitCode == 0 && normal.StandardOutput.ReadToEnd().Contains("stdout forwarding PASS") && normal.StandardError.ReadToEnd().Contains("stderr forwarding PASS"), "normal host/stream forwarding"); }
            Check(!CloseAttempt(), "completed build closes without cancellation");
            StudioLifecycle.Begin(); own = Current(); string work = Path.Combine(root, "owned");
            using (Process child = Spawn(own, "--worker", work))
            using (StudioLifecycle.BuildSession other = new StudioLifecycle.BuildSession())
            using (Process unrelated = Spawn(other, "--worker", Path.Combine(root, "other")))
            {
                try
                {
                    Wait(() => File.Exists(work + ".child") && other.ActiveProcesses >= 2 && own.ActiveProcesses >= 2);
                    Check(!LockFree(work + ".lock"), "worker owns exclusive fixture lock");
                    StudioLifecycle.Confirmation = delegate { return DialogResult.No; };
                    Check(CloseAttempt() && own.ActiveProcesses >= 2, "No preserves live worker and grandchild");
                    StudioLifecycle.Confirmation = delegate { return DialogResult.Yes; };
                    Check(!CloseAttempt(), "Yes cancels active owned build"); WaitExit(child);
                    Check(LockFree(work + ".lock"), "cancel releases exclusive lock");
                    Check(other.ActiveProcesses >= 2 && !unrelated.HasExited, "other build/session untouched");
                }
                finally { other.Cancel(); WaitExit(unrelated); }
            }
            // Reborn: model build completion while the modal prompt is open; callback disposal must not turn Yes into an error.
            StudioLifecycle.Begin(); own = Current();
            StudioLifecycle.Confirmation = delegate { own.Done.Set(); StudioLifecycle.Complete(); return DialogResult.Yes; };
            Check(!CloseAttempt(), "completion while prompt is open");
            // Reborn: publication exclusion must refuse termination until a critical output update is finished.
            StudioLifecycle.Begin(); own = Current(); work = Path.Combine(root, "publication");
            using (Process child = Spawn(own, "--worker", work))
            using (ManualResetEvent acquired = new ManualResetEvent(false))
            using (ManualResetEvent release = new ManualResetEvent(false))
            {
                Wait(() => own.ActiveProcesses >= 2);
                Thread writer = new Thread(delegate() { own.Publish.WaitOne(); acquired.Set(); release.WaitOne(); own.Publish.ReleaseMutex(); }); writer.Start(); acquired.WaitOne();
                StudioLifecycle.Confirmation = delegate { return DialogResult.Yes; }; bool refused = false;
                try { CloseAttempt(); } catch (TargetInvocationException e) { refused = e.InnerException is IOException; }
                finally { release.Set(); writer.Join(); }
                Check(refused && own.ActiveProcesses >= 2, "publication-in-progress prevents unsafe termination");
                Check(!CloseAttempt(), "cancellation succeeds after publication exclusion ends"); WaitExit(child);
            }
            // Reborn: exercise the production PowerShell driver and real SDK lock without building or modifying the user's Reborn mod.
            string source = Path.Combine(sdk, "mods", "Reborn", "data", "mod.xml"), package = Path.Combine(sdk, "builtmods", "RebornTest", "RebornTest.big");
            string sourceHash = Hash(source), packageHash = Hash(package), sdkLock = Path.Combine(sdk, ".sdk.lock");
            if (!LockFree(sdkLock)) throw new Exception("Existing user SDK build is busy; actual cancellation control not started.");
            token = StudioLifecycle.Begin(); own = Current();
            using (Process actual = Start(host, "--run", token, args[1], "-NoProfile", "-File", args[2], "-ModName", "RebornTest", "-StudioSession", token))
            {
                Wait(() => File.Exists(Path.Combine(sdk, ".work", token, "studio-owner.txt")) && own.ActiveProcesses >= 3 && !LockFree(sdkLock));
                StudioLifecycle.Confirmation = delegate { return DialogResult.Yes; };
                Check(!CloseAttempt(), "real driver cancellation through FormClosing"); WaitExit(actual);
                Check(LockFree(sdkLock), "real SDK lock released without deleting its file");
                Check(Hash(source) == sourceHash && Hash(package) == packageHash, "user source and last-good BIG preserved");
                Check(!Directory.Exists(Path.Combine(sdk, ".work", token)), "only cancelled owned work tree cleaned");
            }
            // Reborn: also prove a successful real publication through the new host/mutex path, restoring all developer source/output bytes afterward.
            string developerData = Path.Combine(sdk, "mods", "RebornTest", "data"), developerOutput = Path.Combine(sdk, "builtmods", "RebornTest");
            string savedData = Path.Combine(root, "saved-data"), savedOutput = Path.Combine(root, "saved-output");
            AssertTree(developerData); AssertTree(developerOutput);
            Directory.Move(developerData, savedData);
            try
            {
                Directory.Move(developerOutput, savedOutput);
                try
                {
                    Directory.CreateDirectory(developerData);
                    File.Copy(Path.Combine(sdk, "..", "tests", "fixtures", "live-alpha", "Reborn", "data", "mod.xml"), Path.Combine(developerData, "mod.xml"));
                    token = StudioLifecycle.Begin(); own = Current();
                    string[] request = new string[] { "--run", token, args[1], "-NoProfile", "-File", args[2], "-ModName", "RebornTest", "-StudioSession", token };
                    string command = ""; foreach (string value in request) command += StudioLifecycle.Quote(value) + " ";
                    using (Process publication = Process.Start(new ProcessStartInfo(host, command) { UseShellExecute = false, CreateNoWindow = true }))
                    {
                        WaitExit(publication); Check(publication.ExitCode == 0 && own.ActiveProcesses == 0 && File.Exists(Path.Combine(developerOutput, "build.json")), "real driver publication through job/mutex succeeds");
                        StudioLifecycle.Complete();
                    }
                }
                finally
                {
                    if (Current() != null) { Current().Cancel(); StudioLifecycle.Complete(); }
                    if (Directory.Exists(developerOutput)) { AssertTree(developerOutput); Directory.Delete(developerOutput, true); }
                    Directory.Move(savedOutput, developerOutput);
                }
            }
            finally
            {
                if (Directory.Exists(developerData)) { AssertTree(developerData); Directory.Delete(developerData, true); }
                Directory.Move(savedData, developerData);
            }
            Check(Hash(source) == sourceHash && Hash(package) == packageHash, "successful control restores developer output and leaves user source untouched");
            // Reborn: abrupt owner exit is tested only in an owned child; job handle closure must also terminate its worker/grandchild.
            work = Path.Combine(root, "crash");
            using (Process crash = Start(self, "--owner-crash", host, work)) { WaitExit(crash); Check(crash.ExitCode == 99, "synthetic owner exited abruptly"); }
            Wait(() => LockFree(work + ".lock")); Check(true, "job kill-on-close releases worker after owner crash");
            Wait(() => !RecordedProcessAlive(work + ".child")); Check(true, "owner crash also terminates its grandchild");
            File.WriteAllText(Path.Combine(root, "result.txt"), "Reborn: " + passed + " lifecycle controls PASS; no Studio/game/user build executed.\n");
            Console.WriteLine("Reborn: " + passed + " lifecycle controls PASS; report=" + root); return 0;
        }
        catch (Exception error) { Console.WriteLine(error); return 1; }
        finally { StudioLifecycle.Confirmation = delegate { return DialogResult.Yes; }; if (form != null) { try { CloseAttempt(); } catch { } form.Dispose(); } }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact same close handler, but no Form.Show, message box or live application automation. */
    //-------------------------------------------------------------------------------------------------
    private static bool CloseAttempt(bool earlierVeto = false) { FormClosingEventArgs args = new FormClosingEventArgs(CloseReason.UserClosing, earlierVeto); typeof(Form).GetMethod("OnFormClosing", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(form, new object[] { args }); return args.Cancel; }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect only this harness's session, not another application's fields or process state. */
    //-------------------------------------------------------------------------------------------------
    private static StudioLifecycle.BuildSession Current() { return (StudioLifecycle.BuildSession)typeof(StudioLifecycle).GetField("current", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: host arguments are always quoted and the session is preallocated by this test process. */
    //-------------------------------------------------------------------------------------------------
    private static Process Spawn(StudioLifecycle.BuildSession session, params string[] worker) { string[] args = new string[worker.Length + 3]; args[0] = "--run"; args[1] = session.Token; args[2] = self; Array.Copy(worker, 0, args, 3, worker.Length); return Start(host, args); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: every helper created by these tests is hidden; no existing process is selected or terminated by name. */
    //-------------------------------------------------------------------------------------------------
    private static Process Start(string exe, params string[] args) { string command = ""; foreach (string value in args) command += StudioLifecycle.Quote(value) + " "; return Process.Start(new ProcessStartInfo(exe, command) { UseShellExecute = false, CreateNoWindow = true, RedirectStandardOutput = true, RedirectStandardError = true }); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded waits fail visibly; no indefinite test harness sleeps. */
    //-------------------------------------------------------------------------------------------------
    private static void Wait(Func<bool> condition) { Stopwatch timer = Stopwatch.StartNew(); while (!condition()) { if (timer.ElapsedMilliseconds > 15000) throw new Exception("Lifecycle control timed out."); Thread.Sleep(25); } }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: only exact owned Process objects are waited on. */
    //-------------------------------------------------------------------------------------------------
    private static void WaitExit(Process child) { if (!child.WaitForExit(15000)) throw new Exception("Owned host did not exit."); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: an exclusive open proves real handle release; an existing empty lock file alone is not an active lock. */
    //-------------------------------------------------------------------------------------------------
    private static bool LockFree(string path) { try { using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)) return true; } catch (IOException) { return false; } }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently hash preserved source/package bytes. */
    //-------------------------------------------------------------------------------------------------
    private static string Hash(string path) { using (SHA256 hash = SHA256.Create()) return BitConverter.ToString(hash.ComputeHash(File.ReadAllBytes(path))); }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: check only the PID recorded by this harness's own worker; never terminate an enumerated process. */
    //-------------------------------------------------------------------------------------------------
    private static bool RecordedProcessAlive(string path) { if (!File.Exists(path)) return false; try { using (Process process = Process.GetProcessById(Int32.Parse(File.ReadAllText(path)))) return !process.HasExited; } catch (ArgumentException) { return false; } }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: all directory moves/deletions use already-resolved developer children, with no redirected ancestry or descendants. */
    //-------------------------------------------------------------------------------------------------
    private static void AssertTree(string path)
    {
        string resolved = Path.GetFullPath(path);
        if (!String.Equals(resolved, Path.Combine(sdkBase, "mods", "RebornTest", "data"), StringComparison.OrdinalIgnoreCase) && !String.Equals(resolved, Path.Combine(sdkBase, "builtmods", "RebornTest"), StringComparison.OrdinalIgnoreCase)) throw new IOException("Cleanup is outside the exact developer data/output targets.");
        for (DirectoryInfo at = new DirectoryInfo(Path.GetFullPath(path)); at != null; at = at.Parent) if ((at.Attributes & FileAttributes.ReparsePoint) != 0) throw new IOException("Redirected test tree refused.");
        foreach (string item in Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories)) if ((File.GetAttributes(item) & FileAttributes.ReparsePoint) != 0) throw new IOException("Redirected test descendant refused.");
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: record each separate lifecycle assertion. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(bool condition, string label) { if (!condition) throw new Exception(label); passed++; Console.WriteLine("Reborn: PASS " + label); }
}
