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

// Reborn: cover teleport-storage stock identities and authored storage fields, inherited references and local module IDs and full object composition without editing user sources.
internal static class TeleportStoredObjectsSmokeTest
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
            var grammar=TeleportStoredObjectsContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var originals=GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","TeleportStoredObjectsSpecialPower");var owner=(XmlElement)source.ImportNode(originals[variant<4?variant:0],true);behaviors.AppendChild(owner);
                // Reborn: edit the flag, pair ID and OCL together, including a new local identity and inherited power reference.
                if(variant==4){owner.SetAttribute("id","RebornTeleportStored");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerChronoRiftTeleportLvl1");owner.SetAttribute("GetObjectsCurrentlyAtStorePosition","true");owner.SetAttribute("StoreObjectsLinkID","42");owner.SetAttribute("OCL",catalog.First(a=>a.Name.StartsWith("ObjectCreationList:",StringComparison.Ordinal)&&a.Name!="ObjectCreationList:OCL_Generic5SecondBeacon").Name["ObjectCreationList:".Length..]);}
                if(variant==5){foreach(var field in new[]{"GetObjectsCurrentlyAtStorePosition","StoreObjectsLinkID","OCL"})owner.RemoveAttribute(field);}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("TeleportStoredObjectsSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,TeleportStoredObjectsContract.TypeId);var element=resolved.GetElementsByTagName("TeleportStoredObjectsSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(TeleportStoredObjectsSpecialPowerModuleData),TeleportStoredObjectsContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,TeleportStoredObjectsContract.Wire,typeof(TeleportStoredObjectsSpecialPowerModuleData));_=grammar.Verify(metadata,native,typeof(TeleportStoredObjectsSpecialPowerModuleData),proof.Records);checks++;
                // Reborn: one authored teleport-storage variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted pair ID must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[System.Runtime.InteropServices.Marshal.SizeOf<SpecialPowerModuleData>()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(TeleportStoredObjectsSpecialPowerModuleData),TeleportStoredObjectsContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt teleport-storage payload accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: TeleportStoredObjectsSpecialPower: PASS ({checks} four originals/all teleport fields/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
