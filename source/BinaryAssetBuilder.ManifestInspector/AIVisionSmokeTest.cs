using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: one-item AIUpdate ObjectVisionRange coverage through the production object compile, verifier and stock-learned grammar; no user output.
internal static class AIVisionSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retail flak-trooper AI, edited range/filter variants and explicit multi-item/thing-list refusals. */
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
            var grammar=AIUpdateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0,refusals=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            var defines=GameObjectExtendedRootContract.Original("GlobalData/GlobalDefines.xml","Define");
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var ai=source.GetElementsByTagName("AI",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();ai.RemoveAll();
                var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietAntiVehicleInfantry.xml","AIUpdate").Single(),true);ai.AppendChild(owner);
                foreach(var attribute in owner.SelectNodes(".//@*")!.OfType<XmlAttribute>().Where(a=>a.Value.StartsWith("=$",StringComparison.Ordinal)).ToArray())attribute.Value=defines.Single(e=>e.GetAttribute("name")==attribute.Value[2..]).GetAttribute("value");
                var vision=(XmlElement)owner.GetElementsByTagName("ObjectVisionRange",SchemaSet.XmlNamespace)[0]!;var filter=(XmlElement)vision.GetElementsByTagName("Filter",SchemaSet.XmlNamespace)[0]!;
                // Reborn: 1 edits every filter scalar and both status pointers; 2 drops the status pointer and Range default; 3 uses an empty Filter (all schema defaults).
                if(variant==1){owner.SetAttribute("id","RebornVisionEdit");vision.SetAttribute("Range","250");filter.SetAttribute("Rule","NONE");filter.SetAttribute("Relationship","ENEMIES NEUTRAL");filter.SetAttribute("Alignment","EVIL");filter.SetAttribute("Exclude","STRUCTURE");filter.SetAttribute("StatusBitFlagsExclude","STEALTHED");}
                if(variant==2){vision.RemoveAttribute("Range");filter.RemoveAttribute("StatusBitFlags");}
                if(variant==3)filter.RemoveAllAttributes();
                if(variant==4)vision.ParentNode!.AppendChild(vision.CloneNode(true));
                if(variant==5){var thing=source.CreateElement("IncludeThing",SchemaSet.XmlNamespace);thing.InnerText="SovietAntiVehicleInfantry";filter.AppendChild(thing);}
                try{
                    var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);
                    int cut=Ep1GameObjectRootOwnersProjection.NativeTailStart(full.InstanceBuffer);var bytes=full.InstanceBuffer.AsSpan(0,cut).ToArray();foreach(var field in Ep1GameObjectRootOwnersProjection.Fields)Ep1GameObjectRootOwnersProjection.ClearSlot(bytes,field);
                    var layout=Ep1GameObjectStringTokens.Inspect(bytes,Slots(full.RelocationBuffer).Where(s=>s<cut&&!Ep1GameObjectRootOwnersProjection.Fields.Any(f=>f.PointerSlot==s)).ToArray(),Slots(full.ImportsBuffer).Where(s=>s<cut).ToArray(),true);var native=layout.Ai.Modules.Single().Native;var metadata=layout.Ai.Modules.Single().Tokens;var element=ConscriptModuleScalarContract.Owners(resolved).Single(o=>o.Container=="AI").Element;var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                    var proof=AIUpdateContract.Verify(element,native,references);_=grammar.Verify(metadata,native,typeof(AIUpdateContract.Native),proof.Records);checks++;
                    _=verifier.Verify(source,full,instance,plan.Origins);checks++;
                    if(variant>=4)throw new InvalidOperationException("Unsupported AI vision shape accepted: variant "+variant);
                    // Reborn: a changed vision range byte must be rejected by independent source/native validation.
                    int chooser=(int)BitConverter.ToUInt32(native.InstanceBuffer,68),item=(int)BitConverter.ToUInt32(native.InstanceBuffer,chooser+24);var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[item+4]^=1;
                    try{AIUpdateContract.Verify(element,corrupt,references);throw new InvalidOperationException("Corrupt AI vision payload accepted.");}catch(InvalidDataException){checks++;}
                }
                catch(Exception error) when(variant>=4 && error is not InvalidOperationException){refusals++;Console.WriteLine("  refused variant "+variant+": "+error.GetType().Name+": "+error.Message.Split('\n')[0]);}
            }
            Console.WriteLine($"Reborn: AI ObjectVisionRange: PASS ({checks} retail/edited/default/empty-filter/composition/corruption checks, {refusals} multi-item/thing-list refusals; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: enumerate only the sentinel-framed auxiliary words emitted by the real Tracker. */
    //-------------------------------------------------------------------------------------------------
    private static uint[] Slots(byte[] bytes)=>bytes.Length==0?Array.Empty<uint>():Enumerable.Range(0,bytes.Length/4-1).Select(i=>System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(i*4,4))).ToArray();
}
