using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove concrete external audio dependency identities separately from FX/audio processor readiness.
internal static class FXAudioResolutionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise actual schema inheritance and output preparation with isolated metadata and optional stock manifests. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string[] stockManifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-FXAudio-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            Settings.Current = new Settings { BuildCache = false, ErrorLevel = 1,
                SchemaPath = Path.Combine(fixtures, "FXListPipeline.xsd"), DataRoot = directory, DataPaths = new[] { directory },
                TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", StreamPostfix = "",
                StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>(), ProcessedExternalManifests = Array.Empty<string>() };
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(Settings.Current, new PluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32),
                new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "source.xml");
            File.Copy(Path.Combine(fixtures, "FXListProbe.xml"), source);
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
            InstanceDeclaration sound = document.SelfInstances.Single(value => value.Handle.InstanceName == "FX_DebrisHitGround");
            InstanceDeclaration masks = document.SelfInstances.Single(value => value.Handle.InstanceName == "FX_ALL_AntiGroundAircraft_VoiceDie");
            Require(document.SelfInstances.All(value => value.Handle.TypeHash == 0), "Audio proof unexpectedly enabled an FX compiler.");
            // Reborn: enable only the private dependency seam; no output manager or production plugin is created.
            foreach (InstanceDeclaration instance in document.SelfInstances) instance.Handle.TypeHash = 0x17B3B82Du;
            string originalXml = masks.XmlNode.OuterXml;
            InstanceHandle[] originalHandles = masks.ReferencedInstances.ToArray();
            Require(originalHandles.All(handle => handle.TypeName == "BaseAudioEventInfo"), "Expected official untyped base audio references.");
            ExpectFailure(document, masks, ErrorCode.UnknownReference);
            ExpectFailure(document, masks, ErrorCode.UnknownReference);
            string debris = WriteTarget(directory, "debris", "AudioEvent", sound.ReferencedInstances.Single().InstanceName);
            string first = WriteTarget(directory, "first", "AudioEvent", originalHandles[0].InstanceName);
            string second = WriteTarget(directory, "second", "Multisound", originalHandles[1].InstanceName);
            string[] valid = { debris, first, second };
            Settings.Current.ProcessedExternalManifests = valid;
            CheckResolved(document, masks, new[] { "AudioEvent", "Multisound" });
            CheckResolved(document, sound, new[] { "AudioEvent" });
            // Reborn: duplicate manifest entries must not create duplicate candidate identities.
            string duplicate = WriteTarget(directory, "duplicate", "AudioEvent", originalHandles[0].InstanceName);
            Settings.Current.ProcessedExternalManifests = valid.Append(duplicate).ToArray();
            CheckResolved(document, masks, new[] { "AudioEvent", "Multisound" });
            string ambiguous = WriteTarget(directory, "ambiguous", "Multisound", originalHandles[0].InstanceName);
            Settings.Current.ProcessedExternalManifests = valid.Append(ambiguous).ToArray();
            ExpectFailure(document, masks, ErrorCode.ReferencingError);
            ExpectFailure(document, masks, ErrorCode.ReferencingError);
            Settings.Current.ProcessedExternalManifests = valid;
            CheckResolved(document, masks, new[] { "AudioEvent", "Multisound" });
            // Reborn: a same-name unrelated type cannot satisfy an official audio base reference.
            string wrong = WriteTarget(directory, "wrong", "FXList", originalHandles[1].InstanceName);
            Settings.Current.ProcessedExternalManifests = new[] { first, wrong };
            ExpectFailure(document, masks, ErrorCode.UnknownReference);
            Settings.Current.ProcessedExternalManifests = new[] { first };
            ExpectFailure(document, masks, ErrorCode.UnknownReference);
            // Reborn: schema descendants are transitive; do not hard-code just two observed audio type IDs in the core.
            string descendant = WriteTarget(directory, "descendant", "AudioEventOverridable", originalHandles[0].InstanceName);
            Settings.Current.ProcessedExternalManifests = new[] { descendant, second };
            CheckResolved(document, masks, new[] { "AudioEventOverridable", "Multisound" });
            string exact = WriteTarget(directory, "exact", "BaseAudioEventInfo", originalHandles[0].InstanceName);
            Settings.Current.ProcessedExternalManifests = valid.Concat(new[] { ambiguous, exact }).ToArray();
            CheckResolved(document, masks, new[] { "BaseAudioEventInfo", "Multisound" });
            // Reborn: refresh a same-path identity replacement rather than retaining the previous concrete target.
            DateTime timestamp = File.GetLastWriteTimeUtc(first);
            ExternalLinkSmokeTest.WriteFixture(first, new InstanceHandle("Multisound", originalHandles[0].InstanceName), new ReferencedFileBuffer());
            File.SetLastWriteTimeUtc(first, timestamp.AddSeconds(2));
            Settings.Current.ProcessedExternalManifests = valid;
            CheckResolved(document, masks, new[] { "Multisound", "Multisound" });
            ExternalLinkSmokeTest.WriteFixture(first, new InstanceHandle("AudioEvent", originalHandles[0].InstanceName), new ReferencedFileBuffer());
            File.SetLastWriteTimeUtc(first, timestamp.AddSeconds(4));
            CheckResolved(document, masks, new[] { "AudioEvent", "Multisound" });
            CheckExplicitType(processor, directory, masks.XmlNode, first, ambiguous, second);
            if (stockManifests.Length > 0)
            {
                Settings.Current.ProcessedExternalManifests = stockManifests.Select(Path.GetFullPath).ToArray();
                CheckResolved(document, sound, new[] { "AudioEvent" });
                CheckResolved(document, masks, new[] { "AudioEvent", "Multisound" });
                Console.WriteLine("  Real EP1 manifest audio identities: AudioEvent/AudioEvent/Multisound selected through core dependency preparation.");
            }
            Require(originalXml == masks.XmlNode.OuterXml && originalHandles.Select(handle => handle.Name).SequenceEqual(masks.ReferencedInstances.Select(handle => handle.Name)),
                "Derived resolution mutated normalized XML or original reference handles.");
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "Audio metadata proof emitted game streams.");
            Console.WriteLine("FX audio resolution self-test: OK (official inheritance, ordered concrete IDs/selectors, missing/wrong/ambiguous rejection, exact precedence, duplicate identity, transitive/explicit types, refresh/recovery; no production registration)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: an explicitly typed audio reference may not widen to sibling Multisound identities. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckExplicitType(DocumentProcessor processor, string directory, XmlNode normalized, string first, string ambiguous, string second)
    {
        XmlDocument copy = new(); copy.LoadXml(normalized.OuterXml);
        foreach (XmlElement element in copy.SelectNodes("//*")!.OfType<XmlElement>())
        {
            element.RemoveAttribute("TypeId");
            if (element.LocalName == "Sound")
            {
                string value = element.GetAttribute("Value");
                string name = value.Substring(0, value.LastIndexOf('\\'));
                element.SetAttribute("Value", "AudioEvent:" + name);
            }
        }
        // Reborn: the second fixture uses Multisound and must remain untyped for this first-sound sibling test.
        copy.GetElementsByTagName("Sound")[1]!.Attributes!["Value"]!.Value = copy.GetElementsByTagName("Sound")[1]!.Attributes!["Value"]!.Value.Substring("AudioEvent:".Length);
        string source = Path.Combine(directory, "explicit.xml");
        File.WriteAllText(source, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + copy.DocumentElement!.OuterXml + "</AssetDeclaration>");
        AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
        InstanceDeclaration instance = document.SelfInstances.Single(); instance.Handle.TypeHash = 0x17B3B82Du;
        Settings.Current.ProcessedExternalManifests = new[] { first, ambiguous, second };
        CheckResolved(document, instance, new[] { "AudioEvent", "Multisound" });
        Settings.Current.ProcessedExternalManifests = new[] { ambiguous, second };
        ExpectFailure(document, instance, ErrorCode.UnknownReference);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: metadata fixtures establish identity selection only, not native audio processor/hash compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static string WriteTarget(string directory, string file, string type, string name)
    {
        string path = Path.Combine(directory, file + ".manifest");
        ExternalLinkSmokeTest.WriteFixture(path, new InstanceHandle(type, name), new ReferencedFileBuffer()); return path;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate concrete type/name order and unchanged native selectors after actual dependency preparation. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckResolved(AssetDeclarationDocument document, InstanceDeclaration instance, string[] types)
    {
        var before = FXListNativeSmokeTest.Compile(instance);
        DependencyResolutionSmokeTest.Prepare(document, instance);
        Require(instance.ValidatedReferencedInstances!.Select(handle => handle.TypeName).SequenceEqual(types)
            && instance.ValidatedReferencedInstances.Select(handle => handle.InstanceId).SequenceEqual(instance.ReferencedInstances.Select(handle => handle.InstanceId))
            && instance.AllDependentInstances!.Count == 0 && DependencyResolutionSmokeTest.Visited(document).Count == 1,
            "Derived external references changed type order/name hashes or queued stock target compilation.");
        var after = FXListNativeSmokeTest.Compile(instance);
        Require(before.InstanceBuffer.SequenceEqual(after.InstanceBuffer) && before.RelocationBuffer.SequenceEqual(after.RelocationBuffer)
            && before.ImportsBuffer.SequenceEqual(after.ImportsBuffer), "Concrete metadata resolution changed native selector bytes.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: failures must clear partial metadata and remain failures when the same document is retried. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectFailure(AssetDeclarationDocument document, InstanceDeclaration instance, ErrorCode code)
    {
        try { DependencyResolutionSmokeTest.Prepare(document, instance); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == code)
        {
            Require(instance.ValidatedReferencedInstances == null && instance.AllDependentInstances == null && DependencyResolutionSmokeTest.Visited(document).Count == 0,
                "Failed derived lookup retained partial dependency state."); return;
        }
        throw new InvalidDataException("Expected external audio resolution failure: " + code);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop the metadata proof at the first violated identity or no-write invariant. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
