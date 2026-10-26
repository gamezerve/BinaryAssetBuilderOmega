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

// Reborn: cover attack-notification stock imports, authored notification radiuss, defaults and full object composition without editing user sources.
internal static class NotifyWhenAttackingSmokeTest
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
            var grammar=NotifyWhenAttackingContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBaseDefenseAdvanced.xml","NotifyWhenAttackingBehavior").Single(),true);behaviors.AppendChild(owner);
                // Reborn: edit every own attribute and the friend graph, then distinguish omitted and explicitly empty optional filters.
                if(variant==1){owner.SetAttribute("id","RebornAttackNotification");owner.SetAttribute("NotifyRadius","333.5");owner.SetAttribute("AssistResponseModuleId","RebornAssist");owner.SetAttribute("SleepTime","0.75s");owner.SetAttribute("MaxFriendsToNotify","8");owner.SetAttribute("WeaponSlotID","2");owner.SetAttribute("WeaponSlotType","SECONDARY_WEAPON");owner.SetAttribute("HasMobileFriends","false");owner.SetAttribute("IgnoreDisabledStatus","false");owner.InnerXml="<AcceptableFriends xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='VEHICLE INFANTRY' Exclude='AIRCRAFT' Relationship='ALLIES'><IncludeThing>SovietAntiInfantryInfantry</IncludeThing><ExcludeThing>SovietBaseDefenseAdvanced</ExcludeThing></AcceptableFriends>";}
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornNotifyDefaults");if(variant==3)owner.InnerXml="<AcceptableFriends xmlns='uri:ea.com:eala:asset'/>";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("NotifyWhenAttackingBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,NotifyWhenAttackingContract.TypeId);var element=resolved.GetElementsByTagName("NotifyWhenAttackingBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(NotifyWhenAttackingBehaviorModuleData),NotifyWhenAttackingContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,NotifyWhenAttackingContract.Wire,typeof(NotifyWhenAttackingBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(NotifyWhenAttackingBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored attack-notification variants also traverses the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted notification radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<NotifyWhenAttackingBehaviorModuleData>(nameof(NotifyWhenAttackingBehaviorModuleData.NotifyRadius)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(NotifyWhenAttackingBehaviorModuleData),NotifyWhenAttackingContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt attack-notification payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: NotifyWhenAttackingBehavior: PASS ({checks} original/all-field/filter/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
