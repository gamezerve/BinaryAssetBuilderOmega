using System.Reflection;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Core.SageXml;
using Chunk=Relo.Chunk;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: Studio variable Behaviors retain complete native/metadata/source/dependency proof before the ordinary package publisher sees output.
internal static class ConscriptVariableBehaviorBuild
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: leave established fixed builds untouched and explicitly compile only the newly admitted variable reviewed behavior profile. */
    //-------------------------------------------------------------------------------------------------
    internal static bool TryRun(string entry,string manifest,string globalManifest,string output)
    {
        entry=SdkEnvironmentPreflight.Absolute(entry);var source=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(entry,1048576),entry);
        string fixtureRoot=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        var baseline=SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(Path.Combine(fixtureRoot,"live-alpha","ConscriptBuildTime1","data","mod.xml"),1048576));baseline.DocumentElement!.SetAttribute("BuildTime","4");
        var official=ConscriptSourceInput.Schemas();if(ConscriptSourceInput.FixedAccepted(source,baseline,official))return false;
        var plan=ConscriptVariableBehaviorSource.Admit(source,baseline,official);
        output=SdkEnvironmentPreflight.Absolute(output);SdkEnvironmentPreflight.CheckPath(output);if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh variable behavior output required.");
        var saved=Settings.Current;
        try
        {
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtureRoot,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);schemas.Schemas.Add(official);schemas.Schemas.Compile();typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var catalogs=new[]{manifest,globalManifest}.Select(path=>ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576))).ToArray();foreach(var catalog in catalogs){TypeRegistryAudit.ValidateTarget(catalog.Header.Version,catalog.Header.AllTypesHash);if(!catalog.Header.IsLinked||catalog.Validate().Count!=0)throw new InvalidDataException("Variable build requires valid EP1 stock catalogs.");}
            var assets=catalogs.SelectMany(c=>c.Assets).ToArray();string id=source.DocumentElement!.GetAttribute("id");if(id!="SovietAntiInfantryInfantry"&&assets.Any(a=>a.TypeId==0x942FFF2D&&a.InstanceId==InstanceHandle.GetInstanceId(id)))throw new InvalidDataException("Custom object identity collides with a stock GameObject.");
            var expected=ConscriptReferenceTargets.Read(source,schemas,assets);var verifier=new ConscriptVariableGraphVerifier(official,manifest,baseline,schemas,assets);
            var pair=Build();var tokens=verifier.Verify(source,pair.Native,pair.Instance,plan.Origins);var repeated=Build();var repeatedTokens=verifier.Verify(source,repeated.Native,repeated.Instance,plan.Origins);
            if(!Equal(pair.Native,repeated.Native)||!Equal(tokens,repeatedTokens))throw new InvalidDataException("Repeated variable behavior output differs.");
            int prefix=tokens.InstanceBuffer.Length-pair.Native.InstanceBuffer.Length;var graph=new ConscriptGraphQualification(pair.Instance,tokens,prefix);
            var streams=GameObjectOriginalStreamSmokeTest.Run(tokens,pair.Instance,manifest,globalManifest,true,graph);
            string command=Path.Combine(Path.GetDirectoryName(entry)!,"command.xml");if(File.Exists(command)){ConscriptRecruitmentSmokeTest.Qualify(schemas,manifest);streams=ConscriptRecruitmentSmokeTest.Compose(streams,pair.Instance,schemas,manifest,globalManifest,SdkXmlDiagnostics.Parse(SdkEnvironmentPreflight.Read(command,1048576),command),graph);}
            Directory.CreateDirectory(output);GameObjectConscriptTrialSmokeTest.Package(Path.Combine(output,id=="SovietAntiInfantryInfantry"?"override":"new-id"),source,streams,id,prefix);
            Console.WriteLine($"Variable behavior source build: PASS ({plan.BehaviorCount} authored behaviors; {expected.Length} stock reference occurrences; prefix={prefix}; complete native/metadata/stream/BIG proof; no automatic command, install or game launch).");return true;

            // Reborn: actual Core resolves every source occurrence before one complete native Tracker; expected target identities are derived separately.
            (Chunk Native,InstanceDeclaration Instance) Build()
            {
                var raw=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(source.DocumentElement!.OuterXml));raw.Schemas.Add(official);raw.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));ConscriptBehaviorSurvey.ResolveStrong(raw,schemas,assets);
                var instance=ReferencePipelineSmokeTest.Normalize(schemas,raw.OuterXml);if(!instance.ReferencedInstances.Select(h=>new AssetId(h.TypeId,h.InstanceId)).SequenceEqual(expected))throw new InvalidDataException("Variable source/Core reference occurrence order differs.");
                return(GameObjectBodyProjectionSmokeTest.Compile(SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(instance.XmlNode.OuterXml)),true,true,true),instance);
            }
        }
        finally{Settings.Current=saved;}
    }

    // Reborn: deterministic repeat comparisons include native payload, relocations and imports, not only output size.
    private static bool Equal(Chunk a,Chunk b)=>a.InstanceBuffer.SequenceEqual(b.InstanceBuffer)&&a.RelocationBuffer.SequenceEqual(b.RelocationBuffer)&&a.ImportsBuffer.SequenceEqual(b.ImportsBuffer);
}
