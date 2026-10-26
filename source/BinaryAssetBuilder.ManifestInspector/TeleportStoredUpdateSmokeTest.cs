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

// Reborn: cover teleport-update stock identities and authored storage fields, inherited references and local module IDs and full object composition without editing user sources.
internal static class TeleportStoredUpdateSmokeTest
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
            var grammar=TeleportStoredUpdateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var originals=GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","TeleportStoredObjectsUpdate");var owner=(XmlElement)source.ImportNode(originals[variant<4?variant:0],true);behaviors.AppendChild(owner);
                // Reborn: exercise every authored attribute and both nested filters, with weak thing lists and nonempty status masks.
                if(variant==4){owner.SetAttribute("id","RebornTeleportUpdate");owner.SetAttribute("TriggerFX","FX_ChronoRiftHit");owner.SetAttribute("TargetFX","FX_ChronoSphereHit");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerChronoRiftTeleportLvl1");owner.SetAttribute("GetObjectsCurrentlyAtStorePosition","true");owner.SetAttribute("Telefrag","INSTANT_DEATH_TELEPORTER_ONLY");owner.SetAttribute("FadeInTime","1.25s");owner.SetAttribute("InitialDelayTime","0.5s");owner.SetAttribute("DelayTime","7.5s");owner.SetAttribute("ScaleFadeInTimeByDistance","125.5");owner.SetAttribute("DisabledTypesToProcess","EMP HELD");owner.SetAttribute("Shader","ShaderOverride_ObjectsChronoRift");owner.InnerXml="<InstaKillObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='INFANTRY' StatusBitFlags='UNSELECTABLE'><IncludeThing>SovietAntiInfantryInfantry</IncludeThing></InstaKillObjectFilter><CollidesWithOnTeleport xmlns='uri:ea.com:eala:asset' Rule='ALL' Relationship='ENEMIES' Include='VEHICLE STRUCTURE' Exclude='ORE_NODE'><ExcludeThing>SovietConstructionYard</ExcludeThing></CollidesWithOnTeleport>";}
                if(variant==5){while(owner.Attributes.Count>0)owner.RemoveAttributeNode(owner.Attributes[0]!);owner.SetAttribute("id","RebornTeleportUpdateDefaults");owner.InnerXml="";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("TeleportStoredObjectsUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,TeleportStoredUpdateContract.TypeId);var element=resolved.GetElementsByTagName("TeleportStoredObjectsUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(TeleportStoredObjectsUpdateModuleData),TeleportStoredUpdateContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,TeleportStoredUpdateContract.Wire,typeof(TeleportStoredObjectsUpdateModuleData));_=grammar.Verify(metadata,native,typeof(TeleportStoredObjectsUpdateModuleData),proof.Records);checks++;
                // Reborn: one authored teleport-update variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted fade-in time must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[16]^=1;
                try{SchemaNativeContract.Verify(element,typeof(TeleportStoredObjectsUpdateModuleData),TeleportStoredUpdateContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt teleport-update payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: TeleportStoredObjectsUpdate: PASS ({checks} four originals/all attributes/two filters/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
