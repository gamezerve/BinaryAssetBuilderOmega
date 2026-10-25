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

// Reborn: cover original weapon turret, edited turret lists and nested target chooser settings without editing user sources.
internal static class WeaponTurretSmokeTest
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
            var grammar=WeaponTurretContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietBaseDefenseGround.xml","WeaponSetUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: edit turret timing/angles, add a second turret and change imported weapons without a fixed stock list shape.
                if(variant==1){owner.SetAttribute("id","RebornTurrets");var turret=owner.ChildNodes.OfType<XmlElement>().Single();var settings=turret.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="TurretSettings");settings.SetAttribute("TurretTurnRate","420");settings.SetAttribute("IdleTurnRateModifier","75%");settings.SetAttribute("InvalidIdleScanObjectStatus","UNSELECTABLE");settings.SetAttribute("SetAngleModelStates","true");settings.SetAttribute("ComeToHaltJiggle","2d");settings.SetAttribute("BlindFireDistance","75");var second=(XmlElement)turret.CloneNode(true);second.SetAttribute("ID","2");second.SetAttribute("ForceRotateObjectToMaintainFiringArc","false");second.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="Weapon").SetAttribute("Template","SovietAntiInfantryInfantryAK47");owner.AppendChild(second);}
                // Reborn: an empty hardpoint weapon list and omitted optional chooser retain schema defaults instead of captured values.
                if(variant==2){var turret=owner.ChildNodes.OfType<XmlElement>().Single();foreach(var weapon in turret.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName=="Weapon").ToArray())turret.RemoveChild(weapon);var settings=turret.ChildNodes.OfType<XmlElement>().Single();settings.RemoveAll();}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("WeaponSetUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,WeaponTurretContract.TypeId);var element=resolved.GetElementsByTagName("WeaponSetUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1WeaponSetUpdateModuleData),WeaponTurretContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,WeaponTurretContract.Wire,typeof(Ep1WeaponSetUpdateModuleData));_=grammar.Verify(metadata,native,typeof(Ep1WeaponSetUpdateModuleData),proof.Records);checks++;
                // Reborn: one authored multi-turret variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted disabled-status mask must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[8]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1WeaponSetUpdateModuleData),WeaponTurretContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt weapon turret payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: WeaponSetUpdate: PASS ({checks} original/edit/default/multi-turret/chooser/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
