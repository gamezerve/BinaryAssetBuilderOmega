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

// Reborn: cover original collision records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class FireWeaponCollideSmokeTest
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
            var grammar=FireWeaponCollideContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: cover every original module, a fully edited filtered record, omitted defaults and explicit zero stance.
            var originals=GameObjectExtendedRootContract.Original("Soviet/Units/SovietAntiVehicleInfantry.xml","FireWeaponCollide").Concat(GameObjectExtendedRootContract.Original("EP1/Soviet/Units/SovietGrinderVehicle.xml","FireWeaponCollide")).ToArray();
            foreach(int variant in Enumerable.Range(0,8)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[Math.Min(variant,4)],true);behaviors.AppendChild(owner);
                // Reborn: edit all EP1 collision attributes including every flag and the optional filter's authored reference lists.
                if(variant==5){owner.SetAttribute("id","RebornCollision");owner.SetAttribute("Flags",string.Join(" ",Enum.GetNames<Ep1FireWeaponFlagsType>()));owner.SetAttribute("FireOnce","true");owner.SetAttribute("RequiredStatus","OVER_WATER");owner.SetAttribute("ForbiddenStatus","WEAPON_UPGRADED_01");owner.SetAttribute("WhereToFire","OWNER_POSITION");owner.SetAttribute("DisabledTypesToProcess","TEMPORARILY_BUSY");owner.SetAttribute("DisableType","DEFAULT");owner.SetAttribute("DisableDuration","1.25s");owner.SetAttribute("FiringInterval","0.5s");owner.SetAttribute("ForbiddenStance","AGGRESSIVE");}
                // Reborn: null optional pointers differ from an allocated GUARD ordinal zero; explicit zero timers remain zero.
                if(variant>=6){owner.RemoveAll();owner.SetAttribute("id","RebornCollisionDefaults");if(variant==7){owner.SetAttribute("ForbiddenStance","GUARD");owner.SetAttribute("WhereToFire","TARGET_POSITION");owner.SetAttribute("DisableDuration","0s");owner.SetAttribute("FiringInterval","0s");}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("FireWeaponCollide",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,FireWeaponCollideContract.TypeId);var element=resolved.GetElementsByTagName("FireWeaponCollide",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1FireWeaponCollideModuleData),FireWeaponCollideContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,FireWeaponCollideContract.Wire,typeof(Ep1FireWeaponCollideModuleData));_=grammar.Verify(metadata,native,typeof(Ep1FireWeaponCollideModuleData),proof.Records);checks++;
                // Reborn: authored collision variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 5 or 6 or 7){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted FireOnce value must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1FireWeaponCollideModuleData>(nameof(Ep1FireWeaponCollideModuleData.FireOnce)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1FireWeaponCollideModuleData),FireWeaponCollideContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt collision payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: FireWeaponCollide: PASS ({checks} five-stock/all-flags/filter/optional-stance/timer/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
