using System.Globalization;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: test the observed definition forms and reject all broader evaluator semantics before trusting transformed source fields.
internal static class SdkDefinitionSubsetSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate alias/concat/integer multiply-add, declaration order, atomic closure evidence, strict old profiles and physical confinement. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        Dictionary<string,string> values = new(StringComparer.Ordinal) { ["BASE"] = "local",["LEFT"] = "200",["RIGHT"] = "40",["UNIT"] = "2s",["MAX"] = long.MaxValue.ToString(CultureInfo.InvariantCulture) };
        //-------------------------------------------------------------------------------------------------
        /** Reborn: look up only the owned fixture's backward-visible values, without environment or filesystem fallback. */
        //-------------------------------------------------------------------------------------------------
        string? Lookup(string name) => values.TryGetValue(name,out var value) ? value : null;
        Require(SdkDefinitionSubset.Evaluate("=$BASE",Lookup) == "local" && SdkDefinitionSubset.Evaluate("=$BASE+'.bin'",Lookup) == "local.bin"
            && SdkDefinitionSubset.Evaluate("= ( $LEFT * 2 ) + $RIGHT",Lookup) == "440","Observed definition forms differ.");
        foreach (string expression in new[] { "=$MISSING","=$base","=$LEFT+1","=1+2","=($LEFT*2)+$UNIT","=($MAX*2)+$RIGHT","=$BASE+'a'+'b'","=Call($LEFT)","=$BASE+\"suffix\"","=($LEFT*-2)+$RIGHT","=($LEFT*2.0)+$RIGHT" })
            Reject(() => SdkDefinitionSubset.Evaluate(expression,Lookup));
        Reject(() => SdkDefinitionSubset.Evaluate("=$BASE+'"+new string('x',257)+"'",Lookup));
        values["BASE"] = new string('x',512); Reject(() => SdkDefinitionSubset.Evaluate("=$BASE+'x'",Lookup)); values["BASE"] = "local";
        CultureInfo previous = CultureInfo.CurrentCulture;
        try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("tr-TR"); Require(SdkDefinitionSubset.Evaluate("=($LEFT*2)+$RIGHT",Lookup) == "440","Arithmetic depends on locale."); }
        finally { CultureInfo.CurrentCulture = previous; }
        string root = Path.Combine(Path.GetTempPath(),"Reborn-DefinitionSubset-"+Guid.NewGuid().ToString("N")),data = Path.Combine(root,"Data"); Directory.CreateDirectory(data);
        string entry = Path.Combine(data,"Entry.xml"),defs = Path.Combine(data,"Defines.xml");
        const string body = "<Includes><Include type='all' source='Defines.xml'/></Includes><Asset File='=$FILE' Amount='=$SIZE'/>";
        const string declarations = "<Defines><Define name='BASE' value='local'/><Define name='FILE' value=\"=$BASE+'.bin'\"/><Define name='LEFT' value='200'/><Define name='RIGHT' value='40'/><Define name='SIZE' value='=($LEFT*2)+$RIGHT'/><Define name='ALIAS' value='=$FILE'/></Defines>";
        Write(entry,body); Write(defs,declarations); File.WriteAllBytes(Path.Combine(data,"local.bin"),new byte[] { 1 });
        var paths = Audit(); var result = new SdkIncludeDefineProfile(paths,true).Apply(entry,File.ReadAllBytes(entry));
        Require(result.Bytes != null && result.Evidence.Profile == SdkDefinitionSubset.Name && result.Evidence.Substitutions == 2
            && result.Evidence.EvaluatedDefinitions.Length == 3 && result.Evidence.EvaluatedDefinitions.Single(item => item.Name == "SIZE").EvaluatedValue == "440"
            && result.Evidence.EvaluatedDefinitions.All(item => item.SourcePath == defs),"Definition closure computations/origins differ.");
        Require(new SdkIncludeDefineProfile(paths).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Old Include literal profile admitted expressions.");
        const string xsd = "<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns='uri:ea.com:eala:asset' targetNamespace='uri:ea.com:eala:asset' elementFormDefault='qualified'><xs:simpleType name='FileReference'><xs:restriction base='xs:anyURI'/></xs:simpleType><xs:element name='AssetDeclaration'><xs:complexType><xs:sequence><xs:element name='Includes' type='xs:anyType' minOccurs='0'/><xs:element name='Defines' type='xs:anyType' minOccurs='0'/><xs:element name='Asset' minOccurs='0'><xs:complexType><xs:attribute name='File' type='FileReference'/><xs:attribute name='Amount' type='xs:int'/></xs:complexType></xs:element></xs:sequence></xs:complexType></xs:element></xs:schema>";
        var schemas = SdkEffectiveSchema.Compile(new Dictionary<string,byte[]> { ["entry.xsd"] = Encoding.UTF8.GetBytes(xsd) },"entry.xsd");
        var graph = SdkTypedSourceGraph.BindGraph(schemas.Schemas,paths,definitionExpressions:true);
        Require(graph.ScopedGraphComplete && !graph.ProductionBuildReady && graph.PreprocessingProfile == SdkDefinitionSubset.Name
            && graph.Documents.Single(document => document.SourcePath == entry).Dependencies.Single().Bytes == 1 && File.ReadAllText(entry).Contains("=$SIZE",StringComparison.Ordinal)
            && !Directory.Exists(Path.Combine(root,"NoOutput")),"Definition profile bypassed typed/resource/source isolation.");
        foreach (string invalid in new[] {
            "<Defines><Define name='FILE' value='=$LATER'/><Define name='LATER' value='local.bin'/></Defines>",
            "<Defines><Define name='FILE' value='=$FILE'/></Defines>",
            declarations.Replace("</Defines>","<Define name='UNUSED' value='=1+2'/></Defines>",StringComparison.Ordinal),
            declarations.Replace("name='ALIAS'","name='FILE'",StringComparison.Ordinal) })
        {
            Write(defs,invalid); var rejected = new SdkIncludeDefineProfile(Audit(),true).Apply(entry,File.ReadAllBytes(entry));
            Require(rejected.Bytes == null && rejected.Evidence.ProcessedSha256 == null && rejected.Evidence.Substitutions == 0
                && rejected.Evidence.EvaluatedDefinitions.Length == 0 && rejected.Evidence.DefinitionSources.Length == 0,"Rejected definition evaluation exposed partial evidence.");
        }
        Write(defs,declarations); paths = Audit(); Write(defs,declarations.Replace("200","300",StringComparison.Ordinal));
        Require(new SdkIncludeDefineProfile(paths,true).Apply(entry,File.ReadAllBytes(entry)).Bytes == null,"Definition evaluation bypassed stale source check.");
        Console.WriteLine("SDK definition subset self-test: OK (backward alias/concat/checked integer multiply-add, invariant locale, overflow/units/functions/forward/self/unknown/duplicate/unused rejection, atomic origins, old-profile isolation and typed/resource graph; no EA evaluator equivalence)");

        //-------------------------------------------------------------------------------------------------
        /** Reborn: capture only fresh owned fixture sources while keeping production output absent. */
        //-------------------------------------------------------------------------------------------------
        SdkSourcePathAudit.Report Audit() => SdkSourcePathAudit.Inspect(SdkEnvironmentPreflight.Inspect("ra3ep1",SdkEnvironmentPreflight.BaselineSchemaRoot(),data,entry,Path.Combine(root,"NoOutput"),Array.Empty<string>()));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: unsupported or overflowing diagnostic syntax must fail closed, never evaluate arbitrary code. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Unsupported definition expression accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: create only owned minimal XML fixture bytes, leaving official and synced sources unchanged. */
    //-------------------------------------------------------------------------------------------------
    private static void Write(string path,string body) => File.WriteAllText(path,"<AssetDeclaration xmlns='uri:ea.com:eala:asset'>"+body+"</AssetDeclaration>",new UTF8Encoding(false));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: report bounded diagnostic regressions without equating schema validation with a working game mod. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
