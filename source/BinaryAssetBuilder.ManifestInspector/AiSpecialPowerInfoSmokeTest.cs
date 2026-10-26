using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using SageBinaryData;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: cover the complete EP1 command AI record against retail sources and schema-driven custom graphs, without modifying user mods.
internal static class AiSpecialPowerInfoSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: qualify original command bytes, compile every retail AI shape, exercise all authored fields and publish a private linked command graph. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=Path.GetFullPath(output);if(Directory.Exists(output))throw new InvalidDataException("Fresh AI command evidence directory required.");Directory.CreateDirectory(output);var saved=Settings.Current;
        try{
            string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;var official=ConscriptSourceInput.Schemas();
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            if(Marshal.SizeOf<Ep1AiSpecialPowerInfo>()!=188||Marshal.OffsetOf<Ep1AiSpecialPowerInfo>("RequiredForAttackObjectStatus").ToInt32()!=164||Marshal.OffsetOf<Ep1AiSpecialPowerInfo>("DontUseIfOutRangeTarget").ToInt32()!=185)throw new InvalidDataException("EP1 AI power ABI differs.");
            if(SdkBuildTests.Enabled)LogicCommandCompiler.Qualify(manifest,schemas,catalog);int checks=0;var stockNames=catalog.Select(a=>a.Name).ToHashSet();
            var originals=LogicCommandCompiler.Originals().Where(e=>e.ChildNodes.OfType<XmlElement>().Any(c=>c.LocalName=="AISpecialPowerInfo")&&stockNames.Contains("LogicCommand:"+e.GetAttribute("id"))).ToArray();
            foreach(var original in originals)Check(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(original.OuterXml)),catalog,false);
            Console.WriteLine($"Reborn: AISpecialPowerInfo source/native: {originals.Length} original command graphs passed.");
            var rich=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(originals.First(e=>e.GetAttribute("id")=="Command_ActivateCryoLeapSpecialPower").OuterXml));rich.DocumentElement!.SetAttribute("id","RebornFullAiPowerCommand");var ai=rich.DocumentElement.ChildNodes.OfType<XmlElement>().Single(e=>e.LocalName=="AISpecialPowerInfo");ai.RemoveAll();ai.SetAttribute("id","RebornAiMetadataOnly");
            // Reborn: supply every official AI attribute, including optional false/zero values and both EP1 status masks.
            var type=(XmlSchemaComplexType)official.GlobalTypes[new XmlQualifiedName("AISpecialPowerInfo",SchemaSet.XmlNamespace)]!;
            foreach(XmlSchemaAttribute attribute in type.AttributeUses.Values){string name=attribute.QualifiedName.Name;if(name=="id")continue;string? value=attribute.DefaultValue;value=name switch{"Heuristic"=>"PLAYER_AOE_ATTACK","Manager"=>"SKIRMISH_AI","TargetObjectInclude"=>"VEHICLE INFANTRY","TargetObjectExclude"=>"AIRCRAFT","UseOnlyWhenPowerAdvantageIsAbove"=>"1.25","NextLogicCommand"=>"Command_ActivateCryoLeapSpecialPower","UpdateObject"=>"SovietAntiInfantryInfantry","UseInMicroManagerState"=>"RETREAT COMBAT","MaxUpdatables"=>"7","UseToCounterGarrison"=>"false","RequiredForAttackObjectStatus"=>"UNDER_FROZEN","ForbiddenForAttackObjectStatus"=>"STEALTHED","Distance"=>"177","HealthFraction"=>"0.25","SearchRadius"=>"211","Location"=>"Land","DontUseIfOutRangeTarget"=>"false",_=>value};if(value==null)throw new InvalidDataException("Unhandled AI test attribute: "+name);ai.SetAttribute(name,value);}
            ai.InnerXml="<ObjectFilter xmlns='uri:ea.com:eala:asset' Rule='ANY' Include='VEHICLE INFANTRY' Exclude='AIRCRAFT'><IncludeThing>SovietAntiInfantryInfantry</IncludeThing></ObjectFilter><SpecialPowerWeapon xmlns='uri:ea.com:eala:asset' Weapon='SovietAntiInfantryInfantryRifle' TotalValueGreaterThan='250' AoeAttackPowerLocationPreference='PREFER_MAXIMUM_DAMAGE' AoeAttackPowerBlockAreaAfterActivationTime='2s' AoeAttackEmanationLocation='EMANATE_FROM_TARGET' ToggleRangeHysteresis='7' IsDisablingAttack='true' AffectsFriendlies='true' AffectsSelf='true' ToggleShortRangeTargetForcesTarget='true'/>";
            // Reborn: select an existing weapon by catalog identity rather than assuming a historical AK-47 asset spelling.
            ai.GetElementsByTagName("SpecialPowerWeapon",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().SetAttribute("Weapon",catalog.First(a=>a.Name.StartsWith("WeaponTemplate:",StringComparison.Ordinal)).Name["WeaponTemplate:".Length..]);
            Check(rich,catalog,true);
            var defaults=(XmlDocument)rich.CloneNode(true);var defaultAi=defaults.GetElementsByTagName("AISpecialPowerInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().Single();defaultAi.RemoveAll();defaultAi.SetAttribute("Heuristic","DUMB_FIRE");defaultAi.SetAttribute("Manager","BUILDER");Check(defaults,catalog,true);
            // Reborn: native corruption is checked against current XML, not normalized back to the stock template.
            var invalid=(XmlDocument)defaults.CloneNode(true);invalid.GetElementsByTagName("AISpecialPowerInfo",SchemaSet.XmlNamespace).OfType<XmlElement>().Single().SetAttribute("Heuristic","InvalidHeuristic");try{LogicCommandCompiler.ProofSource(invalid,schemas,catalog);throw new InvalidOperationException("Invalid AI heuristic accepted.");}catch(InvalidDataException){checks++;}
            string entry=Path.Combine(output,"Static_Mod.xml");File.WriteAllText(entry,"<!-- Reborn: private AI command integration. --><AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+rich.DocumentElement.OuterXml+"<LogicCommandSet id='RebornAiCommandSet'><Cmd>RebornFullAiPowerCommand</Cmd></LogicCommandSet></AssetDeclaration>");string prepared=Path.Combine(output,"prepared");ConscriptSourceInput.Prepare(entry,prepared);if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,Path.Combine(output,"built")))throw new InvalidDataException("AI command mixed route not selected.");checks++;
            Console.WriteLine($"Reborn: AISpecialPowerInfo: PASS ({originals.Length} retail AI shapes, all official attributes, both nested graphs, optional/default values, {checks} source/native/corruption/schema/package controls; no user output or game launch).");

            //-------------------------------------------------------------------------------------------------
            /** Reborn: prove every current native byte and reference occurrence, optionally refusing altered AI scalars and an incomplete relocation inventory. */
            //-------------------------------------------------------------------------------------------------
            void Check(XmlDocument source,ManifestAsset[] available,bool corrupt)
            {
                LogicCommandCompiler.Admit(source);var proof=LogicCommandCompiler.ProofSource(source,schemas,available);var instance=ReferencePipelineSmokeTest.Normalize(schemas,SchemaNativeContract.Resolve(source,schemas,available).OuterXml);var native=LogicCommandCompiler.Compile(instance.XmlNode);var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1LogicCommand),0,native,references);checks++;
                if(!corrupt)return;int aiAt=BinaryPrimitives.ReadInt32LittleEndian(native.InstanceBuffer.AsSpan(24));var bad=new Chunk{InstanceBuffer=native.InstanceBuffer.ToArray(),RelocationBuffer=native.RelocationBuffer,ImportsBuffer=native.ImportsBuffer};bad.InstanceBuffer[aiAt+8]^=1;try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1LogicCommand),0,bad,references);throw new InvalidOperationException("Corrupt AI distance accepted.");}catch(InvalidDataException){checks++;}
                bad=new Chunk{InstanceBuffer=native.InstanceBuffer,RelocationBuffer=native.RelocationBuffer.Skip(4).ToArray(),ImportsBuffer=native.ImportsBuffer};try{SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1LogicCommand),0,bad,references);throw new InvalidOperationException("Incomplete AI relocation inventory accepted.");}catch(InvalidDataException){checks++;}
            }
        }finally{Settings.Current=saved;}
    }
}
