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

// Reborn: cover original projectile self-replacement, edited target lists and inherited ability settings without editing user sources.
internal static class ProjectileReplaceSelfSmokeTest
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
            var grammar=ProjectileReplaceSelfContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(ProjectileReplaceSelfContract.Source,"ProjectileReplaceSelfSpecialAbility").Single(),true);behaviors.AppendChild(owner);
                if(variant==1){owner.SetAttribute("id","RebornProjectileReplaceSelfEdited");owner.SetAttribute("LaunchingWeapon","SovietAntiInfantryInfantryAK47");owner.RemoveAttribute("OtherObjectCreationList");owner.SetAttribute("NewObjectUnpackTime","6s");owner.SetAttribute("ReplaceOptions","COPY_TARGET_OBJECT IGNORE_TERRAIN_RESTRICTIONS COPY_UPGRADES DISABLE_DURING_REPLACE CHECK_BUILD_ASSISTANT DISABLE_NEW_OBJECT_DURING_UNPACK CLEAR_LOCATION REPLACE_OVER_ENEMIES TRANSFER_EXPERIENCE");owner.SetAttribute("ClearTriggerDistance","250");owner.SetAttribute("PreparationTime","1.5s");owner.SetAttribute("StartAbilityRange","350");owner.SetAttribute("Options","RECONSTITUTE_STORED_COMMAND BUSY_DURING_PACK");owner.SetAttribute("SetObjectStatusOnTrigger","UNSELECTABLE");owner.SetAttribute("SpecialObjectAttachToBone","RebornAttach");
                    // Reborn: multiple weak target identities are authored list entries, not a fixed Soviet MCV selector.
                    var target=source.CreateElement("ReplacementTemplate",SchemaSet.XmlNamespace);target.InnerText="AlliedMCV";owner.AppendChild(target);}
                if(variant==2){while(owner.Attributes.Count>0)owner.RemoveAttributeNode(owner.Attributes[0]!);owner.SetAttribute("id","RebornProjectileReplaceSelfDefaults");while(owner.HasChildNodes)owner.RemoveChild(owner.FirstChild!);}
                // Reborn: an authored alternative OCL must allocate/import its own target, independently of the original debris list.
                if(variant==3){owner.SetAttribute("id","RebornProjectileReplaceSelfOcl");owner.SetAttribute("OtherObjectCreationList","OCL_SovietParachute");}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("ProjectileReplaceSelfSpecialAbility",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,ProjectileReplaceSelfContract.TypeId);var element=resolved.GetElementsByTagName("ProjectileReplaceSelfSpecialAbility",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(ProjectileReplaceSelfSpecialAbilityModuleData),ProjectileReplaceSelfContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,ProjectileReplaceSelfContract.Wire,typeof(ProjectileReplaceSelfSpecialAbilityModuleData));_=grammar.Verify(metadata,native,typeof(ProjectileReplaceSelfSpecialAbilityModuleData),proof.Records);checks++;
                // Reborn: one authored target-list variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted launching weapon must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<ProjectileReplaceSelfSpecialAbilityModuleData>(nameof(ProjectileReplaceSelfSpecialAbilityModuleData.LaunchingWeapon)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(ProjectileReplaceSelfSpecialAbilityModuleData),ProjectileReplaceSelfContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt projectile replacement payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: ProjectileReplaceSelfSpecialAbility: PASS ({checks} original/edit/default/target-list/ability/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
