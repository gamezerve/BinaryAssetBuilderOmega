using System.Buffers.Binary;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Chunk=Relo.Chunk;
using SageBinaryData;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: upgrade-only and mixed authored graphs prove new IDs, overrides, imports, defaults and real frontend inheritance without live SDK source edits.
internal static class UpgradeTemplateSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: build fresh private fixtures through the same preparation/package backend, then independently reject native/import/relocation corruption. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string manifest,string global,string output)
    {
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh upgrade test output required.");Directory.CreateDirectory(output);
        var saved=Settings.Current;
        try{
            string fixture=ReferencePipelineSmokeTest.FindFixture();Settings.Current=new Settings{SchemaPath=fixture,BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalog=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();UpgradeTemplateCompiler.Qualify(global,schemas,catalog);
            string audio=catalog.First(a=>a.TypeName=="AudioEvent").Name,eva=catalog.First(a=>a.TypeName=="EvaEvent").Name,fx=catalog.First(a=>a.TypeName=="FXList").Name;
            var rich=Xml("<UpgradeTemplate xmlns='uri:ea.com:eala:asset' id='RebornUpgradeRich' Type='OBJECT' BuildTime='1.25s' BuildCost='250' Options='OBJECT_UPGRADE_PROJECTED' GroupOrder='4294967295' GroupName='RebornGroup' SkirmishAIHeuristic='IGNORE' IconImage='RebornIcon' WaypointQueueable='true' UseObjectTemplateForCostDiscount='SovietPowerPlant'><GameDependency ForbiddenModelConditions='UNPACKING'><RequiredObject>SovietRefinery</RequiredObject><NeededUpgrade>Upgrade_AlliesFaction</NeededUpgrade></GameDependency></UpgradeTemplate>");
            foreach(string field in new[]{"DisplayName","Description","TypeDescription","AcquireHint"})rich.DocumentElement!.SetAttribute(field,"Reborn:"+field);
            foreach(string field in new[]{"ResearchSound","UnitSpecificSound"})rich.DocumentElement!.SetAttribute(field,audio);rich.DocumentElement!.SetAttribute("UpgradeFX",fx);
            foreach(string field in new[]{"ResearchCompleteEvaEvent","LocalPlayerGainsUpgradeEvaEvent","AlliedPlayerGainsUpgradeEvaEvent","EnemyPlayerGainsUpgradeEvaEvent","LocalPlayerLosesUpgradeEvaEvent","AlliedPlayerLosesUpgradeEvaEvent","EnemyPlayerLosesUpgradeEvaEvent","LocalPlayerProductionStartedEvaEvent","LocalPlayerBuildOnHoldEvaEvent","LocalPlayerBuildCancelledEvaEvent"})rich.DocumentElement.SetAttribute(field,eva);
            var resolved=SchemaNativeContract.Resolve(rich,schemas,catalog);var instance=ReferencePipelineSmokeTest.Normalize(schemas,resolved.OuterXml);var native=UpgradeTemplateCompiler.Compile(instance.XmlNode);var proof=UpgradeTemplateCompiler.ProofSource(rich,schemas,catalog);var references=instance.ReferencedInstances.Select(r=>new AssetId(r.TypeId,r.InstanceId)).ToArray();
            SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1UpgradeTemplate),0,native,references);var repeat=UpgradeTemplateCompiler.Compile(instance.XmlNode);if(!native.InstanceBuffer.SequenceEqual(repeat.InstanceBuffer)||!native.RelocationBuffer.SequenceEqual(repeat.RelocationBuffer)||!native.ImportsBuffer.SequenceEqual(repeat.ImportsBuffer))throw new InvalidDataException("Upgrade native output is nondeterministic.");
            int faults=0;
            foreach(var field in typeof(Ep1UpgradeTemplate).GetFields()){
                // Reborn: duplicate references can have equivalent occurrence indices; corrupt them out of range, not to another valid occurrence of the same asset.
                var bad=Copy(native);int at=Marshal.OffsetOf<Ep1UpgradeTemplate>(field.Name).ToInt32();bool reference=field.FieldType.IsGenericType&&field.FieldType.GetGenericTypeDefinition().FullName!.StartsWith("Relo.AssetReference`",StringComparison.Ordinal);if(field.FieldType.IsPointer||reference)BinaryPrimitives.WriteUInt32LittleEndian(bad.InstanceBuffer.AsSpan(at),uint.MaxValue);else bad.InstanceBuffer[at]^=1;
                try{Reject(()=>SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1UpgradeTemplate),0,bad,references));}catch(InvalidDataException error){throw new InvalidDataException("Upgrade native corruption control failed: "+field.Name,error);}faults++;
            }
            foreach(bool imports in new[]{false,true}){var bad=Copy(native);if(imports)bad.ImportsBuffer=Array.Empty<byte>();else bad.RelocationBuffer=Array.Empty<byte>();Reject(()=>SchemaNativeContract.Verify(proof.DocumentElement!,typeof(Ep1UpgradeTemplate),0,bad,references));faults++;}
            var cases=new List<object>();
            Build("new-id","<UpgradeTemplate id='RebornNewUpgrade' Type='OBJECT'/>");
            Build("override","<UpgradeTemplate id='Upgrade_AlliesFaction' Type='PLAYER' Options='DO_NOT_TRANSFER'/>");
            Build("rich",rich.DocumentElement.OuterXml);
            string inherited=Path.Combine(output,"inherited");Directory.CreateDirectory(inherited);File.WriteAllText(Path.Combine(inherited,"Base.xml"),Envelope("<UpgradeTemplate id='RebornUpgradeBase' Type='OBJECT' BuildCost='200' Options='OBJECT_UPGRADE_PROJECTED'><GameDependency><RequiredObject>SovietRefinery</RequiredObject></GameDependency></UpgradeTemplate>"));
            Build("inherited","<Includes><Include type='instance' source='Base.xml'/></Includes><Defines><Define name='Cost' value='350'/></Defines><UpgradeTemplate id='RebornInheritedUpgrade' inheritFrom='RebornUpgradeBase' BuildCost='=$Cost' BuildTime='2s'/>");
            // Reborn: mixed GameObjects refer to an explicitly authored new upgrade; no trigger creates an asset by itself.
            var game=Xml(File.ReadAllText(Path.Combine(Path.GetDirectoryName(fixture)!,"live-alpha/ConscriptBuildTime1/data/mod.xml")));game.GetElementsByTagName("TriggeredBy",SchemaSet.XmlNamespace)[0]!.InnerText="RebornNewUpgrade";
            Build("mixed",game.DocumentElement!.OuterXml+"<UpgradeTemplate id='RebornNewUpgrade' Type='OBJECT'/>");
            File.WriteAllText(Path.Combine(output,"upgrade-report.json"),JsonSerializer.Serialize(new{Passed=true,Cases=cases,NativeHeader=Marshal.SizeOf<Ep1UpgradeTemplate>(),NativeFaultsRejected=faults,GameExecuted=false,Published=false},new JsonSerializerOptions{WriteIndented=true}));
            Console.WriteLine("Reborn: UpgradeTemplate: PASS (5 complete source/stream/BIG cases; new ID, override, full fields/imports, instance inheritance/definition and mixed object; "+faults+" native controls; no publication/game).");

            // Reborn: one fresh source graph runs through real frontend output and verifies primary identity classification from the package receipt.
            void Build(string name,string body){string directory=Path.Combine(output,name);Directory.CreateDirectory(directory);string entry=Path.Combine(directory,"Static_Mod.xml");File.WriteAllText(entry,Envelope(body));if(!ConscriptSourceInput.Detect(entry))throw new InvalidDataException("Upgrade frontend routing failed.");string prepared=Path.Combine(directory,"prepared");ConscriptSourceInput.Prepare(entry,prepared);string package=Path.Combine(directory,"build");if(!ConscriptMultiAssetBuild.TryRun(Path.Combine(prepared,"game.xml"),manifest,global,package))throw new InvalidDataException("Upgrade backend routing failed.");string kind=name=="mixed"?"override":"support";using var receipt=JsonDocument.Parse(File.ReadAllText(Path.Combine(package,kind,"trial-plan.json")));if(!receipt.RootElement.GetProperty("OfflineValidated").GetBoolean())throw new InvalidDataException("Upgrade package receipt not validated.");if(name is "new-id" or "inherited" or "rich"&& !receipt.RootElement.GetProperty("NewIdentity").GetBoolean())throw new InvalidDataException("New upgrade misclassified as override.");if(name=="override"&&receipt.RootElement.GetProperty("NewIdentity").GetBoolean())throw new InvalidDataException("Upgrade override misclassified as new.");cases.Add(new{Name=name,AssetCount=receipt.RootElement.GetProperty("AssetCount").GetInt32(),ArchiveSha256=receipt.RootElement.GetProperty("ArchiveSha256").GetString()});Console.WriteLine("Reborn: upgrade package "+name+": PASS.");}
        }finally{Settings.Current=saved;}
    }

    // Reborn: fixtures use the official envelope and namespace; their formatting/attribute order carries no native identity authority.
    private static string Envelope(string body)=>"<!-- Reborn: private upgrade regression fixture. --><AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>";
    // Reborn: owned copies keep negative controls independent of the passing native allocation.
    private static Chunk Copy(Chunk data)=>new(){InstanceBuffer=data.InstanceBuffer.ToArray(),RelocationBuffer=data.RelocationBuffer.ToArray(),ImportsBuffer=data.ImportsBuffer.ToArray()};
    // Reborn: bad native bytes must fail independent checking rather than reuse a successful producer result.
    private static void Reject(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Upgrade corruption accepted.");}
    // Reborn: reuse the existing no-DTD XML parser for all private fixture documents.
    private static XmlDocument Xml(string text)=>SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(text));
}
