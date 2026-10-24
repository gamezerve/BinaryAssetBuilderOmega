using System.Security.Cryptography;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: collect missing source dependencies before fail-fast compilation; this audit never manufactures assets or definitions.
internal static class SdkMissingDependencyAudit
{
    private const string Ea="uri:ea.com:eala:asset",AnnotationNamespace="uri:ea.com:eala:asset:schema";
    internal sealed record Asset(string Type,string Name,uint TypeId,uint InstanceId);
    // Reborn: retain the exact lookup-only manifest generation in the audit without emitting any of its assets.
    internal sealed record StockCatalog(string Manifest,string Sha256,int Assets);
    internal sealed record Finding(string Code,string Severity,string Target,string Owner,string Source,int Line,int Column,string Field,string Detail);
    // Reborn: retain localization catalog/coverage evidence separately from asset reference counts and translated text.
    internal sealed record StringStatus(int CheckedKeys,int StockKeys,int ModKeys,bool CatalogComplete,bool PreparedSourceUsed,string? StockSource,string ModSource);
    internal sealed record Report(Finding[] Findings,int ModAssets,int StockAssets,int CheckedAssetReferences,int UncheckedAssetReferences,bool SourceGraphComplete)
    {
        // Reborn: synthetic asset-only tests need no installed language archive; live builds populate this localization summary.
        public StringStatus? Strings {get;init;}
        // Reborn: catalog provenance describes availability checks, not runtime stream loading or packaged assets.
        public StockCatalog[] StockCatalogs {get;init;}=Array.Empty<StockCatalog>();
        // Reborn: a clean bounded preflight is not proof of complete runtime dependency coverage or compiler support.
        public bool FullDependencyCoverage=>false;
        public bool GameExecuted=>false;
        // Reborn: distinguish captured source evidence from post-inheritance/default expansion and dynamic game-name lookup.
        public string[] Limitations=>new[]{"Explicit source expressions, schema-typed strong references, UpgradeTemplate, Image and LogicCommandSet weak references are checked.","Instance-only owners are not mod-produced assets; inherited/default fields absent from emitted raw owners require subsequent compiler checks.","Other weak IDs, arbitrary strings, dynamically loaded names and unsupported expression values are not proved present.","Stock catalog contains only the explicitly supplied manifests, not a recursive scan of game archives."};
    }
    private sealed record Definition(string Value,string Source,bool Usable,bool Broken);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use the same confined Include graph and explicit stock manifests as the live SDK, without rebuilding any mod. */
    //-------------------------------------------------------------------------------------------------
    internal static int Run(string entry,string sdk,string staticManifest,string globalManifest,string output,string? stockData=null)
    {
        entry=SdkEnvironmentPreflight.Absolute(entry);output=SdkEnvironmentPreflight.Absolute(output);
        var search=SdkSourceSearch.ForMod(entry,sdk,stockData);byte[] initial=SdkEnvironmentPreflight.Read(entry,4*1048576);
        var first=Parse(initial,entry);var root=Path.GetDirectoryName(entry)!;string hash=Convert.ToHexString(SHA256.HashData(initial));
        SdkSourcePathAudit.Report paths;
        if(first.Root?.Name==XName.Get("AssetDeclaration",Ea))
        {
            string fixture=ReferencePipelineSmokeTest.FindFixture();
            var environment=new SdkEnvironmentPreflight.Report("ra3ep1",Path.GetDirectoryName(fixture)!,fixture,0,"",root,entry,hash,Path.GetDirectoryName(output)!,Array.Empty<SdkEnvironmentPreflight.Mapping>(),true,true,false,false,false,"0x5454A8E9",Array.Empty<string>());
            paths=SdkSourcePathAudit.Inspect(environment,search:search,w3xSources:true);
        }
        else paths=new(root,null,null,new[]{new SdkSourcePathAudit.Source(entry,root,hash,initial.Length)},Array.Empty<SdkSourcePathAudit.Edge>(),Array.Empty<SdkSourcePathAudit.Resource>(),Array.Empty<SdkSourcePathAudit.Issue>(),true,false,true,false,false,Array.Empty<string>()){Search=search};
        // Reborn: availability uses SDK-local metadata, independently of registry, installed archives and native compiler development inputs.
        var stock=LoadStockCatalogs(CatalogManifestPaths(sdk));
        var report=Analyze(entry,paths,ConscriptSourceInput.Schemas(),stock.Assets) with{StockCatalogs=stock.Catalogs};
        // Reborn: missing strings join the same readable preflight/report as missing assets and definitions; warnings do not become false native dependency errors.
        var strings=SdkMissingStringAudit.Run(entry,sdk,paths,ConscriptSourceInput.Schemas());
        report=report with{Findings=report.Findings.Concat(strings.Findings).ToArray(),Strings=new(strings.CheckedKeys,strings.StockKeys,strings.ModKeys,strings.CatalogComplete,strings.PreparedSourceUsed,strings.StockSource,strings.ModSource)};
        // Reborn: reports may only be written into an existing non-redirected work directory, never over a source or stock manifest.
        SdkEnvironmentPreflight.CheckPath(output);SdkEnvironmentPreflight.DirectoryPath(Path.GetDirectoryName(output)!);
        if(File.Exists(output) || Directory.Exists(output) || paths.AllowedRoots.Any(r=>SdkEnvironmentPreflight.Inside(r,output)) || stock.Catalogs.Any(c=>output.Equals(c.Manifest,StringComparison.OrdinalIgnoreCase)))throw new InvalidDataException("Fresh dependency-report path outside source roots required.");
        File.WriteAllText(output,JsonSerializer.Serialize(report,new JsonSerializerOptions{WriteIndented=true}));
        // Reborn: record catalog origins in diagnostic detail without claiming their streams were loaded by the game.
        foreach(var catalog in stock.Catalogs)Console.WriteLine($"Reborn: dependency lookup catalog: {catalog.Manifest}; {catalog.Assets} assets; lookup only (not copied or loaded).");
        foreach(var finding in report.Findings)
            Console.WriteLine($"{finding.Severity}: {finding.Code} {finding.Detail} Target='{finding.Target}', owner='{finding.Owner}', field='{finding.Field}', source='{finding.Source}' ({finding.Line},{finding.Column}).");
        Console.WriteLine($"Reborn: dependency audit: {report.Findings.Count(f=>f.Code=="MISSINGDEF")} missing definition uses, {report.Findings.Count(f=>f.Code=="MISSINGASSET")} missing asset uses; {report.ModAssets} mod assets + {report.StockAssets} stock identities; {report.CheckedAssetReferences} typed references checked, {report.UncheckedAssetReferences} deferred. Full runtime coverage: false.");
        Console.WriteLine($"Reborn: string audit: {strings.CheckedKeys} key uses checked; {strings.StockKeys} stock keys + {strings.ModKeys} mod.str keys; {strings.Findings.Count(f=>f.Code=="MISSINGSTRING")} missing string uses; catalog complete={strings.CatalogComplete}; prepared source={strings.PreparedSourceUsed}.");
        Console.WriteLine("Reborn: dependency report: "+output);
        return report.Findings.Any(f=>f.Severity=="Error")?1:0;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate the three catalog copies relative to Studio, without developer-specific paths or installation discovery. */
    //-------------------------------------------------------------------------------------------------
    internal static string[] CatalogManifestPaths(string sdk)=>new[]{"static.manifest","global.manifest","worldbuilder.manifest"}.Select(name=>Path.Combine(SdkEnvironmentPreflight.Absolute(sdk),"catalogs","ra3ep1",name)).Concat(File.Exists(Path.Combine(sdk,"catalogs/ra3ep1/audio.manifest"))?new[]{Path.Combine(sdk,"catalogs/ra3ep1/audio.manifest")}:Array.Empty<string>()).ToArray();

    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the explicit WorldBuilder configuration reader for legacy diagnostic callers. */
    //-------------------------------------------------------------------------------------------------
    internal static string? ConfiguredWorldBuilderManifest(string sdk)
    {
        string config=Path.Combine(sdk,"uprising-alpha.paths");
        return File.Exists(config)?File.ReadLines(config).Where(l=>l.StartsWith("WorldBuilderManifest=",StringComparison.OrdinalIgnoreCase)).Select(l=>l[(l.IndexOf('=')+1)..].Trim()).LastOrDefault():null;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read and validate only explicit manifest metadata, retaining hashes and deduplicating shared stock identities during analysis. */
    //-------------------------------------------------------------------------------------------------
    internal static (Asset[] Assets,StockCatalog[] Catalogs) LoadStockCatalogs(IEnumerable<string> manifests)
    {
        var assets=new List<Asset>();var catalogs=new List<StockCatalog>();
        foreach(string manifest in manifests.Where(p=>!string.IsNullOrWhiteSpace(p)).Select(SdkEnvironmentPreflight.Absolute).Distinct(StringComparer.OrdinalIgnoreCase)){
            byte[] bytes=SdkEnvironmentPreflight.Read(manifest,64*1048576);var document=ManifestReader.Read(bytes);
            if(document.Validate().Count!=0)throw new InvalidDataException("Dependency audit stock manifest is invalid: "+manifest);
            catalogs.Add(new(manifest,Convert.ToHexString(SHA256.HashData(bytes)),document.Assets.Count));
            assets.AddRange(document.Assets.Select(a=>new Asset(a.TypeName,a.Name.Contains(':')?a.Name[(a.Name.IndexOf(':')+1)..]:a.Name,a.TypeId,a.InstanceId)));
        }
        return(assets.ToArray(),catalogs.ToArray());
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: aggregate every captured document's definition uses and emitted typed references without flattening instance-only assets into runtime output. */
    //-------------------------------------------------------------------------------------------------
    internal static Report Analyze(string entry,SdkSourcePathAudit.Report paths,XmlSchemaSet schemas,IEnumerable<Asset> stockAssets)
    {
        var documents=new Dictionary<string,XDocument>(StringComparer.OrdinalIgnoreCase);
        foreach(var source in paths.Sources)
        {
            byte[] bytes=SdkEnvironmentPreflight.Read(source.PhysicalPath,4*1048576);
            if(Convert.ToHexString(SHA256.HashData(bytes))!=source.Sha256)throw new InvalidDataException("Dependency source snapshot changed: "+source.PhysicalPath);
            documents.Add(source.PhysicalPath,Parse(bytes,source.PhysicalPath));
        }
        var findings=new List<Finding>();var tables=new Dictionary<string,Dictionary<string,Definition>>(StringComparer.OrdinalIgnoreCase);var active=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach(var issue in paths.Issues)findings.Add(new("DEPENDENCYPATH","Error",issue.LogicalPath,"Include/source",issue.Location?.File??issue.Document,issue.Location?.Line??0,issue.Location?.Column??0,"Include",issue.Code+": "+issue.Detail+(issue.Location?.Context==true?" [source context; exact field origin unavailable]":"")));
        foreach(string path in documents.Keys)Definitions(path);
        // Reborn: only the entry and its all-Include emission closure count as mod-produced assets; instance witnesses do not satisfy runtime references.
        var emitted=new HashSet<string>(StringComparer.OrdinalIgnoreCase);Emit(entry);
        var mod=emitted.SelectMany(p=>Owners(documents[p])).Where(e=>e.Attribute("id")!=null).Select(e=>Identity(e.Name.LocalName,(string)e.Attribute("id")!)).DistinctBy(a=>(a.TypeId,a.InstanceId)).ToArray();
        var stock=stockAssets.DistinctBy(a=>(a.TypeId,a.InstanceId)).ToArray();
        var modIdentities=mod.Select(a=>(a.TypeId,a.InstanceId)).ToHashSet();
        var catalog=mod.Concat(stock).DistinctBy(a=>(a.TypeId,a.InstanceId)).GroupBy(a=>a.InstanceId).ToDictionary(g=>g.Key,g=>g.ToArray());
        int checkedReferences=0,deferred=0;
        var referenceType=schemas.GlobalTypes[new XmlQualifiedName("AssetReference",Ea)] as XmlSchemaType;
        var weakType=schemas.GlobalTypes[new XmlQualifiedName("WeakReference",Ea)] as XmlSchemaType;
        foreach(string path in emitted)
        {
            var document=documents[path];
            // Reborn: schema annotations are diagnostic typing only; normal compilation still owns schema errors, defaults and inheritance admission.
            document.Validate(schemas,(_,_)=>{},true);
            foreach(var owner in Owners(document))
                foreach(var element in owner.DescendantsAndSelf())
                {
                    foreach(var attribute in element.Attributes().Where(a=>!a.IsNamespaceDeclaration))Check(attribute,attribute.GetSchemaInfo(),attribute.Value,element.Name.LocalName+"/@"+attribute.Name.LocalName);
                    if(!element.HasElements)Check(element,element.GetSchemaInfo(),element.Value,element.Name.LocalName+"/text()");
                }

            //-------------------------------------------------------------------------------------------------
            /** Reborn: check schema-declared strong and covered typed weak references against emitted/stock identities, never guess from ordinary strings. */
            //-------------------------------------------------------------------------------------------------
            void Check(XObject node,IXmlSchemaInfo? info,string value,string field)
            {
                var type=info?.SchemaType;if(type==null)return;
                // Reborn: weak IDs may derive directly from string rather than AssetReference, so count their unproved uses before the strong-type gate.
                // Reborn: UpgradeTemplate weak IDs have explicit asset semantics and must report genuine missing upgrades, not an unsupported module shape.
                bool upgradeWeak=type.QualifiedName.Name=="UpgradeTemplateWeakRef";
                // Reborn: UI images and command sets are schema-typed asset IDs too; weak lookup failures warn without manufacturing native imports.
                // Reborn: absent upgrade weak targets warn like UI weak targets; Core preserves their hashes without manufacturing imports or assets.
                bool diagnosticWeak=upgradeWeak || type.QualifiedName.Name is "ImageWeakRef" or "LogicCommandSetWeakRef";
                if(!upgradeWeak && !diagnosticWeak && weakType!=null && XmlSchemaType.IsDerivedFrom(type,weakType,XmlSchemaDerivationMethod.None)){if(!string.IsNullOrWhiteSpace(value))deferred++;return;}
                if(!upgradeWeak && !diagnosticWeak && (referenceType==null || !XmlSchemaType.IsDerivedFrom(type,referenceType,XmlSchemaDerivationMethod.None)))return;
                // Reborn: list-valued/custom reference encodings require their own tokenization rather than hashing the whole list as one missing name.
                if(type.Datatype?.Variety==XmlSchemaDatatypeVariety.List){deferred++;return;}
                string? required=Annotation(info?.SchemaAttribute?.UnhandledAttributes)??Annotation(info?.SchemaElement?.UnhandledAttributes);
                for(XmlSchemaType? current=type;required==null && current!=null;){required=Annotation(current.UnhandledAttributes);var next=current.BaseXmlSchemaType;if(ReferenceEquals(next,current))break;current=next;}
                value=value.Trim();if(value.Length==0)return;
                if(value.StartsWith('='))
                {
                    try{value=SdkDefinitionSubset.Evaluate(value,n=>tables[path].TryGetValue(n,out var d)&&d.Usable?d.Value:null);}catch(InvalidDataException){deferred++;return;}
                }
                value=value.Trim();if(value.Length==0)return;
                if(required==null || value.Contains('\\') || !schemas.GlobalTypes.Contains(new XmlQualifiedName(required,Ea))){deferred++;return;}
                string selector=required,name=value;int colon=value.IndexOf(':');if(colon>=0){selector=value[..colon];name=value[(colon+1)..];}
                var expected=schemas.GlobalTypes[new XmlQualifiedName(required,Ea)] as XmlSchemaType;
                var selected=schemas.GlobalTypes[new XmlQualifiedName(selector,Ea)] as XmlSchemaType;
                if(selected==null || expected==null){deferred++;return;}
                uint id=new InstanceHandle(name).InstanceId;
                var matches=catalog.GetValueOrDefault(id,Array.Empty<Asset>()).Where(a=>schemas.GlobalTypes[new XmlQualifiedName(a.Type,Ea)] is XmlSchemaType actual && XmlSchemaType.IsDerivedFrom(actual,expected,XmlSchemaDerivationMethod.None) && XmlSchemaType.IsDerivedFrom(actual,selected,XmlSchemaDerivationMethod.None)).ToArray();
                // Reborn: mirror Core's source-before-manifest and exact-before-derived resolution instead of falsely flagging a valid override as ambiguous.
                var local=matches.Where(a=>modIdentities.Contains((a.TypeId,a.InstanceId))).ToArray();if(local.Length!=0)matches=local;
                uint selectedId=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(selector);var exact=matches.Where(a=>a.TypeId==selectedId).ToArray();if(exact.Length!=0)matches=exact;
                checkedReferences++;
                if(matches.Length==0)Add("MISSINGASSET",selector+":"+name,node,path,field,"Unknown asset referenced from emitted source; not found in mod-produced assets or configured stock manifests.",diagnosticWeak?"Warning":"Error");
                else if(matches.Length>1)Add("AMBIGUOUSASSET",selector+":"+name,node,path,field,"Multiple compatible concrete asset identities match this reference.",diagnosticWeak?"Warning":"Error");
            }
        }
        SdkSourceSearch.Verify(paths.Searches);
        return new(findings.Distinct().ToArray(),mod.Length,stock.Length,checkedReferences,deferred,paths.ScopedPathAuditComplete&&!paths.StoppedAtLimit);

        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve imported/earlier-local definition visibility while reporting missing, forward and blocked uses independently. */
        //-------------------------------------------------------------------------------------------------
        Dictionary<string,Definition> Definitions(string path)
        {
            if(tables.TryGetValue(path,out var done))return done;
            var table=new Dictionary<string,Definition>(StringComparer.Ordinal);
            if(!active.Add(path))return table;
            foreach(var edge in paths.Includes.Where(e=>e.Document.Equals(path,StringComparison.OrdinalIgnoreCase) && e.Kind is "all" or "instance"))
                if(edge.PhysicalPath!=null && documents.ContainsKey(edge.PhysicalPath))
                    foreach(var pair in Definitions(edge.PhysicalPath))
                        if(!table.TryAdd(pair.Key,pair.Value) && table[pair.Key].Source!=pair.Value.Source)Add("AMBIGUOUSDEF","$"+pair.Key,documents[path].Root!,path,"Defines","Conflicting definition origins in Include visibility.");
            var xml=documents[path];var definitions=xml.Root?.Elements(XName.Get("Defines",Ea)).Elements(XName.Get("Define",Ea)).ToArray()??Array.Empty<XElement>();
            var localNames=definitions.Select(e=>(string?)e.Attribute("name")??"").ToHashSet(StringComparer.Ordinal);
            foreach(var definition in definitions)
            {
                string name=(string?)definition.Attribute("name")??"",value=(string?)definition.Attribute("value")??"";bool usable=Inspect(value,definition,path,"Define/@value",table,localNames),broken=!usable;
                if(value.StartsWith('=')){try{value=SdkDefinitionSubset.Evaluate(value,n=>table.TryGetValue(n,out var d)&&d.Usable?d.Value:null);}catch(InvalidDataException){usable=false;}}
                if(!table.TryAdd(name,new(value,path,usable,broken)))Add("AMBIGUOUSDEF","$"+name,definition,path,"Define/@name","Duplicate/local definition collides with an imported or earlier definition.");
            }
            foreach(var owner in Owners(xml))foreach(var element in owner.DescendantsAndSelf())
            {
                foreach(var attribute in element.Attributes().Where(a=>!a.IsNamespaceDeclaration))Inspect(attribute.Value,attribute,path,element.Name.LocalName+"/@"+attribute.Name.LocalName,table,localNames);
                if(!element.HasElements)Inspect(element.Value,element,path,element.Name.LocalName+"/text()",table,localNames);
            }
            active.Remove(path);tables.Add(path,table);return table;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: continue after each missing symbol so independent failures and definition-to-definition chains appear together. */
        //-------------------------------------------------------------------------------------------------
        bool Inspect(string value,XObject node,string path,string field,Dictionary<string,Definition> table,HashSet<string> localNames)
        {
            bool valid=true;
            foreach(string name in Symbols(value).Distinct(StringComparer.Ordinal))
                if(!table.TryGetValue(name,out var definition)){Add(localNames.Contains(name)?"FORWARDDEF":"MISSINGDEF","$"+name,node,path,field,localNames.Contains(name)?"Definition is declared later, not visible at this use.":"Undefined definition referenced here.");valid=false;}
                else if(definition.Broken){Add("BLOCKEDDEF","$"+name,node,path,field,"Referenced definition has missing/forward dependencies; see the definition's own use-site diagnostics.");valid=false;}
            return valid;
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: emission follows all edges only and never recursively scans unrelated SDK source directories. */
        //-------------------------------------------------------------------------------------------------
        void Emit(string path){if(!documents.ContainsKey(path)||!emitted.Add(path))return;foreach(var edge in paths.Includes.Where(e=>e.Document.Equals(path,StringComparison.OrdinalIgnoreCase)&&e.Kind=="all"&&e.PhysicalPath!=null))Emit(edge.PhysicalPath!);}

        //-------------------------------------------------------------------------------------------------
        /** Reborn: retain every use site with original XML line information, not only a deduplicated target name. */
        //-------------------------------------------------------------------------------------------------
        void Add(string code,string target,XObject node,string path,string field,string detail,string severity="Error")
        {
            if(findings.Count>=4096)throw new InvalidDataException("Dependency finding limit reached; incomplete audit refused.");
            var line=(IXmlLineInfo)node;var element=node as XElement??(node as XAttribute)?.Parent;
            var owner=element?.AncestorsAndSelf().FirstOrDefault(e=>e.Name==XName.Get("Define",Ea))??element?.AncestorsAndSelf().LastOrDefault(e=>e.Attribute("id")!=null);
            string label=owner==null?"AssetDeclaration":owner.Name.LocalName+":"+(string?)(owner.Attribute("name")??owner.Attribute("id"));
            findings.Add(new(code,severity,target,label,path,line.HasLineInfo()?line.LineNumber:0,line.HasLineInfo()?line.LinePosition:0,field,detail));
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: scan only leading-equals expressions and ignore dollar text inside quoted literals, comments and Lua bodies. */
    //-------------------------------------------------------------------------------------------------
    internal static IEnumerable<string> Symbols(string expression)
    {
        if(!expression.StartsWith('='))yield break;
        for(int index=1;index<expression.Length;index++)
        {
            char c=expression[index];if(c is '\'' or '"'){char quote=c;while(++index<expression.Length){if(expression[index]=='\\'){index++;continue;}if(expression[index]==quote)break;}continue;}
            if(c!='$'||index+1>=expression.Length||!Start(expression[index+1]))continue;
            int begin=++index;while(index+1<expression.Length&&(Start(expression[index+1])||char.IsAsciiDigit(expression[index+1])))index++;
            yield return expression[begin..(index+1)];
        }
        // Reborn: use the same ASCII identifier family as the existing bounded definition evaluator.
        static bool Start(char c)=>char.IsAsciiLetter(c)||c=='_';
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: use actual Core identity hashing, preserving stock identity semantics rather than comparing display strings only. */
    //-------------------------------------------------------------------------------------------------
    internal static Asset Identity(string type,string name)=>new(type,name,BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(type),new InstanceHandle(name).InstanceId);

    // Reborn: standalone assets and declaration-owned assets share one metadata exclusion rule.
    private static IEnumerable<XElement> Owners(XDocument document)=>document.Root?.Name==XName.Get("AssetDeclaration",Ea)?document.Root.Elements().Where(e=>e.Name.NamespaceName==Ea&&e.Name.LocalName is not ("Includes" or "Defines" or "Tags")):document.Root==null?Array.Empty<XElement>():new[]{document.Root};
    // Reborn: only official reference annotations define typed dependency targets.
    private static string? Annotation(XmlAttribute[]? attributes)=>attributes?.FirstOrDefault(a=>a.LocalName=="refType"&&a.NamespaceURI==AnnotationNamespace)?.Value;
    // Reborn: disable external entities and preserve source coordinates without modifying source XML.
    private static XDocument Parse(byte[] bytes,string? path=null)=>SdkXmlDiagnostics.Lines(bytes,path);
}
