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

// Reborn: cover OCL-power stock identities and authored storage fields, inherited references and local module IDs and full object composition without editing user sources.
internal static class OclSpecialPowerSmokeTest
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
            var grammar=OclSpecialPowerContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var all=GameObjectExtendedRootContract.Original("System/PlayerSpellBook.xml","OCLSpecialPower");var originals=new[]{all.First(e=>e.GetAttribute("id")=="ModuleTag_PrecisionStrike"),all.First(e=>e.GetAttribute("id")=="ModuleTag_SatelliteSweep"),all.First(e=>e.GetAttribute("id")=="ModuleTag_FinalSquadronL1"),all.Last()};var owner=(XmlElement)source.ImportNode(originals[variant<4?variant:0],true);behaviors.AppendChild(owner);
                // Reborn: cover every EP1-only field and populated lists, including two four-byte UpgradeOCL items without the obsolete Science word.
                if(variant==4){owner.SetAttribute("id","RebornOclPower");owner.SetAttribute("OCL","OCL_PrecisionStrike");owner.SetAttribute("DestinationOCL","OCL_PrecisionStrikeBeacon");owner.SetAttribute("CreateLocation","CREATE_AT_OFFSET_FROM_TARGET_AND_MOVE_TO_TARGET");owner.SetAttribute("CreateLocationOffset","1375.5");owner.SetAttribute("MaxCreateCount","7");owner.SetAttribute("NumberToSpawn","3");owner.SetAttribute("RegisterObjectsWithSpecialAbilityUpdate","true");string upgrade=catalog.First(a=>a.Name.StartsWith("UpgradeTemplate:",StringComparison.Ordinal)).Name["UpgradeTemplate:".Length..];owner.InnerXml="<UpgradeOCL xmlns='uri:ea.com:eala:asset' OCL='OCL_PrecisionStrike'/><UpgradeOCL xmlns='uri:ea.com:eala:asset' OCL='OCL_SatelliteSweep'/><NearestSecondaryObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Relationship='SAME_PLAYER' Include='STRUCTURE'><IncludeThing>AlliedConstructionYard</IncludeThing></NearestSecondaryObjectFilter><Upgrade xmlns='uri:ea.com:eala:asset'>"+upgrade+"</Upgrade>";}
                if(variant==5){while(owner.Attributes.Count>0)owner.RemoveAttributeNode(owner.Attributes[0]!);owner.SetAttribute("id","RebornOclPowerDefaults");owner.SetAttribute("SpecialPowerTemplate","SpecialPowerPrecisionStrike");owner.InnerXml="";}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("OCLSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,OclSpecialPowerContract.TypeId);var element=resolved.GetElementsByTagName("OCLSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1OCLSpecialPowerModuleData),OclSpecialPowerContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,OclSpecialPowerContract.Wire,typeof(Ep1OCLSpecialPowerModuleData));_=grammar.Verify(metadata,native,typeof(Ep1OCLSpecialPowerModuleData),proof.Records);checks++;
                // Reborn: one authored OCL-power variant also traverses the complete production GameObject allocator and metadata composer.
                if(variant==4){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted location offset must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[484]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1OCLSpecialPowerModuleData),OclSpecialPowerContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt OCL-power payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: prove every official location ordinal directly, including the inserted path-line and three offset modes.
            var locationSchema=(XmlSchemaSimpleType)official.GlobalTypes[new XmlQualifiedName("OCLCreateLocType",SchemaSet.XmlNamespace)]!;var locations=((XmlSchemaSimpleTypeRestriction)locationSchema.Content!).Facets.OfType<XmlSchemaEnumerationFacet>().Select(f=>f.Value!).ToArray();
            for(int ordinal=0;ordinal<locations.Length;ordinal++){string location=locations[ordinal];
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.InnerXml="<OCLSpecialPower xmlns='uri:ea.com:eala:asset' id='RebornLocationOrdinal' SpecialPowerTemplate='SpecialPowerPrecisionStrike' CreateLocation='"+location+"'/>";
                var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("OCLSpecialPower",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var native=ConscriptBehaviorSurvey.Compile(normalized,OclSpecialPowerContract.TypeId);
                if(System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(native.InstanceBuffer.AsSpan(480))!=ordinal)throw new InvalidDataException("EP1 OCL location ordinal differs.");checks++;
            }
            Console.WriteLine($"Reborn: OCLSpecialPower: PASS ({checks} four originals/all attributes/upgrade lists/filter/default/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
