using System.Text;
using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: offline batch review reports every fully preprocessed GameObject admission result in a private fresh directory.
internal static class AlliedUnitSourceReview
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: inspect all requested source graphs together, preserve expanded diagnostic copies and never publish mod/game outputs. */
    //-------------------------------------------------------------------------------------------------
    internal static int Run(string entry,string sdk,string output)
    {
        if(Directory.Exists(output)||File.Exists(output))throw new InvalidDataException("Fresh batch review directory required.");
        var input=ConscriptSourcePreparation.Load(entry,SdkSourceSearch.ForMod(entry,sdk,null));var schemas=ConscriptSourceInput.Schemas();
        var baseline=new XmlDocument();baseline.Load(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha","ConscriptBuildTime1","data","mod.xml"));
        Directory.CreateDirectory(output);var results=new List<object>();int failed=0;
        foreach(var asset in new[]{input.Game}.Concat(input.AdditionalAssets).Where(a=>a.DocumentElement!.LocalName=="GameObject")){
            string id=asset.DocumentElement!.GetAttribute("id"),error="";
            // Reborn: diagnostic filenames use the same valid asset identity rule as compilation, never arbitrary source paths.
            if(!ConscriptSourceInput.Identity("GameObject",id))throw new InvalidDataException("Invalid batch GameObject identity: "+id);
            File.WriteAllText(Path.Combine(output,id+".xml"),asset.OuterXml,new UTF8Encoding(false));
            try{ConscriptSourceInput.Admit(asset,baseline,schemas);}catch(InvalidDataException problem){error=problem.Message;failed++;}
            results.Add(new{Id=id,Passed=error.Length==0,Error=error});Console.WriteLine(id+": "+(error.Length==0?"PASS":error));
        }
        File.WriteAllText(Path.Combine(output,"review.json"),JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));return failed==0?0:1;
    }
}
