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

// Reborn: cover Uprising aura stock layouts, filters, imports, masks, defaults and full object composition without editing user sources.
internal static class AttributeModifierAuraSmokeTest
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
            var grammar=AttributeModifierAuraContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBunker.xml","AttributeModifierAuraUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: exercise every own attribute, the SHRINK category bit, strong imports, inherited upgrades and nested filter children.
                if(variant==1){
                    owner.SetAttribute("id","RebornAura");
                    owner.SetAttribute("AttributeModifierName",catalog.First(a=>a.Name.StartsWith("AttributeModifier:")&&a.Name!="AttributeModifier:"+owner.GetAttribute("AttributeModifierName")).Name.Split(':')[1]);
                    owner.SetAttribute("AntiFX",catalog.First(a=>a.Name.StartsWith("FXList:")).Name.Split(':')[1]);
                    foreach(var pair in new[]{("RefreshDelay","0.2s"),("Range","123"),("RequiredConditions","MOUNTED"),("AntiCategory","BUFF SHRINK"),("MaxActiveRank","3"),("RequiredObjectStatusFlags","GENERIC_TOGGLE_STATE"),("DisabledTypesToProcess","HELD EMP")})owner.SetAttribute(pair.Item1,pair.Item2);
                    foreach(string name in new[]{"RunWhileDead","AllowSelf","AffectContainedOnly","AffectHordeMembersOnly","AffectAttacheeOnly","ShowDebugCircle","RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode"})owner.SetAttribute(name,"true");
                    owner.SetAttribute("AllowPowerWhenAttacking","false");owner.SetAttribute("InitiallyActive","false");owner.SetAttribute("WaypointModeTerminal","false");
                    owner.InnerXml="<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_3' Frames='12' Trigger='2'/><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedAirPower</TriggeredBy><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech2</TriggeredBy><ConflictsWith xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</ConflictsWith><ObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Relationship='ALLIES ENEMIES' Include='INFANTRY' Exclude='SURPRISE_ATTACKER'><IncludeThing>SovietHeavyAntiVehicleInfantry</IncludeThing><ExcludeThing>SovietAntiInfantryInfantry</ExcludeThing></ObjectFilter>";
                }
                // Reborn: omitted own fields retain schema defaults, including HELD, and permit an absent or empty filter.
                if(variant>=2){owner.RemoveAll();owner.SetAttribute("id","RebornAuraDefaults");if(variant==3)owner.InnerXml="<ObjectFilter xmlns='uri:ea.com:eala:asset'/>";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("AttributeModifierAuraUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,AttributeModifierAuraContract.TypeId);var element=resolved.GetElementsByTagName("AttributeModifierAuraUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1AttributeModifierAuraUpdateModuleData),AttributeModifierAuraContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,AttributeModifierAuraContract.Wire,typeof(Ep1AttributeModifierAuraUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1AttributeModifierAuraUpdateModuleData),proof.Records);checks++;
                // Reborn: authored aura variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>0){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted radius must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1AttributeModifierAuraUpdateModuleData>(nameof(Ep1AttributeModifierAuraUpdateModuleData.Range)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1AttributeModifierAuraUpdateModuleData),AttributeModifierAuraContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt aura payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: AttributeModifierAura: PASS ({checks} three-stock/all-aura-fields/SHRINK/filter/upgrade/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
