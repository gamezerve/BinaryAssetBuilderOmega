using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: an empty original declaration can be source-qualified, but must never be labeled native-qualified or generate a placeholder asset.
internal static class StockEmptySourceQualification
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain production graph, definitions, schema and source freshness checks while explicitly recording that no runtime assets exist. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string entry,string receipt,string sdk,string? stockData=null)
    {
        var search=SdkSourceSearch.ForMod(entry,sdk,stockData);
        var input=ConscriptSourcePreparation.Load(entry,search,allowEmpty:true);
        if(input.Game.DocumentElement?.LocalName!="AssetDeclaration"||input.Origins.Length!=0||input.AdditionalAssets.Length!=0)
            throw new InvalidDataException("Source-only qualification requires a genuinely empty emitted declaration closure.");
        var schemas=ConscriptSourceInput.Schemas();
        foreach(var source in input.Sources)
        {
            var bytes=File.ReadAllBytes(source.Path);
            if(Convert.ToHexString(SHA256.HashData(bytes))!=source.Sha256)throw new InvalidDataException("Empty source changed during qualification.");
            var document=SdkXmlDiagnostics.Parse(bytes,source.Path);
            if(document.DocumentElement?.LocalName!="AssetDeclaration")continue;
            document.Schemas.Add(schemas);
            document.Validate((_,error)=>throw SdkXmlDiagnostics.SchemaError(error));
        }
        SdkSourceSearch.Verify(input.Searches);
        receipt=SdkEnvironmentPreflight.Absolute(receipt);
        if(!receipt.StartsWith(Path.Combine(Path.GetFullPath(sdk),".work")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Empty-source evidence must stay in SDK work.");
        SdkEnvironmentPreflight.CheckPath(receipt);
        using var output=new FileStream(receipt,FileMode.CreateNew,FileAccess.Write,FileShare.Read);
        JsonSerializer.Serialize(output,new{SourceOnlyQualified=true,AssetCount=0,NativeAssetsEmitted=false,Sources=input.Sources});
        Console.WriteLine("Reborn: empty original declaration source-qualified; no native asset was emitted or qualified.");
    }
}
