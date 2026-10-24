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

// Reborn: cover the real reactor explosion plus authored offsets, probability rows, flags and weapon retargeting without editing user sources.
internal static class FireWeaponDeathSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: run complete source/native/import/metadata checks and a corrupt-output refusal through the existing production EP1 dispatcher. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));
            var grammar=FireWeaponDeathContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;
            foreach(int variant in Enumerable.Range(0,3)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Structures/SovietPowerPlantAdvanced.xml","FireWeaponWhenDead").Single(),true);behaviors.AppendChild(owner);
                if(variant==1){owner.SetAttribute("id","RebornDeathWeaponVariant");owner.SetAttribute("DeathWeapon","CatalystBuildingDeathWeapon");owner.SetAttribute("DelayTime","1.5s");owner.SetAttribute("FireAtGround","true");owner.SetAttribute("UseMyKillerAsWeaponSource","true");owner.SetAttribute("FocusFireKindOfFlag","STRUCTURE");
                    var offset=source.CreateElement("WeaponOffset",SchemaSet.XmlNamespace);offset.SetAttribute("x","12.5");offset.SetAttribute("y","-3");owner.InsertBefore(offset,owner.FirstChild);
                    foreach(string chance in new[]{"20","75"}){var row=source.CreateElement("WeaponFireProbability",SchemaSet.XmlNamespace);row.SetAttribute("DeathType","ALL");row.SetAttribute("ChancePercentage",chance);owner.AppendChild(row);}}
                if(variant==2){owner.SetAttribute("id","RebornDeathWeaponDefaults");foreach(string field in new[]{"InitiallyActive","DeathWeapon","ContinueToBezierDestination"})owner.RemoveAttribute(field);var mux=owner.GetElementsByTagName("DieMuxData",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();mux.RemoveAll();}
                _=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("FireWeaponWhenDead",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,FireWeaponDeathContract.TypeId);var element=resolved.GetElementsByTagName("FireWeaponWhenDead",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData),FireWeaponDeathContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,FireWeaponDeathContract.Wire,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData),proof.Records);checks++;
                // Reborn: independently checking the authored weapon selector catches an incorrect import as well as missing fields.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1FireWeaponWhenDeadBehaviorModuleData>(nameof(Ep1FireWeaponWhenDeadBehaviorModuleData.DeathWeapon)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1FireWeaponWhenDeadBehaviorModuleData),FireWeaponDeathContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt death-weapon payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: FireWeaponWhenDead: PASS ({checks} reactor/edit/default/optional/probability/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
