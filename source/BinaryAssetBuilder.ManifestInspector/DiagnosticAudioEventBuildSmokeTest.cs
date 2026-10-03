using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: general AudioEvent admission must retain six-family ordering, immutable snapshots and exclusive publication.
internal static class DiagnosticAudioEventBuildSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise local event closure, actual AudioFile fingerprints, hostile source shapes and publication recovery through the public build service. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] stocks)
    {
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-DiagnosticAudioEvent-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(directory);
        Settings saved = Settings.Current;
        string source = Path.Combine(directory,"source.xml"), events = Path.Combine(directory,"events.xml");
        File.WriteAllText(Path.Combine(directory,"basics.xml"),Xml("<ShaderOverride id=\"LocalShader\"><Rule ReplaceShaderName=\"Null.fx\" ReplaceTechniqueName=\"Default\" /></ShaderOverride>"
            +"<ObjectFilterAsset id=\"LocalFilter\"><Filter /></ObjectFilterAsset>"));
        File.WriteAllText(Path.Combine(directory,"sound.xml"),Xml("<Includes><Include type=\"instance\" source=\"events.xml\" /></Includes>"
            +"<Multisound id=\"AConsumer\" Control=\"PLAY_ONE\"><Subsound>AudioEvent:ZEvent</Subsound></Multisound>"));
        string original = Xml("<AudioEvent id=\"ZEvent\" Volume=\"60\" Control=\"INTERRUPT\" SubmixSlider=\"SOUNDFX\">"
            +"<PitchShift Low=\"-5\" High=\"5\" /><Sound>WImpact_DebrisVsGrounda</Sound><Sound Weight=\"800\">WImpact_DebrisVsGroundb</Sound></AudioEvent>"
            +"<AudioEvent id=\"Unused\"><Sound>MissingUnusedFile</Sound></AudioEvent>");
        File.WriteAllText(events,original);
        File.WriteAllText(source,Xml("<Includes><Include type=\"all\" source=\"basics.xml\" /><Include type=\"instance\" source=\"sound.xml\" /></Includes>"
            +"<FXList id=\"LocalFX\"><NuggetList><Sound Value=\"AConsumer\" /></NuggetList></FXList><AttributeModifier id=\"LocalModifier\" StartFX=\"LocalFX\" />"));
        InstanceHandle[] audio = new[] { "WImpact_DebrisVsGrounda","WImpact_DebrisVsGroundb","WImpact_DebrisVsGroundc" }.Select(name => new InstanceHandle("AudioFile",name)).ToArray();
        string[] paths = audio.Select((handle,index) =>
        { string path = Path.Combine(directory,"audio-"+index+".manifest"); ExternalLinkSmokeTest.WriteFixture(path,handle,new ReferencedFileBuffer(),typeHash:0x53C81E47u,tokenized:false); return path; }).ToArray();
        string[] mappings = paths.Select((path,index) => path+"=data/audio-"+index+".manifest").ToArray();
        string first = BoundedDiagnosticBuild.Build(source,Path.Combine(directory,"first"),mappings); Check(first,audio.Take(2).ToArray());
        string repeat = BoundedDiagnosticBuild.Build(source,Path.Combine(directory,"repeat"),mappings); DiagnosticMultisoundBuildSmokeTest.Same(first,repeat);
        Require(File.ReadAllText(events) == original && ReferenceEquals(saved,Settings.Current),"Command modified source/settings.");
        // Reborn: frozen graph snapshots survive later leaf edits; fresh source changes must resolve current identities.
        DiagnosticSourceGraph frozen = DiagnosticSourceGraph.Read(source);
        File.WriteAllText(events,original.Replace("WImpact_DebrisVsGroundb","MissingChanged",StringComparison.Ordinal));
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved);
        string frozenDirectory = Path.Combine(directory,"frozen"); Directory.CreateDirectory(frozenDirectory); frozen.Write(frozenDirectory,new List<string>());
        DiagnosticMultisoundBuildSmokeTest.Same(first,BoundedDiagnosticBuild.Build(Path.Combine(frozenDirectory,"source.xml"),Path.Combine(directory,"frozen-output"),mappings));
        File.WriteAllText(events,original.Replace("WImpact_DebrisVsGroundb","WImpact_DebrisVsGroundc",StringComparison.Ordinal));
        Check(BoundedDiagnosticBuild.Build(source,Path.Combine(directory,"edited"),mappings),new[] { audio[0],audio[2] });
        File.WriteAllText(events,original);
        byte[] approved = File.ReadAllBytes(paths[0]); string staged;
        try
        {
            staged = BoundedDiagnosticBuild.Build(source,Path.Combine(directory,"snapshot-stage"),mappings,_ =>
            {
                File.WriteAllText(events,original.Replace("WImpact_DebrisVsGrounda","MissingChanged",StringComparison.Ordinal));
                ExternalLinkSmokeTest.WriteFixture(paths[0],new InstanceHandle("AudioEvent",audio[0].InstanceName),new ReferencedFileBuffer());
            });
        }
        finally { File.WriteAllText(events,original); File.WriteAllBytes(paths[0],approved); }
        DiagnosticMultisoundBuildSmokeTest.Same(first,staged);
        foreach (bool tokenized in new[] { false,true })
        {
            try
            {
                ExternalLinkSmokeTest.WriteFixture(paths[0],audio[0],new ReferencedFileBuffer(),typeHash:tokenized ? 0x53C81E47u : 0x53C81E46u,tokenized:tokenized);
                Exception error = DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved);
                Require(error is InvalidDataException && error.Message.Contains("External audio fingerprint",StringComparison.Ordinal),"AudioFile fingerprint did not reject at admission.");
            }
            finally { File.WriteAllBytes(paths[0],approved); }
        }
        string duplicate = Path.Combine(directory,"duplicate.manifest"); File.Copy(paths[0],duplicate);
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings.Append(duplicate+"=data/duplicate.manifest").ToArray(),saved);
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings.Skip(1).ToArray(),saved);
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings.Skip(1).ToArray(),saved);
        DiagnosticMultisoundBuildSmokeTest.Same(first,BoundedDiagnosticBuild.Build(source,Path.Combine(directory,"recovered"),mappings));
        // Reborn: all authored reference-list variants reject selectors/formulas before normalization can erase them.
        foreach (string child in new[] { "Attack","Sound","Decay" })
            foreach (string text in new[] { "WImpact_DebrisVsGrounda\\0","=Unknown" })
            {
                File.WriteAllText(events,Xml("<AudioEvent id=\"ZEvent\"><"+child+">"+text+"</"+child+"></AudioEvent>"));
                Exception error = DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved);
                Require(error is InvalidDataException && error.Message.Contains("AudioFile selectors",StringComparison.Ordinal),"Authored selector/expression was not rejected in preflight.");
            }
        foreach (string invalid in new[]
        {
            "<AudioEvent id=\"ZEvent\" TypeId=\"0\" />", "<AudioEvent id=\"ZEvent\" inheritFrom=\"Other\" />",
            "<AudioEvent id=\"ZEvent\" Volume=\"=Unknown\" />", "<AudioEvent id=\"ZEvent\" Control=\"SMART_LIMITING\" />",
            "<AudioEvent id=\"ZEvent\" SubmixSlider=\"VOICE\" />", "<AudioEvent id=\"ZEvent\"><Sound Volume=\"25\">WImpact_DebrisVsGrounda</Sound></AudioEvent>",
            "<AudioEvent id=\"ZEvent\"><Sound>AudioEvent:ZEvent</Sound></AudioEvent>",
            "<AudioEvent id=\"ZEvent\"><Sound><Nested /></Sound></AudioEvent>", "<AudioEvent id=\"ZEvent\"><PitchShift><Sound>x</Sound></PitchShift></AudioEvent>",
            "<AudioEvent id=\"ZEvent\"><LimitGroup>Other</LimitGroup></AudioEvent>", "<AudioEvent id=\"ZEvent\"><MinRangeShift Low=\"0\" High=\"1\" /></AudioEvent>",
            "<AudioEvent id=\"ZEvent\" /><AudioEvent id=\"zevent\" />", "<AudioFile id=\"ZEvent\" />",
            string.Concat(Enumerable.Range(0,33).Select(index => "<AudioEvent id=\"Root"+index+"\" />"))
        })
        { File.WriteAllText(events,Xml(invalid)); DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved); }
        File.WriteAllText(events,original);
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved,Path.GetDirectoryName(first));
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved,hook:stage =>
        { string path = Path.Combine(stage,"diagnostic.bin"); byte[] bytes = File.ReadAllBytes(path); bytes[4] ^= 1; File.WriteAllBytes(path,bytes); });
        string race = Path.Combine(directory,"race");
        DiagnosticMultisoundBuildSmokeTest.Reject(source,directory,mappings,saved,race,_ =>
        { Directory.CreateDirectory(race); File.WriteAllText(Path.Combine(race,"owner.txt"),"preserve"); });
        Require(File.ReadAllText(Path.Combine(race,"owner.txt")) == "preserve" && !File.Exists(Path.Combine(race,"diagnostic.manifest")),"Raced output was replaced.");
        string empty = Path.Combine(directory,"empty.xml"); File.WriteAllText(empty,Xml("<AudioEvent id=\"Empty\" />"));
        ManifestDocument emptyResult = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(empty,Path.Combine(directory,"empty"),Array.Empty<string>())));
        Require(emptyResult.Assets.Single().InstanceDataSize == 152 && emptyResult.Assets.Single().References.Count == 0,"Empty event unexpectedly required mappings.");
        // Reborn: positive command coverage includes all three reference lists and every admitted optional event range.
        string ranges = Path.Combine(directory,"ranges.xml");
        File.WriteAllText(ranges,Xml("<AudioEvent id=\"Ranges\" SubmixSlider=\"SOUNDFX\" Control=\"LOOP IMMEDIATE_DECAY_ON_KILL\">"
            +"<PitchShift Low=\"-5\" High=\"5\" /><PerFilePitchShift Low=\"-1\" High=\"1\" /><Delay Low=\"0\" High=\"15\" />"
            +"<InitialDelay Low=\"0\" High=\"30\" /><NonInterruptibleTime Low=\"0s\" High=\"1.5s\" />"
            +"<Attack>WImpact_DebrisVsGrounda</Attack><Sound>WImpact_DebrisVsGroundb</Sound><Decay>WImpact_DebrisVsGroundc</Decay></AudioEvent>"));
        string rangesOutput = BoundedDiagnosticBuild.Build(ranges,Path.Combine(directory,"ranges"),mappings);
        ManifestDocument rangesMetadata = ManifestReader.Read(File.ReadAllBytes(rangesOutput)); byte[] rangesBin = File.ReadAllBytes(Path.ChangeExtension(rangesOutput,".bin"));
        Require(rangesMetadata.Assets.Single().InstanceDataSize == 232 && rangesMetadata.Assets.Single().RelocationDataSize == 40
            && rangesMetadata.Assets.Single().ImportsDataSize == 16 && rangesMetadata.Assets.Single().References.SequenceEqual(audio.Select(handle => new AssetId(handle.TypeId,handle.InstanceId)))
            && BitConverter.ToUInt32(rangesBin,160) == 1 && BitConverter.ToUInt32(rangesBin,172) == 2 && BitConverter.ToUInt32(rangesBin,184) == 3,
            "Three-list/range command admission differs.");
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        ManifestDocument example = ManifestReader.Read(File.ReadAllBytes(BoundedDiagnosticBuild.Build(Path.Combine(fixtures,"DiagnosticAudioEventProbe.xml"),
            Path.Combine(directory,"example"),mappings)));
        Require(example.Assets.Select(asset => asset.TypeName).SequenceEqual(new[] { "AudioEvent","Multisound","FXList","AttributeModifier" })
            && example.Header.TotalInstanceDataSize == 368,"Committed AudioEvent command example differs.");
        if (stocks.Length > 0)
        {
            string[] realMappings = stocks.Select((path,index) => path+"=data/stock-"+index+".manifest").ToArray();
            Check(BoundedDiagnosticBuild.Build(source,Path.Combine(directory,"stock"),realMappings),audio.Take(2).ToArray());
            Console.WriteLine("  Actual EP1 diagnostic AudioEvent build: six families, local dependency order and unique AudioFile fingerprints OK.");
        }
        Require(!Directory.EnumerateDirectories(directory,".reborn-diagnostic-*").Any() && ReferenceEquals(saved,Settings.Current),"Command retained staging/settings.");
        Console.WriteLine("Diagnostic AudioEvent build self-test: OK (six families, frozen/edited source and metadata, fingerprint/missing recovery, authored selectors/options, existing/corrupt/raced publication)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently check six-family source order, native sizes, concrete tables and AudioFile/local sound selectors. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(string path,InstanceHandle[] audio)
    {
        ManifestDocument metadata = ManifestReader.Read(File.ReadAllBytes(path)); byte[] bin = File.ReadAllBytes(Path.ChangeExtension(path,".bin"));
        Require(metadata.Assets.Select(asset => asset.TypeName).SequenceEqual(new[] { "ShaderOverride","ObjectFilterAsset","AudioEvent","Multisound","FXList","AttributeModifier" })
            && metadata.Header.TotalInstanceDataSize == 532 && metadata.Header.AssetReferenceBufferSize == 40 && bin.Length == 540
            && new FileInfo(Path.ChangeExtension(path,".relo")).Length == 56 && new FileInfo(Path.ChangeExtension(path,".imp")).Length == 44,"Six-family totals/order differ.");
        Require(metadata.Assets[2].TypeHash == 0x560C2E45u && metadata.Assets[2].SourceFile == "input-0003.xml"
            && metadata.Assets[3].SourceFile == "input-0002.xml"
            && metadata.Assets[2].References.SequenceEqual(audio.Select(handle => new AssetId(handle.TypeId,handle.InstanceId)))
            && metadata.Assets[3].References.Single() == new AssetId(0x844D7B9Fu,InstanceHandle.GetInstanceId("ZEvent"))
            && metadata.Assets[4].References.Single() == new AssetId(0xA3A7AF37u,InstanceHandle.GetInstanceId("AConsumer"))
            && metadata.Assets[5].References.Single() == new AssetId(0x86682E78u,InstanceHandle.GetInstanceId("LocalFX"))
            && BitConverter.ToUInt32(bin,324) == 1 && BitConverter.ToUInt32(bin,336) == 2
            && BitConverter.ToUInt32(bin,376) == 1 && BitConverter.ToUInt32(bin,480) == 1 && BitConverter.ToUInt32(bin,496) == 1,"Six-family concrete selectors/sources differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the official namespace in owned authored fixtures. */
    //-------------------------------------------------------------------------------------------------
    private static string Xml(string body) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\">"+body+"</AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first snapshot/admission/publication contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
