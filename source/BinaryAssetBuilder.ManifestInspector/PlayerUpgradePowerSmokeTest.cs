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

// Reborn: cover player-upgrade stock identities and authored upgrade lists, inherited references and local module IDs and full object composition without editing user sources.
internal static class PlayerUpgradePowerSmokeTest
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
            var grammar=PlayerUpgradePowerContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var original=GameObjectExtendedRootContract.Original(variant==1?"EP1/System/PlayerSpellBook_EP1.xml":"System/PlayerSpellBook.xml","PlayerUpgradeSpecialPower").First();var owner=(XmlElement)source.ImportNode(original,true);behaviors.AppendChild(owner);
                // Reborn: multiple upgrades are source-owned strong references; an omitted list is schema-legal and has no hidden upgrade injection.
                if(variant==2){owner.SetAttribute("id","RebornPlayerUpgrade");owner.InnerXml="<Upgrade xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedHighTechnology</Upgrade><Upgrade xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedAirPower</Upgrade><Upgrade xmlns='uri:ea.com:eala:asset'>Upgrade_YurikoSpeed01</Upgrade>";}
                if(variant==3){owner.SetAttribute("id","RebornEmptyPlayerUpgrade");owner.InnerXml="";}
                if(variant==4){owner.SetAttribute("id","RebornRetargetedPlayerUpgrade");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerAirPower");owner.SetAttribute("StartsPaused","true");owner.SetAttribute("InitiateFX","FX_ChronoRiftHit");owner.InnerXml="<Upgrade xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedAirPower</Upgrade>";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("PlayerUpgradeSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,PlayerUpgradePowerContract.TypeId);var element=resolved.GetElementsByTagName("PlayerUpgradeSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(PlayerUpgradeSpecialPowerModuleData),PlayerUpgradePowerContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,PlayerUpgradePowerContract.Wire,typeof(PlayerUpgradeSpecialPowerModuleData));_=grammar.Verify(metadata,native,typeof(PlayerUpgradeSpecialPowerModuleData),proof.Records);checks++;
                // Reborn: one authored player-upgrade variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==2){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted upgrade count must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[System.Runtime.InteropServices.Marshal.SizeOf<SpecialPowerModuleData>()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(PlayerUpgradeSpecialPowerModuleData),PlayerUpgradePowerContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt player-upgrade payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: PlayerUpgradeSpecialPower: PASS ({checks} base and EP1 originals/multiple upgrades/empty/retarget/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
