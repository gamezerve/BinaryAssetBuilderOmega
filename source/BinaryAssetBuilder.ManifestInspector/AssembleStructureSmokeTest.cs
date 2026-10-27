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

// Reborn: cover original assembly records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class AssembleStructureSmokeTest
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
            var grammar=AssembleStructureContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: original, edited, empty-default and expanded weak-reference lists all use the actual production dispatcher.
            foreach(int variant in Enumerable.Range(0,4)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(GameObjectExtendedRootContract.Original("Soviet/Units/SovietEngineer.xml","AssembleStructureSpecialAbilityUpdate").Single(),true);behaviors.AppendChild(owner);
                // Reborn: assembly timing, model condition, busy state, inherited range and template identities are independently authored.
                if(variant==1){owner.SetAttribute("id","RebornAssemble");owner.SetAttribute("NewObjectAssemblyTime","0.25s");owner.SetAttribute("BusyDuringAssembly","false");owner.SetAttribute("ModelConditionsOnAssembly","BUILD_VARIATION_TWO");owner.SetAttribute("StartAbilityRange","75");owner.InnerXml="<StructureTemplate xmlns='uri:ea.com:eala:asset'>SovietBunker</StructureTemplate><StructureTemplate xmlns='uri:ea.com:eala:asset'>SovietPowerPlant</StructureTemplate>";}
                // Reborn: zero structures are legal in the official XSD; absence preserves 0s/false/INVALID defaults.
                if(variant==2){owner.RemoveAll();owner.SetAttribute("id","RebornAssembleDefaults");}
                if(variant==3){owner.SetAttribute("id","RebornAssembleMany");owner.RemoveAttribute("NewObjectAssemblyTime");owner.RemoveAttribute("BusyDuringAssembly");owner.RemoveAttribute("ModelConditionsOnAssembly");owner.InnerXml=string.Concat(Enumerable.Repeat("<StructureTemplate xmlns='uri:ea.com:eala:asset'>SovietBunker</StructureTemplate>",8));}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("AssembleStructureSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,AssembleStructureContract.TypeId);var element=resolved.GetElementsByTagName("AssembleStructureSpecialAbilityUpdate",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(AssembleStructureSpecialAbilityUpdateModuleData),AssembleStructureContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,AssembleStructureContract.Wire,typeof(AssembleStructureSpecialAbilityUpdateModuleData));_=grammar.Verify(metadata,native,typeof(AssembleStructureSpecialAbilityUpdateModuleData),proof.Records);checks++;
                // Reborn: authored assembly variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 1){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted assembly time must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<AssembleStructureSpecialAbilityUpdateModuleData>(nameof(AssembleStructureSpecialAbilityUpdateModuleData.NewObjectAssemblyTime)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(AssembleStructureSpecialAbilityUpdateModuleData),AssembleStructureContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt assembly payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: invalid schema values remain errors after replacing the former unsupported-family gate.
            foreach(string invalid in new[]{"BusyDuringAssembly='unknown'","ModelConditionsOnAssembly='UNKNOWN'"}){
                var source=(XmlDocument)baseline.CloneNode(true);source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().InnerXml="<AssembleStructureSpecialAbilityUpdate xmlns='uri:ea.com:eala:asset' id='Invalid' "+invalid+"/>";
                try{_=SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Invalid assembly source accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: AssembleStructureSpecialAbilityUpdate: PASS ({checks} stock/edited/default/empty-and-eight-structure/weak-ID/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
