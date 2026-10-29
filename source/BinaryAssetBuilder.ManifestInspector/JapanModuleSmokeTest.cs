using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.XmlCompiler;
using SageBinaryData;
using Chunk=Relo.Chunk;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: Japan behavior, hierarchy, turret vision and Egg-expression controls use original EP1 records without mod edits or game launches.
internal static class JapanModuleSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify native stock records, compile actual source leaves, check metadata and reject corrupt generated graphs. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global)
    {
        using var names=GameObjectModuleReview.ReuseNameCatalog();var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global,WorldBuilderAssetResolver.CatalogPath()}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).GroupBy(a=>new AssetId(a.TypeId,a.InstanceId)).Select(g=>g.First()).ToArray();
            var baseline=SdkXmlDiagnostics.Parse(File.ReadAllBytes(Path.Combine(fixtures,"live-alpha/ConscriptBuildTime1/data/mod.xml")));var grammar=JapanModuleContract.Qualify(manifest,baseline,schemas,catalog);int checks=0,refusals=0;
            foreach(var family in JapanModuleContract.Families){Probe(JapanModuleContract.Originals(family.Path,family.Source,family.Owner).First(),family.Type,FastHash.GetHashCode(family.Wire),family.Wire,grammar);}
            // Reborn: the Sentinel's populated locomotor pointer complements the earlier null Dolphin shape.
            var linearGrammar=AlliedUnitModuleContract.Qualify(manifest,baseline,schemas,catalog);
            Probe(JapanModuleContract.Originals("EP1/Japan/Units/JapanSentinelVehicle.xml","LinearDamageUpdate","JapanSentinelVehicle").First(),typeof(Ep1LinearDamageUpdateModuleData),FastHash.GetHashCode("LinearDamageUpdateModuleData"),"LinearDamageUpdateModuleData",linearGrammar);
            var turretGrammar=WeaponTurretContract.Qualify(manifest,baseline,schemas,catalog);WeaponTurretContract.QualifyHierarchical(turretGrammar,manifest,baseline,schemas,catalog);
            Probe(JapanModuleContract.Originals("Japan/Structures/JapanBaseDefense.xml","WeaponSetUpdate","JapanBaseDefense").Single(),typeof(Ep1WeaponSetUpdateModuleData),WeaponTurretContract.TypeId,WeaponTurretContract.Wire,turretGrammar);
            Probe(JapanModuleContract.Originals("Japan/Units_SinglePlayerCampaign/JapanMechaKing.xml","WeaponSetUpdate","JapanMechaKing").Single(),typeof(Ep1WeaponSetUpdateModuleData),WeaponTurretContract.TypeId,WeaponTurretContract.Wire,turretGrammar);
            // Reborn: production Egg concatenation preserves quoted flag suffixes; earlier literal-only profiles and undefined-symbol refusal remain intact.
            byte[] expression=Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><Defines><Define name='EGG_KINDOFS' value='SELECTABLE VEHICLE'/></Defines><GameObject id='EggProbe' KindOf=\"=$EGG_KINDOFS+' STRUCTURE'\"/></AssetDeclaration>");
            var expanded=SdkLocalDefineProfile.ApplyBeforeInheritance(expression,numericExpressions:true);if(expanded.Bytes==null||SdkXmlDiagnostics.Parse(expanded.Bytes).GetElementsByTagName("GameObject",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().GetAttribute("KindOf")!="SELECTABLE VEHICLE STRUCTURE"||SdkLocalDefineProfile.ApplyBeforeInheritance(expression).Bytes!=null)throw new InvalidDataException("Egg expression profile result differs.");checks++;
            var missing=Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(expression).Replace("=$EGG_KINDOFS","=$UNKNOWN"));if(SdkLocalDefineProfile.ApplyBeforeInheritance(missing,numericExpressions:true).Bytes!=null)throw new InvalidDataException("Undefined Egg concatenation was accepted.");refusals++;
            Console.WriteLine($"Japan modules: PASS ({checks} generated/source-expression controls, {refusals} corrupt/undefined refusals; original behavior, turret vision and six-turret hierarchy qualification; no game launch).");
            //-------------------------------------------------------------------------------------------------
            /** Reborn: source values independently bind every generated byte, allocation and import before metadata is compared to retail grammar. */
            //-------------------------------------------------------------------------------------------------
            void Probe(XmlElement original,Type type,uint wire,string schema,StockMetadataGrammar selected){
                var source=(XmlDocument)baseline.CloneNode(true);var behaviors=source.GetElementsByTagName("Behaviors",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();behaviors.RemoveAll();behaviors.AppendChild(source.ImportNode(original,true));var resolved=SchemaNativeContract.Resolve(source,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var normalized=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml));var native=ConscriptBehaviorSurvey.Compile(normalized.GetElementsByTagName(original.LocalName,SchemaSet.XmlNamespace)[0]!,wire);var element=resolved.GetElementsByTagName(original.LocalName,SchemaSet.XmlNamespace).OfType<XmlElement>().Single();var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();var proof=SchemaNativeContract.Verify(element,type,wire,native,references);var tokens=Ep1BehaviorMetadataTokenizer.Tokenize(native,official,schema,type);_=selected.Verify(tokens,native,type,proof.Records);checks++;
                var corrupt=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer.ToArray(),ImportsBuffer=native.ImportsBuffer.ToArray()};if(corrupt.RelocationBuffer.Length!=0)corrupt.RelocationBuffer=corrupt.RelocationBuffer[4..];else if(corrupt.ImportsBuffer.Length!=0)corrupt.ImportsBuffer=corrupt.ImportsBuffer[4..];else corrupt.InstanceBuffer[8]^=0x40;bool refused=false;try{SchemaNativeContract.Verify(element,type,wire,corrupt,references);}catch(InvalidDataException){refused=true;}if(!refused)throw new InvalidDataException("Corrupt Japan native data was accepted: "+schema);refusals++;
            }
        }finally{Settings.Current=saved;}
    }
}
