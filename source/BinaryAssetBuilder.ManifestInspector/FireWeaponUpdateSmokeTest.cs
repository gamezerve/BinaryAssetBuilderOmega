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

// Reborn: cover original firing update records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class FireWeaponUpdateSmokeTest
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
            var grammar=FireWeaponUpdateContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: two retail units and authored/default/expanded nugget graphs exercise the complete EP1 layout.
            var originals=new[]{"Soviet/Units/SovietHeavyAntiVehicleInfantry.xml","Soviet/Units/SovietAntiVehicleVehicleTech2.xml"}.SelectMany(path=>GameObjectExtendedRootContract.Original(path,"FireWeaponUpdate")).ToArray();
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[Math.Min(variant,1)],true);behaviors.AppendChild(owner);
                // Reborn: the shared interval, per-nugget delay/OneShot/offset, weapon targets and status masks vary independently.
                if(variant==2){owner.RemoveAll();owner.SetAttribute("id","RebornFireUpdate");owner.SetAttribute("FireInterval","0.25s");owner.SetAttribute("AliveOnly","true");owner.SetAttribute("ActiveWhenDisabled","HELD EMP");owner.InnerXml="<FireWeaponNugget xmlns='uri:ea.com:eala:asset' WeaponName='SovietAntiInfantryInfantryAK47' FireDelay='0.5s' OneShot='true'><Offset x='1.25' y='-2.5' z='3.75'/></FireWeaponNugget><FireWeaponNugget xmlns='uri:ea.com:eala:asset' WeaponName='SovietHeavyAntiVehicleInfantryShortCircuit' FireDelay='0s'/><ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='GENERIC_TOGGLE_STATE' ForbiddenStatus='WEAPON_UPGRADED_01'/>";}
                // Reborn: empty firing lists and omitted optional fields are legal; authored zero intervals must not become the Tesla sample's 0.13s.
                if(variant>=3){owner.RemoveAll();owner.SetAttribute("id","RebornFireDefaults"+variant);if(variant==4){owner.SetAttribute("FireInterval","0s");owner.InnerXml=string.Concat(Enumerable.Repeat("<FireWeaponNugget xmlns='uri:ea.com:eala:asset' WeaponName='SovietHeavyAntiVehicleInfantryShortCircuit'/>",8));}}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("FireWeaponUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,FireWeaponUpdateContract.TypeId);var element=resolved.GetElementsByTagName("FireWeaponUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1FireWeaponUpdateModuleData),FireWeaponUpdateContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,FireWeaponUpdateContract.Wire,typeof(Ep1FireWeaponUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1FireWeaponUpdateModuleData),proof.Records);checks++;
                // Reborn: authored firing update variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 2){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted root interval must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1FireWeaponUpdateModuleData>(nameof(Ep1FireWeaponUpdateModuleData.FireInterval)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1FireWeaponUpdateModuleData),FireWeaponUpdateContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt firing update payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: invalid schema values remain errors after replacing the former unsupported-family gate.
            foreach(string invalid in new[]{"<FireWeaponUpdate xmlns='uri:ea.com:eala:asset' id='Invalid' AliveOnly='unknown'/>","<FireWeaponUpdate xmlns='uri:ea.com:eala:asset' id='Invalid' HeroModeTrigger='true'/>","<FireWeaponUpdate xmlns='uri:ea.com:eala:asset' id='Invalid'><FireWeaponNugget FireInterval='1s'/></FireWeaponUpdate>"}){
                var source=(XmlDocument)baseline.CloneNode(true);source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().InnerXml=invalid;
                try{_=SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Invalid firing source accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: FireWeaponUpdate: PASS ({checks} two-stock/root-interval/delays/offsets/OneShot/status/empty-and-eight-nuggets/import/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
