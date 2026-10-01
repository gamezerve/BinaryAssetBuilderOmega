using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove isolated FX compiler entries and hostile current-state rejection with production/cache policies closed.
internal static class Ep1FXListProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: route official FX fixtures through descriptor, document, real dependency preparation and checked native compiler stages. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stockManifests)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-FXProfile-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            XmlDocument config = new(); config.Load(Path.Combine(fixtures, "Ep1FXListProfile.xml"));
            XmlNamespaceManager namespaces = new(config.NameTable); namespaces.AddNamespace("ea", "uri:ea.com:eala:asset");
            Settings settings = new(); settings.ReadXml(new Node(config.DocumentElement!.CreateNavigator(), namespaces));
            settings.SchemaPath = Path.Combine(fixtures, "FXListPipeline.xsd"); settings.DataRoot = directory;
            settings.DataPaths = new[] { directory }; settings.TargetPlatform = TargetPlatform.Win32;
            settings.CustomPostfix = ""; settings.StreamPostfix = ""; settings.StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>();
            settings.ErrorLevel = 1; settings.ProcessedExternalManifests = Array.Empty<string>(); Settings.Current = settings;
            PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32);
            Require(registry.DefaultPlugin is Ra3Ep1FXListPlugin && !settings.BuildCache && !registry.CanReuseCompiledDocuments,
                "FX descriptor/reuse policy differs.");
            ExtendedTypeInformation info = registry.GetExtendedTypeInformation(0x86682E78u);
            Require(info.TypeHash == 0x17B3B82Du && info.ProcessingHash == (0x17B3B82Du ^ 0x45503141u)
                && !info.Tokenized && !info.HasCustomData && !info.UseBuildCache && registry.DefaultPlugin.AllTypesHash == 0x5454A8E9u,
                "FX metadata or processing domain differs.");
            ExpectPolicy(registry.ValidateProductionOutput);
            PluginDescriptor mapped = new() { IsEnabled = true, AssetTypes = "FXList", UseBuildCache = true,
                QualifiedName = settings.Plugins.Single().QualifiedName };
            PluginRegistry explicitRegistry = new(new[] { mapped }, TargetPlatform.Win32);
            Require(!explicitRegistry.GetExtendedTypeInformation(0x86682E78u).UseBuildCache && !explicitRegistry.CanReuseCompiledDocuments,
                "Explicit FX registration bypassed cache restrictions.");
            ExpectPolicy(explicitRegistry.ValidateProductionOutput);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(settings, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            string source = Path.Combine(directory, "source.xml"); File.Copy(Path.Combine(fixtures, "FXListProbe.xml"), source);
            var options = new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!, options);
            Require(document.SelfInstances.Count == 3, "FX source root count differs.");
            InstanceDeclaration masks = document.SelfInstances.Single(value => value.Handle.InstanceName == "FX_ALL_AntiGroundAircraft_VoiceDie");
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks));
            string[] targets = document.SelfInstances.SelectMany(value => value.ReferencedInstances).Select((handle, ordinal) =>
            {
                string path = Path.Combine(directory, "audio-" + ordinal + ".manifest");
                string type = handle.InstanceName == "GDI_Generic_VoiceDieMS" ? "Multisound" : "AudioEvent";
                ExternalLinkSmokeTest.WriteFixture(path, new InstanceHandle(type, handle.InstanceName), new ReferencedFileBuffer()); return path;
            }).ToArray();
            settings.ProcessedExternalManifests = targets;
            Dictionary<string, AssetBuffer> baseline = new(StringComparer.Ordinal);
            foreach (InstanceDeclaration instance in document.SelfInstances)
            {
                var expected = FXListNativeSmokeTest.Compile(instance);
                DependencyResolutionSmokeTest.Prepare(document, instance);
                string normalized = instance.XmlNode.OuterXml;
                AssetBuffer actual = registry.DefaultPlugin.ProcessInstance(instance);
                Require(actual.InstanceData.SequenceEqual(expected.InstanceBuffer) && actual.RelocationData.SequenceEqual(expected.RelocationBuffer)
                    && actual.ImportsData.SequenceEqual(expected.ImportsBuffer) && instance.XmlNode.OuterXml == normalized,
                    "FX compiler differs from native goldens or changed caller XML.");
                Equal(actual, registry.DefaultPlugin.ProcessInstance(instance)); baseline.Add(instance.Handle.Name, actual);
            }
            XmlElement root = (XmlElement)masks.XmlNode;
            XmlElement list = root.ChildNodes.OfType<XmlElement>().Single();
            XmlElement first = list.ChildNodes.OfType<XmlElement>().First();
            XmlElement second = list.ChildNodes.OfType<XmlElement>().Last();
            foreach (var change in new[] { (root, "id", "ChangedRoot"), (root, "TypeId", "0"), (list, "TypeId", "0"), (first, "TypeId", "0"),
                (root, "inheritFrom", "BaseFX"), (root, "CullTracking", "1s"), (root, "PlayEvenIfShrouded", "true"), (root, "Tailorable", "true"),
                (first, "Weather", "SUNNY"), (first, "OnlyIfOnLand", "true"), (first, "PlayIfSourceIsStealthed", "true"), (first, "StopIfPlayed", "true"),
                (first, "RequiredSourceModelConditions", "USER_1"), (first, "RequiredSecondaryModelConditions", "FLYING"),
                (first, "ExcludedSourceModelConditions", "FLYING"), (first, "Value", "OtherAudio\\0"),
                (first, "Value", "TEMP_RA2_AlliedAir_VoiceCrash\\1"), (first, "Value", "TEMP_RA2_AlliedAir_VoiceCrash\\4294967295"),
                (first, "Value", "TEMP_RA2_AlliedAir_VoiceCrash\\0\\0"), (first, "Value", "=Unknown"), (first, "Unknown", "x") })
                CheckMutation(registry.DefaultPlugin, masks, change.Item1, change.Item2, change.Item3);
            string soundValue = second.GetAttribute("Value"); second.SetAttribute("Value", first.GetAttribute("Value"));
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { second.SetAttribute("Value", soundValue); }
            XmlElement extra = (XmlElement)first.CloneNode(true); list.AppendChild(extra);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { list.RemoveChild(extra); }
            XmlElement filter = root.OwnerDocument.CreateElement("SourceObjectFilter", root.NamespaceURI); first.AppendChild(filter);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { first.RemoveChild(filter); }
            XmlAttribute foreign = root.OwnerDocument.CreateAttribute("probe", "Value", "urn:unsupported"); foreign.Value = "x"; first.Attributes.Append(foreign);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { first.RemoveAttributeNode(foreign); }
            XmlProcessingInstruction instruction = root.OwnerDocument.CreateProcessingInstruction("probe", "data"); list.AppendChild(instruction);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { list.RemoveChild(instruction); }
            // Reborn: corrupt only the validated concrete table, then restore it for the next independent negative case.
            InstanceHandle resolved = masks.ValidatedReferencedInstances![0];
            foreach (InstanceHandle wrong in new[] { new InstanceHandle("BaseAudioEventInfo", resolved.InstanceName),
                new InstanceHandle("FXList", resolved.InstanceName), new InstanceHandle("AudioEvent", "ChangedName"), masks.ValidatedReferencedInstances[1], null! })
            {
                masks.ValidatedReferencedInstances[0] = wrong;
                try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { masks.ValidatedReferencedInstances[0] = resolved; }
            }
            masks.ValidatedReferencedInstances.Add(resolved);
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); }
            finally { masks.ValidatedReferencedInstances.RemoveAt(masks.ValidatedReferencedInstances.Count - 1); }
            masks.ReferencedFiles.Add("UnexpectedFile");
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { masks.ReferencedFiles.Clear(); }
            masks.WeakReferencedInstances.Add(new InstanceHandle("AudioEvent", "UnexpectedWeak"));
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { masks.WeakReferencedInstances.Clear(); }
            masks.HasCustomData = true;
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { masks.HasCustomData = false; }
            uint hash = masks.Handle.TypeHash; masks.Handle.TypeHash = 0;
            try { ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks)); } finally { masks.Handle.TypeHash = hash; }
            settings.ProcessedExternalManifests = Array.Empty<string>();
            try { DependencyResolutionSmokeTest.Prepare(document, masks); throw new InvalidDataException("Missing audio target was accepted."); }
            catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference) { }
            ExpectRejected(() => registry.DefaultPlugin.ProcessInstance(masks));
            settings.ProcessedExternalManifests = targets; DependencyResolutionSmokeTest.Prepare(document, masks);
            Equal(baseline[masks.Handle.Name], registry.DefaultPlugin.ProcessInstance(masks));
            // Reborn: forced document reload must not reuse poisoned schema/selector state despite caller precompiled options.
            first.SetAttribute("TypeId", "0");
            AssetDeclarationDocument repeated = processor.ProcessDocumentInternal(source, source, null!, options);
            foreach (InstanceDeclaration instance in repeated.SelfInstances)
            { DependencyResolutionSmokeTest.Prepare(repeated, instance); Equal(baseline[instance.Handle.Name], registry.DefaultPlugin.ProcessInstance(instance)); }
            Require(options.UsePrecompiled, "FX processing mutated caller options.");
            CheckLiteralInputs(processor, registry.DefaultPlugin, directory);
            settings.ProcessedExternalManifests = targets;
            processor.Cache = null!;
            ExpectPolicy(() => processor.ProcessDocumentInternal("missing", "missing", null!, new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Ra3Ep1FXListPlugin isolated = new(); ExpectRejected(() => isolated.GetExtendedTypeInformation(0x86682E78u));
            isolated.Initialize(TargetPlatform.Win32); isolated.GetExtendedTypeInformation(0x86682E78u).TypeHash = 0;
            Require(isolated.GetExtendedTypeInformation(0x86682E78u).TypeHash == 0x17B3B82Du, "FX metadata was caller-mutable.");
            ExpectRejected(() => isolated.GetExtendedTypeInformation(0xBCC23F6Cu));
            ExpectRejected(() => isolated.ReInitialize(TargetPlatform.Xbox360)); ExpectRejected(() => isolated.ProcessInstance(repeated.SelfInstances.First()));
            isolated.ReInitialize(TargetPlatform.Win32); Equal(baseline[repeated.SelfInstances.First().Handle.Name], isolated.ProcessInstance(repeated.SelfInstances.First()));
            if (stockManifests.Length > 0) CheckStock(registry.DefaultPlugin, repeated, stockManifests);
            Require(!Directory.EnumerateFiles(directory, "*.bin").Any(), "FX compiler profile emitted game streams.");
            Console.WriteLine("EP1 FX profile self-test: OK (descriptor/document/three compiler entries, concrete dependency order, native equality, current-value/identity controls, missing recovery, fresh reload, platform/production/cache gates)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept new literal and explicitly typed sounds, refresh changed sources, and reject a schema-valid unproven nugget family. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckLiteralInputs(DocumentProcessor processor, IAssetBuilderPlugin plugin, string directory)
    {
        string source = Path.Combine(directory, "literal.xml");
        foreach (string name in new[] { "RebornDiagnosticSoundOne", "RebornDiagnosticSoundTwo" })
        {
            File.WriteAllText(source, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><FXList id=\"RebornLiteralFX\"><NuggetList><Sound Value=\"AudioEvent:"
                + name + "\" /></NuggetList></FXList></AssetDeclaration>");
            string manifest = Path.Combine(directory, name + ".manifest");
            ExternalLinkSmokeTest.WriteFixture(manifest, new InstanceHandle("AudioEvent", name), new ReferencedFileBuffer());
            Settings.Current.ProcessedExternalManifests = new[] { manifest };
            AssetDeclarationDocument document = processor.ProcessDocumentInternal(source, source, null!,
                new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true });
            InstanceDeclaration instance = document.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(document, instance);
            AssetBuffer buffer = plugin.ProcessInstance(instance);
            Require(instance.ReferencedInstances.Single().TypeName == "AudioEvent" && instance.ValidatedReferencedInstances!.Single().InstanceName == name
                && buffer.InstanceData.Length == 80 && buffer.RelocationData.Length == 12 && buffer.ImportsData.Length == 8
                && BitConverter.ToUInt32(buffer.InstanceData, 76) == 1, "New typed sound did not compile or changed source used stale audio identity.");
        }
        source = Path.Combine(directory, "unsupported.xml");
        File.WriteAllText(source, "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><FXList id=\"RebornUnsupportedFX\"><NuggetList><EvaEvent /></NuggetList></FXList></AssetDeclaration>");
        AssetDeclarationDocument unsupported = processor.ProcessDocumentInternal(source, source, null!,
            new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = false });
        InstanceDeclaration other = unsupported.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(unsupported, other);
        ExpectRejected(() => plugin.ProcessInstance(other));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare actual checked compiler buffers and concretely resolved references with selected stock FX slices. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckStock(IAssetBuilderPlugin plugin, AssetDeclarationDocument document, string[] manifests)
    {
        Settings.Current.ProcessedExternalManifests = manifests.Select(Path.GetFullPath).ToArray();
        Dictionary<string, (Relo.Chunk Data, uint[] Names)> chunks = new(StringComparer.Ordinal);
        Dictionary<string, (uint Type, uint Name)[]> identities = new(StringComparer.Ordinal);
        foreach (InstanceDeclaration instance in document.SelfInstances)
        {
            DependencyResolutionSmokeTest.Prepare(document, instance);
            AssetBuffer buffer = plugin.ProcessInstance(instance);
            chunks.Add(instance.Handle.Name, (new Relo.Chunk { InstanceBuffer = buffer.InstanceData,
                RelocationBuffer = buffer.RelocationData, ImportsBuffer = buffer.ImportsData },
                instance.ValidatedReferencedInstances!.Select(handle => handle.InstanceId).ToArray()));
            identities.Add(instance.Handle.Name, instance.ValidatedReferencedInstances.Select(handle => (handle.TypeId, handle.InstanceId)).ToArray());
        }
        int count = 0;
        foreach (string path in manifests)
        {
            ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
            if (!manifest.Assets.Any(asset => chunks.ContainsKey(asset.Name))) continue;
            foreach (ManifestAsset asset in manifest.Assets.Where(asset => chunks.ContainsKey(asset.Name)))
                Require(asset.References.Select(reference => (reference.TypeId, reference.InstanceId)).SequenceEqual(identities[asset.Name]),
                    "Checked FX compiler concrete dependency table differs from stock.");
            FXListNativeSmokeTest.Compare(chunks, path); count++;
        }
        Require(count > 0, "Stock FX comparison requires the static manifest containing all three selected roots.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore caller-owned XML after every negative current-state mutation. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckMutation(IAssetBuilderPlugin plugin, InstanceDeclaration instance, XmlElement element, string name, string value)
    {
        bool existed = element.HasAttribute(name); string saved = element.GetAttribute(name); element.SetAttribute(name, value);
        try { ExpectRejected(() => plugin.ProcessInstance(instance)); }
        finally { if (existed) element.SetAttribute(name, saved); else element.RemoveAttribute(name); }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare exact native bytes, relocations and import offsets at every successful compiler entry. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(AssetBuffer expected, AssetBuffer actual)
    {
        Require(expected.InstanceData.SequenceEqual(actual.InstanceData) && expected.RelocationData.SequenceEqual(actual.RelocationData)
            && expected.ImportsData.SequenceEqual(actual.ImportsData), "FX output bytes changed.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require intentional rejection, not incidental unsafe marshalling failures. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRejected(Action action)
    {
        try { action(); } catch (Exception error) when (error is NotSupportedException or InvalidOperationException or XmlSchemaException) { return; }
        throw new InvalidDataException("Experimental FX accepted unsupported state.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify production guards reject before accessing missing input or cache/output state. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectPolicy(Action action)
    {
        try { action(); }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("RA3EP1-FXList-Experimental-v1", StringComparison.Ordinal)) { return; }
        throw new InvalidDataException("FX production output policy was bypassed.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop the isolated FX compiler proof at the first byte, policy or identity mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
