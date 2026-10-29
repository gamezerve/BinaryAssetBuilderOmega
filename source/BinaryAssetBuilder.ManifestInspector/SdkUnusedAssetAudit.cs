using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: advisory unused detection joins final static/global assets, retail graphs and schema-typed weak uses; it never deletes assets.
internal static class SdkUnusedAssetAudit
{
    internal sealed record WeakUse(AssetId Owner,AssetId[] Targets);
    // Reborn: UI templates selected through command/power registries cannot be judged unused solely by missing incoming asset edges.
    // UnitAbilityButtonTemplate has a required LogicCommand selector; Joypad templates also have direct layout refs, but ability/power variants need not have an incoming one.
    private static readonly HashSet<string> RuntimeSelectedUiTypes=new(StringComparer.Ordinal){"UnitAbilityButtonTemplate","JoypadCommandBarButtonTemplate"};
    private const string Ea="uri:ea.com:eala:asset";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: weak IDs are valid use sites even though they do not become native IMP records; resolve concrete targets using the same schema inheritance as Core. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetId[] WeakReferences(XmlNode node,XmlDocument fallback,SchemaSet schemas,ManifestAsset[] available)
    {
        if(node.SchemaInfo?.SchemaType==null){var copy=new XmlDocument{XmlResolver=null};copy.LoadXml(fallback.DocumentElement!.OuterXml);copy.Schemas.Add(schemas.Schemas);copy.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));node=copy.DocumentElement!;}
        var weak=schemas.Schemas.GlobalTypes[new XmlQualifiedName("WeakReference",Ea)] as XmlSchemaType;if(weak==null)throw new InvalidDataException("WeakReference schema is missing.");
        var catalog=available.GroupBy(a=>a.InstanceId).ToDictionary(g=>g.Key,g=>g.ToArray());var targets=new HashSet<AssetId>();
        foreach(XmlNode field in node.SelectNodes("descendant-or-self::*|descendant-or-self::*/@*")!){var info=field.SchemaInfo;var type=info?.SchemaType;if(type==null||!XmlSchemaType.IsDerivedFrom(type,weak,XmlSchemaDerivationMethod.None))continue;
            string? required=Annotation(info?.SchemaAttribute?.UnhandledAttributes)??Annotation(info?.SchemaElement?.UnhandledAttributes);for(var current=type;required==null&&current!=null;){required=Annotation(current.UnhandledAttributes);var next=current.BaseXmlSchemaType;if(ReferenceEquals(next,current))break;current=next;}
            if(required==null||schemas.GetXmlType(required) is not XmlSchemaType expected)continue;string raw=(field is XmlAttribute attribute?attribute.Value:field.InnerText).Trim();
            foreach(string value in type.Datatype?.Variety==XmlSchemaDatatypeVariety.List?raw.Split((char[]?)null,StringSplitOptions.RemoveEmptyEntries):new[]{raw}){if(value.Length==0||value.StartsWith('='))continue;int colon=value.IndexOf(':');string name=colon<0?value:value[(colon+1)..];var selected=colon<0?expected:schemas.GetXmlType(value[..colon]);if(selected==null)continue;
                foreach(var asset in catalog.GetValueOrDefault(InstanceHandle.GetInstanceId(name),Array.Empty<ManifestAsset>()))if(schemas.GetXmlType(asset.TypeName) is XmlSchemaType actual&&XmlSchemaType.IsDerivedFrom(actual,expected,XmlSchemaDerivationMethod.None)&&XmlSchemaType.IsDerivedFrom(actual,selected,XmlSchemaDerivationMethod.None))targets.Add(new(asset.TypeId,asset.InstanceId));
            }
        }return targets.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: capture effective normalized weak references while schemas/catalogs are already available; final-package auditing needs no second source build. */
    //-------------------------------------------------------------------------------------------------
    internal static void WriteUsage(string output,IEnumerable<(InstanceDeclaration Instance,XmlDocument Source)> built,SchemaSet schemas,ManifestAsset[] available)
    {
        var uses=built.Select(b=>new WeakUse(new(b.Instance.Handle.TypeId,b.Instance.Handle.InstanceId),WeakReferences(b.Instance.XmlNode,b.Source,schemas,available))).ToArray();Directory.CreateDirectory(output);using var file=new FileStream(Path.Combine(output,"asset-usage.json"),FileMode.CreateNew,FileAccess.Write,FileShare.None);JsonSerializer.Serialize(file,uses);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: statically classifiable types use the incoming-reference rule; runtime-selected UI/targeted experience templates and existing retail identities remain entry points, and replaced stock owners cannot keep obsolete edges alive. */
    //-------------------------------------------------------------------------------------------------
    internal static ManifestAsset[] FindUnused(IEnumerable<ManifestAsset> mod,IEnumerable<ManifestAsset> stock,IEnumerable<WeakUse> weak,IEnumerable<SdkSelfAttributeInheritance.Overlay>? inheritance=null)
    {
        var emitted=mod.ToArray();var originals=stock.ToArray();var weakUses=weak.ToArray();var local=emitted.Select(WorldBuilderAssetResolver.Id).ToHashSet();var retail=originals.Select(WorldBuilderAssetResolver.Id).ToHashSet();var used=emitted.SelectMany(a=>a.References).Concat(originals.Where(a=>!local.Contains(WorldBuilderAssetResolver.Id(a))).SelectMany(a=>a.References)).Concat(weakUses.Where(w=>local.Contains(w.Owner)).SelectMany(w=>w.Targets)).ToHashSet();
        // Reborn: flattening consumes inheritFrom before native imports exist; retain the actual source base chain rooted in emitted mod assets.
        used.UnionWith(InheritedTargets(local,inheritance??Array.Empty<SdkSelfAttributeInheritance.Overlay>()));
        // Reborn: experience tables are selected by their outgoing GameObject Target and rank; terminal ranks need no incoming prerequisite edge.
        uint gameObjectType=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("GameObject"),experienceType=BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode("ExperienceLevelTemplate");
        var knownTargets=emitted.Concat(originals).Where(a=>a.TypeId==gameObjectType).Select(WorldBuilderAssetResolver.Id).ToHashSet();
        var runtimeExperience=weakUses.Where(w=>w.Owner.TypeId==experienceType&&local.Contains(w.Owner)&&w.Targets.Any(t=>knownTargets.Contains(t))).Select(w=>w.Owner).ToHashSet();
        return emitted.DistinctBy(WorldBuilderAssetResolver.Id).Where(a=>!RuntimeSelectedUiTypes.Contains(a.TypeName)&&!runtimeExperience.Contains(WorldBuilderAssetResolver.Id(a))&&!retail.Contains(WorldBuilderAssetResolver.Id(a))&&!used.Contains(WorldBuilderAssetResolver.Id(a))).OrderBy(a=>a.Name,StringComparer.Ordinal).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: follow witnessed inheritance through source-only intermediates without treating unused instance-library declarations as consumers. */
    //-------------------------------------------------------------------------------------------------
    private static HashSet<AssetId> InheritedTargets(IEnumerable<AssetId> roots,IEnumerable<SdkSelfAttributeInheritance.Overlay> overlays)
    {
        var edges=overlays.Distinct().GroupBy(e=>Id(e.Type,e.DerivedId)).ToDictionary(g=>g.Key,g=>g.Select(e=>Id(e.Type,e.BaseId)).Distinct().ToArray());
        var visited=new HashSet<AssetId>();var used=new HashSet<AssetId>();var queue=new Queue<AssetId>(roots);
        while(queue.TryDequeue(out var owner))if(visited.Add(owner)&&edges.TryGetValue(owner,out var targets))foreach(var target in targets)if(target!=owner){used.Add(target);queue.Enqueue(target);}
        return used;
        // Reborn: source overlay handles use the same typed hashes as emitted and stock manifest assets.
        static AssetId Id(string type,string name)=>new(BinaryAssetBuilder.Core.Hashing.FastHash.GetHashCode(type),InstanceHandle.GetInstanceId(name));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: audit the final assembled package once so later static consumers can use early globals; report original source coordinates where provenance exists. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string package,string sdk,string buildRoot,string entry)
    {
        var mod=new List<ManifestAsset>();using(var archive=BigArchive.Open(package))foreach(var member in archive.Entries.Where(e=>e.Name.EndsWith(".manifest",StringComparison.OrdinalIgnoreCase))){var document=ManifestReader.Read(archive.ReadEntry(member,checked((int)member.StoredSize)));TypeRegistryAudit.ValidateTarget(document.Header.Version,document.Header.AllTypesHash);if(document.Validate().Count!=0)throw new InvalidDataException("Invalid final package manifest: "+member.Name);mod.AddRange(document.Assets);}
        var stock=SdkMissingDependencyAudit.CatalogManifestPaths(sdk).SelectMany(path=>ManifestReader.Read(File.ReadAllBytes(path)).Assets).ToArray();var weak=Directory.GetFiles(buildRoot,"asset-usage.json",SearchOption.AllDirectories).SelectMany(path=>JsonSerializer.Deserialize<WeakUse[]>(File.ReadAllBytes(path))??throw new InvalidDataException("Invalid asset usage receipt: "+path)).ToArray();
        // Reborn: new preparation receipts retain logical inheritance edges; older receipts remain valid with no such evidence.
        var inheritance=new List<SdkSelfAttributeInheritance.Overlay>();var origins=new List<ConscriptCompileLog>();foreach(string receipt in Directory.GetFiles(buildRoot,"source-input.json",SearchOption.AllDirectories).OrderBy(p=>p,StringComparer.Ordinal)){using var json=JsonDocument.Parse(File.ReadAllBytes(receipt));origins.Add(ConscriptCompileLog.ForPrepared(json.RootElement.GetProperty("GamePath").GetString()!));if(json.RootElement.TryGetProperty("InheritanceUses",out var uses))inheritance.AddRange(uses.Deserialize<SdkSelfAttributeInheritance.Overlay[]>()??Array.Empty<SdkSelfAttributeInheritance.Overlay>());}var fallback=ConscriptCompileLog.ForPrepared(entry);var findings=new List<object>();
        foreach(var asset in FindUnused(mod,stock,weak,inheritance)){int colon=asset.Name.IndexOf(':');string id=colon<0?asset.Name:asset.Name[(colon+1)..];var origin=(origins.LastOrDefault(o=>o.HasOrigin(asset.TypeName,id))??fallback).Location(asset.TypeName,id);Console.WriteLine("Warning: UNUSEDASSET ' "+asset.Name+" ' has no static or inheritance reference in emitted mod assets or stock catalogs; dynamic/script use may exist. Source: "+origin+".");findings.Add(new{asset.Name,origin.File,origin.Line,origin.Column,origin.Context});}
        File.WriteAllText(Path.Combine(buildRoot,"unused-assets.json"),JsonSerializer.Serialize(new{ModAssets=mod.Select(WorldBuilderAssetResolver.Id).Distinct().Count(),WeakEdges=weak.Sum(w=>w.Targets.Length),InheritanceEdges=inheritance.Distinct().Count(),Findings=findings,Advisory=true,GameExecuted=false},new JsonSerializerOptions{WriteIndented=true}));Console.WriteLine("Asset usage audit: "+mod.Select(WorldBuilderAssetResolver.Id).Distinct().Count()+" packaged assets; "+findings.Count+" without static or inheritance references.");
    }

    // Reborn: only the official schema reference annotation supplies the required target family.
    private static string? Annotation(XmlAttribute[]? attributes)=>attributes?.FirstOrDefault(a=>a.LocalName=="refType"&&a.NamespaceURI=="uri:ea.com:eala:asset:schema")?.Value;
}
