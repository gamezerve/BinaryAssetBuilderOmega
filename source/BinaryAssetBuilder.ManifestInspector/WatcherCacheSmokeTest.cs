using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.IO;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Core.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise retained resident sessions and watcher callbacks without relying on nondeterministic OS event timing.
internal static class WatcherCacheSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify reported same-signature edits, quiet-file handling, stream routing and synchronized event snapshots. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(), "Reborn-Ep1WatcherCache-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        string changed = Path.Combine(root, "changed.dat");
        string quiet = Path.Combine(root, "quiet.dat");
        DateTime stamp = new(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc);
        WriteFixed(changed, "AAAA", stamp);
        WriteFixed(quiet, "QUIET", stamp);
        SessionCache cache = new();
        cache.InitializeCache(new List<string>());
        FileHashItem first = File(cache, changed);
        FileHashItem secondConfig = File(cache, changed, "high");
        FileHashItem untouched = File(cache, quiet);
        uint baseline = first.Hash;
        Require(secondConfig.Hash == baseline, "Configuration changed identical source content hash.");
        uint quietHash = untouched.Hash;
        Checkpoint(cache);
        WriteFixed(changed, "BBBB", stamp);
        cache.InitializeCache(new List<string> { changed.ToUpperInvariant().Replace('\\', '/'), changed });
        Require(first.IsDirty && secondConfig.IsDirty && first.Hash != baseline && secondConfig.Hash == first.Hash,
            "Reported same-time/same-size edit did not refresh every configuration.");
        Require(!untouched.IsDirty && untouched.Hash == quietHash, "Unrelated unchanged file was rejected or invalidated.");
        Require(cache.DirtyStreams == null, "Changed non-document file did not conservatively dirty all streams.");
        Checkpoint(cache);
        WriteFixed(quiet, "LONGER-QUIET", stamp);
        cache.InitializeCache(new List<string>());
        Require(untouched.IsDirty && untouched.Hash != quietHash, "Resident fallback reused a prior build's memoized metadata.");
        Checkpoint(cache);
        WriteFixed(quiet, "NEW-SIZE", stamp);
        bool omittedRejected = false;
        try { cache.InitializeCache(new List<string> { changed }); }
        catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.PathMonitor) { omittedRejected = true; }
        Require(omittedRejected, "Actually dirty unreported metadata did not trigger the monitor consistency error.");
        cache.InitializeCache(new List<string>());
        _ = untouched.Hash;
        // Reborn: a pending forced refresh must not disappear if the signature is serialized before its content hash is consumed.
        first.Reset(true);
        XmlDocument xml = new();
        using (StringWriter text = new())
        {
            using (XmlWriter writer = XmlWriter.Create(text)) { writer.WriteStartElement("File"); first.WriteXml(writer); writer.WriteEndElement(); }
            xml.LoadXml(text.ToString());
        }
        FileHashItem pending = new();
        pending.ReadXml(new Node(xml.DocumentElement!.CreateNavigator()!, new XmlNamespaceManager(xml.NameTable)));
        Require(pending.IsDirty && pending.Hash == first.Hash, "Pending forced rehash did not survive serialization.");
        TestStreamHints(root, stamp);
        TestMonitor(root);
        Require(SessionCache.CacheVersion == 21u, "Active EP1 watcher-session version differs.");
        // Reborn: prove the immediately previous version is dropped before its old resident records are reused.
        Checkpoint(cache);
        FieldInfo lastField = typeof(SessionCache).GetField("_last", BindingFlags.NonPublic | BindingFlags.Instance)!;
        object oldSession = lastField.GetValue(cache)!;
        oldSession.GetType().GetProperty("Version")!.SetValue(oldSession, 20u);
        cache.InitializeCache(new List<string>());
        Require(lastField.GetValue(cache) == null, "Version-20 session survived watcher-policy invalidation.");
        Console.WriteLine("Watcher/cache self-test: OK (same-signature edits, resident refresh, two configurations, quiet files, omitted changes, forced serialization, stream hints, rename/create callbacks, nested config, concurrency/trust)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: route reported document changes through real SessionCache checks with focused cached stream-hint metadata. */
    //-------------------------------------------------------------------------------------------------
    private static void TestStreamHints(string root, DateTime stamp)
    {
        string path = Path.Combine(root, "hinted.xml");
        WriteFixed(path, "AAAA", stamp);
        SessionCache cache = new();
        cache.InitializeCache(new List<string>());
        FileHashItem item = File(cache, path);
        uint original = item.Hash;
        AssetDeclarationDocument document = new();
        Type lastType = typeof(AssetDeclarationDocument).GetNestedType("LastState", BindingFlags.NonPublic)!;
        object last = Activator.CreateInstance(lastType, true)!;
        lastType.GetField("StreamHints")!.SetValue(last, new List<string> { "stream-one", "stream-two" });
        typeof(AssetDeclarationDocument).GetField("_last", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(document, last);
        cache.SaveDocumentToCache(path, "", TargetPlatform.Win32, document);
        Checkpoint(cache);
        WriteFixed(path, "BBBB", stamp);
        cache.InitializeCache(new List<string> { path });
        Require(cache.DirtyStreams != null && cache.DirtyStreams.OrderBy(value => value).SequenceEqual(new[] { "stream-one", "stream-two" })
            && item.Hash != original, "Reported document failed to invalidate its known stream hints.");
        Checkpoint(cache);
        cache.InitializeCache(new List<string>());
        Require(cache.DirtyStreams != null && cache.DirtyStreams.Count == 0 && !item.IsDirty,
            "Unchanged resident document kept dirty stream hints.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invoke actual callback handlers deterministically and inspect real watcher configuration with immediate cleanup. */
    //-------------------------------------------------------------------------------------------------
    private static void TestMonitor(string root)
    {
        PathMonitor monitor = new(Array.Empty<string>());
        MethodInfo changed = typeof(PathMonitor).GetMethod("OnChanged", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo renamed = typeof(PathMonitor).GetMethod("OnRenamed", BindingFlags.NonPublic | BindingFlags.Instance)!;
        MethodInfo error = typeof(PathMonitor).GetMethod("OnError", BindingFlags.NonPublic | BindingFlags.Instance)!;
        changed.Invoke(monitor, new object[] { monitor, new FileSystemEventArgs(WatcherChangeTypes.Created, root, "created.xml") });
        changed.Invoke(monitor, new object[] { monitor, new FileSystemEventArgs(WatcherChangeTypes.Changed, root, "CREATED.XML") });
        renamed.Invoke(monitor, new object[] { monitor, new RenamedEventArgs(WatcherChangeTypes.Renamed, root, "new.xml", "old.xml") });
        List<string> snapshot = monitor.GetChangedFiles();
        Require(snapshot.Count == 3 && snapshot.Contains(Path.Combine(root, "old.xml")) && snapshot.Contains(Path.Combine(root, "new.xml")),
            "Rename endpoints, created callback or case-insensitive deduplication failed.");
        monitor.Reset();
        Require(monitor.GetChangedFiles().Count == 0 && monitor.IsResultTrustable, "Clean event reset failed.");
        Task.WaitAll(Enumerable.Range(0, 100).Select(i => Task.Run(() => changed.Invoke(monitor,
            new object[] { monitor, new FileSystemEventArgs(WatcherChangeTypes.Changed, root, $"parallel{i}.xml") }))).ToArray());
        Require(monitor.GetChangedFiles().Count == 100 && !monitor.IsResultTrustable && snapshot.Count == 3,
            "Concurrent callbacks lost events, changed an old snapshot or bypassed the trust limit.");
        monitor.Reset();
        error.Invoke(monitor, new object[] { monitor, new ErrorEventArgs(new IOException("Synthetic watcher error")) });
        monitor.Reset();
        Require(!monitor.IsResultTrustable, "Reset falsely recovered an unrepaired watcher error.");
        PathMonitor configured = new(new[] { root });
        var watchers = (List<FileSystemWatcher>)typeof(PathMonitor).GetField("_watchers", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(configured)!;
        try { Require(watchers.Single().IncludeSubdirectories, "Watcher ignored nested asset directories."); }
        finally { foreach (FileSystemWatcher watcher in watchers) watcher.Dispose(); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain actual current cache records in memory without writing a synthetic session XML file. */
    //-------------------------------------------------------------------------------------------------
    private static void Checkpoint(SessionCache cache)
    {
        typeof(SessionCache).GetMethod("MakeCacheable", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(cache, null);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retrieve configuration-aware file records through the production cache interface. */
    //-------------------------------------------------------------------------------------------------
    private static FileHashItem File(SessionCache cache, string path, string configuration = "")
    {
        Require(cache.TryGetFile(path, configuration, TargetPlatform.Win32, out FileHashItem item), "Fixture file was not found.");
        return item;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create same-signature changes without altering any user/game/cache file. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteFixed(string path, string text, DateTime stamp)
    {
        System.IO.File.WriteAllText(path, text);
        System.IO.File.SetLastWriteTimeUtc(path, stamp);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on the first watcher or retained-session invalidation mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
