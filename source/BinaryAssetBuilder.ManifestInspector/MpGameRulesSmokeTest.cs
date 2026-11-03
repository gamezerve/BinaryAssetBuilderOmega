using System.Buffers.Binary;
using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify authored cash rules/defaults and real global publication without rebuilding a user mod.
internal static class MpGameRulesSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify retail offsets, authored choices, timer/default edits, required child validation and linked stream readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string source,string output)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh multiplayer-rules evidence directory required.");Directory.CreateDirectory(output);
        var saved=Settings.Current;
        try{
            Settings.Current=new Settings{SchemaPath=ReferencePipelineSmokeTest.FindFixture(),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
            schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=ManifestReader.Read(File.ReadAllBytes(manifest)).Assets.Concat(ManifestReader.Read(File.ReadAllBytes(global)).Assets).ToArray();
            MpGameRulesCompiler.Qualify(global,schemas,catalog);
            var authored=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(source,1048576));var root=authored.GetElementsByTagName("MpGameRules",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
            var xml=new XmlDocument();xml.AppendChild(xml.ImportNode(root,true));var native=MpGameRulesCompiler.Compile(xml.DocumentElement!);
            var proof=SchemaNativeContract.Resolve(xml,schemas,catalog);proof.DocumentElement!.RemoveAttribute("id");SchemaNativeContract.Verify(proof.DocumentElement,typeof(Ep1MpGameRules),0,native,Array.Empty<AssetId>());
            var omitted=(XmlElement)root.CloneNode(true);omitted.RemoveAttribute("NumMinutesBeforePlayersCanTransferMoney");omitted.GetElementsByTagName("SkirmishStartCash",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().RemoveAttribute("DefaultChoiceIndex");
            var defaults=MpGameRulesCompiler.Compile(omitted).InstanceBuffer;if(W(defaults,4)!=5||W(defaults,20)!=1)throw new InvalidDataException("MpGameRules XSD defaults differ.");
            omitted.SetAttribute("NumMinutesBeforePlayersCanTransferMoney","17");if(W(MpGameRulesCompiler.Compile(omitted).InstanceBuffer,4)!=17)throw new InvalidDataException("Authored transfer timer ignored.");
            var invalid=new XmlDocument();invalid.AppendChild(invalid.ImportNode(root,true));invalid.DocumentElement!.RemoveChild(invalid.DocumentElement.ChildNodes.OfType<XmlElement>().Single());invalid.Schemas.Add(schemas.Schemas);bool rejected=false;invalid.Validate((_,_)=>rejected=true);if(!rejected)throw new InvalidDataException("Required SkirmishStartCash was accepted when absent.");
            string entry=Path.Combine(output,"MapMetaData_Global.xml");File.WriteAllBytes(entry,File.ReadAllBytes(source));string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);
            if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built"),earlyGlobal:true))throw new InvalidDataException("Multiplayer rules global route not selected.");
            string path=Directory.GetFiles(Path.Combine(output,"built","stream-readback"),"*.manifest").Single();var linked=ManifestReader.Read(File.ReadAllBytes(path));var asset=linked.Assets.Single(a=>a.Name=="MpGameRules:"+root.GetAttribute("id"));
            if(asset.TypeHash!=0x34D91648u||asset.Tokenized!=0||asset.InstanceDataSize!=24||asset.References.Count!=0||!W3DContainerCompiler.ReadStock(path,linked,asset).SequenceEqual(native.InstanceBuffer))throw new InvalidDataException("MpGameRules linked fingerprint/payload differs.");
            Console.WriteLine($"Reborn: MpGameRules PASS (retail EP1 layout; authored cash {W(native.InstanceBuffer,8)}/{W(native.InstanceBuffer,12)}/{W(native.InstanceBuffer,16)}, index {W(native.InstanceBuffer,20)}; defaults, timer, required child, global package/readback; no user build or game).");
        }finally{Settings.Current=saved;}
    }
    // Reborn: read independently witnessed signed 32-bit stock offsets for test assertions.
    private static int W(byte[] data,int offset)=>BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset,4));
}
