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

// Reborn: cover ore delivery and both reactive-armor trackers with editable/default values and full object composition without editing user sources.
internal static class MinerBehaviorSmokeTest
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
            var grammar=MinerBehaviorContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            for(int variant=0;variant<2;variant++){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                foreach(string name in new[]{"OreMinerBehavior","TrackIncomingWeaponFireBehavior","TrackDamage"}){var owner=(XmlElement)source.ImportNode(MinerBehaviorContract.Original(name),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);owner.SetAttribute("id","Reborn"+name);behaviors.AppendChild(owner);
                    // Reborn: stock values do not constrain authored ore capacity, duration, status validation or damage-tracker flags.
                    if(name=="OreMinerBehavior"){if(variant==0){owner.RemoveAttribute("DeliveryCapacity");owner.RemoveAttribute("DeliveryTime");owner.InnerXml="";}else{owner.SetAttribute("DeliveryCapacity","1250");owner.SetAttribute("DeliveryTime","0.5s");owner.InnerXml="<ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='WEAPON_UPGRADED_01' ForbiddenStatus='GENERIC_TOGGLE_STATE'/>";}}
                    if(name=="TrackDamage"){owner.SetAttribute("TimeToTrack",variant==0?"1s":"0.25s");if(variant==0)owner.RemoveAttribute("InstantWeaponsOnly");else owner.SetAttribute("InstantWeaponsOnly","true");}}
                // Reborn: populated multi-item weak equivalence lists compose alongside authored miner modules without changing strong import order.
                if(variant==1)foreach(string target in new[]{"SovietMiner","SovietMiner_Naval"}){var entry=source.CreateElement("EquivalentTo",SchemaSet.XmlNamespace);entry.InnerText=target;source.DocumentElement!.InsertBefore(entry,source.GetElementsByTagName("SkirmishAIInformation",SchemaSet.XmlNamespace).OfType<XmlElement>().Single());}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var refs=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                foreach(var type in MinerBehaviorContract.Types){string name=type==typeof(OreMinerModuleData)?"OreMinerBehavior":type==typeof(TrackDamageModuleData)?"TrackDamage":"TrackIncomingWeaponFireBehavior";uint typeId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(type.Name);var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(name,SchemaSet.XmlNamespace).OfType<XmlElement>().Single(),typeId);var element=resolved.GetElementsByTagName(name,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var proof=SchemaNativeContract.Verify(element,type,typeId,native,refs);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,type.Name,type);_=grammar.Verify(metadata,native,type,proof.Records);checks++;}
                var full=GameObjectBodyProjectionSmokeTest.Compile(normalized,true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;
            }
            Console.WriteLine($"Reborn: miner behaviors: PASS ({checks} native/metadata and complete-object checks; editable delivery/status and damage tracking, original witness/defaults; no game launch).");
        }finally{Settings.Current=saved;}
    }
}
