using System.Reflection;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise retained and disk-loaded document metadata without claiming a diagnostic root is an approved EP1 compiler.
internal static class DocumentReuseSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove unchanged reuse and actual source/dependent-file reload through the production document lifecycle. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(), "Reborn-Ep1DocumentReuse-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        Settings previous = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "DependencyHashPipeline.xsd"),
                DataRoot = root, DataPaths = new[] { root }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            TestCycles(Path.Combine(root, "resident"), false, false);
            TestCycles(Path.Combine(root, "plain"), true, false);
            TestCycles(Path.Combine(root, "compressed"), true, true);
            TestMissingRetainedDependency(Path.Combine(root, "missing-retained"));
            Require(!Directory.EnumerateFiles(root, "*.manifest", SearchOption.AllDirectories).Any()
                && !Directory.EnumerateFiles(root, "*.asset", SearchOption.AllDirectories).Any(),
                "Document metadata proof unexpectedly emitted native/compiler output.");
            Console.WriteLine("Document reuse self-test: OK (resident/plain/compressed sessions, unchanged reuse, source/size/reported dependency reload, removed dependency, missing-file rejection)");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: run the same source/dependency mutations against retained objects and both production session serialization modes. */
    //-------------------------------------------------------------------------------------------------
    private static void TestCycles(string root, bool disk, bool compressed)
    {
        Directory.CreateDirectory(root);
        string source = Path.Combine(root, "probe.xml"), data = Path.Combine(root, "payload.dat");
        DateTime stamp = new(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc);
        WriteSource(source, 1, true, stamp);
        File.WriteAllText(data, "AAAA");
        File.SetLastWriteTimeUtc(data, stamp);
        SessionCache cache = new();
        cache.LoadCache(Path.Combine(root, "session.xml"));
        cache.InitializeCache(new List<string>());
        var initial = Process(cache, source, root);
        Require(initial.Loaded && initial.Reason == "New Document", "Initial source was not parsed.");
        cache = Next(cache, disk, compressed, new List<string>());
        var unchanged = Process(cache, source, root);
        Require(!unchanged.Loaded && unchanged.Identity == initial.Identity && unchanged.Dependency == initial.Dependency,
            "Unchanged cached document was reparsed or changed identity.");
        WriteSource(source, 2, true, stamp);
        cache = Next(cache, disk, compressed, new List<string> { source });
        Require(cache.DirtyStreams == null, "A changed document without stream hints did not request all streams.");
        var changedSource = Process(cache, source, root);
        Require(changedSource.Loaded && changedSource.Reason == "content changed" && changedSource.Identity != initial.Identity,
            "Reported same-signature XML edit reused stale document metadata.");
        File.WriteAllText(data, "LONGER");
        File.SetLastWriteTimeUtc(data, stamp);
        cache = Next(cache, disk, compressed, new List<string>());
        var changedSize = Process(cache, source, root);
        Require(changedSize.Loaded && changedSize.Reason == "dependent file changed"
            && changedSize.Identity != changedSource.Identity && changedSize.Dependency != changedSource.Dependency,
            "Timestamp-preserving dependent size edit did not reload the unchanged source.");
        File.WriteAllText(data, "CHANGE");
        File.SetLastWriteTimeUtc(data, stamp);
        cache = Next(cache, disk, compressed, new List<string> { data });
        var changedReported = Process(cache, source, root);
        Require(changedReported.Loaded && changedReported.Reason == "dependent file changed"
            && changedReported.Identity != changedSize.Identity, "Reported same-signature dependent edit reused stale identity.");
        cache = Next(cache, disk, compressed, new List<string>());
        var quiet = Process(cache, source, root);
        Require(!quiet.Loaded && quiet.Identity == changedReported.Identity, "Reloaded document did not become reusable.");
        WriteSource(source, 2, false, stamp);
        cache = Next(cache, disk, compressed, new List<string> { source });
        var removed = Process(cache, source, root);
        Require(removed.Loaded && removed.Dependency == 0, "Source reload retained an obsolete dependent file.");
        File.Delete(data);
        cache = Next(cache, disk, compressed, new List<string>());
        var irrelevant = Process(cache, source, root);
        Require(!irrelevant.Loaded && irrelevant.Identity == removed.Identity,
            "Deleting an unreferenced former dependency reloaded the document.");
        WriteSource(source, 2, true, stamp);
        cache = Next(cache, disk, compressed, new List<string> { source });
        bool missing = false;
        try { Process(cache, source, root); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.FileNotFound) { missing = true; }
        Require(missing, "Missing reintroduced dependent file did not report FileNotFound.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: checkpoint through real SaveCache/LoadCache or retain the same objects for the resident-path comparison. */
    //-------------------------------------------------------------------------------------------------
    private static SessionCache Next(SessionCache cache, bool disk, bool compressed, List<string> changed)
    {
        if (disk)
        {
            string path = cache.CacheFileName;
            cache.SaveCache(compressed);
            cache = new SessionCache();
            cache.LoadCache(path);
            Require(typeof(SessionCache).GetField("_last", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(cache) != null,
                "Disk cache load silently fell back to a fresh session.");
        }
        else typeof(SessionCache).GetMethod("MakeCacheable", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(cache, null);
        cache.InitializeCache(changed);
        return cache;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: an unchanged cached source must reject a deleted live dependency rather than silently retain its old identity. */
    //-------------------------------------------------------------------------------------------------
    private static void TestMissingRetainedDependency(string root)
    {
        Directory.CreateDirectory(root);
        string source = Path.Combine(root, "probe.xml"), data = Path.Combine(root, "payload.dat");
        DateTime stamp = new(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc);
        WriteSource(source, 1, true, stamp);
        File.WriteAllText(data, "AAAA");
        SessionCache cache = new();
        string savedPath = Path.Combine(root, "session.xml");
        cache.LoadCache(savedPath);
        cache.InitializeCache(new List<string>());
        var initial = Process(cache, source, root);
        cache = Next(cache, true, false, new List<string>());
        Require(!Process(cache, source, root).Loaded, "Missing-dependency fixture did not first demonstrate reuse.");
        File.Delete(data);
        cache = Next(cache, true, false, new List<string>());
        bool rejected = false;
        try { Process(cache, source, root); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.FileNotFound) { rejected = true; }
        Require(rejected, "Cached source accepted a deleted dependent file.");
        // Reborn: recover from the last good disk checkpoint, not an unproven partially failed document transaction.
        File.WriteAllText(data, "AAAA");
        cache = new SessionCache();
        cache.LoadCache(savedPath);
        cache.InitializeCache(new List<string>());
        var recovered = Process(cache, source, root);
        Require(recovered.Identity == initial.Identity && recovered.Dependency == initial.Dependency,
            "Restored dependency failed to recover the prior document identity.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain production metadata reuse while leaving the synthetic root unregistered and all native output absent. */
    //-------------------------------------------------------------------------------------------------
    private static (bool Loaded, string Reason, uint Identity, uint Dependency) Process(SessionCache cache, string source, string root)
    {
        PluginRegistry plugins = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
        Require(plugins.CanReuseCompiledDocuments, "Diagnostic registry cannot exercise reuse.");
        DocumentProcessor processor = new(Settings.Current, plugins, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
            { Cache = cache, SchemaSet = new SchemaSet(false) };
        using OutputManager output = new(processor, null, Path.Combine(root, "unused-output"), Path.Combine(root, "unused-intermediate"), null, null, null);
        AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, output,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
        Require(document.State == DocumentState.Complete && document.SelfInstances.Count == 1
            && document.SelfInstances.Single().Handle.TypeHash == 0 && output.Assets.Count == 0,
            "Diagnostic document state or unregistered type policy differs.");
        object state = typeof(AssetDeclarationDocument).GetField("_current", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(document)!;
        return (document.IsLoaded, (string?)state.GetType().GetField("ChangeReason")!.GetValue(state) ?? "",
            document.SelfInstances.Single().Handle.InstanceHash, (uint)state.GetType().GetField("DependentFileHash")!.GetValue(state)!);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep XML lengths and timestamps stable so reported source changes must force real hashing. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteSource(string path, int value, bool dependent, DateTime stamp)
    {
        string data = dependent ? " Data=\"payload.dat\"" : "";
        File.WriteAllText(path, $"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><DependencyProbe id=\"Probe\" Payload=\"{value}\"{data} /></AssetDeclaration>");
        File.SetLastWriteTimeUtc(path, stamp);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop retained-document proof at its first lifecycle, identity or serialization mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
