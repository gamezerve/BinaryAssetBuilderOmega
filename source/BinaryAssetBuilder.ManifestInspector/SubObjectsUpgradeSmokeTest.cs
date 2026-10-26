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

// Reborn: cover source-sized subobject nuggets, upgrade conditions, textures, animation and defaults and full object composition without editing user sources.
internal static class SubObjectsUpgradeSmokeTest
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
            var grammar=SubObjectsUpgradeContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            var originals=GameObjectExtendedRootContract.Original("Allied/Units/AlliedBomberAircraft.xml","SubObjectsUpgrade").Concat(GameObjectExtendedRootContract.Original("Allied/Structures/AlliedConstructionYard.xml","SubObjectsUpgrade")).Concat(GameObjectExtendedRootContract.Original("EP1/Allied/Units/AlliedGunshipAircraft.xml","SubObjectsUpgrade")).ToArray();
            var textures=catalog.Where(a=>a.TypeName=="Texture").Select(a=>a.Name["Texture:".Length..]).Distinct().Take(3).ToArray();
            foreach(int variant in Enumerable.Range(0,7)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[variant<4?variant:0],true);behaviors.AppendChild(owner);
                // Reborn: change inherited upgrade flags/lists and every nugget field, including two real texture-reference pairs and per-nugget animation.
                if(variant==4){owner.RemoveAll();owner.SetAttribute("id","RebornSubObjects");foreach(string name in new[]{"RequiresAllTriggers","RequiresAllConflictingTriggers","Permanent","WaypointMode"})owner.SetAttribute(name,"true");owner.SetAttribute("WaypointModeTerminal","false");owner.InnerXml=$"<CustomAnimAndDuration xmlns='uri:ea.com:eala:asset' AnimState='USER_3' Frames='12' Trigger='2'/><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedAirPower</TriggeredBy><TriggeredBy xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech2</TriggeredBy><ConflictsWith xmlns='uri:ea.com:eala:asset'>Upgrade_AlliedTech3</ConflictsWith><Nugget xmlns='uri:ea.com:eala:asset' ShowSubObjects='RebornWing RebornEngine' HideSubObjects='OldWing OldEngine' Upgrade='Upgrade_AlliedAirPower' FadeTime='1.25s' WaitBeforeFadeTime='0.7s' SkipFadeOnCreate='true' HideSubObjectsOnRemove='true' UnHideSubObjectsOnRemove='true'><UpgradeTexture Original='{textures[0]}' New='{textures[1]}'/><UpgradeTexture Original='{textures[1]}' New='{textures[2]}'/><CustomAnimAndDuration AnimState='USER_4' Frames='15' Trigger='3'/></Nugget><Nugget xmlns='uri:ea.com:eala:asset' Upgrade='Upgrade_AlliedTech2' HideSubObjects='RebornHidden'/>";}
                if(variant>=5){owner.RemoveAll();owner.SetAttribute("id","RebornSubObjectsDefaults");owner.InnerXml=string.Concat(Enumerable.Range(0,variant==5?1:5).Select(i=>"<Nugget xmlns='uri:ea.com:eala:asset' Upgrade='Upgrade_AlliedAirPower'/>"));}
                // Reborn: the public preparation admission must accept the original EA animation order before any direct variable-route normalization.
                if(variant==0){ConscriptSourceInput.Admit(source,baseline,official);checks++;}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("SubObjectsUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,SubObjectsUpgradeContract.TypeId);var element=resolved.GetElementsByTagName("SubObjectsUpgrade",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1SubObjectsUpgradeModuleData),SubObjectsUpgradeContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,SubObjectsUpgradeContract.Wire,typeof(Ep1SubObjectsUpgradeModuleData));_=grammar.Verify(metadata,native,typeof(Ep1SubObjectsUpgradeModuleData),proof.Records);checks++;
                // Reborn: authored subobject-upgrade variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant>=4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted weak upgrade handle must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceBuffer.AsSpan(40)))+24]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1SubObjectsUpgradeModuleData),SubObjectsUpgradeContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt subobject-upgrade payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: reject truly invalid zero-nugget and missing-required-upgrade inputs under the original schema.
            foreach(bool missingUpgrade in new[]{false,true}){var invalid=(XmlDocument)baseline.CloneNode(true);var behaviors=invalid.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.InnerXml=missingUpgrade?"<SubObjectsUpgrade xmlns='uri:ea.com:eala:asset' id='RebornInvalid'><Nugget/></SubObjectsUpgrade>":"<SubObjectsUpgrade xmlns='uri:ea.com:eala:asset' id='RebornInvalid'/>";try{ConscriptVariableBehaviorSource.Admit(invalid,baseline,official);throw new InvalidOperationException("Invalid subobject upgrade accepted.");}catch(InvalidDataException){checks++;}}
            Console.WriteLine($"Reborn: SubObjectsUpgrade: PASS ({checks} four originals/all-field/multiple-nuggets/texture/animation/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
