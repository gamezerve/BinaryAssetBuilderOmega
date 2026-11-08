using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;
using System.Buffers.Binary;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: resolve missing runtime assets from linked EP1 WorldBuilder data without loading its entire BIN or rebuilding retail XML.
internal static class WorldBuilderAssetResolver
{
    internal sealed record Native(ManifestAsset Asset,byte[] Bin,byte[] Relo,byte[] Imp);
    // Reborn: authored chunks can borrow their existing sidecars while retail chunks retain their owned arrays.
    private sealed record NativeView(ManifestAsset Asset,ReadOnlyMemory<byte> Bin,ReadOnlyMemory<byte> Relo,ReadOnlyMemory<byte> Imp);
    internal sealed record Result(Dictionary<string,byte[]> Streams,Dictionary<string,byte[]> CustomData,string[] Imported,long BytesRead,string[]? ReferencedWorldBuilder=null);
    private static readonly string[] Extensions={".manifest",".bin",".relo",".imp"};

    //-------------------------------------------------------------------------------------------------
    /** Reborn: warn only about pending WorldBuilder use sites absent from both the completed package and loaded retail catalogs. */
    //-------------------------------------------------------------------------------------------------
    internal static SdkMissingDependencyAudit.Finding[] Unresolved(IEnumerable<SdkMissingDependencyAudit.Finding> findings,IEnumerable<AssetId> available)
    {
        var present=available.ToHashSet();return findings.Where(f=>f.Code=="WORLDBUILDERASSET").Where(f=>{int colon=f.Target.IndexOf(':');if(colon<=0)throw new InvalidDataException("Invalid WorldBuilder diagnostic identity: "+f.Target);var asset=SdkMissingDependencyAudit.Identity(f.Target[..colon],f.Target[(colon+1)..]);return !present.Contains(new(asset.TypeId,asset.InstanceId));}).Select(f=>f with{Severity="Warning",Detail="remains unresolved after WorldBuilder resolution; it is absent from the compiled mod and loaded stock streams."}).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the final assembled BIG is authoritative, including earlier mod-global streams; WorldBuilder metadata itself is not a runtime provider. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckPending(string package,string dependencyReport,string catalogs)
    {
        var report=System.Text.Json.JsonSerializer.Deserialize<SdkMissingDependencyAudit.Report>(File.ReadAllText(dependencyReport))??throw new InvalidDataException("Dependency report is missing.");if(!report.Findings.Any(f=>f.Code=="WORLDBUILDERASSET"))return;
        var available=new HashSet<AssetId>();foreach(string name in new[]{"static.manifest","global.manifest","audio.manifest"}){string path=Path.Combine(catalogs,name);if(File.Exists(path))foreach(var asset in ManifestReader.Read(File.ReadAllBytes(path)).Assets)available.Add(Id(asset));}
        using var archive=BigArchive.Open(package);foreach(var entry in archive.Entries.Where(e=>e.Name.EndsWith(".manifest",StringComparison.OrdinalIgnoreCase)))foreach(var asset in ManifestReader.Read(archive.ReadEntry(entry,checked((int)entry.StoredSize))).Assets)available.Add(Id(asset));foreach(var finding in Unresolved(report.Findings,available))Console.WriteLine(SdkMissingDependencyAudit.FormatFinding(finding));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: locate WorldBuilder metadata independently of whether an optional AudioFile lookup catalog exists. */
    //-------------------------------------------------------------------------------------------------
    internal static string CatalogPath()
    {
        // Reborn: portable fixtures live inside the SDK, so resolve its own catalog before the legacy repository location.
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent)
            foreach(string relative in new[]{"catalogs/ra3ep1/worldbuilder.manifest","alpha/catalogs/ra3ep1/worldbuilder.manifest"}){
                string path=Path.Combine(folder.FullName,relative);if(File.Exists(path))return path;
            }
        throw new FileNotFoundException("WorldBuilder resolution requires catalogs/ra3ep1/worldbuilder.manifest inside the SDK.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: opt into a local retail art catalog for the exact map ID being compiled. */
    //-------------------------------------------------------------------------------------------------
    internal static string? MapArtPath(string catalogDirectory,string? mapId)
    {
        if (mapId == null || !ConscriptSourceInput.Identity(mapId))
            return null;

        string path = Path.Combine(catalogDirectory,"map-art",mapId,"map.manifest");
        return File.Exists(path) ? path : null;
    }

    // Reborn: retail map catalogs supply only models and their textures.
    internal static bool IsMapArt(ManifestAsset asset) => asset.InstanceDataSize > 0 && asset.TypeName is
        "W3DContainer" or "W3DHierarchy" or "W3DMesh" or "W3DCollisionBox" or "W3DAnimation" or "Texture";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: the retail map is a WorldBuilder patch. Only entries with their own native bytes supply map art. */
    //-------------------------------------------------------------------------------------------------
    internal static ManifestDocument ReadMapArt(string path)
    {
        var map = ManifestReader.Read(SdkEnvironmentPreflight.Read(path,64*1048576));
        TypeRegistryAudit.ValidateTarget(map.Header.Version,map.Header.AllTypesHash);
        if (!map.Header.IsLinked || map.Validate().Count != 0 ||
            map.ReferencedManifests.Any(r => r.IsPatch && !r.Path.Equals("worldbuilder.manifest",StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException("Valid linked EP1 map art catalog required: "+path);
        return map;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve native relocation/import tables while copying only the retail assets actually needed. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Resolve(Dictionary<string,byte[]> streams,string worldBuilder,IEnumerable<ManifestAsset> loaded,IEnumerable<AssetId> weakSeeds,IReadOnlyDictionary<AssetId,string>? runtimeProviders=null,string? mapArt=null,bool referenceWorldBuilder=false)
    {
        var mod = ManifestReader.Read(streams["diagnostic.manifest"]);
        var catalogs = new Dictionary<string,ManifestDocument>(StringComparer.OrdinalIgnoreCase)
        {
            [worldBuilder] = ManifestReader.Read(File.ReadAllBytes(worldBuilder))
        };
        foreach (var doc in catalogs.Values.Prepend(mod))
        {
            TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);
            if (!doc.Header.IsLinked || doc.Validate().Count != 0 || doc.ReferencedManifests.Any(r => r.IsPatch && !(referenceWorldBuilder && ReferenceEquals(doc,mod) && r.Path.Equals("worldbuilder.manifest",StringComparison.OrdinalIgnoreCase))))
                throw new InvalidDataException("Retail asset resolution requires valid linked EP1 streams; only the retail WorldBuilder map patch base is admitted.");
        }
        if (mapArt != null)
            catalogs.Add(mapArt,ReadMapArt(mapArt));

        var present = mod.Assets.Concat(loaded).Select(Id).ToHashSet();
        var byId = catalogs[worldBuilder].Assets.ToDictionary(Id,asset => (Asset:asset,Path:worldBuilder));
        if (mapArt != null)
            foreach (var asset in catalogs[mapArt].Assets.Where(IsMapArt))
                byId[Id(asset)] = (asset,mapArt);
        var selected = new List<(ManifestAsset Asset,string Path)>();
        var visited = new HashSet<AssetId>();
        foreach (var seed in mod.Assets.SelectMany(a => a.References).Concat(weakSeeds))
            Visit(seed);
        // Reborn: linked patch loaders scan the base forwards; reject an already malformed patch even when no dependencies are added.
        if (referenceWorldBuilder && mod.ReferencedManifests.Any(r=>r.IsPatch))
            ValidateMapBaseOrder(mod,catalogs[worldBuilder]);
        if (selected.Count == 0)
            return new(streams,new(),Array.Empty<string>(),0);

        var native = new List<Native>();
        // Reborn: map-only base records retain retail fingerprints/reference tables with no copied native or CDATA bytes.
        var referencedWorldBuilder = new List<string>();
        var custom = new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
        long bytesRead = 0;
        foreach (var group in selected.GroupBy(item => item.Path,StringComparer.OrdinalIgnoreCase))
        {
            var retail = catalogs[group.Key];
            // Reborn: match shipped map manifests: zero-sized entries borrow their native records from worldbuilder.manifest.
            if (referenceWorldBuilder && group.Key.Equals(worldBuilder,StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in group)
                {
                    var asset=item.Asset with { InstanceDataSize=0,RelocationDataSize=0,ImportsDataSize=0 };
                    native.Add(new(asset,Array.Empty<byte>(),Array.Empty<byte>(),Array.Empty<byte>()));
                    referencedWorldBuilder.Add(asset.Name);
                }
                continue;
            }
            var offsets = new Dictionary<AssetId,long[]>();
            long[] at = {8,8,8};
            foreach (var asset in retail.Assets)
            {
                offsets.Add(Id(asset),at.ToArray());
                at[0] = checked(at[0]+asset.InstanceDataSize);
                at[1] = checked(at[1]+asset.RelocationDataSize);
                at[2] = checked(at[2]+asset.ImportsDataSize);
            }

            var readers = new FileStream[3];
            try
            {
                for (int i=0;i<3;i++)
                {
                    string path = Path.ChangeExtension(group.Key,Extensions[i+1]);
                    readers[i] = File.OpenRead(path);
                    byte[] header = new byte[8];
                    readers[i].ReadExactly(header);
                    uint magic = i==0 ? 0xBABB0000u : i==1 ? 0xBABE0000u : 0xBAB10000u;
                    if (Word(header,0)!=magic || Word(header,4)!=retail.Header.StreamChecksum || readers[i].Length!=at[i])
                        throw new InvalidDataException("Retail linked sidecar header/length differs: "+path);
                }

                foreach (var (asset,_) in group)
                {
                    var sizes = new[] {asset.InstanceDataSize,asset.RelocationDataSize,asset.ImportsDataSize};
                    var parts = new byte[3][];
                    for (int i=0;i<3;i++)
                    {
                        parts[i] = new byte[sizes[i]];
                        readers[i].Position = offsets[Id(asset)][i];
                        readers[i].ReadExactly(parts[i]);
                        bytesRead += sizes[i];
                    }
                    ValidateNative(asset,parts[0],parts[1],parts[2]);
                    native.Add(new(asset,parts[0],parts[1],parts[2]));
                    // Reborn: external texture resources stay with the catalog supplying their corresponding native record.
                    string name = $"{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata";
                    string data = Path.Combine(Path.GetDirectoryName(group.Key)!,"cdata",name);
                    bool embedded = asset.TypeName=="Texture" && parts[0].Length>=12 && Word(parts[0],4)==12 && Word(parts[0],8)==parts[0].Length-12;
                    if (File.Exists(data))
                        custom.Add("data\\mod\\cdata\\"+name,File.ReadAllBytes(data));
                    else if (asset.TypeName=="AudioFile" || asset.TypeName=="Texture" && !embedded)
                        throw new FileNotFoundException("Retail asset requires external custom data: "+asset.Name+"; expected "+data,data);
                }
            }
            finally
            {
                foreach (var reader in readers)
                    reader?.Dispose();
            }
        }
        // Reborn: imported dependencies precede authored consumers; reorder whole asset chunks without changing their asset-relative offsets or reference indices.
        var candidates=ReadNativeViews(mod,streams).Concat(native.Select(n=>new NativeView(n.Asset,n.Bin,n.Relo,n.Imp))).ToDictionary(n=>Id(n.Asset));var sorted=new List<NativeView>();var ordering=new HashSet<AssetId>();foreach(var id in candidates.Keys)Order(id);var all=sorted.ToArray();
        // Reborn: dependency DFS can reverse borrowed records; the engine requires those records to follow the retail base order.
        if (referenceWorldBuilder) all=OrderMapBase(all,catalogs[worldBuilder]);
        // Reborn: imported assets can introduce an AudioFile or other external provider not used by the authored source; add only the newly required runtime names.
        var runtimeAdditions=native.SelectMany(n=>n.Asset.References).Where(id=>!candidates.ContainsKey(id)&&runtimeProviders?.ContainsKey(id)==true).Select(id=>runtimeProviders![id]).Distinct(StringComparer.OrdinalIgnoreCase);var result=SerializeViews(all,mod,streams["diagnostic.manifest"],runtimeAdditions,referenceWorldBuilder ? "worldbuilder.manifest" : null,referenceWorldBuilder ? catalogs[worldBuilder] : null);
        var check=ManifestReader.Read(result["diagnostic.manifest"]);if(check.Validate().Count!=0||check.Assets.Count!=all.Length)throw new InvalidDataException("Resolved WorldBuilder manifest readback failed.");
        // Reborn: structural manifest validation alone does not prove the runtime's forward-only base lookup can find every borrowed record.
        if (referenceWorldBuilder) ValidateMapBaseOrder(check,catalogs[worldBuilder]);
        // Reborn: exact readback compares sidecar slices without allocating another complete native graph.
        var recovered=ReadNativeViews(check,result);for(int i=0;i<all.Length;i++)if(all[i].Asset.Name!=recovered[i].Asset.Name||!all[i].Asset.References.SequenceEqual(recovered[i].Asset.References)||!all[i].Bin.Span.SequenceEqual(recovered[i].Bin.Span)||!all[i].Relo.Span.SequenceEqual(recovered[i].Relo.Span)||!all[i].Imp.Span.SequenceEqual(recovered[i].Imp.Span))throw new InvalidDataException("Resolved WorldBuilder native readback differs: "+all[i].Asset.Name);
        return new(result,custom,selected.Where(a=>!referenceWorldBuilder || !a.Path.Equals(worldBuilder,StringComparison.OrdinalIgnoreCase)).Select(a=>a.Asset.Name).ToArray(),bytesRead,referencedWorldBuilder.ToArray());

        // Reborn: original stock cycles remain intact; all acyclic local/imported edges are emitted dependency-first.
        void Order(AssetId id){if(!candidates.TryGetValue(id,out var item)||!ordering.Add(id))return;foreach(var reference in item.Asset.References)Order(reference);sorted.Add(item);}
        // Reborn: a visited set also permits stock cycles; local or already loaded identities always stop retail traversal.
        void Visit(AssetId id){if(present.Contains(id)||!visited.Add(id))return;if(!byId.TryGetValue(id,out var item))throw new InvalidDataException("Retail dependency has no mod, loaded or extractable provider: "+id);foreach(var reference in item.Asset.References)Visit(reference);selected.Add(item);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: keep inherited entries in retail order, interleaving authored payloads only where their dependencies require them. */
    //-------------------------------------------------------------------------------------------------
    private static NativeView[] OrderMapBase(NativeView[] assets,ManifestDocument world)
    {
        var items=assets.ToDictionary(n=>Id(n.Asset));
        var ranks=assets.Select((n,i)=>(Key:Id(n.Asset),Rank:i)).ToDictionary(n=>n.Key,n=>n.Rank);
        var borrowed=assets.Where(n=>n.Bin.Length==0 && n.Relo.Length==0 && n.Imp.Length==0).Select(n=>Id(n.Asset)).ToHashSet();
        var baseOrder=world.Assets.Select(Id).Where(borrowed.Contains).ToArray();
        if(baseOrder.Length!=borrowed.Count)throw new InvalidDataException("Map patch contains an empty record with no WorldBuilder base provider.");
        var next=items.Keys.ToDictionary(id=>id,_=>new HashSet<AssetId>());
        var incoming=items.Keys.ToDictionary(id=>id,_=>0);
        for(int i=1;i<baseOrder.Length;i++)AddEdge(baseOrder[i-1],baseOrder[i]);
        foreach(var item in assets)
        {
            var owner=Id(item.Asset);
            foreach(var dependency in item.Asset.References)
            {
                if(!items.ContainsKey(dependency) || dependency==owner)continue;
                if(borrowed.Contains(owner) && borrowed.Contains(dependency))continue;
                // Reborn: retain the existing DFS cut for authored cycles; mixed base/payload edges still constrain insertion.
                if(!borrowed.Contains(owner) && !borrowed.Contains(dependency) && ranks[dependency]>=ranks[owner])continue;
                AddEdge(dependency,owner);
            }
        }
        var ready=new PriorityQueue<AssetId,int>();
        foreach(var pair in incoming)if(pair.Value==0)ready.Enqueue(pair.Key,ranks[pair.Key]);
        var ordered=new List<NativeView>(assets.Length);
        while(ready.TryDequeue(out var id,out _))
        {
            ordered.Add(items[id]);
            foreach(var consumer in next[id])if(--incoming[consumer]==0)ready.Enqueue(consumer,ranks[consumer]);
        }
        if(ordered.Count!=assets.Length)
            throw new InvalidDataException("Map WorldBuilder base order conflicts with authored dependency ordering: "+string.Join(", ",incoming.Where(p=>p.Value!=0).Take(4).Select(p=>items[p.Key].Asset.Name)));
        return ordered.ToArray();

        // Reborn: one ordering edge counts once even when several reference slots target the same asset.
        void AddEdge(AssetId dependency,AssetId consumer){if(next[dependency].Add(consumer))incoming[consumer]++;}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: simulate the engine's forward-only base scan and require exact fingerprints before accepting a zero-payload patch record. */
    //-------------------------------------------------------------------------------------------------
    internal static void ValidateMapBaseOrder(ManifestDocument map,ManifestDocument world)
    {
        var positions=world.Assets.Select((asset,index)=>(Key:Id(asset),Index:index)).ToDictionary(p=>p.Key,p=>p.Index);
        int previous=-1;
        foreach(var asset in map.Assets.Where(a=>a.InstanceDataSize==0 && a.RelocationDataSize==0 && a.ImportsDataSize==0))
        {
            if(!positions.TryGetValue(Id(asset),out int at) || at<=previous)
                throw new InvalidDataException("Map WorldBuilder patch base order is invalid at "+asset.Name+"; borrowed entries must follow the retail manifest order.");
            var original=world.Assets[at];
            if(asset.TypeHash!=original.TypeHash || asset.InstanceHash!=original.InstanceHash || asset.Tokenized!=original.Tokenized || !asset.References.SequenceEqual(original.References))
                throw new InvalidDataException("Map WorldBuilder patch fingerprint/reference table differs: "+asset.Name);
            previous=at;
        }
        // Reborn: the runtime reserves logical stream/chunk sizes even though inherited records consume zero bytes in local files.
        var logical=map.Assets.Select(a=>a.InstanceDataSize==0 && a.RelocationDataSize==0 && a.ImportsDataSize==0 ? world.Assets[positions[Id(a)]] : a).ToArray();
        if(map.Header.TotalInstanceDataSize!=logical.Sum(a=>(long)a.InstanceDataSize) || map.Header.MaxInstanceChunkSize!=logical.Max(a=>a.InstanceDataSize)
            || map.Header.MaxRelocationChunkSize!=logical.Max(a=>a.RelocationDataSize) || map.Header.MaxImportsChunkSize!=logical.Max(a=>a.ImportsDataSize))
            throw new InvalidDataException("Map WorldBuilder patch allocation header omits or misstates inherited native sizes.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: schema-typed authored GameObject weak IDs, including OCL CreateObject, seed copying without being converted into strong imports. */
    //-------------------------------------------------------------------------------------------------
    internal static AssetId[] WeakSeeds(string prepared)
    {
        var schemas=ConscriptSourceInput.Schemas();var ids=new HashSet<AssetId>();foreach(string path in new[]{prepared,Path.Combine(Path.GetDirectoryName(prepared)!,"additional.xml"),Path.Combine(Path.GetDirectoryName(prepared)!,"command.xml")}.Distinct().Where(File.Exists)){
            var wrapper=new XmlDocument{XmlResolver=null};wrapper.Load(path);var roots=wrapper.DocumentElement!.LocalName=="AssetDeclaration"?wrapper.DocumentElement.ChildNodes.OfType<XmlElement>():new[]{wrapper.DocumentElement};
            foreach(var root in roots){var document=new XmlDocument{XmlResolver=null};document.LoadXml(root.OuterXml);document.Schemas.Add(schemas);document.Validate((_,e)=>throw SdkXmlDiagnostics.SchemaError(e));foreach(XmlNode node in document.SelectNodes("//*|//@*")!){if(node.SchemaInfo?.SchemaType?.QualifiedName.Name!="GameObjectWeakRef")continue;string value=(node is XmlAttribute a?a.Value:node.InnerText).Trim();if(value.Length!=0)ids.Add(new(0x942FFF2D,InstanceHandle.GetInstanceId(value)));}}
        }return ids.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: repair all three packaged stream variants together and update the private receipt only after exact archive readback. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string package,string catalogDirectory,string staticManifest,string globalManifest,string prepared,string? context,string? mapStaticPackage=null)
    {
        // Reborn: only one native family is needed for resolution; audio/custom data and duplicate LOD families stay on disk.
        Dictionary<string,byte[]> streams;
        HashSet<string> originalNames;
        using(var big=BigArchive.Open(package)){
            var entries=big.Entries.ToDictionary(e=>e.Name,StringComparer.OrdinalIgnoreCase);
            originalNames=entries.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
            streams=Extensions.ToDictionary(e=>"diagnostic"+e,e=>big.ReadEntry(entries["data\\mod"+e],checked((int)entries["data\\mod"+e].StoredSize)));
        }
        var providers=new[]{(Path:staticManifest,Runtime:"data\\static.manifest"),(Path:globalManifest,Runtime:"data\\global.manifest"),(Path:Path.Combine(catalogDirectory,"audio.manifest"),Runtime:"data\\audio.manifest")}.Where(p=>File.Exists(p.Path)).Concat(SdkGlobalStreams.Load(context).Select(s=>(Path:s.Manifest,Runtime:s.Runtime))).ToArray();
        var mod=ManifestReader.Read(streams["diagnostic.manifest"]);
        // Reborn: map packages retain their own reference convention; only top-level LOD streams resolve from data.
        bool isTopLevelMod=!mod.Assets.Any(asset=>asset.TypeName=="GameMap");
        var runtimeProviders=new Dictionary<AssetId,string>();var loaded=new List<ManifestAsset>();foreach(var provider in providers)foreach(var asset in ManifestReader.Read(File.ReadAllBytes(provider.Path)).Assets){loaded.Add(asset);runtimeProviders[Id(asset)]=isTopLevelMod?SdkGlobalStreams.RelativeToData(provider.Runtime):provider.Runtime;}
        // Reborn: WorldBuilder fallback must also recognize map imports owned by the freshly compiled mod static stream.
        if (mapStaticPackage != null)
        {
            if (isTopLevelMod) throw new InvalidDataException("A map stream is required for the mod static dependency package.");
            foreach (var asset in MapPackageStage.ReadModStaticCatalog(mapStaticPackage).Assets)
            {
                loaded.Add(asset);
                runtimeProviders[Id(asset)] = MapPackageStage.ModStaticRuntime;
            }
        }
        var world=ManifestReader.Read(File.ReadAllBytes(Path.Combine(catalogDirectory,"worldbuilder.manifest")));var known=loaded.Concat(mod.Assets).Select(Id).ToHashSet();var wb=world.Assets.Select(Id).ToHashSet();
        // Reborn: absent weak names remain diagnostics rather than forcing an unrelated retail lookup; only WorldBuilder-owned weak IDs seed extraction.
        var weak=WeakSeeds(prepared).Where(id=>!known.Contains(id)&&wb.Contains(id));
        // Reborn: package map-local art from the same catalog used during model name resolution.
        string? mapId=mod.Assets.SingleOrDefault(asset=>asset.TypeName=="GameMap")?.Name.Split(':',2)[1];
        string? mapArt=MapArtPath(catalogDirectory,mapId);
        // Reborn: only authored map streams borrow the retail patch base; static builds keep the existing extraction behavior.
        var result=Resolve(streams,Path.Combine(catalogDirectory,"worldbuilder.manifest"),loaded,weak,runtimeProviders,mapArt,referenceWorldBuilder:!isTopLevelMod);
        var referenced=result.ReferencedWorldBuilder??Array.Empty<string>();
        if(result.Imported.Length==0 && referenced.Length==0)return;
        var files=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
        foreach(string suffix in new[]{"","_l","_m"})foreach(string extension in Extensions)files.Add("data\\mod"+suffix+extension,result.Streams["diagnostic"+extension]);
        // Reborn: each LOD family has its own custom-data lookup directory even when it reuses exactly the same retail payload.
        foreach(var file in result.CustomData)foreach(string suffix in new[]{"","_l","_m"}){
            string name=file.Key.Replace("data\\mod\\","data\\mod"+suffix+"\\",StringComparison.Ordinal);
            if(originalNames.Contains(name))throw new InvalidDataException("Duplicate WorldBuilder custom resource: "+name);
            files.Add(name,file.Value);
        }
        var repacked=RewritePack(package,files);
        string receiptPath=Path.Combine(Path.GetDirectoryName(package)!,"trial-plan.json");var receipt=JsonNode.Parse(File.ReadAllText(receiptPath))!;receipt["WorldBuilderAssets"]=System.Text.Json.JsonSerializer.SerializeToNode(result.Imported);receipt["WorldBuilderAssetCount"]=result.Imported.Length;receipt["ReferencedWorldBuilderAssets"]=System.Text.Json.JsonSerializer.SerializeToNode(referenced);receipt["ReferencedWorldBuilderAssetCount"]=referenced.Length;receipt["PackagedAssetCount"]=ManifestReader.Read(result.Streams["diagnostic.manifest"]).Assets.Count;receipt["ArchiveEntryCount"]=repacked.Entries;receipt["ArchiveSha256"]=repacked.Sha256;File.WriteAllText(receiptPath,receipt.ToJsonString(new(){WriteIndented=true}));
        // Reborn: map summaries distinguish shared retail references from the genuinely map-local art that still owns payloads.
        if (isTopLevelMod)
        {
            Console.WriteLine($"Resolving WorldBuilder assets: {result.Imported.Length} added with dependencies; {result.BytesRead:N0} native bytes read; {result.CustomData.Count} custom resources packaged.");
            foreach(var name in result.Imported)Console.WriteLine("WorldBuilder asset added: "+name);
        }
        else
        {
            Console.WriteLine($"Map retail dependencies: {referenced.Length} WorldBuilder base references; no WorldBuilder BIN/RELO/IMP/CDATA copied; {result.Imported.Length} map-local art assets retained.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve every source native chunk and its own 1-based reference table, checking sentinels and pointer/import bounds before reuse. */
    //-------------------------------------------------------------------------------------------------
    internal static void ValidateNative(ManifestAsset asset,byte[] bin,byte[] relo,byte[] imp)
    {
        // Reborn: an empty list allocated last keeps its items pointer at the asset end (e.g. a zero-count mesh array at +bin.Length), so the end offset is valid.
        foreach(var pair in new[]{(Data:relo,Import:false),(Data:imp,Import:true)}){if(pair.Data.Length==0)continue;if(pair.Data.Length%4!=0||Word(pair.Data,pair.Data.Length-4)!=uint.MaxValue)throw new InvalidDataException("WorldBuilder auxiliary sentinel differs: "+asset.Name);for(int i=0;i<pair.Data.Length-4;i+=4){uint offset=Word(pair.Data,i);if(offset>bin.Length-4)throw new InvalidDataException("WorldBuilder auxiliary slot exceeds native asset: "+asset.Name);uint value=Word(bin,(int)offset);if(pair.Import?value>asset.References.Count:value>bin.Length)throw new InvalidDataException("WorldBuilder pointer/import index exceeds asset: "+asset.Name);}}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: split linked output on metadata sizes without rewriting selectors, and refuse trailing or truncated sidecars. */
    //-------------------------------------------------------------------------------------------------
    internal static Native[] ReadNative(ManifestDocument metadata, Dictionary<string, byte[]> streams)
    {
        return ReadNativeViews(metadata,streams).Select(n=>new Native(n.Asset,n.Bin.ToArray(),n.Relo.ToArray(),n.Imp.ToArray())).ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate linked sidecars once and borrow exact asset slices without duplicating their native payloads. */
    //-------------------------------------------------------------------------------------------------
    private static NativeView[] ReadNativeViews(ManifestDocument metadata, Dictionary<string, byte[]> streams)
    {
        int[] at = { 8, 8, 8 };
        var result = new List<NativeView>();

        for (int i = 0; i < 3; i++)
        {
            var bytes = streams["diagnostic" + Extensions[i + 1]];
            uint magic = i == 0 ? 0xBABB0000u : i == 1 ? 0xBABE0000u : 0xBAB10000u;
            if (bytes.Length < 8 || Word(bytes, 0) != magic || Word(bytes, 4) != metadata.Header.StreamChecksum)
                throw new InvalidDataException($"Linked stream checksum/magic differs: {Extensions[i + 1]}, ExpectedMagic=0x{magic:X8}, ActualMagic=0x{(bytes.Length >= 4 ? Word(bytes, 0) : 0):X8}, ExpectedChecksum=0x{metadata.Header.StreamChecksum:X8}, ActualChecksum=0x{(bytes.Length >= 8 ? Word(bytes, 4) : 0):X8}");
        }

        foreach (var asset in metadata.Assets)
        {
            int[] sizes = { asset.InstanceDataSize, asset.RelocationDataSize, asset.ImportsDataSize };
            var parts = new ReadOnlyMemory<byte>[3];

            for (int i = 0; i < 3; i++)
            {
                var bytes = streams["diagnostic" + Extensions[i + 1]];
                parts[i] = bytes.AsMemory(at[i], sizes[i]);
                at[i] = checked(at[i] + sizes[i]);
            }

            result.Add(new(asset, parts[0], parts[1], parts[2]));
        }

        for (int i = 0; i < 3; i++)
        {
            if (at[i] != streams["diagnostic" + Extensions[i + 1]].Length)
                throw new InvalidDataException("Linked sidecar length differs.");
        }

        return result.ToArray();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit a fresh linked manifest while retaining runtime reference names and all original asset fingerprints. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,byte[]> Serialize(Native[] assets,ManifestDocument original,byte[] originalMetadata,IEnumerable<string>? runtimeAdditions=null)
    {
        return SerializeViews(assets.Select(n=>new NativeView(n.Asset,n.Bin,n.Relo,n.Imp)).ToArray(),original,originalMetadata,runtimeAdditions);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize each native sidecar into one exact-size allocation instead of growing and cloning a MemoryStream. */
    //-------------------------------------------------------------------------------------------------
    private static Dictionary<string,byte[]> SerializeViews(NativeView[] assets,ManifestDocument original,byte[] originalMetadata,IEnumerable<string>? runtimeAdditions,string? patchBase=null,ManifestDocument? patchCatalog=null)
    {
        using var entries=new MemoryStream();using var references=new MemoryStream();using var names=new MemoryStream();using var sources=new MemoryStream();using var rw=new BinaryWriter(references,Encoding.UTF8,true);
        foreach(var n in assets){var a=n.Asset;using var entry=new AssetEntry{TypeId=a.TypeId,InstanceId=a.InstanceId,TypeHash=a.TypeHash,InstanceHash=a.InstanceHash,Tokenized=a.Tokenized==1,NameOffset=(int)names.Length,SourceFileNameOffset=(int)sources.Length,AssetReferenceOffset=(int)references.Length,AssetReferenceCount=a.References.Count,InstanceDataSize=n.Bin.Length,RelocationDataSize=n.Relo.Length,ImportsDataSize=n.Imp.Length};entry.SaveToStream(entries,false);names.Write(Encoding.UTF8.GetBytes(a.Name+'\0'));sources.Write(Encoding.UTF8.GetBytes(a.SourceFile+'\0'));foreach(var id in a.References){rw.Write(id.TypeId);rw.Write(id.InstanceId);}}
        rw.Flush();var runtimeBuffer=new ReferencedFileBuffer();
        // Reborn: preserve the original map dependency convention, with one explicit retail WorldBuilder patch base.
        if(patchBase!=null && !original.ReferencedManifests.Any(r=>r.IsPatch && r.Path.Equals(patchBase,StringComparison.OrdinalIgnoreCase)))runtimeBuffer.AddReference(patchBase,true);
        foreach(var reference in original.ReferencedManifests)runtimeBuffer.AddReference(reference.Path,reference.IsPatch);foreach(string name in runtimeAdditions??Array.Empty<string>())if(!original.ReferencedManifests.Any(r=>r.Path.Equals(name,StringComparison.OrdinalIgnoreCase)))runtimeBuffer.AddReference(name,false);using var runtimeStream=new MemoryStream();runtimeBuffer.SaveToStream(runtimeStream);byte[] runtime=runtimeStream.ToArray();
        using var digest=IncrementalHash.CreateHash(HashAlgorithmName.SHA256);foreach(var n in assets){digest.AppendData(n.Bin.Span);digest.AppendData(n.Relo.Span);digest.AppendData(n.Imp.Span);}digest.AppendData(entries.GetBuffer().AsSpan(0,checked((int)entries.Length)));digest.AppendData(references.GetBuffer().AsSpan(0,checked((int)references.Length)));digest.AppendData(originalMetadata);digest.AppendData(runtime);uint checksum=Word(digest.GetHashAndReset(),0);
        // Reborn: patch headers describe the effective native allocation, including inherited chunks; sidecars below still contain only local bytes.
        var baseAssets=patchCatalog?.Assets.ToDictionary(Id);
        var logical=assets.Select(n=>n.Bin.Length==0 && n.Relo.Length==0 && n.Imp.Length==0 && baseAssets!=null && baseAssets.TryGetValue(Id(n.Asset),out var inherited)
            ? inherited : n.Asset with { InstanceDataSize=n.Bin.Length,RelocationDataSize=n.Relo.Length,ImportsDataSize=n.Imp.Length }).ToArray();
        using var metadata=new MemoryStream();using(var header=new BinaryAssetBuilder.Utility.ManifestHeader{IsLinked=true,AllTypesHash=original.Header.AllTypesHash,StreamChecksum=checksum,AssetCount=(uint)assets.Length,TotalInstanceDataSize=checked((uint)logical.Sum(a=>(long)a.InstanceDataSize)),MaxInstanceChunkSize=(uint)logical.Max(a=>a.InstanceDataSize),MaxRelocationChunkSize=(uint)logical.Max(a=>a.RelocationDataSize),MaxImportsChunkSize=(uint)logical.Max(a=>a.ImportsDataSize),AssetReferenceBufferSize=(uint)references.Length,ReferenceManifestNameBufferSize=(uint)runtime.Length,AssetNameBufferSize=(uint)names.Length,SourceFileNameBufferSize=(uint)sources.Length})header.SaveToStream(metadata,false);entries.WriteTo(metadata);references.WriteTo(metadata);metadata.Write(runtime);names.WriteTo(metadata);sources.WriteTo(metadata);
        var output=new Dictionary<string,byte[]>{{"diagnostic.manifest",metadata.ToArray()}};
        for(int i=0;i<3;i++){
            int length=checked((int)(8L+assets.Sum(n=>(long)(i==0?n.Bin.Length:i==1?n.Relo.Length:n.Imp.Length))));
            var data=new byte[length];
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(0,4),i==0?0xBABB0000u:i==1?0xBABE0000u:0xBAB10000u);
            BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(4,4),checksum);
            int at=8;
            foreach(var n in assets){var part=i==0?n.Bin:i==1?n.Relo:n.Imp;part.Span.CopyTo(data.AsSpan(at,part.Length));at=checked(at+part.Length);}
            output.Add("diagnostic"+Extensions[i+1],data);
        }
        return output;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: replace native families and stream every unchanged resource from the old BIG, verifying all bytes before replacement. */
    //-------------------------------------------------------------------------------------------------
    internal static (int Entries,string Sha256) RewritePack(string package,IReadOnlyDictionary<string,byte[]> replacements)
    {
        string temporary=package+".worldbuilder.tmp";
        bool created=false;
        try{
            int entryCount;
            string hash;
            using(var original=BigArchive.Open(package)){
                var updates=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);
                foreach(var file in replacements)updates.Add(file.Key,file.Value);
                var entries=original.Entries.ToDictionary(e=>e.Name,StringComparer.OrdinalIgnoreCase);
                var files=entries.ToDictionary(e=>e.Key,e=>e.Value.StoredSize,StringComparer.OrdinalIgnoreCase);
                // Reborn: dictionary assignment keeps the original entry order; genuinely new resources append in declaration order.
                foreach(var file in updates)files[file.Key]=checked((uint)file.Value.Length);
                var buffer=new byte[65536];
                var compared=new byte[buffer.Length];
                using(var output=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)){
                    created=true;
                    WritePack(output,files,(name,destination)=>{
                        if(updates.TryGetValue(name,out var bytes)){destination.Write(bytes);return;}
                        var entry=entries[name];
                        for(long offset=0;offset<entry.StoredSize;){
                            int count=(int)Math.Min(buffer.Length,entry.StoredSize-offset);
                            original.ReadEntryRange(entry,offset,buffer.AsSpan(0,count));
                            destination.Write(buffer.AsSpan(0,count));offset+=count;
                        }
                    });
                }
                using(var verified=BigArchive.Open(temporary)){
                    if(verified.Entries.Count!=files.Count)throw new InvalidDataException("WorldBuilder BIG directory differs.");
                    var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach(var entry in verified.Entries){
                        if(!names.Add(entry.Name)||!files.TryGetValue(entry.Name,out uint length)||entry.StoredSize!=length)
                            throw new InvalidDataException("WorldBuilder BIG entry differs: "+entry.Name);
                        bool replaced=updates.TryGetValue(entry.Name,out var bytes);
                        for(long offset=0;offset<entry.StoredSize;){
                            int count=(int)Math.Min(buffer.Length,entry.StoredSize-offset);
                            verified.ReadEntryRange(entry,offset,buffer.AsSpan(0,count));
                            ReadOnlySpan<byte> expected;
                            if(replaced)expected=bytes!.AsSpan(checked((int)offset),count);
                            else{original.ReadEntryRange(entries[entry.Name],offset,compared.AsSpan(0,count));expected=compared.AsSpan(0,count);}
                            if(!buffer.AsSpan(0,count).SequenceEqual(expected))throw new InvalidDataException("WorldBuilder BIG readback differs: "+entry.Name);
                            offset+=count;
                        }
                    }
                }
                using(var input=File.OpenRead(temporary))hash=Convert.ToHexString(SHA256.HashData(input));
                entryCount=files.Count;
            }
            // Reborn: close the source before replacing it; an unrelated pre-existing temporary file is never deleted.
            File.Move(temporary,package,true);
            return(entryCount,hash);
        }finally{if(created&&File.Exists(temporary))File.Delete(temporary);}
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: package linked families and optional custom resources in the existing BIG4 format with checked sizes. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] Pack(Dictionary<string,byte[]> files)
    {
        int header=checked((16+files.Keys.Sum(k=>9+Encoding.ASCII.GetByteCount(k))+7)&~7);byte[] bytes=new byte[checked(header+files.Values.Sum(v=>v.Length))];Encoding.ASCII.GetBytes("BIG4").CopyTo(bytes,0);BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(4),(uint)bytes.Length);BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(8),(uint)files.Count);BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(12),(uint)header);int directory=16,offset=header;foreach(var file in files){BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(directory),(uint)offset);BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(directory+4),(uint)file.Value.Length);var name=Encoding.ASCII.GetBytes(file.Key+'\0');name.CopyTo(bytes,directory+8);file.Value.CopyTo(bytes,offset);directory+=8+name.Length;offset+=file.Value.Length;}return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stream BIG4 payloads in directory order, preserving legacy bytes without a contiguous archive allocation. */
    //-------------------------------------------------------------------------------------------------
    internal static void WritePack(Stream output,Dictionary<string,byte[]> files)
    {
        WritePack(output,files.ToDictionary(file=>file.Key,file=>(uint)file.Value.Length),(name,destination)=>destination.Write(files[name]));
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write a BIG4 directory independently of how its payloads are stored. */
    //-------------------------------------------------------------------------------------------------
    internal static void WritePack(Stream output,IReadOnlyDictionary<string,uint> files,Action<string,Stream> copyEntry)
    {
        long header=checked((16L+files.Keys.Sum(k=>9L+Encoding.ASCII.GetByteCount(k))+7)&~7L);
        long total=checked(header+files.Values.Sum(v=>(long)v));
        if(header>int.MaxValue||total>uint.MaxValue)throw new InvalidDataException("BIG exceeds its 32-bit archive format.");
        var directory=new byte[(int)header];Encoding.ASCII.GetBytes("BIG4").CopyTo(directory,0);
        BinaryPrimitives.WriteUInt32LittleEndian(directory.AsSpan(4),(uint)total);
        BinaryPrimitives.WriteUInt32BigEndian(directory.AsSpan(8),(uint)files.Count);
        BinaryPrimitives.WriteUInt32BigEndian(directory.AsSpan(12),(uint)header);
        int at=16;long offset=header;
        foreach(var file in files){
            BinaryPrimitives.WriteUInt32BigEndian(directory.AsSpan(at),(uint)offset);
            BinaryPrimitives.WriteUInt32BigEndian(directory.AsSpan(at+4),file.Value);
            var name=Encoding.ASCII.GetBytes(file.Key+'\0');name.CopyTo(directory,at+8);at+=8+name.Length;offset+=file.Value;
        }
        output.Write(directory);foreach(var file in files)copyEntry(file.Key,output);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every archived byte in bounded slices, including large shared LOD payloads. */
    //-------------------------------------------------------------------------------------------------
    internal static void VerifyPack(string path,Dictionary<string,byte[]> files)
    {
        using var check=BigArchive.Open(path);
        if(check.Entries.Count!=files.Count)throw new InvalidDataException("WorldBuilder BIG directory differs.");
        var names=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        // Reborn: readback retains one bounded buffer even when hundreds of megabytes share the same expected payload.
        var buffer=new byte[65536];
        foreach(var entry in check.Entries){
            if(!names.Add(entry.Name)||!files.TryGetValue(entry.Name,out var expected)||entry.StoredSize!=expected.Length)
                throw new InvalidDataException("WorldBuilder BIG entry differs: "+entry.Name);
            for(int offset=0;offset<expected.Length;){
                int count=Math.Min(65536,expected.Length-offset);
                check.ReadEntryRange(entry,offset,buffer.AsSpan(0,count));
                if(!buffer.AsSpan(0,count).SequenceEqual(expected.AsSpan(offset,count)))
                    throw new InvalidDataException("WorldBuilder BIG readback differs: "+entry.Name);
                offset+=count;
            }
        }
    }

    // Reborn: identities and little-endian words are shared by selection, validation and private regression fixtures.
    internal static AssetId Id(ManifestAsset asset)=>new(asset.TypeId,asset.InstanceId);
    private static uint Word(byte[] bytes,int offset)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));
}
