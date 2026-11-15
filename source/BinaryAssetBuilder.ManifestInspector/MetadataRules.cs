using System.Text.Json;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: use immutable, independently captured row rules for output verification in both build modes.
internal static class MetadataRules
{
    private static readonly Lazy<SortedDictionary<string,StockMetadataGrammar.Rule[]>> Embedded = new(Load);
    [ThreadStatic] private static SortedDictionary<string,StockMetadataGrammar.Rule[]>? captured;
    [ThreadStatic] private static List<string>? errors;
    internal static bool Capturing => captured != null;

    // Reborn: qualify only the families that an authored build actually uses.
    internal static Lazy<StockMetadataGrammar> Deferred(string family,Func<StockMetadataGrammar> compareRetail)
        => new(()=>Get(family,compareRetail));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare embedded rules to retail only in developer mode and fail on drift. */
    //-------------------------------------------------------------------------------------------------
    internal static StockMetadataGrammar Get(string family,Func<StockMetadataGrammar> compareRetail)
    {
        if(Capturing)
        {
            try
            {
                var actual=compareRetail();
                captured![family]=actual.Export();
                return actual;
            }
            catch(InvalidDataException error)
            {
                throw new InvalidDataException(family+": "+error.Message,error);
            }
        }
        if(!Embedded.Value.TryGetValue(family,out var rules))
            throw new InvalidDataException("Embedded EP1 metadata rules missing for "+family+".");
        if(SdkBuildTests.Enabled)
        {
            var retail=compareRetail().Export();
            if(!JsonSerializer.SerializeToUtf8Bytes(rules).AsSpan().SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(retail)))
                throw new InvalidDataException("Embedded EP1 metadata rules differ from the retail witnesses for "+family+".");
        }
        return StockMetadataGrammar.Import(rules);
    }

    // Reborn: report every unqualified family in one capture, with its name and failure.
    internal static void RecordCaptureError(InvalidDataException error)=>errors!.Add(error.Message);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: regenerate the checked-in rule resource with the same retail readers as developer builds. */
    //-------------------------------------------------------------------------------------------------
    internal static void Capture(string preparedSource,string staticManifest,string globalManifest,string output)
    {
        var previous=Environment.GetEnvironmentVariable(SdkBuildTests.EnvironmentKey);
        captured=new(StringComparer.Ordinal);
        errors=new();
        try
        {
            // Reborn: capture binary grammar independently of the retail XML regression pass.
            Environment.SetEnvironmentVariable(SdkBuildTests.EnvironmentKey,"0");
            ConscriptMultiAssetBuild.TryRun(preparedSource,staticManifest,globalManifest,output,forceMixed:true);
            if(errors.Count!=0)throw new InvalidDataException("Metadata rule capture failed:\n"+string.Join("\n",errors));
            using var file=new FileStream(output,FileMode.CreateNew);
            JsonSerializer.Serialize(file,captured);
            Console.WriteLine("Reborn: captured "+captured.Count+" EP1 metadata grammars.");
        }
        finally
        {
            captured=null;
            errors=null;
            Environment.SetEnvironmentVariable(SdkBuildTests.EnvironmentKey,previous);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: load version-matched rules directly from the compiler assembly. */
    //-------------------------------------------------------------------------------------------------
    private static SortedDictionary<string,StockMetadataGrammar.Rule[]> Load()
    {
        using var stream=typeof(MetadataRules).Assembly.GetManifestResourceStream("Ep1MetadataRules.json")
            ??throw new InvalidDataException("Embedded EP1 metadata resource missing.");
        return JsonSerializer.Deserialize<SortedDictionary<string,StockMetadataGrammar.Rule[]>>(stream)
            ??throw new InvalidDataException("Embedded EP1 metadata resource is empty.");
    }
}
