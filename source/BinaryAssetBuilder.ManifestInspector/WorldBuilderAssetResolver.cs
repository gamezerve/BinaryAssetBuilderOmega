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
    internal sealed record Result(Dictionary<string,byte[]> Streams,Dictionary<string,byte[]> CustomData,string[] Imported,long BytesRead);
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
        for(var folder=new DirectoryInfo(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!);folder!=null;folder=folder.Parent){string path=Path.Combine(folder.FullName,"alpha/catalogs/ra3ep1/worldbuilder.manifest");if(File.Exists(path))return path;}throw new FileNotFoundException("WorldBuilder resolution requires catalogs/ra3ep1/worldbuilder.manifest inside the SDK.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read only selected native ranges, preserve per-asset import selectors and relocation offsets, and retain mod/loaded-stream precedence. */
    //-------------------------------------------------------------------------------------------------
    internal static Result Resolve(Dictionary<string,byte[]> streams,string worldBuilder,IEnumerable<ManifestAsset> loaded,IEnumerable<AssetId> weakSeeds,IReadOnlyDictionary<AssetId,string>? runtimeProviders=null)
    {
        var mod=ManifestReader.Read(streams["diagnostic.manifest"]);var retail=ManifestReader.Read(File.ReadAllBytes(worldBuilder));
        foreach(var doc in new[]{mod,retail}){TypeRegistryAudit.ValidateTarget(doc.Header.Version,doc.Header.AllTypesHash);if(!doc.Header.IsLinked||doc.Validate().Count!=0||doc.ReferencedManifests.Any(r=>r.IsPatch))throw new InvalidDataException("WorldBuilder resolution requires valid linked EP1 streams without patch bases.");}
        var present=mod.Assets.Concat(loaded).Select(Id).ToHashSet();var byId=retail.Assets.ToDictionary(Id);var selected=new List<ManifestAsset>();var visited=new HashSet<AssetId>();
        foreach(var seed in mod.Assets.SelectMany(a=>a.References).Concat(weakSeeds))Visit(seed);
        if(selected.Count==0)return new(streams,new(),Array.Empty<string>(),0);
        var offsets=new Dictionary<AssetId,long[]>();long[] at={8,8,8};foreach(var asset in retail.Assets){offsets.Add(Id(asset),at.ToArray());at[0]=checked(at[0]+asset.InstanceDataSize);at[1]=checked(at[1]+asset.RelocationDataSize);at[2]=checked(at[2]+asset.ImportsDataSize);}
        var readers=new FileStream[3];var native=new List<Native>();var custom=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);long bytesRead=0;
        try{
            for(int i=0;i<3;i++){string path=Path.ChangeExtension(worldBuilder,Extensions[i+1]);readers[i]=File.OpenRead(path);byte[] header=new byte[8];readers[i].ReadExactly(header);uint magic=i==0?0xBABB0000u:i==1?0xBABE0000u:0xBAB10000u;if(Word(header,0)!=magic||Word(header,4)!=retail.Header.StreamChecksum||readers[i].Length!=at[i])throw new InvalidDataException("WorldBuilder linked sidecar header/length differs: "+path);}
            foreach(var asset in selected){var sizes=new[]{asset.InstanceDataSize,asset.RelocationDataSize,asset.ImportsDataSize};var parts=new byte[3][];for(int i=0;i<3;i++){parts[i]=new byte[sizes[i]];readers[i].Position=offsets[Id(asset)][i];readers[i].ReadExactly(parts[i]);bytesRead+=sizes[i];}ValidateNative(asset,parts[0],parts[1],parts[2]);native.Add(new(asset,parts[0],parts[1],parts[2]));
                // Reborn: textures with an external PC resource must bring their matching hash-named custom data; embedded DDS textures need none.
                string name=$"{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata";string data=Path.Combine(Path.GetDirectoryName(worldBuilder)!,"cdata",name);bool embedded=asset.TypeName=="Texture"&&parts[0].Length>=12&&Word(parts[0],4)==12&&Word(parts[0],8)==parts[0].Length-12;
                if(File.Exists(data))custom.Add("data\\mod\\cdata\\"+name,File.ReadAllBytes(data));else if(asset.TypeName=="AudioFile"||asset.TypeName=="Texture"&&!embedded)throw new FileNotFoundException("WorldBuilder asset requires external custom data: "+asset.Name+"; expected "+data,data);
            }
        }finally{foreach(var reader in readers)reader?.Dispose();}
        // Reborn: imported dependencies precede authored consumers; reorder whole asset chunks without changing their asset-relative offsets or reference indices.
        var candidates=ReadNative(mod,streams).Concat(native).ToDictionary(n=>Id(n.Asset));var sorted=new List<Native>();var ordering=new HashSet<AssetId>();foreach(var id in candidates.Keys)Order(id);var all=sorted.ToArray();
        // Reborn: imported assets can introduce an AudioFile or other external provider not used by the authored source; add only the newly required runtime names.
        var runtimeAdditions=native.SelectMany(n=>n.Asset.References).Where(id=>!candidates.ContainsKey(id)&&runtimeProviders?.ContainsKey(id)==true).Select(id=>runtimeProviders![id]).Distinct(StringComparer.OrdinalIgnoreCase);var result=Serialize(all,mod,streams["diagnostic.manifest"],runtimeAdditions);
        var check=ManifestReader.Read(result["diagnostic.manifest"]);if(check.Validate().Count!=0||check.Assets.Count!=all.Length)throw new InvalidDataException("Resolved WorldBuilder manifest readback failed.");
        var recovered=ReadNative(check,result);for(int i=0;i<all.Length;i++)if(all[i].Asset.Name!=recovered[i].Asset.Name||!all[i].Asset.References.SequenceEqual(recovered[i].Asset.References)||!all[i].Bin.SequenceEqual(recovered[i].Bin)||!all[i].Relo.SequenceEqual(recovered[i].Relo)||!all[i].Imp.SequenceEqual(recovered[i].Imp))throw new InvalidDataException("Resolved WorldBuilder native readback differs: "+all[i].Asset.Name);
        return new(result,custom,selected.Select(a=>a.Name).ToArray(),bytesRead);

        // Reborn: original stock cycles remain intact; all acyclic local/imported edges are emitted dependency-first.
        void Order(AssetId id){if(!candidates.TryGetValue(id,out var item)||!ordering.Add(id))return;foreach(var reference in item.Asset.References)Order(reference);sorted.Add(item);}
        // Reborn: a visited set also permits stock cycles; local or already loaded identities always stop retail traversal.
        void Visit(AssetId id){if(present.Contains(id)||!visited.Add(id))return;if(!byId.TryGetValue(id,out var asset))throw new InvalidDataException("WorldBuilder dependency has no mod/loaded/WorldBuilder provider: "+id);foreach(var reference in asset.References)Visit(reference);selected.Add(asset);}
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
    internal static void Run(string package,string catalogDirectory,string staticManifest,string globalManifest,string prepared,string? context)
    {
        var files=new Dictionary<string,byte[]>(StringComparer.OrdinalIgnoreCase);using(var big=BigArchive.Open(package))foreach(var entry in big.Entries)files.Add(entry.Name,big.ReadEntry(entry,checked((int)entry.StoredSize)));
        var streams=Extensions.ToDictionary(e=>"diagnostic"+e,e=>files["data\\mod"+e]);
        var providers=new[]{(Path:staticManifest,Runtime:"data\\static.manifest"),(Path:globalManifest,Runtime:"data\\global.manifest"),(Path:Path.Combine(catalogDirectory,"audio.manifest"),Runtime:"data\\audio.manifest")}.Where(p=>File.Exists(p.Path)).Concat(SdkGlobalStreams.Load(context).Select(s=>(Path:s.Manifest,Runtime:s.Runtime))).ToArray();
        var runtimeProviders=new Dictionary<AssetId,string>();var loaded=new List<ManifestAsset>();foreach(var provider in providers)foreach(var asset in ManifestReader.Read(File.ReadAllBytes(provider.Path)).Assets){loaded.Add(asset);runtimeProviders[Id(asset)]=provider.Runtime;}var mod=ManifestReader.Read(streams["diagnostic.manifest"]);var world=ManifestReader.Read(File.ReadAllBytes(Path.Combine(catalogDirectory,"worldbuilder.manifest")));var known=loaded.Concat(mod.Assets).Select(Id).ToHashSet();var wb=world.Assets.Select(Id).ToHashSet();
        // Reborn: absent weak names remain diagnostics rather than forcing an unrelated retail lookup; only WorldBuilder-owned weak IDs seed extraction.
        var weak=WeakSeeds(prepared).Where(id=>!known.Contains(id)&&wb.Contains(id));var result=Resolve(streams,Path.Combine(catalogDirectory,"worldbuilder.manifest"),loaded,weak,runtimeProviders);
        if(result.Imported.Length==0)return;
        foreach(string suffix in new[]{"","_l","_m"})foreach(string extension in Extensions)files["data\\mod"+suffix+extension]=result.Streams["diagnostic"+extension];
        // Reborn: each LOD family has its own custom-data lookup directory even when it reuses exactly the same retail payload.
        foreach(var file in result.CustomData)foreach(string suffix in new[]{"","_l","_m"})files.Add(file.Key.Replace("data\\mod\\","data\\mod"+suffix+"\\",StringComparison.Ordinal),file.Value);byte[] bytes=Pack(files);string temporary=package+".worldbuilder.tmp";
        // Reborn: a pre-existing temporary file belongs to another operation and must never be removed after CreateNew refuses it.
        bool created=false;try{using(var output=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)){created=true;output.Write(bytes);}using(var check=BigArchive.Open(temporary)){if(check.Entries.Count!=files.Count)throw new InvalidDataException("WorldBuilder BIG directory differs.");foreach(var entry in check.Entries)if(!check.ReadEntry(entry,checked((int)entry.StoredSize)).SequenceEqual(files[entry.Name]))throw new InvalidDataException("WorldBuilder BIG readback differs: "+entry.Name);}File.Move(temporary,package,true);}finally{if(created&&File.Exists(temporary))File.Delete(temporary);}
        string receiptPath=Path.Combine(Path.GetDirectoryName(package)!,"trial-plan.json");var receipt=JsonNode.Parse(File.ReadAllText(receiptPath))!;receipt["WorldBuilderAssets"]=System.Text.Json.JsonSerializer.SerializeToNode(result.Imported);receipt["WorldBuilderAssetCount"]=result.Imported.Length;receipt["PackagedAssetCount"]=ManifestReader.Read(result.Streams["diagnostic.manifest"]).Assets.Count;receipt["ArchiveEntryCount"]=files.Count;receipt["ArchiveSha256"]=Convert.ToHexString(SHA256.HashData(bytes));File.WriteAllText(receiptPath,receipt.ToJsonString(new(){WriteIndented=true}));
        Console.WriteLine($"Resolving WorldBuilder assets: {result.Imported.Length} added with dependencies; {result.BytesRead:N0} native bytes read; {result.CustomData.Count} custom resources packaged.");foreach(var name in result.Imported)Console.WriteLine("WorldBuilder asset added: "+name);
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
        int[] at = { 8, 8, 8 };
        var result = new List<Native>();

        foreach (var asset in metadata.Assets)
        {
            int[] sizes = { asset.InstanceDataSize, asset.RelocationDataSize, asset.ImportsDataSize };
            var parts = new byte[3][];

            for (int i = 0; i < 3; i++)
            {
                var bytes = streams["diagnostic" + Extensions[i + 1]];
                uint magic = i == 0 ? 0xBABB0000u : i == 1 ? 0xBABE0000u : 0xBAB10000u;

                if (bytes.Length < 8 || Word(bytes, 0) != magic || Word(bytes, 4) != metadata.Header.StreamChecksum)
                    throw new InvalidDataException($"Linked stream checksum/magic differs: {Extensions[i + 1]}, ExpectedMagic=0x{magic:X8}, ActualMagic=0x{(bytes.Length >= 4 ? Word(bytes, 0) : 0):X8}, ExpectedChecksum=0x{metadata.Header.StreamChecksum:X8}, ActualChecksum=0x{(bytes.Length >= 8 ? Word(bytes, 4) : 0):X8}");

                parts[i] = bytes.AsSpan(at[i], sizes[i]).ToArray();
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
        using var entries=new MemoryStream();using var references=new MemoryStream();using var names=new MemoryStream();using var sources=new MemoryStream();using var rw=new BinaryWriter(references,Encoding.UTF8,true);
        foreach(var n in assets){var a=n.Asset;using var entry=new AssetEntry{TypeId=a.TypeId,InstanceId=a.InstanceId,TypeHash=a.TypeHash,InstanceHash=a.InstanceHash,Tokenized=a.Tokenized==1,NameOffset=(int)names.Length,SourceFileNameOffset=(int)sources.Length,AssetReferenceOffset=(int)references.Length,AssetReferenceCount=a.References.Count,InstanceDataSize=n.Bin.Length,RelocationDataSize=n.Relo.Length,ImportsDataSize=n.Imp.Length};entry.SaveToStream(entries,false);names.Write(Encoding.UTF8.GetBytes(a.Name+'\0'));sources.Write(Encoding.UTF8.GetBytes(a.SourceFile+'\0'));foreach(var id in a.References){rw.Write(id.TypeId);rw.Write(id.InstanceId);}}
        rw.Flush();var runtimeBuffer=new ReferencedFileBuffer();foreach(var reference in original.ReferencedManifests)runtimeBuffer.AddReference(reference.Path,reference.IsPatch);foreach(string name in runtimeAdditions??Array.Empty<string>())if(!original.ReferencedManifests.Any(r=>r.Path.Equals(name,StringComparison.OrdinalIgnoreCase)))runtimeBuffer.AddReference(name,false);using var runtimeStream=new MemoryStream();runtimeBuffer.SaveToStream(runtimeStream);byte[] runtime=runtimeStream.ToArray();
        using var digest=IncrementalHash.CreateHash(HashAlgorithmName.SHA256);foreach(var n in assets){digest.AppendData(n.Bin);digest.AppendData(n.Relo);digest.AppendData(n.Imp);}digest.AppendData(entries.ToArray());digest.AppendData(references.ToArray());digest.AppendData(originalMetadata);digest.AppendData(runtime);uint checksum=Word(digest.GetHashAndReset(),0);
        using var metadata=new MemoryStream();using(var header=new BinaryAssetBuilder.Utility.ManifestHeader{IsLinked=true,AllTypesHash=original.Header.AllTypesHash,StreamChecksum=checksum,AssetCount=(uint)assets.Length,TotalInstanceDataSize=checked((uint)assets.Sum(a=>(long)a.Bin.Length)),MaxInstanceChunkSize=(uint)assets.Max(a=>a.Bin.Length),MaxRelocationChunkSize=(uint)assets.Max(a=>a.Relo.Length),MaxImportsChunkSize=(uint)assets.Max(a=>a.Imp.Length),AssetReferenceBufferSize=(uint)references.Length,ReferenceManifestNameBufferSize=(uint)runtime.Length,AssetNameBufferSize=(uint)names.Length,SourceFileNameBufferSize=(uint)sources.Length})header.SaveToStream(metadata,false);entries.WriteTo(metadata);references.WriteTo(metadata);metadata.Write(runtime);names.WriteTo(metadata);sources.WriteTo(metadata);
        var output=new Dictionary<string,byte[]>{{"diagnostic.manifest",metadata.ToArray()}};for(int i=0;i<3;i++){using var data=new MemoryStream();using var writer=new BinaryWriter(data);writer.Write(i==0?0xBABB0000u:i==1?0xBABE0000u:0xBAB10000u);writer.Write(checksum);foreach(var n in assets)writer.Write(i==0?n.Bin:i==1?n.Relo:n.Imp);output.Add("diagnostic"+Extensions[i+1],data.ToArray());}return output;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: package linked families and optional custom resources in the existing BIG4 format with checked sizes. */
    //-------------------------------------------------------------------------------------------------
    internal static byte[] Pack(Dictionary<string,byte[]> files)
    {
        int header=checked((16+files.Keys.Sum(k=>9+Encoding.ASCII.GetByteCount(k))+7)&~7);byte[] bytes=new byte[checked(header+files.Values.Sum(v=>v.Length))];Encoding.ASCII.GetBytes("BIG4").CopyTo(bytes,0);BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(4),(uint)bytes.Length);BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(8),(uint)files.Count);BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(12),(uint)header);int directory=16,offset=header;foreach(var file in files){BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(directory),(uint)offset);BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(directory+4),(uint)file.Value.Length);var name=Encoding.ASCII.GetBytes(file.Key+'\0');name.CopyTo(bytes,directory+8);file.Value.CopyTo(bytes,offset);directory+=8+name.Length;offset+=file.Value.Length;}return bytes;
    }

    // Reborn: identities and little-endian words are shared by selection, validation and private regression fixtures.
    internal static AssetId Id(ManifestAsset asset)=>new(asset.TypeId,asset.InstanceId);
    private static uint Word(byte[] bytes,int offset)=>BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset,4));
}
