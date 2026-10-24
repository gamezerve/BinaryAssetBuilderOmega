using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;
using Tracker=Relo.Tracker;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: exercise original and edited airfield modules privately, including optional filter/vector storage and actual Core reference indices.
internal static class AirfieldBehaviorSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove real dispatch, source admission, complete native ownership and stock-qualified metadata without changing user mods or launching a game. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));int checks=0;
            foreach(var type in new[]{typeof(Ep1RecallUnitsSpecialPowerModuleData),typeof(Ep1ParkingPlaceBehaviorModuleData)}){
                var grammar=AirfieldBehaviorContract.Qualify(type,manifest,baseline,schemas,catalog);string wire=AirfieldBehaviorContract.Wire(type),name=wire.Replace("ModuleData","");
                var original=GameObjectExtendedRootContract.Original("Soviet/Structures/SovietAirfield.xml",name).Single();
                for(int variant=0;variant<3;variant++){
                    var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(original,true);behaviors.AppendChild(owner);
                    if(variant>0){owner.SetAttribute("id","RebornAirfield"+variant);if(name=="RecallUnitsSpecialPower"){owner.SetAttribute("MaxAircraftToRecall","8");owner.SetAttribute("PlayReturnVoiceFromRecalledUnit","false");}
                        else{owner.SetAttribute("NumRows","3");owner.SetAttribute("NumCols","4");owner.SetAttribute("HelipadBoneName","RebornLanding");owner.SetAttribute("QueueDistance","37.5");owner.SetAttribute("ProducedAboveHeight","725");owner.SetAttribute("DoorsUnpackOpen","true");var vector=source.CreateElement("DefaultRallyPointOffset",SchemaSet.XmlNamespace);vector.SetAttribute("x","12.5");vector.SetAttribute("y","-3");owner.AppendChild(vector);}}
                    if(variant==2){if(name=="RecallUnitsSpecialPower"){owner.RemoveAttribute("MaxAircraftToRecall");owner.RemoveAttribute("PlayReturnVoiceFromRecalledUnit");}else{foreach(var child in owner.ChildNodes.OfType<XmlElement>().ToArray())owner.RemoveChild(child);owner.RemoveAttribute("HelipadBoneName");}}
                    _=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var authored=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName(name,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();authored.SetAttribute("TypeId",FastHash.GetHashCode(wire).ToString(System.Globalization.CultureInfo.InvariantCulture));
                    var native=Compile(authored,type);var proof=SchemaNativeContract.Verify(resolved.GetElementsByTagName(name,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),type,FastHash.GetHashCode(wire),native,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());
                    var metadata=BinaryAssetBuilder.XmlCompiler.Ep1BehaviorMetadataTokenizer.Tokenize(native,official,wire,type);_=grammar.Verify(metadata,native,type,proof.Records);checks++;
                    // Reborn: a changed trailing scalar must fail the independent source proof, even if the metadata table is otherwise valid.
                    var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[type==typeof(Ep1RecallUnitsSpecialPowerModuleData)?476:8]^=1;
                    try{SchemaNativeContract.Verify(resolved.GetElementsByTagName(name,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),type,FastHash.GetHashCode(wire),corrupt,instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray());throw new InvalidOperationException("Corrupt airfield scalar accepted.");}catch(InvalidDataException){checks++;}
                }
            }
            Console.WriteLine($"Reborn: airfield behaviors: PASS ({checks} original/edit/default/optional/corruption checks; retail native and metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: dispatch through the same real polymorphic behavior writer used by normal GameObject compilation. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe Chunk Compile(XmlElement source,Type type)
    {
        var namespaces=new XmlNamespaceManager(source.OwnerDocument!.NameTable);namespaces.AddNamespace("ea",SchemaSet.XmlNamespace);var node=new Node(source.CreateNavigator()!,namespaces);BehaviorModuleData** root;
        using var tracker=new Tracker((void**)&root,4,false);Marshaler.Marshal(node,root,tracker);var chunk=new Chunk();if(!tracker.MakeRelocatable(chunk))throw new InvalidDataException("Airfield behavior allocation failed.");
        // Reborn: polymorphic pointer framing is removed so the independent proof receives a standalone leaf with rebased relocation/import slots.
        return (Chunk)typeof(ConscriptVariableGraphVerifier).GetMethod("Extract",BindingFlags.NonPublic|BindingFlags.Static)!.Invoke(null,new object[]{chunk,4,chunk.InstanceBuffer.Length})!;
    }
}
