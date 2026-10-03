using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Core.Session;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: checked AudioEvent entries stay isolated from production output, codecs and public diagnostic-build admission.
internal static class Ep1AudioEventProfileSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove policies, prepared AudioFile tables, five native entries, current-value rejection/recovery and optional actual game slices. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stocks)
    {
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        string directory = Path.Combine(Path.GetTempPath(), "Reborn-AudioEventProfile-" + Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        try
        {
            XmlDocument config = new(); config.Load(Path.Combine(fixtures, "Ep1AudioEventProfile.xml"));
            XmlNamespaceManager ns = new(config.NameTable); ns.AddNamespace("ea", "uri:ea.com:eala:asset");
            Settings settings = new(); settings.ReadXml(new Node(config.DocumentElement!.CreateNavigator()!, ns));
            settings.SchemaPath = Path.Combine(fixtures, "AudioEventPipeline.xsd"); settings.DataRoot = directory; settings.DataPaths = new[] { directory };
            settings.TargetPlatform = TargetPlatform.Win32; settings.CustomPostfix = ""; settings.StreamPostfix = "";
            settings.StringHashBinDescriptors = Array.Empty<StringHashBinDescriptor>(); settings.ErrorLevel = 1;
            settings.ProcessedExternalManifests = Array.Empty<string>(); Settings.Current = settings;
            PluginRegistry registry = new(settings.Plugins, TargetPlatform.Win32); IAssetBuilderPlugin plugin = registry.DefaultPlugin;
            Require(plugin is Ra3Ep1AudioEventPlugin && !settings.BuildCache && !registry.CanReuseCompiledDocuments, "AudioEvent descriptor/policy differs.");
            var info = registry.GetExtendedTypeInformation(0x844D7B9Fu);
            Require(info.TypeHash == 0x560C2E45u && info.ProcessingHash == (0x560C2E45u ^ 0x45503141u) && !info.Tokenized
                && !info.HasCustomData && !info.UseBuildCache && info.Type == typeof(Marshaler.Ep1AudioEvent)
                && plugin.AllTypesHash == 0x5454A8E9u, "AudioEvent metadata differs.");
            Policy(registry.ValidateProductionOutput);
            PluginRegistry mapped = new(new[] { new PluginDescriptor { IsEnabled = true, AssetTypes = "AudioEvent", UseBuildCache = true,
                QualifiedName = settings.Plugins.Single().QualifiedName } }, TargetPlatform.Win32);
            Require(!mapped.GetExtendedTypeInformation(0x844D7B9Fu).UseBuildCache && !mapped.CanReuseCompiledDocuments, "Explicit audio mapping enabled cache/reuse.");
            Policy(mapped.ValidateProductionOutput);
            SessionCache cache = new(); cache.InitializeCache(new List<string>());
            DocumentProcessor processor = new(settings, registry, new VerifierPluginRegistry(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32))
                { Cache = cache, SchemaSet = new SchemaSet(false) };
            List<AssetDeclarationDocument> documents = new();
            foreach (string file in new[] { "AudioEventProbe.xml", "AudioEventExtendedProbe.xml" })
            { string source = Path.Combine(directory, file); File.Copy(Path.Combine(fixtures, file), source); documents.Add(Read(processor, source)); }
            var instances = documents.SelectMany(doc => doc.SelfInstances).ToArray(); Require(instances.Length == 5, "Five checked roots were not loaded.");
            InstanceDeclaration main = instances[0]; Reject(() => plugin.ProcessInstance(main));
            string[] targets = instances.SelectMany(instance => instance.ReferencedInstances).Select(handle => handle.InstanceName)
                .Distinct(StringComparer.Ordinal).Select((name, index) => Target(directory, "stock-" + index, name)).ToArray();
            settings.ProcessedExternalManifests = targets;
            Dictionary<string, AssetBuffer> baseline = new(StringComparer.Ordinal);
            foreach (AssetDeclarationDocument document in documents)
                foreach (InstanceDeclaration instance in document.SelfInstances)
                {
                    Relo.Chunk native = AudioEventNativeSmokeTest.Compile(instance); DependencyResolutionSmokeTest.Prepare(document, instance);
                    string xml = instance.XmlNode.OuterXml; AssetBuffer actual = plugin.ProcessInstance(instance);
                    Equal(actual, new AssetBuffer { InstanceData = native.InstanceBuffer, RelocationData = native.RelocationBuffer, ImportsData = native.ImportsBuffer });
                    Equal(actual, plugin.ProcessInstance(instance)); Require(xml == instance.XmlNode.OuterXml, "Profile mutated caller XML.");
                    baseline.Add(instance.Handle.Name, actual);
                }
            XmlElement root = (XmlElement)main.XmlNode, sound = root.ChildNodes.OfType<XmlElement>().First(child => child.LocalName == "Sound");
            foreach (var change in new[] { (root,"id","Changed"), (root,"id",new string('x',129)), (root,"TypeId","0"), (root,"inheritFrom","Other"), (root,"Volume","=Unknown"),
                (root,"Volume","NaN"), (root,"Volume","1e39"), (root,"Control","SMART_LIMITING"), (root,"Control","UNKNOWN"),
                (root,"Priority","UNKNOWN"), (root,"Type","UNKNOWN"), (root,"SubmixSlider","VOICE"),
                (sound,"Weight","-1"), (sound,"Weight","=Unknown"), (sound,"Volume","25"), (sound,"TypeId","0"), (sound,"Unknown","x") })
                Mutate(plugin, main, change.Item1, change.Item2, change.Item3);
            string originalText = sound.InnerText;
            foreach (string bad in new[] { "=Unknown", "Changed\\0", "WImpact_DebrisVsGrounda\\1", "AudioEvent:WImpact_DebrisVsGrounda\\0",
                "WImpact_DebrisVsGrounda\\4294967295", "WImpact_DebrisVsGrounda\\0\\0" })
            { sound.InnerText = bad; try { Reject(() => plugin.ProcessInstance(main)); } finally { sound.InnerText = originalText; } }
            string weight = sound.GetAttribute("Weight"); sound.SetAttribute("Weight","7");
            Require(BitConverter.ToUInt32(plugin.ProcessInstance(main).InstanceData,156) == 7, "Current weight edit used stale bytes.");
            sound.SetAttribute("Weight",weight); Equal(baseline[main.Handle.Name], plugin.ProcessInstance(main));
            // Reborn: current valid root and range edits must produce fresh native bytes, while stale PSVI cannot authorize malformed values.
            string volume = root.GetAttribute("Volume"); root.SetAttribute("Volume","55");
            Require(BitConverter.ToSingle(plugin.ProcessInstance(main).InstanceData,4) == 55 * 0.01f,"Current root scalar edit used stale bytes.");
            root.SetAttribute("Volume",volume);
            XmlElement pitch = root.ChildNodes.OfType<XmlElement>().Single(child => child.LocalName == "PitchShift");
            foreach (var change in new[] { ("High","oops"), ("High","=Unknown"), ("High","NaN"), ("TypeId","0"), ("Unknown","x") })
                Mutate(plugin,main,pitch,change.Item1,change.Item2);
            XmlElement time = root.ChildNodes.OfType<XmlElement>().Single(child => child.LocalName == "NonInterruptibleTime");
            Mutate(plugin,main,time,"High","1e39s");
            string high = pitch.GetAttribute("High"); pitch.SetAttribute("High","10");
            AssetBuffer rangeEdit = plugin.ProcessInstance(main); int target = (int)BitConverter.ToUInt32(rangeEdit.InstanceData,84);
            Require(BitConverter.ToSingle(rangeEdit.InstanceData,target + 4) == 10,"Current range edit used stale bytes.");
            pitch.SetAttribute("High",high); Equal(baseline[main.Handle.Name],plugin.ProcessInstance(main));
            XmlAttribute foreign = root.OwnerDocument.CreateAttribute("probe","Weight","urn:unsupported"); foreign.Value = "1000"; sound.Attributes.Append(foreign);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { sound.RemoveAttributeNode(foreign); }
            root.AppendChild(pitch);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { root.PrependChild(pitch); }
            XmlElement nested = root.OwnerDocument.CreateElement("Nested",root.NamespaceURI); sound.AppendChild(nested);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { sound.RemoveChild(nested); }
            XmlProcessingInstruction instruction = root.OwnerDocument.CreateProcessingInstruction("probe","data"); root.AppendChild(instruction);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { root.RemoveChild(instruction); }
            XmlComment oversized = root.OwnerDocument.CreateComment(new string('x',65536)); root.AppendChild(oversized);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { root.RemoveChild(oversized); }
            InstanceHandle concrete = main.ValidatedReferencedInstances![0], original = main.ReferencedInstances[0];
            foreach (InstanceHandle bad in new[] { new InstanceHandle("AudioEvent",concrete.InstanceName), new InstanceHandle("AudioFile","Changed"),
                main.ValidatedReferencedInstances[1], null! })
            { main.ValidatedReferencedInstances[0] = bad; try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ValidatedReferencedInstances[0] = concrete; } }
            main.ReferencedInstances[0] = new InstanceHandle("AudioFile","Changed");
            try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ReferencedInstances[0] = original; }
            main.ValidatedReferencedInstances.Add(concrete);
            try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ValidatedReferencedInstances.RemoveAt(main.ValidatedReferencedInstances.Count - 1); }
            main.HasCustomData = true; try { Reject(() => plugin.ProcessInstance(main)); } finally { main.HasCustomData = false; }
            main.ReferencedFiles.Add("Unexpected"); try { Reject(() => plugin.ProcessInstance(main)); } finally { main.ReferencedFiles.Clear(); }
            main.WeakReferencedInstances.Add(concrete); try { Reject(() => plugin.ProcessInstance(main)); } finally { main.WeakReferencedInstances.Clear(); }
            uint hash = main.Handle.TypeHash; main.Handle.TypeHash = 0; try { Reject(() => plugin.ProcessInstance(main)); } finally { main.Handle.TypeHash = hash; }
            settings.ProcessedExternalManifests = Array.Empty<string>();
            Missing(documents[0],main); Missing(documents[0],main); Reject(() => plugin.ProcessInstance(main));
            settings.ProcessedExternalManifests = targets; DependencyResolutionSmokeTest.Prepare(documents[0],main); Equal(baseline[main.Handle.Name],plugin.ProcessInstance(main));
            sound.SetAttribute("TypeId","0");
            AssetDeclarationDocument fresh = Read(processor,Path.Combine(directory,"AudioEventProbe.xml"));
            DependencyResolutionSmokeTest.Prepare(fresh,fresh.SelfInstances.Single()); Equal(baseline[main.Handle.Name],plugin.ProcessInstance(fresh.SelfInstances.Single()));
            CheckLimits(processor,plugin,directory);
            Ra3Ep1AudioEventPlugin isolated = new(); Reject(() => isolated.GetExtendedTypeInformation(0x844D7B9Fu));
            isolated.Initialize(TargetPlatform.Win32); isolated.GetExtendedTypeInformation(0x844D7B9Fu).TypeHash = 0;
            Require(isolated.GetExtendedTypeInformation(0x844D7B9Fu).TypeHash == 0x560C2E45u,"Metadata was caller-mutable.");
            Reject(() => isolated.GetExtendedTypeInformation(0x166B084Du)); Reject(() => isolated.ReInitialize(TargetPlatform.Xbox360));
            Reject(() => isolated.ProcessInstance(fresh.SelfInstances.Single())); isolated.ReInitialize(TargetPlatform.Win32);
            if (stocks.Length > 0)
            {
                settings.ProcessedExternalManifests = stocks.Select(Path.GetFullPath).ToArray();
                Dictionary<string,(Relo.Chunk Data,uint Id,uint[] References)> expected = new(StringComparer.Ordinal);
                foreach (string file in new[] { "AudioEventProbe.xml", "AudioEventExtendedProbe.xml" })
                {
                    AssetDeclarationDocument doc = Read(processor,Path.Combine(directory,file));
                    foreach (InstanceDeclaration instance in doc.SelfInstances)
                    {
                        DependencyResolutionSmokeTest.Prepare(doc,instance); AssetBuffer actual = plugin.ProcessInstance(instance);
                        Equal(actual,baseline[instance.Handle.Name]);
                        expected.Add(instance.Handle.Name,(new Relo.Chunk { InstanceBuffer = actual.InstanceData, RelocationBuffer = actual.RelocationData, ImportsBuffer = actual.ImportsData },
                            instance.Handle.InstanceId,instance.ValidatedReferencedInstances!.Select(handle => handle.InstanceId).ToArray()));
                    }
                }
                int comparisons = 0;
                foreach (string stock in stocks.Where(path => ManifestReader.Read(File.ReadAllBytes(path)).Assets.Any(asset => expected.ContainsKey(asset.Name))))
                { AudioEventNativeSmokeTest.Compare(expected,stock); comparisons++; }
                Require(comparisons > 0,"Optional stock proof requires a manifest containing all five selected AudioEvents.");
            }
            processor.Cache = null!;
            Policy(() => processor.ProcessDocumentInternal("missing","missing",null!,new DocumentProcessor.ProcessOptions { GenerateOutput = true }));
            Require(!Directory.EnumerateFiles(directory,"*.bin").Any(),"AudioEvent profile emitted streams.");
            Console.WriteLine("EP1 AudioEvent profile self-test: OK (five checked native entries; descriptor/policies; current values/selectors/identities; missing recovery; 32/33 boundary; platform/cache/output closure)");
        }
        finally { Settings.Current = saved; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove empty, exact 32-reference limit, repeated concrete aliases and unsupported range admission boundaries. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckLimits(DocumentProcessor processor,IAssetBuilderPlugin plugin,string directory)
    {
        string source = Path.Combine(directory,"limits.xml");
        Settings.Current.ProcessedExternalManifests = Enumerable.Range(0,33).Select(index => Target(directory,"limit-"+index,"Limit"+index)).ToArray();
        foreach (int count in new[] { 0,32,33 })
        {
            File.WriteAllText(source,Xml("<AudioEvent id=\"Limit\">"+string.Concat(Enumerable.Range(0,count).Select(index => "<Sound>AudioFile:Limit"+index+"</Sound>"))+"</AudioEvent>"));
            AssetDeclarationDocument doc = Read(processor,source); InstanceDeclaration instance = doc.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(doc,instance);
            if (count == 33) Reject(() => plugin.ProcessInstance(instance));
            else Require(plugin.ProcessInstance(instance).InstanceData.Length == 152 + count * 12,"AudioEvent count boundary differs.");
        }
        foreach (string children in new[] { "<Sound>Limit0</Sound><Sound>AudioFile:Limit0</Sound>", "<MinRangeShift Low=\"0\" High=\"1\" />",
            "<VolumeSliderMultiplier Slider=\"SOUNDFX\" Multiplier=\"1\" />" })
        {
            File.WriteAllText(source,Xml("<AudioEvent id=\"Rejected\">"+children+"</AudioEvent>"));
            AssetDeclarationDocument doc = Read(processor,source); InstanceDeclaration instance = doc.SelfInstances.Single(); DependencyResolutionSmokeTest.Prepare(doc,instance);
            Reject(() => plugin.ProcessInstance(instance));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write metadata-only AudioFile fixtures with stock fingerprints; this does not claim audio payload compatibility. */
    //-------------------------------------------------------------------------------------------------
    private static string Target(string directory,string file,string name)
    { string path = Path.Combine(directory,file+".manifest"); ExternalLinkSmokeTest.WriteFixture(path,new InstanceHandle("AudioFile",name),new ReferencedFileBuffer(),typeHash:0x53C81E47u,tokenized:false); return path; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: force fresh schema-bound current source despite caller precompiled requests. */
    //-------------------------------------------------------------------------------------------------
    private static AssetDeclarationDocument Read(DocumentProcessor processor,string path) => processor.ProcessDocumentInternal(path,path,null!,new DocumentProcessor.ProcessOptions { GenerateOutput = false, UsePrecompiled = true });

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep owned input strings in the official namespace. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">"+body+"</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require repeated dependency loss to clear prepared tables. */
    //-------------------------------------------------------------------------------------------------
    private static void Missing(AssetDeclarationDocument doc,InstanceDeclaration instance)
    { try { DependencyResolutionSmokeTest.Prepare(doc,instance); } catch (BinaryAssetBuilderException error) when (error.ErrorCode == ErrorCode.UnknownReference) { Require(instance.ValidatedReferencedInstances == null,"Missing target retained prepared state."); return; } throw new InvalidDataException("Missing AudioFile was accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: restore caller attributes after each eligibility rejection. */
    //-------------------------------------------------------------------------------------------------
    private static void Mutate(IAssetBuilderPlugin plugin,InstanceDeclaration instance,XmlElement element,string name,string value)
    { bool present = element.HasAttribute(name); string old = element.GetAttribute(name); element.SetAttribute(name,value); try { Reject(() => plugin.ProcessInstance(instance)); } finally { if (present) element.SetAttribute(name,old); else element.RemoveAttribute(name); } }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare complete native buffers, not just sizes. */
    //-------------------------------------------------------------------------------------------------
    private static void Equal(AssetBuffer a,AssetBuffer b) => Require(a.InstanceData.SequenceEqual(b.InstanceData) && a.RelocationData.SequenceEqual(b.RelocationData) && a.ImportsData.SequenceEqual(b.ImportsData),"AudioEvent profile buffers differ.");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: count only deliberate eligibility, schema or readiness failures. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is NotSupportedException or XmlSchemaValidationException or InvalidOperationException) { return; } throw new InvalidDataException("Unsafe AudioEvent profile input was accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: output policy must reject before missing source/cache access. */
    //-------------------------------------------------------------------------------------------------
    private static void Policy(Action action)
    { try { action(); } catch (BinaryAssetBuilderException error) when (error.Message.Contains("RA3EP1-AudioEvent-Experimental-v1",StringComparison.Ordinal)) { return; } throw new InvalidDataException("AudioEvent production policy was bypassed."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first checked-entry contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
