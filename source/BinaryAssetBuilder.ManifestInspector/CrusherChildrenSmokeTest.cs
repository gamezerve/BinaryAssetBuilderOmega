using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover crusher root graphs and authored nested lists, defaults and full object composition without editing user sources.
internal static class CrusherChildrenSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete source/native/relocation/metadata checks and a corrupt-output refusal through the existing production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var grammar=GameObjectExtendedRootContract.Qualify("CrusherInfo",manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var old=GameObjectExtendedRootContract.Owners(source,"CrusherInfo").Single();string path=variant==0?"Soviet/Units/SovietMiner.xml":"EP1/Soviet/Units/SovietGrinderVehicle.xml";var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(path,"CrusherInfo").Single(),true);old.ParentNode!.ReplaceChild(owner,old);
                // Reborn: cover source-sized repeated levels and kill delays, populated/empty/omitted optional conditions and filters, and removal of both lists.
                if(variant==2){owner.RemoveAll();owner.InnerXml="<ExtraCrushLevels xmlns='uri:ea.com:eala:asset' CrusherLevel='-1' CrushableLevel='30' CrushEqualLevelProps='true' ModelConditionMatch='OVER_WATER DAMAGED'/><ExtraCrushLevels xmlns='uri:ea.com:eala:asset' ModelConditionMatch=''><ObjectFilter Rule='ANY' Include='VEHICLE INFANTRY'><ExcludeThing>SovietScoutVehicle</ExcludeThing></ObjectFilter></ExtraCrushLevels><ExtraCrushLevels xmlns='uri:ea.com:eala:asset'/><ExtraCrushKillDelays xmlns='uri:ea.com:eala:asset' CrushKillDelay='0.75s' CrushDecelerationPercent='25%'><ObjectFilter Rule='ANY' Include='INFANTRY'/></ExtraCrushKillDelays><ExtraCrushKillDelays xmlns='uri:ea.com:eala:asset'><ObjectFilter/></ExtraCrushKillDelays>";}
                if(variant==3){owner.RemoveAll();owner.InnerXml="<ExtraCrushLevels xmlns='uri:ea.com:eala:asset'/><ExtraCrushKillDelays xmlns='uri:ea.com:eala:asset'><ObjectFilter/></ExtraCrushKillDelays>";}
                if(variant==4)owner.RemoveAll();
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("CrusherInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var native=Compile(normalized);var element=GameObjectExtendedRootContract.Owners(resolved,"CrusherInfo").Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1CrusherInfo),0,native,references);var metadata=Ep1BehaviorMetadataTokenizer.TokenizeRootRecord(native,official,"CrusherInfo");_=grammar.Verify(metadata,native,typeof(Ep1CrusherInfo),proof.Records);checks++;
                if(variant>=1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: corrupt a populated list count, or a root scalar when lists are absent, independently of the metadata producer.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[variant<4?24:0]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1CrusherInfo),0,corrupt,references);throw new InvalidOperationException("Corrupt crusher graph accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: a late native exception without a source node must point to the original XML child rather than private generated game.xml.
            string scratch=Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(fixtures))!,"alpha/.work/CrusherOrigin-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(scratch);string original=Path.Combine(scratch,"OriginalCrusher.xml"),entry=Path.Combine(scratch,"game.xml");string xml="<GameObject xmlns='uri:ea.com:eala:asset' id='RebornCrusherOrigin'>\n<CrusherInfo>\n<ExtraCrushLevels/>\n</CrusherInfo>\n</GameObject>";File.WriteAllText(original,xml);File.WriteAllText(entry,xml);File.WriteAllText(Path.Combine(scratch,"source-input.json"),System.Text.Json.JsonSerializer.Serialize(new{Origins=new[]{new{Type="GameObject",Id="RebornCrusherOrigin",Path=original}}}));var originError=new InvalidDataException("EP1 populated extra-crush child ABI not reviewed.");ConscriptCompileLog.ForPrepared(entry).NativeError(originError,SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(xml)));var origin=SdkXmlDiagnostics.Origin(originError);if(origin==null||origin.Context||origin.Line!=3||!origin.File.Equals(original,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Late crusher source location did not identify the original child.");checks++;
            Console.WriteLine($"Reborn: CrusherInfo children: PASS ({checks} populated retail graphs, repeated level/delay/filter/condition/default/removal/composition/corruption/source-origin checks; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile one normalized crusher owner through its actual EP1 writer on a standalone Tracker, preserving the Core occurrence table. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlElement source)
    {
        var document=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.OuterXml));var ns=new XmlNamespaceManager(document.NameTable);ns.AddNamespace("ea",SchemaSet.XmlNamespace);Ep1CrusherInfo* root;using var tracker=new Relo.Tracker((void**)&root,(uint)sizeof(Ep1CrusherInfo),false);Marshaler.Marshal(new Node(document.DocumentElement!.CreateNavigator()!,ns),root,tracker);var result=new Chunk();if(!tracker.MakeRelocatable(result))throw new InvalidDataException("Crusher Tracker failed.");return result;
    }

}
