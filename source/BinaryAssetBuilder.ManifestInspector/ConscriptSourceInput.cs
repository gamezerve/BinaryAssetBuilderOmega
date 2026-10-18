using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: explicit EA-shaped XML sources replace hidden recruitment declarations; this frontend does not pretend to compile W3X or implement all inheritance semantics.
internal static class ConscriptSourceInput
{
    private const string Ea="uri:ea.com:eala:asset";
    internal sealed record SourceFile(string Path,string Sha256);
    internal sealed record Input(XmlDocument Game,XmlDocument? Command,SourceFile[] Sources)
    {
        // Reborn: source hashes and alias-precedence witnesses are independent publication requirements.
        public SdkSourceSearch.Selection[] Searches { get; init; }=Array.Empty<SdkSourceSearch.Selection>();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: group source-language checks without launching the game or building another ladder of one-field runtime trials. */
    //-------------------------------------------------------------------------------------------------
    internal static void SelfTest(string entry,string output)
    {
        if(Directory.Exists(output) || File.Exists(output))throw new InvalidDataException("Fresh frontend test directory required.");
        var source=Load(entry);var schemas=Schemas();string fixture=Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha","ConscriptBuildTime1","data","mod.xml");
        var variant=Parse(Encoding.UTF8.GetBytes(source.Game.DocumentElement!.OuterXml));var attributes=variant.DocumentElement!.Attributes.OfType<XmlAttribute>().Reverse().ToArray();foreach(var a in attributes)variant.DocumentElement.RemoveAttributeNode(a);foreach(var a in attributes)variant.DocumentElement.Attributes.Append(a);
        Admit(variant,Read(fixture),schemas);variant.DocumentElement.RemoveAttribute("FenceWidth");Admit(variant,Read(fixture),schemas);
        // Reborn: supported primitive defaults are editable through the schema/ABI contract, not compared to a stock-only scalar whitelist.
        variant.DocumentElement.SetAttribute("FenceWidth","1");Admit(variant,Read(fixture),schemas);bool refused=false;
        // Reborn: official boolean lexical forms/default omissions are semantic input, while bad primitive types and overflow still fail.
        variant.DocumentElement.SetAttribute("IsTrainable","1");variant.DocumentElement.RemoveAttribute("BuildTime");Admit(variant,Read(fixture),schemas);if(variant.DocumentElement.GetAttribute("IsTrainable")!="true")throw new InvalidDataException("XML boolean was not canonicalized.");
        foreach(var badValue in new[]{("Scale","NaN"),("TransportSlotCount","256"),("IsTrainable","2"),("EnergyProduction","2147483648")}){var invalid=(XmlDocument)source.Game.CloneNode(true);invalid.DocumentElement!.SetAttribute(badValue.Item1,badValue.Item2);refused=false;try{Admit(invalid,Read(fixture),schemas);}catch(InvalidDataException){refused=true;}if(!refused)throw new InvalidDataException("Invalid scalar accepted: "+badValue.Item1);}
        Directory.CreateDirectory(output);string single=Path.Combine(output,"object-only.xml");File.WriteAllText(single,"<?xml version='1.0'?><!-- Reborn: explicit object-only source must not acquire a generated command. --><AssetDeclaration xmlns='"+Ea+"'><Tags/><Includes/>"+source.Game.DocumentElement.OuterXml+"</AssetDeclaration>");
        var objectOnly=Load(single);if(objectOnly.Command!=null || objectOnly.Sources.Length!=1)throw new InvalidDataException("A hidden command was generated.");
        string cycle=Path.Combine(output,"cycle.xml");File.WriteAllText(cycle,"<!-- Reborn: confined include-cycle refusal control. --><AssetDeclaration xmlns='"+Ea+"'><Includes><Include type='all' source='cycle.xml'/></Includes></AssetDeclaration>");refused=false;try{Load(cycle);}catch(InvalidDataException){refused=true;}if(!refused)throw new InvalidDataException("Include cycle accepted.");
        // Reborn: exercise real instance/definition preparation from authored XML, including observable edits and atomic refusal controls.
        string basePath=Path.Combine(output,"base.xml"),defines=Path.Combine(output,"defines.xml"),derived=Path.Combine(output,"derived.xml");
        var baseXml=Parse(Encoding.UTF8.GetBytes(source.Game.DocumentElement.OuterXml));baseXml.DocumentElement!.SetAttribute("id","BaseConscriptTemplate");
        File.WriteAllText(basePath,"<!-- Reborn: complete authored instance base. --><AssetDeclaration xmlns='"+Ea+"'>"+baseXml.DocumentElement.OuterXml+"</AssetDeclaration>");
        File.WriteAllText(defines,"<!-- Reborn: definition owner. --><AssetDeclaration xmlns='"+Ea+"'><Defines><Define name='BUILD_TIME' value='3'/></Defines></AssetDeclaration>");
        string inherited="<!-- Reborn: actual Core inheritance and imported definition. --><AssetDeclaration xmlns='"+Ea+"'><Includes><Include type='all' source='DATA:defines.xml'/><Include type='instance' source='base.xml'/></Includes><GameObject id='RebornConscriptTrial' inheritFrom='BaseConscriptTemplate' BuildTime='=$BUILD_TIME'/></AssetDeclaration>";
        File.WriteAllText(derived,inherited);var expanded=Load(derived);Admit(expanded.Game,Read(fixture),schemas);
        if(expanded.Game.DocumentElement!.GetAttribute("BuildTime")!="3" || expanded.Game.DocumentElement.HasAttribute("inheritFrom") || expanded.Command!=null || expanded.Sources.Length!=3)throw new InvalidDataException("Instance/definition output or base publication differs.");
        foreach(var invalid in new[]{inherited.Replace("=$BUILD_TIME","=$MISSING"),inherited.Replace("BaseConscriptTemplate' BuildTime","MissingBase' BuildTime"),inherited.Replace("type='instance'","type='reference'"),inherited.Replace("<Includes>","<Tags><Unexpected/></Tags><Includes>")})
        {File.WriteAllText(derived,invalid);refused=false;try{Load(derived);}catch(InvalidDataException){refused=true;}if(!refused)throw new InvalidDataException("Unsupported inheritance/metadata accepted.");}
        Console.WriteLine("EA source frontend: PASS (explicit graph; attribute/default controls; no hidden command; cycle refusal; actual instance base + DATA definition edit; missing definition/base and reference/metadata refusals; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: route EA wrappers/local include graphs without treating namespace declarations as working XInclude or resource processors. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Detect(string entry,string? sdk=null,string? stockData=null)
    {
        var seen=new HashSet<string>(StringComparer.OrdinalIgnoreCase);string root=Path.GetDirectoryName(Path.GetFullPath(entry))!;
        var search=sdk==null?null:SdkSourceSearch.ForMod(entry,sdk,stockData);
        return Visit(Path.GetFullPath(entry),root);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: inspect only confined XML include candidates; actual admission still validates every directive in Prepare. */
        //-------------------------------------------------------------------------------------------------
        bool Visit(string path,string confinement)
        {
            if(!seen.Add(path))return false;if(seen.Count>32)throw new InvalidDataException("Source routing exceeds 32 XML files.");
            var xml=Read(path);var element=xml.DocumentElement!;
            if(element.NamespaceURI!=Ea)return false;
            if(element.LocalName=="GameObject" || element.ChildNodes.OfType<XmlElement>().Any(e=>e.NamespaceURI==Ea && e.LocalName=="GameObject"))return true;
            foreach(XmlElement include in element.GetElementsByTagName("Include",Ea))
            {
                string source=include.GetAttribute("source");if(!source.EndsWith(".xml",StringComparison.OrdinalIgnoreCase))continue;
                if(search!=null){var selected=search.Select(path,source,confinement);if(Visit(selected.PhysicalPath,selected.Root))return true;}
                else if(source.StartsWith("DATA:",StringComparison.OrdinalIgnoreCase)){if(Visit(Resolve(root,Path.Combine(root,"mod.xml"),source[5..]),root))return true;}
                else if(!source.Contains(':') && Visit(Resolve(root,path,source),root))return true;
            }
            return false;
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare one explicit GameObject and optional UNIT_BUILD command from AssetDeclaration/all-XML includes, preserving source provenance and significant scripts. */
    //-------------------------------------------------------------------------------------------------
    internal static void Prepare(string entry,string output,string? sdk=null,string? stockData=null)
    {
        if(Directory.Exists(output) || File.Exists(output))throw new InvalidDataException("Fresh source-preparation output required.");
        var input=ConscriptSourcePreparation.Load(entry,sdk==null?null:SdkSourceSearch.ForMod(entry,sdk,stockData));var schemas=Schemas();
        var baseline=Read(Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"live-alpha","ConscriptBuildTime1","data","mod.xml"));
        Admit(input.Game,baseline,schemas);
        if(input.Command!=null){input.Command.Schemas.Add(schemas);input.Command.Validate((_,e)=>throw new InvalidDataException(e.Message));}
        Directory.CreateDirectory(output);string game=Path.Combine(output,"game.xml"),command=Path.Combine(output,"command.xml");
        File.WriteAllText(game,input.Game.DocumentElement!.OuterXml,new UTF8Encoding(false));if(input.Command!=null)File.WriteAllText(command,input.Command.DocumentElement!.OuterXml,new UTF8Encoding(false));
        foreach(var source in input.Sources)if(Convert.ToHexString(SHA256.HashData(SdkEnvironmentPreflight.Read(source.Path,1048576)))!=source.Sha256)throw new InvalidDataException("Source changed during preparation: "+source.Path);
        File.WriteAllText(Path.Combine(output,"source-input.json"),JsonSerializer.Serialize(new{GamePath=game,CommandPath=input.Command==null?null:command,Identity=input.Game.DocumentElement.GetAttribute("id"),PreparedSourceSha256=Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input.Game.DocumentElement.OuterXml))),Sources=input.Sources,Searches=input.Searches,AutomaticCommandGenerated=false},new JsonSerializerOptions{WriteIndented=true}));
        Console.WriteLine("Reborn: explicit EA XML prepared; "+input.Sources.Length+" source file(s), "+(input.Command==null?"one GameObject; no command generated.":"GameObject + authored LogicCommand; no hidden asset generation."));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate schema defaults on both documents and compare structure, not attribute order, formatting, comments or xmlns declaration placement. */
    //-------------------------------------------------------------------------------------------------
    internal static void Admit(XmlDocument input,XmlDocument baseline,XmlSchemaSet schemas)
    {
        // Reborn: materialize defaults only in comparison copies; emitting optional empty-string defaults would create extra native allocations in legacy marshalers.
        var qualified=Parse(Encoding.UTF8.GetBytes(input.DocumentElement!.OuterXml));var model=Parse(Encoding.UTF8.GetBytes(baseline.DocumentElement!.OuterXml));
        foreach(var doc in new[]{qualified,model}){doc.Schemas.Add(schemas);doc.Validate((_,e)=>throw new InvalidDataException(e.Message));foreach(XmlAttribute attribute in doc.SelectNodes("//@*")!)attribute.Value=attribute.Value;}
        if(!Identity(input.DocumentElement!.GetAttribute("id")))throw new InvalidDataException("GameObject ID must be an ASCII identifier of 1..128 characters.");
        // Reborn: schema/native primitive capabilities validate all supported root values before structural masking; no gameplay-specific numeric caps are introduced.
        GameObjectScalarContract.Validate(input);
        // Reborn: fixed EP1 enums, KindOf masks and weak IDs use their schema/native contract rather than remaining trial-shape differences.
        GameObjectRootAttributeContract.Validate(input);
        // Reborn: existing root imports may retarget assets without changing dependency count or traversal topology.
        GameObjectStrongReferenceContract.Validate(input,baseline);
        // Reborn: existing root string allocations may edit bounded ASCII content without moving graph owners.
        GameObjectRootStringContract.Validate(input,baseline);
        foreach(var field in ConscriptEditableScalars.Fields)ConscriptEditableScalars.Read(qualified,field);
        // Reborn: materialize schema-added default attributes before cloning; XmlDocument.CloneNode can lose their default value children.
        var copy=Parse(Encoding.UTF8.GetBytes(qualified.DocumentElement!.OuterXml));copy.DocumentElement!.SetAttribute("id",model.DocumentElement!.GetAttribute("id"));
        foreach(var field in ConscriptEditableScalars.Fields)ConscriptEditableScalars.Element(copy,field).SetAttribute(field.Attribute,ConscriptEditableScalars.Element(model,field).GetAttribute(field.Attribute));
        GameObjectScalarContract.Mask(copy,model);
        GameObjectRootAttributeContract.Mask(copy,model);
        GameObjectStrongReferenceContract.Mask(copy,model);
        GameObjectRootStringContract.Mask(copy,model);
        if(Signature(copy.DocumentElement)!=Signature(model.DocumentElement))throw new InvalidDataException("GameObject requires an unsupported structural/reference/module capability: "+Difference(copy.DocumentElement,model.DocumentElement,"GameObject")+". Schema-valid primitive edits are enabled; general graph changes are not enabled yet.");
        // Reborn: make attribute traversal deterministic before Core assigns strong-reference occurrence indices; caller formatting must not reorder imports.
        Align(input.DocumentElement,baseline.DocumentElement);
        //-------------------------------------------------------------------------------------------------
        /** Reborn: preserve element/list order and authored values while putting attributes in one qualified graph traversal order. */
        //-------------------------------------------------------------------------------------------------
        static void Align(XmlElement element,XmlElement model){var order=model.Attributes.OfType<XmlAttribute>().Select((a,i)=>(Key:a.NamespaceURI+"|"+a.LocalName,Index:i)).ToDictionary(a=>a.Key,a=>a.Index);var attributes=element.Attributes.OfType<XmlAttribute>().OrderBy(a=>order.GetValueOrDefault(a.NamespaceURI+"|"+a.LocalName,int.MaxValue)).ToArray();foreach(var a in attributes)element.RemoveAttributeNode(a);foreach(var a in attributes)element.Attributes.Append(a);var children=element.ChildNodes.OfType<XmlElement>().ToArray();var models=model.ChildNodes.OfType<XmlElement>().ToArray();for(int i=0;i<children.Length;i++)Align(children[i],models[i]);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: custom IDs are source-authored rather than a magic trial name; runtime catalog collisions are checked before compiling them. */
    //-------------------------------------------------------------------------------------------------
    internal static bool Identity(string id)=>Regex.IsMatch(id,"^[A-Za-z_][A-Za-z0-9_]{0,127}$",RegexOptions.CultureInvariant);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: name the first unsupported source difference without dumping the full object/script or hiding it behind a fingerprint error. */
    //-------------------------------------------------------------------------------------------------
    private static string Difference(XmlElement actual,XmlElement model,string path)
    {
        foreach(var a in actual.Attributes.OfType<XmlAttribute>().Concat(model.Attributes.OfType<XmlAttribute>()).Where(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/").DistinctBy(a=>(a.NamespaceURI,a.LocalName)))if(actual.GetAttribute(a.LocalName,a.NamespaceURI)!=model.GetAttribute(a.LocalName,a.NamespaceURI))return path+"/@"+a.LocalName+" (authored="+actual.GetAttribute(a.LocalName,a.NamespaceURI)+", baseline="+model.GetAttribute(a.LocalName,a.NamespaceURI)+")";
        var children=actual.ChildNodes.OfType<XmlElement>().ToArray();var models=model.ChildNodes.OfType<XmlElement>().ToArray();if(children.Length!=models.Length)return path+" child count";
        if(children.Length==0 && actual.InnerText!=model.InnerText)return path+" text (length "+actual.InnerText.Length+" versus "+model.InnerText.Length+")";
        for(int i=0;i<children.Length;i++){if(children[i].LocalName!=models[i].LocalName || children[i].NamespaceURI!=models[i].NamespaceURI)return path+" child order at "+i;if(Signature(children[i])!=Signature(models[i]))return Difference(children[i],models[i],path+"/"+children[i].LocalName+"["+i+"]");}
        return path+" attribute/text shape";
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prepare explicit all/instance XML sources and bounded definitions; unsupported resource processors still fail explicitly. */
    //-------------------------------------------------------------------------------------------------
    internal static Input Load(string entry)=>ConscriptSourcePreparation.Load(entry);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: accept normal namespace declarations but reject unknown wrapper fields and mixed text rather than silently discarding malformed metadata. */
    //-------------------------------------------------------------------------------------------------
    internal static IEnumerable<XmlElement> Envelope(XmlElement root)
    {
        foreach(XmlAttribute attribute in root.Attributes)if(attribute.NamespaceURI!="http://www.w3.org/2000/xmlns/" && !(attribute.NamespaceURI=="http://www.w3.org/2001/XMLSchema-instance" && attribute.LocalName is "schemaLocation" or "noNamespaceSchemaLocation"))throw new InvalidDataException("Unsupported AssetDeclaration attribute: "+attribute.Name);
        if(root.ChildNodes.Cast<XmlNode>().Any(n=>n is not XmlElement && n is not XmlComment && !string.IsNullOrWhiteSpace(n.InnerText)))throw new InvalidDataException("AssetDeclaration mixed text is invalid.");
        return root.ChildNodes.OfType<XmlElement>();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: namespaces are compared by expanded names; retain element order and leaf/script text rather than inventing commutative module/list semantics. */
    //-------------------------------------------------------------------------------------------------
    private static string Signature(XmlElement root)
    {
        var output=new StringBuilder();Append(root);return output.ToString();
        //-------------------------------------------------------------------------------------------------
        /** Reborn: length-prefixed tokens prevent delimiter collisions while ignoring only nonsemantic formatting/namespace declaration attributes. */
        //-------------------------------------------------------------------------------------------------
        void Token(string value){output.Append(value.Length).Append(':').Append(value);}
        //-------------------------------------------------------------------------------------------------
        /** Reborn: comments/indentation between elements are not asset content; literal leaf text remains significant. */
        //-------------------------------------------------------------------------------------------------
        void Append(XmlElement element){Token(element.NamespaceURI);Token(element.LocalName);foreach(var attribute in element.Attributes.OfType<XmlAttribute>().Where(a=>a.NamespaceURI!="http://www.w3.org/2000/xmlns/").OrderBy(a=>a.NamespaceURI,StringComparer.Ordinal).ThenBy(a=>a.LocalName,StringComparer.Ordinal)){Token(attribute.NamespaceURI);Token(attribute.LocalName);Token(attribute.Value);}Token("children");var children=element.ChildNodes.OfType<XmlElement>().ToArray();if(children.Length==0)Token(element.InnerText);else foreach(XmlNode child in element.ChildNodes){if(child is XmlElement next)Append(next);else if(child is not XmlComment && !string.IsNullOrWhiteSpace(child.InnerText))Token(child.InnerText);}Token("end");}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate against the unchanged admitted official types with standalone aliases only; never fetch xsi:schemaLocation over the network. */
    //-------------------------------------------------------------------------------------------------
    internal static XmlSchemaSet Schemas()
    {
        XmlSchemaSet? official=null;SdkEffectiveSchema.Inspect(shieldCandidate:true,reviewHooks:true,onAdmitted:s=>official=s);if(official==null)throw new InvalidDataException("Reviewed EP1 schemas unavailable.");
        var aliases=new XmlSchema{TargetNamespace=Ea,ElementFormDefault=XmlSchemaForm.Qualified};foreach(string name in new[]{"GameObject","LogicCommand"})aliases.Items.Add(new XmlSchemaElement{Name=name,SchemaTypeName=new XmlQualifiedName(name,Ea)});official.Add(aliases);official.Compile();return official;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: includes are relative to their declaring XML and confined to the selected mod data root; DATA/ART aliases need a separate source-root pipeline. */
    //-------------------------------------------------------------------------------------------------
    private static string Resolve(string root,string owner,string source)
    {
        if(string.IsNullOrWhiteSpace(source) || source.Contains(':') || Path.IsPathRooted(source) || !source.EndsWith(".xml",StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Only local .xml Includes are enabled, not DATA:/ART:/W3X: "+source);
        string path=Path.GetFullPath(Path.Combine(Path.GetDirectoryName(owner)!,source.Replace('/',Path.DirectorySeparatorChar)));
        if(!path.StartsWith(root+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new InvalidDataException("Include escapes mod data root: "+source);SdkEnvironmentPreflight.CheckPath(path);return path;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: detach authored assets from envelopes without replacing their element namespace or copying unrelated declarations. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Detach(XmlElement element){var doc=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};doc.AppendChild(doc.ImportNode(element,true));if(doc.DocumentElement!.Prefix.Length==0 && !doc.DocumentElement.HasAttribute("xmlns"))doc.DocumentElement.SetAttribute("xmlns",Ea);return doc;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bounded source reads reject DTD/entities, preserve scripts and accept normal XML declarations/comments. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Read(string path)=>Parse(SdkEnvironmentPreflight.Read(path,1048576));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: xsi/xi namespace declarations alone do not execute remote schema fetching or XInclude. */
    //-------------------------------------------------------------------------------------------------
    private static XmlDocument Parse(byte[] bytes){using var stream=new MemoryStream(bytes,false);using var reader=XmlReader.Create(stream,new XmlReaderSettings{DtdProcessing=DtdProcessing.Prohibit,XmlResolver=null,MaxCharactersInDocument=1048576});var doc=new XmlDocument{PreserveWhitespace=true,XmlResolver=null};doc.Load(reader);return doc;}
}
