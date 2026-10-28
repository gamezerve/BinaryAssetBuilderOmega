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

// Reborn: cover naval circular damage and health drain with editable/default values and full object composition without editing user sources.
internal static class NavalUpdateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete original/default/edited source/native/relocation/metadata and whole-object checks through the production EP1 dispatcher. */
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
            var grammar=NavalUpdateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<3;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                foreach(var family in NavalUpdateContract.Families){var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(family.Path,family.Source).First(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                    // Reborn: authored/default records are independent of the captured original IDs and values.
                    if(variant>0){owner.RemoveAll();owner.SetAttribute("id","Reborn"+family.Source+variant);}
                    // Reborn: circular damage always includes its schema-required inline filter, even for default probes.
                    if(variant>0&&family.Source=="CircularDamageUpdate")owner.InnerXml="<ObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY'/>";
                    if(variant==2&&family.Source=="CircularDamageUpdate"){
                        owner.SetAttribute("Flags","ONLY_ON_WATER");owner.SetAttribute("MaxRadius","250");owner.SetAttribute("ArcDistance","12");owner.SetAttribute("FXTimeout","0.1s");owner.SetAttribute("SweepFXList","FX_NONE");owner.SetAttribute("SweepWeapon","SovietAntiInfantryInfantryAK47");
                        owner.InnerXml="<ObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='VEHICLE INFANTRY' Relationship='ENEMIES'><IncludeThing>AlliedMCV</IncludeThing></ObjectFilter>";
                    }
                    if(variant==2&&family.Source=="LoseHealthUpdate"){
                        owner.SetAttribute("HealthTrigger","150%");owner.SetAttribute("DamagePerSecond","17.5");owner.SetAttribute("DeathType","NORMAL");owner.InnerXml="<ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='GENERIC_TOGGLE_STATE' ForbiddenStatus='UNDER_IRON_CURTAIN WEAPON_UPGRADED_01'/>";
                    }
                }

                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                foreach(var family in NavalUpdateContract.Families){uint typeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(family.Wire);var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),typeId);var element=resolved.GetElementsByTagName(family.Source,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,family.Type,typeId,native,refs);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,family.Wire,family.Type);_=grammar.Verify(metadata,native,family.Type,proof.Records);checks++;
                    // Reborn: changing a proved flag/threshold word must fail without relying on the producer writer.
                    var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[8]^=1;
                    try{SchemaNativeContract.Verify(element,family.Type,typeId,corrupt,refs);throw new InvalidOperationException("Corrupt naval update accepted.");}catch(InvalidDataException){checks++;}
                }
                var full=GameObjectBodyProjectionSmokeTest.Compile(normalized,true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: naval updates: PASS ({checks} original/default/edit native/metadata and complete-object checks; inline filters, flags, typed FX/weapon imports, health thresholds above 100%, optional status gates and corrupt-native rejection; no game launch).");
        }finally{Settings.Current=saved;}
    }
}
