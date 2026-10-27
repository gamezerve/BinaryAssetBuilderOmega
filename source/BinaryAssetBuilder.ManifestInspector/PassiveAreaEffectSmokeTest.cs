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

// Reborn: cover original passive effect records, schema defaults, filters and optional enum pointers and full object composition without editing user sources.
internal static class PassiveAreaEffectSmokeTest
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
            var grammar=PassiveAreaEffectContract.Qualify(manifest,baseline,schemas,catalog);int checks=0;var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,catalog);
            // Reborn: three original Engineer records and fully authored/default/expanded lists exercise EP1 rather than KW semantics.
            var originals=new[]{"EP1/Soviet/Units_Campaign/SovietChiefEngineer.xml","Allied/Units/AlliedEngineer.xml","Neutral/Structures/HospitalTechStructure.xml"}.SelectMany(PassiveAreaEffectContract.Originals).ToArray();
            foreach(int variant in Enumerable.Range(0,6)){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();var owner=(XmlElement)source.ImportNode(originals[Math.Min(variant,2)],true);behaviors.AppendChild(owner);
                // Reborn: exercise every own field, the SHRINK bit, typed attachment pointee, strong imports and both filter/status records.
                if(variant==3){
                    owner.RemoveAll();owner.SetAttribute("id","RebornPassiveHeal");
                    foreach(var pair in new[]{("EffectRadius","-1"),("MaxBeneficiaries","5"),("HealingPercentPerSecond","5%"),("HealingPointsPerSecond","3.5"),("SurveyDelayFrames","7"),("UpgradeRequired","Upgrade_AlliedTech2"),("AntiCategoryMask","BUFF SHRINK"),("AntiFX",catalog.First(a=>a.TypeName=="FXList").Name),("HealFX",catalog.Last(a=>a.TypeName=="FXList").Name),("AttachModuleId","ModuleTag_AI"),("DisabledTypesToProcess","HELD EMP")})owner.SetAttribute(pair.Item1,pair.Item2);
                    foreach(string flag in new[]{"NonStackable","AffectAttached","AffectWoundedOnly","AffectUnderAttack","AffectContained","ShowDebugCircle"})owner.SetAttribute(flag,"true");
                    owner.InnerXml=string.Concat(catalog.Where(a=>a.TypeName=="AttributeModifier").Take(2).Select(a=>"<Modifier xmlns='uri:ea.com:eala:asset'>"+a.Name+"</Modifier>"))+"<AllowFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Relationship='ALLIES' Include='INFANTRY'><ExcludeThing>SovietScoutVehicle</ExcludeThing></AllowFilter><ContainerFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='VEHICLE STRUCTURE' Relationship='ALLIES' StatusBitFlags='CONTAINER_OCCUPIED'/><ObjectStatusValidation xmlns='uri:ea.com:eala:asset' RequiredStatus='GENERIC_TOGGLE_STATE' ForbiddenStatus='WEAPON_UPGRADED_01'/>";
                }
                // Reborn: all optional records may be omitted, and modifier cardinality is source-driven rather than a captured sample count.
                if(variant==4){owner.RemoveAll();owner.SetAttribute("id","RebornPassiveDefaults");}
                if(variant==5){owner.RemoveAll();owner.SetAttribute("id","RebornPassiveModifiers");owner.InnerXml=string.Concat(Enumerable.Repeat("<Modifier xmlns='uri:ea.com:eala:asset'>"+catalog.First(a=>a.TypeName=="AttributeModifier").Name+"</Modifier>",8));}
                var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)).GetElementsByTagName("PassiveAreaEffectBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();
                var native=ConscriptBehaviorSurvey.Compile(normalized,PassiveAreaEffectContract.TypeId);var element=resolved.GetElementsByTagName("PassiveAreaEffectBehavior",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
                var proof=SchemaNativeContract.Verify(element,typeof(Ep1PassiveAreaEffectBehaviorModuleData),PassiveAreaEffectContract.TypeId,native,references);var metadata=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,PassiveAreaEffectContract.Wire,typeof(Ep1PassiveAreaEffectBehaviorModuleData));_=grammar.Verify(metadata,native,typeof(Ep1PassiveAreaEffectBehaviorModuleData),proof.Records);checks++;
                // Reborn: authored passive effect variants also traverse the complete production GameObject allocator and metadata composer.
                if(variant is 0 or 3){var full=GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true);_=verifier.Verify(source,full,instance,plan.Origins);checks++;}
                // Reborn: changing the emitted healing percentage must be rejected by independent source/native validation.
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};corrupt.InstanceBuffer[Marshal.OffsetOf<Ep1PassiveAreaEffectBehaviorModuleData>(nameof(Ep1PassiveAreaEffectBehaviorModuleData.HealingPercentPerSecond)).ToInt32()]^=1;
                try{SchemaNativeContract.Verify(element,typeof(Ep1PassiveAreaEffectBehaviorModuleData),PassiveAreaEffectContract.TypeId,corrupt,references);throw new InvalidOperationException("Corrupt passive effect payload accepted.");}catch(InvalidDataException){checks++;}
            }
            // Reborn: invalid schema values remain errors after replacing the former unsupported-family gate.
            foreach(string invalid in new[]{"AffectContained='unknown'","UnknownField='1'"}){
                var source=(XmlDocument)baseline.CloneNode(true);source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().InnerXml="<PassiveAreaEffectBehavior xmlns='uri:ea.com:eala:asset' id='Invalid' "+invalid+"/>";
                try{_=SchemaNativeContract.Resolve(source,schemas,catalog);throw new InvalidOperationException("Invalid passive effect source accepted.");}catch(InvalidDataException){checks++;}
            }
            Console.WriteLine($"Reborn: PassiveAreaEffectBehavior: PASS ({checks} three-stock/all-fields/attachment/two-filters/status/empty-and-eight-modifiers/strong-import/composition/corruption checks; original native/metadata witnesses; no user output or game launch).");
        }finally{Settings.Current=saved;}
    }
}
