using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover truly absent, default and authored VisionInfo with complete source/native/metadata proof and no user-source edits.
internal static class OptionalVisionInfoSmokeTest
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
            int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var vision=ConscriptVariableContainers.Find(source,"VisionInfo")!;
                if(variant<2){vision.ParentNode!.RemoveChild(vision);if(variant==1){var body=ConscriptVariableContainers.Find(source,"Body")!;body.InnerXml="<PropBody xmlns='uri:ea.com:eala:asset' id='RebornSpellbookBody'/>";ConscriptVariableContainers.Find(source,"ObjectResourceInfo")?.ParentNode!.RemoveChild(ConscriptVariableContainers.Find(source,"ObjectResourceInfo")!);foreach(var movement in source.DocumentElement!.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="LocomotorSet").ToArray())movement.ParentNode!.RemoveChild(movement);}}
                else{vision.RemoveAll();if(variant==3){string[] fields={"VisionRange","ShroudClearingRange","ShroudClearingHeight","VisionSide","VisionRear","VisionBonusTestRadius","VisionBonusTestSegments","VisionBonusPercentPerFoot","MaxVisionBonusPercent","MinVisionBonusPercent"};string[] values={"777","333","75","80%","65%","175","13","2%","75%","-25%"};for(int i=0;i<fields.Length;i++)vision.SetAttribute(fields[i],values[i]);}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var receipt=ConscriptEditableScalars.ReceiptValues(source);if(variant<2&&(receipt["VisionRange"]!=null||receipt["ShroudClearingRange"]!=null)||variant==1&&(receipt["MaxHealth"]!=null||receipt["Amount"]!=null||receipt["Speed"]!=null))throw new InvalidDataException("Omitted optional scalar acquired an invented receipt value.");checks++;
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);int pointer=System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(native.InstanceBuffer.AsSpan(560));if((variant<2)!=(pointer==0))throw new InvalidDataException("Optional VisionInfo native presence changed.");var tokens=verifier.Verify(source,native,instance,plan.Origins);var field=GameObjectStockReview.Tokens(tokens.InstanceBuffer,0)[BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("VisionInfo")];if(field.Type!=0x649F97D1u||(variant<2)!=(field.Offset==0))throw new InvalidDataException("Optional VisionInfo root declaration changed.");checks++;
                // Reborn: null vision metadata cannot silently point at another owner's table.
                if(variant==0){var bad=new Chunk{InstanceBuffer=tokens.InstanceBuffer.ToArray(),RelocationBuffer=tokens.RelocationBuffer,ImportsBuffer=tokens.ImportsBuffer};for(int at=0;at<bad.InstanceBuffer.Length;at+=12)if(System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bad.InstanceBuffer.AsSpan(at))==BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("VisionInfo")){System.Buffers.Binary.BinaryPrimitives.WriteUInt32LittleEndian(bad.InstanceBuffer.AsSpan(at+8),1);break;}try{verifier.Verify(source,native,instance,plan.Origins,bad);throw new InvalidOperationException("Non-null metadata accepted for omitted VisionInfo.");}catch(InvalidDataException){checks++;}}
            }
            // Reborn: genuine invalid values and duplicate optional elements still fail schema/source validation.
            foreach(bool duplicate in new[]{false,true}){var invalid=(XmlDocument)baseline.CloneNode(true);var owner=ConscriptVariableContainers.Find(invalid,"VisionInfo")!;if(duplicate)owner.ParentNode!.AppendChild(owner.CloneNode(true));else owner.SetAttribute("VisionRange","NaN");try{ConscriptVariableBehaviorSource.Admit(invalid,baseline,official);throw new InvalidOperationException("Invalid VisionInfo source accepted.");}catch(InvalidDataException){checks++;}}
            Console.WriteLine($"Reborn: optional VisionInfo: PASS ({checks} absent/empty/authored, PropBody without resource or locomotor, null receipts, native/metadata, corrupt metadata and invalid/duplicate source controls; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
