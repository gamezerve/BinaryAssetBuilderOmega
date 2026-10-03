using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: checked Multisound compiler entries remain isolated from production output and public diagnostic admission.
internal static class Ep1MultisoundProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test descriptor/policies, native equality, real dependency preparation, hostile state and optional stock comparisons. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stockManifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-MultisoundProfile-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            XmlDocument config = new(); config.Load(Path.Combine(fixtures, "Ep1MultisoundProfile.xml"));
            XmlNamespaceManager namespaces = new(config.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
            Settings settings = new(); settings.ReadXml(new Node(config.DocumentElement!.CreateNavigator()!, namespaces));
            settings.SchemaPath = Path.Combine(fixtures, "MultisoundPipeline.xsd"); settings.DataRoot = directory;
            settings.DataPaths = new[] { directory }; settings.TargetPlatform = TargetPlatform.Win32; settings.CustomPostfix = "";
            settings.StreamPostfix = ""; settings.StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>();
            settings.ErrorLevel = 1; settings.ProcessedExternalManifests = Array.Empty<string>(); Settings.Current = settings;
            PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32); IAssetBuilderPlugin plugin = registry.DefaultPlugin;
            Require(plugin is Ra3Ep1MultisoundPlugin && !settings.BuildCache && !registry.CanReuseCompiledDocuments, "Multisound descriptor/policy differs.");
            ExtendedTypeInformation info = registry.GetExtendedTypeInformation(0xA3A7AF37u);
            Require(info.TypeHash == 0xF79C5A89u && info.ProcessingHash == (0xF79C5A89u ^ 0x45503141u)
                && info.Type == typeof(Marshaler.Ep1Multisound) && !info.Tokenized && !info.HasCustomData && !info.UseBuildCache
                && plugin.AllTypesHash == 0x5454A8E9u, "Multisound metadata domain differs.");
            ExpectPolicy(registry.ValidateProductionOutput);
            PluginRegistry mapped = new(new[] { new PluginDescriptor { IsEnabled = true, AssetTypes = "Multisound", UseBuildCache = true,
                QualifiedName = settings.Plugins.Single().QualifiedName } }, TargetPlatform.Win32);
            Require(!mapped.GetExtendedTypeInformation(0xA3A7AF37u).UseBuildCache && !mapped.CanReuseCompiledDocuments, "Explicit mapping enabled sound cache/reuse.");
            ExpectPolicy(mapped.ValidateProductionOutput);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(settings, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "source.xml"); File.Copy(Path.Combine(fixtures, "MultisoundProbe.xml"), source);
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!, options);
            InstanceDeclaration main = document.SelfInstances.Single(instance => instance.Handle.InstanceName == "GDI_Generic_VoiceDieMS");
            Reject(() => plugin.ProcessInstance(main));
            string[] targets = document.SelfInstances.SelectMany(instance => instance.ReferencedInstances).Select(handle => handle.InstanceName)
                .Distinct(StringComparer.Ordinal).Select((name, index) => WriteTarget(directory, "stock-" + index, new InstanceHandle("AudioEvent", name))).ToArray();
            settings.ProcessedExternalManifests = targets;
            Dictionary<string, AssetBuffer> baseline = new(StringComparer.Ordinal);
            foreach (InstanceDeclaration instance in document.SelfInstances)
            {
                Relo.Chunk expected = MultisoundNativeSmokeTest.Compile(instance); DependencyResolutionSmokeTest.Prepare(document, instance);
                string xml = instance.XmlNode.OuterXml; AssetBuffer actual = plugin.ProcessInstance(instance);
                Equal(actual, new AssetBuffer { InstanceData = expected.InstanceBuffer, RelocationData = expected.RelocationBuffer, ImportsData = expected.ImportsBuffer });
                Equal(actual, plugin.ProcessInstance(instance)); Require(instance.XmlNode.OuterXml == xml, "Sound profile changed caller XML.");
                baseline.Add(instance.Handle.Name, actual);
            }
            XmlElement root = (XmlElement)main.XmlNode, first = root.ChildNodes.OfType<XmlElement>().First();
            foreach (var change in new[] { (root, "id", "ChangedRoot"), (root, "TypeId", "0"), (root, "Control", "LOOP"),
                (root, "inheritFrom", "Other"), (root, "Unknown", "x"), (first, "TypeId", "0"), (first, "Weight", "-1"),
                (first, "Weight", "=Unknown"), (first, "Volume", "100"), (first, "PitchShiftLow", "0"),
                (first, "PlayPercent", "100"), (first, "VolumeShift", "0") })
                Mutate(plugin, main, change.Item1, change.Item2, change.Item3);
            string text = first.InnerText;
            foreach (string bad in new[] { "ChangedAudio\\0", "GDI_Commando_VoiceDie\\1", "GDI_Commando_VoiceDie\\4294967295",
                "GDI_Commando_VoiceDie\\0\\0", "Multisound:GDI_Commando_VoiceDie\\0", "=Unknown" })
            { first.InnerText = bad; try { Reject(() => plugin.ProcessInstance(main)); } finally { first.InnerText = text; } }
            // Reborn: current valid scalar edits are revalidated and compiled, not ignored in favor of earlier native bytes.
            string weight = first.GetAttribute("Weight"); first.SetAttribute("Weight", "7");
            AssetBuffer edited = plugin.ProcessInstance(main);
            Require(BitConverter.ToUInt32(edited.InstanceData, 20) == 7 && !edited.InstanceData.SequenceEqual(baseline[main.Handle.Name].InstanceData), "Current weight edit used stale output.");
            first.SetAttribute("Weight", weight); Equal(baseline[main.Handle.Name], plugin.ProcessInstance(main));
            XmlElement nested = root.OwnerDocument.CreateElement("Nested", root.NamespaceURI); first.AppendChild(nested);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { first.RemoveChild(nested); }
            XmlProcessingInstruction instruction = root.OwnerDocument.CreateProcessingInstruction("probe", "data"); root.AppendChild(instruction);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { root.RemoveChild(instruction); }
            XmlAttribute foreign = root.OwnerDocument.CreateAttribute("probe", "Weight", "urn:unsupported"); foreign.Value = "1000"; first.Attributes.Append(foreign);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { first.RemoveAttributeNode(foreign); }
            InstanceHandle resolved = main.ValidatedReferencedInstances![0];
            foreach (InstanceHandle wrong in new[] { new InstanceHandle("BaseAudioEventInfo", resolved.InstanceName), new InstanceHandle("FXList", resolved.InstanceName),
                new InstanceHandle("AudioEvent", "ChangedName"), main.ValidatedReferencedInstances[1], null! })
            {
                main.ValidatedReferencedInstances[0] = wrong;
                try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ValidatedReferencedInstances[0] = resolved; }
            }
            main.ValidatedReferencedInstances.Add(resolved);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ValidatedReferencedInstances.RemoveAt(main.ValidatedReferencedInstances.Count - 1); }
            // Reborn: corrupting the pre-resolution table cannot authorize a current XML reference with another original identity.
            InstanceHandle original = main.ReferencedInstances[0]; main.ReferencedInstances[0] = new InstanceHandle("AudioEvent", "ChangedOriginal");
            try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ReferencedInstances[0] = original; }
            main.ReferencedFiles.Add("Unexpected"); try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ReferencedFiles.Clear(); }
            main.WeakReferencedInstances.Add(resolved); try { Reject(() => plugin.ProcessInstance(main)); } finally { main.WeakReferencedInstances.Clear(); }
            main.HasCustomData = true; try { Reject(() => plugin.ProcessInstance(main)); } finally { main.HasCustomData = false; }
            uint hash = main.Handle.TypeHash; main.Handle.TypeHash = 0;
            try { Reject(() => plugin.ProcessInstance(main)); } finally { main.Handle.TypeHash = hash; }
            settings.ProcessedExternalManifests = Array.Empty<string>();
            Missing(document, main); Missing(document, main); Reject(() => plugin.ProcessInstance(main));
            settings.ProcessedExternalManifests = targets; DependencyResolutionSmokeTest.Prepare(document, main);
            Equal(baseline[main.Handle.Name], plugin.ProcessInstance(main));
            first.SetAttribute("TypeId", "0");
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            foreach (InstanceDeclaration instance in repeated.SelfInstances)
            { DependencyResolutionSmokeTest.Prepare(repeated, instance); Equal(baseline[instance.Handle.Name], plugin.ProcessInstance(instance)); }
            Require(options.UsePrecompiled, "Sound processing changed caller options.");
            CheckLiteralInputs(processor, plugin, directory);
            Ra3Ep1MultisoundPlugin isolated = new(); Reject(() => isolated.GetExtendedTypeInformation(0xA3A7AF37u));
            isolated.Initialize(TargetPlatform.Win32); isolated.GetExtendedTypeInformation(0xA3A7AF37u).TypeHash = 0;
            Require(isolated.GetExtendedTypeInformation(0xA3A7AF37u).TypeHash == 0xF79C5A89u, "Sound metadata was caller-mutable.");
            Reject(() => isolated.GetExtendedTypeInformation(0x844D7B9Fu)); Reject(() => isolated.ReInitialize(TargetPlatform.Xbox360));
            Reject(() => isolated.ProcessInstance(repeated.SelfInstances.First())); isolated.ReInitialize(TargetPlatform.Win32);
            Equal(baseline[repeated.SelfInstances.First().Handle.Name], isolated.ProcessInstance(repeated.SelfInstances.First()));
            if (stockManifests.Length > 0) CheckStock(plugin, repeated, stockManifests);
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "Sound compiler profile wrote production streams.");
            Console.WriteLine("EP1 Multisound profile self-test: OK (descriptor/metadata/policies, stock native entries, current identity/weight/selector/table checks, missing/ambiguous/duplicate rejection, typed/nested targets, reload, platform/cache/output closure)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove typed and inherited resolution, missing/ambiguous siblings, duplicate leaves, empty roots and source refresh. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckLiteralInputs(DocumentProcessor processor, IAssetBuilderPlugin plugin, string directory)
    {
        string source = Path.Combine(directory, "literal.xml");
        foreach (string name in new[] { "RebornSoundOne", "RebornSoundTwo" })
        {
            File.WriteAllText(source, Xml("<Multisound id=\"Literal\"><Subsound>AudioEvent:" + name + "</Subsound></Multisound>"));
            Settings.Current.ProcessedExternalManifests = new[] { WriteTarget(directory, name, new InstanceHandle("AudioEvent", name)) };
            AssetDeclarationDocument doc = Read(processor, source); InstanceDeclaration instance = doc.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(doc, instance);
            AssetBuffer actual = plugin.ProcessInstance(instance);
            Require(instance.ValidatedReferencedInstances!.Single().InstanceName == name && actual.InstanceData.Length == 44
                && BitConverter.ToUInt32(actual.InstanceData, 16) == 1, "Literal refresh used stale sound identity/output.");
        }
        string eventPath = WriteTarget(directory, "sibling-event", new InstanceHandle("AudioEvent", "Sibling"));
        string multiPath = WriteTarget(directory, "sibling-multi", new InstanceHandle("Multisound", "Sibling"));
        Settings.Current.ProcessedExternalManifests = new[] { eventPath, multiPath };
        File.WriteAllText(source, Xml("<Multisound id=\"Ambiguous\"><Subsound>Sibling</Subsound></Multisound>"));
        AssetDeclarationDocument ambiguous = Read(processor, source);
        try { DependencyResolutionSmokeTest.Prepare(ambiguous, ambiguous.SelfInstances.Single()); throw new InvalidDataException("Ambiguous sound was accepted."); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.ReferencingError) { }
        Reject(() => plugin.ProcessInstance(ambiguous.SelfInstances.Single()));
        foreach (string type in new[] { "AudioEvent", "Multisound" })
        {
            File.WriteAllText(source, Xml("<Multisound id=\"Typed\"><Subsound>" + type + ":Sibling</Subsound></Multisound>"));
            AssetDeclarationDocument doc = Read(processor, source); InstanceDeclaration instance = doc.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(doc, instance);
            plugin.ProcessInstance(instance); Require(instance.ValidatedReferencedInstances!.Single().TypeName == type, "Typed sound widened to a sibling.");
        }
        Settings.Current.ProcessedExternalManifests = new[] { eventPath };
        File.WriteAllText(source, Xml("<Multisound id=\"WrongTyped\"><Subsound>Multisound:Sibling</Subsound></Multisound>"));
        AssetDeclarationDocument wrong = Read(processor, source); Missing(wrong, wrong.SelfInstances.Single());
        File.WriteAllText(source, Xml("<Multisound id=\"Duplicate\"><Subsound>AudioEvent:Sibling</Subsound><Subsound>Sibling</Subsound></Multisound>"));
        AssetDeclarationDocument duplicate = Read(processor, source); InstanceDeclaration dup = duplicate.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(duplicate, dup);
        Reject(() => plugin.ProcessInstance(dup));
        File.WriteAllText(source, Xml("<Multisound id=\"Empty\" />"));
        AssetDeclarationDocument empty = Read(processor, source); InstanceDeclaration zero = empty.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(empty, zero);
        AssetBuffer blank = plugin.ProcessInstance(zero); Require(blank.InstanceData.Length == 16 && blank.RelocationData.Length == 0 && blank.ImportsData.Length == 0, "Empty sound root differs.");
        // Reborn: distinct prepared targets prove both the exact 32-child boundary and rejection of the 33rd, independently from duplicate rejection.
        string[] limitTargets = Enumerable.Range(0, 33).Select(index => WriteTarget(directory, "limit-" + index,
            new InstanceHandle("AudioEvent", "LimitSound" + index))).ToArray();
        Settings.Current.ProcessedExternalManifests = limitTargets;
        foreach (int count in new[] { 32, 33 })
        {
            File.WriteAllText(source, Xml("<Multisound id=\"Limit\">" + string.Concat(Enumerable.Range(0, count)
                .Select(index => "<Subsound>LimitSound" + index + "</Subsound>")) + "</Multisound>"));
            AssetDeclarationDocument doc = Read(processor, source); InstanceDeclaration instance = doc.SelfInstances.Single();
            DependencyResolutionSmokeTest.Prepare(doc, instance);
            if (count == 33) Reject(() => plugin.ProcessInstance(instance));
            else Require(plugin.ProcessInstance(instance).InstanceData.Length == 912, "Exactly 32 sound children were not admitted.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare checked compiler buffers and concretely prepared tuples with the three selected stock Multisounds. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckStock(IAssetBuilderPlugin plugin, AssetDeclarationDocument document, string[] manifests)
    {
        Settings.Current.ProcessedExternalManifests = manifests.Select(Path.GetFullPath).ToArray();
        Dictionary<string, (Relo.Chunk Data, uint[] Names)> chunks = new(StringComparer.Ordinal);
        Dictionary<string, (uint Type, uint Name)[]> identities = new(StringComparer.Ordinal);
        foreach (InstanceDeclaration instance in document.SelfInstances)
        {
            DependencyResolutionSmokeTest.Prepare(document, instance); AssetBuffer buffer = plugin.ProcessInstance(instance);
            chunks.Add(instance.Handle.Name, (new Relo.Chunk { InstanceBuffer = buffer.InstanceData, RelocationBuffer = buffer.RelocationData, ImportsBuffer = buffer.ImportsData },
                instance.ValidatedReferencedInstances!.Select(handle => handle.InstanceId).ToArray()));
            identities.Add(instance.Handle.Name, instance.ValidatedReferencedInstances.Select(handle => (handle.TypeId, handle.InstanceId)).ToArray());
        }
        int count = 0;
        foreach (string path in manifests)
        {
            ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(path));
            if (!metadata.Assets.Any(asset => chunks.ContainsKey(asset.Name))) continue;
            foreach (ManifestAsset asset in metadata.Assets.Where(asset => chunks.ContainsKey(asset.Name)))
                Require(asset.References.Select(reference => (reference.TypeId, reference.InstanceId)).SequenceEqual(identities[asset.Name]), "Prepared sound tuples differ from stock.");
            MultisoundNativeSmokeTest.Compare(chunks, path); count++;
        }
        Require(count > 0, "Stock profile comparison requires a manifest with all three selected Multisounds.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create tiny metadata fixtures only; no synthetic audio payload compatibility is implied. */
    //-------------------------------------------------------------------------------------------------
    private static string WriteTarget(string directory, string file, InstanceHandle handle)
    {
        string path = Path.Combine(directory, file + ".manifest"); ExternalLinkSmokeTest.WriteFixture(path, handle, new ReferencedFileBuffer(),
            typeHash: handle.TypeName == "AudioEvent" ? 0x560C2E45u : 0xF79C5A89u, tokenized: false); return path;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: force fresh schema-bound documents without mutating caller precompiled options. */
    //-------------------------------------------------------------------------------------------------
    private static AssetDeclarationDocument Read(DocumentProcessor processor, string path) => processor.ProcessDocumentInternal(path, path, null!,
        new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the official asset namespace in owned test source strings. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">" + body + "</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require deliberate missing-reference rejection during repeated dependency preparation. */
    //-------------------------------------------------------------------------------------------------
    private static void Missing(AssetDeclarationDocument document, InstanceDeclaration instance)
    {
        try { DependencyResolutionSmokeTest.Prepare(document, instance); }
        catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference) { return; }
        throw new InvalidDataException("Missing sound dependency was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore caller XML attributes after each independent rejection case. */
    //-------------------------------------------------------------------------------------------------
    private static void Mutate(IAssetBuilderPlugin plugin, InstanceDeclaration instance, XmlElement element, string name, string value)
    {
        bool present = element.HasAttribute(name); string saved = element.GetAttribute(name); element.SetAttribute(name, value);
        try { Reject(() => plugin.ProcessInstance(instance)); } finally { if (present) element.SetAttribute(name, saved); else element.RemoveAttribute(name); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every native buffer byte for determinism and recovery. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(AssetBuffer first, AssetBuffer second) => Require(first.InstanceData.SequenceEqual(second.InstanceData)
        && first.RelocationData.SequenceEqual(second.RelocationData) && first.ImportsData.SequenceEqual(second.ImportsData), "Sound native buffers differ.");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept only intentional eligibility/schema/readiness failures, not incidental unsafe marshal exceptions. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception error) when (error is NotSupportedException or InvalidOperationException or XmlSchemaException) { return; }
        throw new InvalidDataException("Experimental sound accepted unsupported state.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove policy rejection happens before missing input/cache access. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); } catch (BinaryAssetBuilderException error) when (error.Message.Contains("RA3EP1-Multisound-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("Sound production policy was bypassed.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first compiler-profile contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidDataException(message); }
}
