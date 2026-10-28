using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise original, inherited and edited locomotors through the native writer and independent schema proof.
internal static class LocomotorTemplateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare original EP1 bytes, then prove edited/inherited sources and refuse a corrupted payload. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();LocomotorTemplateCompiler.Qualify(manifest,schemas,catalog);int checks=0;
            // Reborn: the retail Gigafortress-style scout override (TurnTime values over HumanLocomotor) plus every optional record kind.
            var human=LocomotorTemplateCompiler.Original("HumanLocomotor",schemas);human.DocumentElement!.SetAttribute("id","RebornScoutLocomotor");human.DocumentElement.SetAttribute("TurnTimeSeconds","0.5s");human.DocumentElement.SetAttribute("TurnTimeDamagedSeconds","0.5s");
            var water=LocomotorTemplateCompiler.Original("JapanAntiAirShipWaterLocomotor_Attacking",schemas);water.DocumentElement!.SetAttribute("LookAheadMult","2.5");water.DocumentElement.SetAttribute("CanMoveBackward","true");water.DocumentElement.SetAttribute("EnteringModelConditions","OVER_WATER");water.DocumentElement.SetAttribute("EnteringModelConditionsTime","1.5s");
            var empty=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<LocomotorTemplate xmlns='uri:ea.com:eala:asset' id='RebornDefaultLocomotor'/>"));
            foreach(var source in new[]{human,water,empty}){
                var proof=LocomotorTemplateCompiler.ProofSource(source,schemas,catalog);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=LocomotorTemplateCompiler.Compile(instance.XmlNode);var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                SchemaNativeContract.Verify(proof.DocumentElement!,typeof(LocomotorTemplate),0,native,refs);checks++;
                // Reborn: the turn-time word is independently tied to current source, not a captured stock constant.
                var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[System.Runtime.InteropServices.Marshal.OffsetOf<LocomotorTemplate>(nameof(LocomotorTemplate.TurnTimeSeconds)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(LocomotorTemplate),0,bad,refs);throw new InvalidOperationException("Corrupt locomotor payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: LocomotorTemplate: PASS (seven original native graphs, {checks} inherited/edited/optional-record/default/corruption checks; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
