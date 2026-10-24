using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: localization keys are not asset IDs; audit their stock-CSF/mod-STR union without changing either table.
internal static class SdkMissingStringAudit
{
    internal sealed record Result(SdkMissingDependencyAudit.Finding[] Findings,int CheckedKeys,int StockKeys,int ModKeys,bool CatalogComplete,string? StockSource,string ModSource,bool PreparedSourceUsed);
    // Reborn: upgrade AcquireHint is a localization key even when its spelling does not include a category colon.
    private static readonly HashSet<string> KeyFields=new(StringComparer.Ordinal){"DisplayName","DisplayNameTransformed","Description","DescriptionTransformed","TypeDescription","TypeDescriptionTransformed","AcquireHint","TextLabel","StringLabel","RandomStringLabel"};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read the SDK-local stock table; unavailable or broken tables produce coverage warnings rather than false missing-key claims. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Run(string entry,string sdk,SdkSourcePathAudit.Report paths,XmlSchemaSet schemas)
    {
        string stockSource=StockCatalogPath(sdk);
        // Reborn: nested AdditionalMaps entries use the same mod localization table as Static_Mod.xml.
        string modData=paths.Search?.ModData??Path.GetDirectoryName(entry)!;
        string modSource=Path.Combine(Path.GetDirectoryName(modData)!,"Misc","Data","mod.str");
        try{
            stockSource=SdkEnvironmentPreflight.Absolute(stockSource);var stock=SdkStringCatalog.Stock(stockSource);SdkEnvironmentPreflight.CheckPath(modSource);var mod=File.Exists(modSource)?SdkStringCatalog.Str(SdkEnvironmentPreflight.Read(modSource,SdkStringCatalog.MaximumBytes)):new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            return Analyze(entry,paths,schemas,stock,mod) with{StockSource=stockSource,ModSource=modSource};
        }catch(Exception error)when(error is IOException or InvalidDataException or UnauthorizedAccessException or ArgumentException){
            var finding=new SdkMissingDependencyAudit.Finding("STRINGCATALOG","Warning","localization catalogs","Localization",entry,0,0,"catalogs/ra3ep1/gamestrings.csf / Misc/Data/mod.str","Missing-string checking is incomplete: "+error.Message);
            return new(new[]{finding},0,0,0,false,stockSource,modSource,false);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: resolve the original string table relative to Studio so catalog lookup survives SDK relocation without a game install. */
    //-------------------------------------------------------------------------------------------------
    internal static string StockCatalogPath(string sdk)=>Path.Combine(SdkEnvironmentPreflight.Absolute(sdk),"catalogs","ra3ep1","gamestrings.csf");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: check effective supported source when available, preserving inheritance/definition expansion and original source locations; raw fallback remains explicitly scoped. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Analyze(string entry,SdkSourcePathAudit.Report paths,XmlSchemaSet schemas,IEnumerable<string> stockKeys,IEnumerable<string> modKeys)
    {
        var stock=new HashSet<string>(stockKeys,StringComparer.OrdinalIgnoreCase);var mod=new HashSet<string>(modKeys,StringComparer.OrdinalIgnoreCase);var known=new HashSet<string>(stock,StringComparer.OrdinalIgnoreCase);known.UnionWith(mod);
        var prefixes=known.Where(k=>k.Contains(':')).Select(k=>k[..k.IndexOf(':')]).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var snapshots=paths.Sources.Select(s=>new SdkXmlDiagnostics.Snapshot(s.PhysicalPath,SdkEnvironmentPreflight.Read(s.PhysicalPath,4*1048576))).ToArray();
        foreach(var snapshot in snapshots){var expected=paths.Sources.Single(s=>s.PhysicalPath==snapshot.Path);if(Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(snapshot.Bytes))!=expected.Sha256)throw new InvalidDataException("Localization source snapshot changed: "+snapshot.Path);}
        var locations=new SdkXmlDiagnostics.SourceMap(snapshots);var documents=new List<(XmlDocument Xml,string Context)>();bool prepared=false;
        // Reborn: use the same source frontend for supported GameObject/command-set mods so replaced inherited text does not produce stale-key warnings.
        try{var input=ConscriptSourcePreparation.Load(entry,paths.Search);documents.Add((input.Game,entry));documents.AddRange(input.AdditionalAssets.Select(x=>(x,entry)));if(input.Command!=null)documents.Add((input.Command,entry));locations=input.Locations??locations;prepared=true;}
        catch(Exception error)when(error is InvalidDataException or XmlException or XmlSchemaException){
            var emitted=new HashSet<string>(StringComparer.OrdinalIgnoreCase);Emit(entry);
            foreach(var snapshot in snapshots.Where(s=>emitted.Contains(s.Path))){var document=SdkXmlDiagnostics.Parse(snapshot.Bytes,snapshot.Path);if(document.DocumentElement!.LocalName=="AssetDeclaration"){foreach(XmlElement child in document.DocumentElement.ChildNodes.OfType<XmlElement>().Where(e=>e.LocalName is not ("Includes" or "Defines" or "Tags"))){documents.Add((SdkXmlDiagnostics.Parse(Encoding.UTF8.GetBytes(child.OuterXml)),snapshot.Path));}}else documents.Add((document,snapshot.Path));}
            // Reborn: source fallback is useful for already-invalid/unsupported XML but is not advertised as expanded inheritance coverage.
            void Emit(string path){if(!emitted.Add(path))return;foreach(var edge in paths.Includes.Where(e=>e.Document.Equals(path,StringComparison.OrdinalIgnoreCase)&&e.Kind=="all"&&e.PhysicalPath!=null))Emit(edge.PhysicalPath!);}
        }
        var findings=new List<SdkMissingDependencyAudit.Finding>();int checkedKeys=0;
        foreach(var (document,context) in documents){
            document.Schemas.Add(schemas);document.Validate((_,_)=>{});var owner=document.DocumentElement!;
            foreach(XmlElement element in owner.SelectNodes("descendant-or-self::*")!){
                foreach(XmlAttribute attribute in element.Attributes)Check(attribute,attribute.Value,attribute.LocalName,element.LocalName+"/@"+attribute.LocalName);
                if(!element.ChildNodes.OfType<XmlElement>().Any())Check(element,element.InnerText,element.LocalName,element.LocalName+"/text()");
            }
            // Reborn: only semantic localization fields or whole key-shaped schema strings are candidates; Lua, asset handles, IDs, includes and arbitrary prose are not localization keys.
            void Check(XmlNode node,string value,string name,string field){
                if(node.NamespaceURI=="http://www.w3.org/2000/xmlns/"||name is "id" or "ID" or "inheritFrom" or "source"||value.Length==0||value.StartsWith('='))return;
                // Reborn: literal code bodies are never localization use sites, even if their entire text resembles a known key prefix.
                for(XmlElement? scope=node as XmlElement??(node as XmlAttribute)?.OwnerElement;scope!=null;scope=scope.ParentNode as XmlElement)if(scope.LocalName is "Script" or "ScriptBody" or "Code" or "Lua" or "LUA")return;
                var info=node is XmlAttribute attribute?attribute.SchemaInfo:((XmlElement)node).SchemaInfo;var type=info.SchemaType;
                if(type?.Datatype?.TypeCode!=XmlTypeCode.String)return;
                foreach(string reference in new[]{"AssetReference","WeakReference"})if(schemas.GlobalTypes[new XmlQualifiedName(reference,"uri:ea.com:eala:asset")] is XmlSchemaType basis&&XmlSchemaType.IsDerivedFrom(type,basis,XmlSchemaDerivationMethod.None))return;
                value=value.Trim();if(value.Length==0)return;int colon=value.IndexOf(':');bool shaped=colon>0&&colon<value.Length-1&&!value.Any(char.IsWhiteSpace)&&prefixes.Contains(value[..colon]);
                if(!KeyFields.Contains(name)&&!shaped)return;checkedKeys++;if(known.Contains(value))return;
                var error=SdkXmlDiagnostics.At(new InvalidDataException("Unknown localization key."),node);locations.Enrich(error,context,document);var origin=SdkXmlDiagnostics.Origin(error);
                findings.Add(new("MISSINGSTRING","Warning",value,owner.LocalName+":"+owner.GetAttribute("id"),origin?.File??context,origin?.Line??0,origin?.Column??0,field,"String key is absent from both the selected stock CSF and Misc/Data/mod.str."+(origin?.Context==true?" [source context; exact field origin unavailable]":"")));
            }
        }
        return new(findings.Distinct().ToArray(),checkedKeys,stock.Count,mod.Count,true,null,"",prepared);
    }
}
