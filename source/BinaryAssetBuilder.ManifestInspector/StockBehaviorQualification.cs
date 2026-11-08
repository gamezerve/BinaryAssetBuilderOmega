using System.Reflection;
using System.Text;
using System.Text.Json;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: qualify explicit original behavior families independently of unrelated art/include failures in their complete source files.
internal static class StockBehaviorQualification
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain each original native field/metadata proof and its real owner while never publishing a mod. */
    //-------------------------------------------------------------------------------------------------
    internal static int Run(string manifest,string global,string report)
    {
        string fixtures=Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        // Reborn: the explicit stock manifest identifies its SDK even when this command runs from a development output folder.
        DirectoryInfo? root=new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(manifest))!);
        while(root!=null&&!Directory.Exists(Path.Combine(root.FullName,"SageXML")))root=root.Parent;
        string sdk=root?.FullName??throw new InvalidDataException("Original SDK root not found.");
        report=Path.GetFullPath(report);
        if(!report.StartsWith(Path.Combine(sdk,".work")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Behavior proof report must stay in SDK private workspace.");
        var saved=Settings.Current;
        try
        {
            Settings.Current=new Settings{SchemaPath=Path.Combine(fixtures,"TracerDrawPipeline.xsd"),BuildCache=false,StringHashBinDescriptors=Array.Empty<StringHashBinDescriptor>()};
            var schemas=new SchemaSet(false);
            foreach(var seed in schemas.Schemas.Schemas().Cast<System.Xml.Schema.XmlSchema>().ToArray())schemas.Schemas.Remove(seed);
            schemas.Schemas.Add(ConscriptSourceInput.Schemas());schemas.Schemas.Compile();
            typeof(SchemaSet).GetMethod("SetupHelperSchemaTypes",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(schemas,null);
            var available=new[]{manifest,global}.SelectMany(path=>ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576)).Assets).ToArray();
            var baseline=SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes("<GameObject xmlns=\""+SchemaSet.XmlNamespace+"\" id=\"RebornIndependentBehaviorProof\"><Behaviors/><Body><PropBody id=\"RebornIndependentBehaviorBody\"/></Body></GameObject>"));
            var results=new List<object>();int failures=0;
            foreach(var family in AdditionalStockBehaviorContract.Families)
            {
                try
                {
                    _=AdditionalStockBehaviorContract.Qualify(family,manifest,baseline,schemas,available);
                    results.Add(new{Type=family.Source,Status="Passed",WitnessKind=family.RuntimeAbi?"OriginalRuntimeAbiWithNativeBase":"OriginalAssetBytes",OriginalFile=family.File,OriginalOwner=family.Asset,Message=(string?)null});
                    Console.WriteLine("Reborn: original behavior proof passed: "+family.Source);
                }
                catch(Exception error)
                {
                    failures++;results.Add(new{Type=family.Source,Status="Failed",WitnessKind=family.RuntimeAbi?"OriginalRuntimeAbiWithNativeBase":"OriginalAssetBytes",OriginalFile=family.File,OriginalOwner=family.Asset,Message=error.Message});
                    Console.WriteLine("Reborn: original behavior proof failed: "+family.Source+": "+error.Message);
                }
            }
            using var output=new FileStream(report,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
            JsonSerializer.Serialize(output,new{NativeWitnessesOnly=false,RuntimeAbiFamilies=AdditionalStockBehaviorContract.Families.Count(f=>f.RuntimeAbi),ModPublished=false,Results=results},new JsonSerializerOptions{WriteIndented=true});
            return failures==0?0:1;
        }
        finally{Settings.Current=saved;}
    }
}
