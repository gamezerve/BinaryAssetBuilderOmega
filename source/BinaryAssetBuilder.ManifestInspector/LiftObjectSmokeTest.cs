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

// Reborn: cover original lift records, defaults, authored shaders/state filters and complete composition without editing user sources.
internal static class LiftObjectSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete source/native/relocation/metadata checks and a corrupt-output refusal through the existing production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;string? savedDeveloper=System.Environment.GetEnvironmentVariable("REBORN_DEVELOPER_BUILD_TESTS");
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            baseline.DocumentElement!.SetAttribute("BuildTime","4");
            var grammar=LiftObjectContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;
            // Reborn: repeated shared retail comparisons are already covered; exercise production fast composition after qualifying all three originals.
            System.Environment.SetEnvironmentVariable("REBORN_DEVELOPER_BUILD_TESTS","0");var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,5)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();
                var witness=LiftObjectContract.Witnesses[Math.Min(variant,2)];var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original(witness.Path,"LiftObjectUpdate").Single(),true);ConscriptVariableBehaviorSource.DropLeafFormatting(owner);behaviors.AppendChild(owner);
                // Reborn: default and fully edited probes remove original IDs and allocations before authoring independent values.
                if(variant>=3){owner.RemoveAll();owner.SetAttribute("id","RebornLift"+variant);}
                if(variant==4){
                    owner.SetAttribute("LiftObjectLinkID","17");owner.SetAttribute("CrusherModifiesVelocity","true");owner.SetAttribute("LiftVelocity","7.5");owner.SetAttribute("MaxElevationFromGround","300");owner.SetAttribute("TimeIncrement","0.25s");owner.SetAttribute("MaxTimeLifted","12s");owner.SetAttribute("MaxStructureShakeVelocity","0.5");owner.SetAttribute("AirplaneCrashWeapon","JapanCommandoTech1AirplaneCrashWeapon");owner.SetAttribute("SoftLandingWeapon","JapanCommandoSoftLanding");owner.SetAttribute("RotationSpeed","2.5");owner.SetAttribute("Shader","ShaderOverride_ObjectsChronoRift");owner.SetAttribute("ShakeIntensity","0.2");owner.SetAttribute("ShakeRadius","60");owner.SetAttribute("ShakeFade","0.75");owner.SetAttribute("DisabledTypesToProcess","EMP");
                    owner.InnerXml="<ModelStateObjectFilters xmlns='uri:ea.com:eala:asset'><LiftedUnitModelState ModelState='USER_1 USER_9'><ObjectFilter Rule='ANY' Include='VEHICLE INFANTRY' Relationship='ENEMIES'><IncludeThing>AlliedMCV</IncludeThing></ObjectFilter></LiftedUnitModelState><LiftedUnitModelState ModelState='USER_2'/></ModelStateObjectFilters>";
                }
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("LiftObjectUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(LiftObjectContract.Wire));var element=resolved.GetElementsByTagName("LiftObjectUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1LiftObjectUpdateModuleData),BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(LiftObjectContract.Wire),native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,LiftObjectContract.Wire,typeof(Ep1LiftObjectUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1LiftObjectUpdateModuleData),proof.Records);checks++;
                // Reborn: original, default and edited lift variants also traverses the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 3 or 4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted lift velocity must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1LiftObjectUpdateModuleData>(nameof(Ep1LiftObjectUpdateModuleData.LiftVelocity)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1LiftObjectUpdateModuleData),BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(LiftObjectContract.Wire),corrupt,references);throw new InvalidOperationException("Corrupt lift payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: LiftObjectUpdate: PASS ({checks} three-original/default/edit/shader/state-filter/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;System.Environment.SetEnvironmentVariable("REBORN_DEVELOPER_BUILD_TESTS",savedDeveloper);}
    }
}
