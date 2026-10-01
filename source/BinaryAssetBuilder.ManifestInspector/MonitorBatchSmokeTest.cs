using System.Reflection;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.IO;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: validate atomic notification generations and the exact cache handoff used by the builder, not OS event delivery timing.
internal static class MonitorBatchSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove late-event preservation, failed-batch restoration, captured trust and positive-report fallback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(), "Reborn-Ep1MonitorBatch-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        PathMonitor monitor = new(Array.Empty<string>());
        Notify(monitor, root, "first.xml");
        PathMonitor.ChangeBatch first = monitor.DrainChanges(false);
        Require(first.IsTrustable && first.Paths.Count == 1 && first.EventCount == 1, "Initial atomic batch differs.");
        Notify(monitor, root, "later.xml");
        Notify(monitor, root, "first.xml");
        monitor.CompleteBatch(first);
        PathMonitor.ChangeBatch later = monitor.DrainChanges(false);
        Require(later.EventCount == 2 && later.Paths.Count == 2 && first.Paths.Count == 1,
            "Acknowledgement erased late events or changed the consumed snapshot.");
        ExpectRejected(() => monitor.RestoreBatch(first));
        Notify(monitor, root, "newer.xml");
        monitor.RestoreBatch(later);
        PathMonitor.ChangeBatch merged = monitor.DrainChanges(false);
        Require(merged.Paths.Count == 3 && merged.EventCount == 3, "Failed batch did not merge with later events.");
        ExpectRejected(() => monitor.RestoreBatch(later));
        PathMonitor foreign = new(Array.Empty<string>());
        ExpectRejected(() => foreign.CompleteBatch(merged));
        monitor.CompleteBatch(merged);
        ExpectRejected(() => monitor.CompleteBatch(merged));
        Require(monitor.DrainChanges(false).Paths.Count == 0, "Completed batches remained queued.");
        // Reborn: trust is captured with the drained counter, not a stale caller check preceding more callbacks.
        Require(monitor.IsResultTrustable, "Fresh monitor was not trusted.");
        for (int i = 0; i < PathMonitor.EventLimit; i++) Notify(monitor, root, "overflow.xml");
        PathMonitor.ChangeBatch overflow = monitor.DrainChanges(false);
        Require(!overflow.IsTrustable && overflow.EventCount == PathMonitor.EventLimit && overflow.Paths.Count == 1,
            "Deduplication or prior trust check incorrectly made an overflow batch complete.");
        monitor.RestoreBatch(overflow);
        PathMonitor.ChangeBatch restoredOverflow = monitor.DrainChanges(false);
        Require(!restoredOverflow.IsTrustable, "Restored uncertain batch became trusted.");
        monitor.CompleteBatch(restoredOverflow);
        TestHandoff(root);
        TestPartialReports(root);
        TestConcurrentDrain(root);
        Console.WriteLine("Monitor batch self-test: OK (late/repeated paths, immutable generations, restore/replay/ownership, captured overflow trust, builder handoff success/failure, partial positive hashes, 500 concurrent events, counter saturation)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: inject callbacks during the production handoff helper so cache initialization cannot clear later notifications. */
    //-------------------------------------------------------------------------------------------------
    private static void TestHandoff(string root)
    {
        PathMonitor monitor = new(Array.Empty<string>());
        CapturingCache cache = new();
        Notify(monitor, root, "before.xml");
        cache.DuringInitialize = () => Notify(monitor, root, "during.xml");
        monitor.InitializeCache(cache, true);
        Require(cache.Paths!.SequenceEqual(new[] { Path.Combine(root, "before.xml") }) && cache.Complete,
            "Builder handoff did not consume the expected complete batch.");
        cache.DuringInitialize = null;
        monitor.InitializeCache(cache, true);
        Require(cache.Paths!.SequenceEqual(new[] { Path.Combine(root, "during.xml") }), "Successful initialization lost a concurrent callback.");
        Notify(monitor, root, "failed.xml");
        cache.DuringInitialize = () => { Notify(monitor, root, "failed.xml"); Notify(monitor, root, "new-during-failure.xml"); };
        cache.Fail = true;
        bool failed = false;
        try { monitor.InitializeCache(cache, true); }
        catch (IOException) { failed = true; }
        Require(failed, "Synthetic cache failure did not propagate.");
        cache.Fail = false;
        cache.DuringInitialize = null;
        monitor.InitializeCache(cache, true);
        Require(cache.Paths!.Count == 2 && cache.Paths.Contains(Path.Combine(root, "failed.xml"))
            && cache.Paths.Contains(Path.Combine(root, "new-during-failure.xml")), "Failed initialization discarded old or new events.");
        Notify(monitor, root, "nonresident.xml");
        monitor.InitializeCache(cache, false);
        Require(cache.Paths!.Count == 0 && !cache.Complete, "Nonresident handoff changed metadata-fallback semantics.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: incomplete batches retain known positive changes and find metadata changes without falsely claiming complete coverage. */
    //-------------------------------------------------------------------------------------------------
    private static void TestPartialReports(string root)
    {
        string known = Path.Combine(root, "known.dat");
        string omitted = Path.Combine(root, "omitted.dat");
        DateTime stamp = new(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc);
        File.WriteAllText(known, "AAAA");
        File.SetLastWriteTimeUtc(known, stamp);
        File.WriteAllText(omitted, "ONE");
        SessionCache cache = new();
        cache.InitializeCache(new List<string>());
        Require(cache.TryGetFile(known, "", TargetPlatform.Win32, out FileHashItem reported)
            && cache.TryGetFile(omitted, "", TargetPlatform.Win32, out _), "Partial-report files were not cached.");
        cache.TryGetFile(omitted, "", TargetPlatform.Win32, out FileHashItem unreported);
        uint oldKnown = reported.Hash, oldOmitted = unreported.Hash;
        typeof(SessionCache).GetMethod("MakeCacheable", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(cache, null);
        File.WriteAllText(known, "BBBB");
        File.SetLastWriteTimeUtc(known, stamp);
        File.WriteAllText(omitted, "LONGER");
        PathMonitor monitor = new(Array.Empty<string>());
        for (int i = 0; i < PathMonitor.EventLimit; i++) Notify(monitor, root, "known.dat");
        monitor.InitializeCache(cache, true);
        Require(reported.Hash != oldKnown && unreported.Hash != oldOmitted && cache.DirtyStreams == null,
            "Overflow fallback lost positive same-signature changes or rejected omitted metadata changes.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: conserve every unique event across concurrent atomic drains and prevent counter wraparound from restoring trust. */
    //-------------------------------------------------------------------------------------------------
    private static void TestConcurrentDrain(string root)
    {
        PathMonitor monitor = new(Array.Empty<string>());
        List<string> collected = new();
        long events = 0;
        Task writer = Task.Run(() => { for (int i = 0; i < 500; i++) Notify(monitor, root, $"concurrent{i}.xml"); });
        for (int i = 0; i < 20; i++)
        {
            PathMonitor.ChangeBatch batch = monitor.DrainChanges(false);
            collected.AddRange(batch.Paths);
            events += batch.EventCount;
            monitor.CompleteBatch(batch);
        }
        writer.GetAwaiter().GetResult();
        PathMonitor.ChangeBatch final = monitor.DrainChanges(false);
        collected.AddRange(final.Paths);
        events += final.EventCount;
        monitor.CompleteBatch(final);
        Require(events == 500 && collected.Count == 500 && collected.Distinct(StringComparer.OrdinalIgnoreCase).Count() == 500,
            "Concurrent draining lost or duplicated events.");
        typeof(PathMonitor).GetField("_numEvents", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(monitor, int.MaxValue - 1);
        Notify(monitor, root, "saturate-one.xml");
        Notify(monitor, root, "saturate-two.xml");
        PathMonitor.ChangeBatch saturated = monitor.DrainChanges(false);
        Require(saturated.EventCount == int.MaxValue && !saturated.IsTrustable, "Event count wrapped into trusted range.");
        Notify(monitor, root, "saturate-new.xml");
        monitor.RestoreBatch(saturated);
        PathMonitor.ChangeBatch merged = monitor.DrainChanges(false);
        Require(merged.EventCount == int.MaxValue && !merged.IsTrustable && merged.Paths.Count == 3, "Restored event count overflowed.");
        monitor.CompleteBatch(merged);
    }

    // Reborn: controlled cache callbacks exercise the real helper and then delegate successful initialization to production SessionCache.
    private sealed class CapturingCache : SessionCache
    {
        public List<string>? Paths;
        public bool Complete;
        public bool Fail;
        public Action? DuringInitialize;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: inject events or initialization failure at the exact batch handoff boundary, without fabricating compiler readiness. */
        //-------------------------------------------------------------------------------------------------
        public override void InitializeCache(List<string> knownChangedFiles, bool notificationsComplete)
        {
            Paths = new List<string>(knownChangedFiles);
            Complete = notificationsComplete;
            DuringInitialize?.Invoke();
            if (Fail) throw new IOException("Synthetic cache initialization failure.");
            base.InitializeCache(knownChangedFiles, notificationsComplete);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke the real change callback directly so tests do not depend on native notification latency. */
    //-------------------------------------------------------------------------------------------------
    private static void Notify(PathMonitor monitor, string root, string name)
    {
        typeof(PathMonitor).GetMethod("OnChanged", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(monitor,
            new object[] { monitor, new FileSystemEventArgs(WatcherChangeTypes.Changed, root, name) });
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid replay/ownership operations must reject without altering the queue. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException) { return; }
        throw new InvalidDataException("Invalid batch ownership/state was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop notification-generation proof at its first mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
