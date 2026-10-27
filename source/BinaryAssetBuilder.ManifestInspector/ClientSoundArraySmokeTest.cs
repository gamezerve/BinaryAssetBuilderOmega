using System.Buffers.Binary;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test selector sound arrays against original Bear data, all sound ordinals and complete object composition without editing user sources.
internal static class ClientSoundArraySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify populated/absent/empty sound overrides, sibling voice arrays, real references and native/metadata refusal boundaries. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=Xml(File.ReadAllText(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));baseline.DocumentElement!.SetAttribute("BuildTime","4");var grammar=new StockMetadataGrammar();ClientSoundArrayContract.Qualify(grammar,manifest,baseline,schemas,catalog);var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);int checks=0;
            for(int variant=0;variant<5;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var clients=source.GetElementsByTagName("ClientBehaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();clients.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietScoutInfantry.xml","ModelConditionSoundSelectorClientBehavior").Single(),true);clients.AppendChild(owner);
                if(variant==1){owner.SetAttribute("id","RebornAllSoundTypes");var array=owner.GetElementsByTagName("AudioArraySound",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();array.RemoveAll();foreach(string name in Enum.GetNames<Ep1ThingTemplateSoundType>()){var entry=source.CreateElement("AudioEntry",SchemaSet.XmlNamespace);entry.SetAttribute("Sound","SOV_Bear_MoveStartWater");entry.SetAttribute("AudioType",name);array.AppendChild(entry);}}
                // Reborn: absent and allocated-empty arrays remain distinct; a present empty Sound word intentionally disables one override slot.
                if(variant==2)owner.InnerXml="<Override xmlns='uri:ea.com:eala:asset'><AudioArrayVoice/></Override>";
                if(variant==3)owner.InnerXml="<Override xmlns='uri:ea.com:eala:asset'><AudioArraySound/></Override>";
                if(variant==4)owner.InnerXml="<Override xmlns='uri:ea.com:eala:asset' RequiredFlags='OVER_WATER'><AudioArraySound><AudioEntry Sound='' AudioType='soundAmbient'/></AudioArraySound></Override><Override xmlns='uri:ea.com:eala:asset' RequiredFlags='DAMAGED'><AudioArraySound><AudioEntry Sound='SOV_Bear_MoveStartWater' AudioType='soundMoveStart'/></AudioArraySound></Override>";
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var copy=Xml(instance.XmlNode.OuterXml);var carrier=Xml("<ClientProbe xmlns='uri:ea.com:eala:asset'/>");carrier.DocumentElement!.AppendChild(carrier.ImportNode(copy.GetElementsByTagName("ClientBehaviors",SchemaSet.XmlNamespace)[0]!,true));var native=Ep1ClientBehaviorsNativeSmokeTest.Compile(carrier.DocumentElement!);var leaf=Ep1ClientBehaviorsNativeSmokeTest.Leaf(native,0);var element=resolved.GetElementsByTagName("ModelConditionSoundSelectorClientBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),ClientSoundArrayContract.TypeId,leaf,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());var tokens=SchemaNativeContract.For(typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData)).Leaf(leaf);_=grammar.Verify(tokens,leaf,typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),proof.Records);checks++;
                if(variant is 0 or 1 or 3){var full=GameObjectBodyProjectionSmokeTest.Compile(copy,true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: a stale sound reference and an invalid sound ordinal cannot pass source proof or metadata generation.
                if(variant==1){int overrides=(int)Word(leaf.InstanceBuffer,12),array=(int)Word(leaf.InstanceBuffer,overrides+128),items=(int)Word(leaf.InstanceBuffer,array+4);var bad=Clone(leaf);Put(bad.InstanceBuffer,items,uint.MaxValue);Refuse(()=>SchemaNativeContract.Verify(element,typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData),ClientSoundArrayContract.TypeId,bad,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray()));checks++;bad=Clone(leaf);Put(bad.InstanceBuffer,items+4,39);Refuse(()=>SchemaNativeContract.For(typeof(Ep1ModelConditionSoundSelectorClientBehaviorModuleData)).Leaf(bad));checks++;
                    var missing=(XmlDocument)source.CloneNode(true);((XmlElement)missing.GetElementsByTagName("AudioArraySound",SchemaSet.XmlNamespace)[0]!.FirstChild!).SetAttribute("Sound","RebornMissingSelectorSound");Refuse(()=>SchemaNativeContract.Resolve(missing,schemas,catalog));checks++;
                }
            }
            Console.WriteLine($"Reborn: client AudioArraySound: PASS ({checks} original Bear/39 sound types/voice siblings/absent/empty/multiple-override/full-object/reference/ordinal/missing-target checks; original native and metadata proof; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
    // Reborn: detached in-memory XML leaves editable mod files unchanged.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
    // Reborn: corrupt only a copied native graph, retaining the exact original auxiliary inventories for validation.
    private static Chunk Clone(Chunk native)=>new(){InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};
    // Reborn: native record words use explicit little-endian reads.
    private static uint Word(byte[] bytes,int at)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(at,4));
    // Reborn: mutation controls write only to owned test buffers.
    private static void Put(byte[] bytes,int at,uint value)=>BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(at,4),value);
    // Reborn: genuine invalid values must fail rather than being silently skipped.
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidOperationException("Invalid client sound override accepted.");}
}
