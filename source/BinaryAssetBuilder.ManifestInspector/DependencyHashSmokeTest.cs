using System.Reflection;
using System.Text;
using System.Xml;
using BinaryAssetBuilder;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Core.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: distinguish runtime asset-ID dependencies from embedded file-content hashing without inventing an approved EP1 compiler.
internal static class DependencyHashSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test real file-hash invalidation and focused full-document dependency identity propagation. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-Ep1DependencyHash-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        TestFileSignatures(directory);
        Settings previous = Settings.Current;
        try
        {
            string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
            Settings.Current = new Settings { BuildCache = false, SchemaPath = Path.Combine(fixtures, "DependencyHashPipeline.xsd"),
                DataRoot = directory, DataPaths = new[] { directory }, TargetPlatform = TargetPlatform.Win32,
                CustomPostfix = "", StreamPostfix = "", StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>() };
            string target = Path.Combine(directory, "target.xml");
            string parent = Path.Combine(directory, "parent.xml");
            string blob = Path.Combine(directory, "payload.dat");
            WriteTarget(target, 1);
            WriteParent(parent, "Target", "Target", true);
            File.WriteAllText(blob, "AAAA");
            var first = Build(parent);
            var repeated = Build(parent);
            Require(first == repeated, "Unchanged fresh document builds changed identity hashes.");
            WriteTarget(target, 2);
            var changedTarget = Build(parent);
            Require(changedTarget.Parent == first.Parent && changedTarget.Target != first.Target,
                "Target content changed parent identity or failed to change target identity.");
            WriteParent(parent, "OtherTarget", "Target", true);
            var changedStrongId = Build(parent);
            Require(changedStrongId.Parent != changedTarget.Parent, "Changing a strong reference ID did not invalidate parent identity.");
            WriteParent(parent, "Target", "OtherTarget", true);
            var changedWeakId = Build(parent);
            Require(changedWeakId.Parent != changedTarget.Parent, "Changing a weak reference ID did not invalidate parent identity.");
            WriteParent(parent, "Target", "Target", true);
            File.WriteAllText(blob, "BBBB");
            var changedFile = Build(parent);
            Require(changedFile.Parent != changedTarget.Parent && changedFile.Dependent != changedTarget.Dependent
                && changedFile.Target == changedTarget.Target, "File dependency content did not invalidate its parent/document identity.");
            WriteParent(parent, "Target", "Target", false);
            var noFile = Build(parent);
            File.WriteAllText(blob, "CCCCC");
            Require(Build(parent) == noFile, "Unreferenced file content changed asset identity.");
            WriteParent(parent, "Target", "Target", true);
            File.Delete(blob);
            bool missingRejected = false;
            try { Build(parent); }
            catch (BinaryAssetBuilderException exception) when (exception.ErrorCode == ErrorCode.FileNotFound) { missingRejected = true; }
            Require(missingRejected, "Missing file dependency did not report FileNotFound.");
            Require(!Directory.EnumerateFiles(directory, "*.manifest").Any(), "Hash proof unexpectedly emitted production output.");
            Console.WriteLine("Dependency hash self-test: OK (file size/timestamp/reset/serialization/deletion, full document references, file-content propagation, ID changes, missing-file error)");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use real FileHashItem reset cycles, proving retained timestamps cannot hide length changes or restored missing files. */
    //-------------------------------------------------------------------------------------------------
    private static void TestFileSignatures(string directory)
    {
        string path = Path.Combine(directory, "signature.dat");
        DateTime stamp = new(2020, 1, 2, 3, 4, 6, DateTimeKind.Utc);
        File.WriteAllText(path, "ONE");
        File.SetLastWriteTimeUtc(path, stamp);
        FileHashItem item = new(path, "", TargetPlatform.Win32);
        uint first = item.Hash;
        item.Reset();
        Require(!item.IsDirty && item.Hash == first, "Unchanged file signature was dirty.");
        string serialized = Serialize(item);
        FileHashItem restored = Deserialize(serialized);
        Require(!restored.IsDirty && restored.Hash == first, "Length-aware file signature did not round-trip.");
        XmlDocument old = new();
        old.LoadXml(serialized);
        old.DocumentElement!.SetAttribute("d", string.Join(';', old.DocumentElement.GetAttribute("d").Split(';').Take(5)));
        FileHashItem legacy = Deserialize(old.OuterXml);
        Require(legacy.IsDirty && legacy.Hash == first, "Old timestamp-only record did not force rehash.");
        File.WriteAllText(path, "LONGER");
        File.SetLastWriteTimeUtc(path, stamp);
        item.Reset();
        Require(item.IsDirty && item.Hash != first, "Timestamp-preserving size change retained a stale hash.");
        uint larger = item.Hash;
        File.Delete(path);
        item.Reset();
        Require(!item.Exists && item.Hash == 0, "Missing file retained its old hash.");
        File.WriteAllText(path, "LONGER");
        File.SetLastWriteTimeUtc(path, stamp);
        item.Reset();
        Require(item.IsDirty && item.Hash == larger, "Restoring an old signature retained the missing-file zero hash.");
        File.WriteAllText(path, "CHANGE");
        File.SetLastWriteTimeUtc(path, stamp.AddSeconds(2));
        item.Reset();
        Require(item.IsDirty && item.Hash != larger, "Same-size timestamp-changing edit retained a stale hash.");
        Require(SessionCache.CacheVersion > 18u, "EP1 session version did not invalidate timestamp-only cache records.");
        // Reborn: exercise actual session initialization with an old format and a current document processor revision.
        SessionCache oldSession = new();
        Type lastType = typeof(SessionCache).GetNestedType("LastState", BindingFlags.NonPublic)!;
        object last = Activator.CreateInstance(lastType, true)!;
        lastType.GetProperty("Version")!.SetValue(last, 18u);
        lastType.GetProperty("DocumentProcessorVersion")!.SetValue(last, DocumentProcessor.Version);
        FieldInfo lastField = typeof(SessionCache).GetField("_last", BindingFlags.NonPublic | BindingFlags.Instance)!;
        lastField.SetValue(oldSession, last);
        oldSession.InitializeCache(new List<string>());
        Require(lastField.GetValue(oldSession) == null, "Actual session initialization retained the old cache format.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: isolate each document comparison in a fresh session; this tests source-to-identity semantics, not compiled-session reuse. */
    //-------------------------------------------------------------------------------------------------
    private static (uint Parent, uint Target, uint Dependent) Build(string source)
    {
        PluginRegistry registry = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
        SessionCache cache = new();
        cache.InitializeCache(new List<string>());
        DocumentProcessor processor = new(Settings.Current, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
            { Cache = cache, SchemaSet = new SchemaSet(false) };
        AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
        InstanceDeclaration parent = document.SelfInstances.Single();
        InstanceDeclaration target = document.AllInstances.Single();
        Require(parent.Handle.TypeHash == 0 && target.Handle.TypeHash == 0,
            "Diagnostic unregistered roots unexpectedly became production-capable.");
        Require(parent.ReferencedInstances.Count == 1 && parent.WeakReferencedInstances.Count == 1
            && parent.ReferencedInstances[0].TypeId == parent.Handle.TypeId
            && parent.WeakReferencedInstances[0].TypeId == parent.Handle.TypeId
            && parent.XmlNode.Attributes!["Strong"]!.Value.EndsWith("\\0", StringComparison.Ordinal)
            && !parent.XmlNode.Attributes["Weak"]!.Value.Contains(':'), "Reference normalization or counts differ.");
        object state = typeof(AssetDeclarationDocument).GetField("_current", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(document)!;
        uint dependent = (uint)state.GetType().GetField("DependentFileHash")!.GetValue(state)!;
        return (parent.Handle.InstanceHash, target.Handle.InstanceHash, dependent);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit a tiny target document whose stable ID separates content changes from runtime reference changes. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteTarget(string path, int value)
    {
        File.WriteAllText(path, $"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><DependencyProbe id=\"Target\" Payload=\"{value}\" /></AssetDeclaration>");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve parent source except the dependency field under test; all-includes avoid reference-stream production builds. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteParent(string path, string strong, string weak, bool file)
    {
        string data = file ? " Data=\"payload.dat\"" : "";
        // Reborn: the official WeakReference pattern accepts unqualified instance IDs; its schema supplies the asset type.
        File.WriteAllText(path, $"<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><Includes><Include type=\"all\" source=\"target.xml\" /></Includes><DependencyProbe id=\"Parent\" Strong=\"{strong}\" Weak=\"{weak}\"{data} /></AssetDeclaration>");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the real file-signature fields rather than manufacturing a test-only cache format. */
    //-------------------------------------------------------------------------------------------------
    private static string Serialize(FileHashItem item)
    {
        StringBuilder xml = new();
        using (XmlWriter writer = XmlWriter.Create(xml, new XmlWriterSettings { OmitXmlDeclaration = true }))
        { writer.WriteStartElement("FileHashItem"); item.WriteXml(writer); writer.WriteEndElement(); }
        return xml.ToString();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: load modern and legacy signature records through the production XML reader. */
    //-------------------------------------------------------------------------------------------------
    private static FileHashItem Deserialize(string xml)
    {
        XmlDocument document = new();
        document.LoadXml(xml);
        FileHashItem item = new();
        item.ReadXml(new Node(document.DocumentElement!.CreateNavigator()!, new XmlNamespaceManager(document.NameTable)));
        return item;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first file-cache or document-dependency identity mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
