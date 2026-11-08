using System.Reflection;
using System.Text.Json;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: independently check every explicit root family even when an unrelated missing include blocks its aggregate source file.
internal static class StockAssetRootQualification
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all original family witnesses with their complete native layouts and retain each failure without publishing a mod. */
    //-------------------------------------------------------------------------------------------------
    internal static int Run(string manifest,string global,string report)
    {
        string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        // Reborn: locate the same SDK resource ancestry without assuming a repository or portable folder depth.
        // Reborn: an explicit original manifest locates the SDK even when this proof runs from a separate source-build executable.
        DirectoryInfo? root=new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(manifest))!);
        while(root!=null&&!Directory.Exists(Path.Combine(root.FullName,"SageXML")))root=root.Parent;
        string sdk=root?.FullName??throw new InvalidDataException("Original SDK root not found.");
        report=Path.GetFullPath(report);
        string work=Path.Combine(sdk,".work")+Path.DirectorySeparatorChar;
        if(!report.StartsWith(work,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Root proof report must remain in the SDK private workspace.");
        var saved=Settings.Current;
        try
        {
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);
            foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
            schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();
            typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var available=new[]{manifest,global}.SelectMany(p=>ManifestReader.Read(SdkEnvironmentPreflight.Read(p,64*1048576)).Assets).ToArray();
            var results=new List<object>();int failures=0;
            foreach(var family in StockAssetRootCompiler.Families)
            {
                try
                {
                    StockAssetRootCompiler.Qualify(family,manifest,global,schemas,available);
                    results.Add(new{Type=family.Name,Status="Passed",WitnessKind=family.EngineLayout?"OriginalRuntimeSchemaAbi":"OriginalAssetBytes",Message=(string?)null});
                    Console.WriteLine("Reborn: original root proof passed: "+family.Name);
                }
                catch(Exception error)
                {
                    failures++;results.Add(new{Type=family.Name,Status="Failed",WitnessKind=family.EngineLayout?"OriginalRuntimeSchemaAbi":"OriginalAssetBytes",Message=error.Message});
                    Console.WriteLine("Reborn: original root proof failed: "+family.Name+": "+error.Message);
                }
            }
            using var output=new FileStream(report,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
            JsonSerializer.Serialize(output,new{NativeWitnessesOnly=false,OriginalAssetByteFamilies=StockAssetRootCompiler.Families.Count(f=>!f.EngineLayout),OriginalRuntimeAbiFamilies=StockAssetRootCompiler.Families.Count(f=>f.EngineLayout),ModPublished=false,Results=results},new JsonSerializerOptions{WriteIndented=true});
            return failures==0?0:1;
        }
        finally{Settings.Current=saved;}
    }
}
