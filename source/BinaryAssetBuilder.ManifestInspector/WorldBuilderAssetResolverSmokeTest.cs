using System.Text;
using System.Security.Cryptography;
using System.Xml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: resolver regression owns only a fresh private directory and reads selected retail chunks without publishing a mod.
internal static class WorldBuilderAssetResolverSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: verify actual model/collapse closure, local precedence, deterministic native extraction, package variants and rejected corrupt sidecars. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string output,string catalogs)
    {
        if(Directory.Exists(output))throw new InvalidDataException("Fresh resolver evidence directory required.");Directory.CreateDirectory(output);string world=Path.Combine(catalogs,"worldbuilder.manifest");byte[] metadata=File.ReadAllBytes(world);var stock=ManifestReader.Read(metadata);var loaded=new[]{"static.manifest","global.manifest","audio.manifest"}.Where(n=>File.Exists(Path.Combine(catalogs,n))).SelectMany(n=>ManifestReader.Read(File.ReadAllBytes(Path.Combine(catalogs,n))).Assets).ToArray();
        var targets=new[]{"W3DContainer:VL_RUSSIANPORTBUILDING","GameObject:VL_RussianPortBuilding_Collapse"}.Select(name=>stock.Assets.Single(a=>a.Name.Equals(name,StringComparison.OrdinalIgnoreCase))).ToArray();
        var owner=new ManifestAsset(1,2,3,4,0,targets.Length,0,0,4,0,0,0,"ResolverFixture:Owner","fixture.xml",targets.Select(WorldBuilderAssetResolver.Id).ToArray());var initial=WorldBuilderAssetResolver.Serialize(new[]{new WorldBuilderAssetResolver.Native(owner,new byte[4],Array.Empty<byte>(),Array.Empty<byte>())},stock,metadata);
        // Reborn: top-level mod dependencies resolve from data, matching the current Studio resolver's relative discovery names.
        var providers=new Dictionary<AssetId,string>();foreach(string name in new[]{"static.manifest","global.manifest","audio.manifest"}.Where(n=>File.Exists(Path.Combine(catalogs,n))))foreach(var asset in ManifestReader.Read(File.ReadAllBytes(Path.Combine(catalogs,name))).Assets)providers[WorldBuilderAssetResolver.Id(asset)]=name;
        var first=WorldBuilderAssetResolver.Resolve(initial,world,loaded,Array.Empty<AssetId>(),providers);var second=WorldBuilderAssetResolver.Resolve(initial,world,loaded,Array.Empty<AssetId>(),providers);
        Require(first.Imported.Contains(targets[0].Name)&&first.Imported.Contains(targets[1].Name),"Both direct model and weak-spawn target fixture must be copied.");Require(first.Imported.Any(n=>n.StartsWith("W3DMesh:")),"Model dependency closure missing.");Require(first.Streams.All(p=>p.Value.SequenceEqual(second.Streams[p.Key])),"Extraction is not deterministic.");Require(first.BytesRead<new FileInfo(Path.ChangeExtension(world,".bin")).Length,"Whole WorldBuilder BIN was read.");
        var result=ManifestReader.Read(first.Streams["diagnostic.manifest"]);var loadedIds=loaded.Select(WorldBuilderAssetResolver.Id).ToHashSet();Require(result.Assets.All(a=>!loadedIds.Contains(WorldBuilderAssetResolver.Id(a))),"Loaded static/global/audio asset was unnecessarily copied.");Require(result.Assets.Last().Name==owner.Name,"Imported dependencies must precede their authored consumer.");
        var none=WorldBuilderAssetResolver.Resolve(initial,world,loaded.Concat(targets),Array.Empty<AssetId>());Require(none.Imported.Length==0&&ReferenceEquals(none.Streams,initial),"Local/loaded target precedence failed.");
        var noStrong=WorldBuilderAssetResolver.Serialize(new[]{new WorldBuilderAssetResolver.Native(owner with{References=Array.Empty<AssetId>(),AssetReferenceCount=0},new byte[4],Array.Empty<byte>(),Array.Empty<byte>())},stock,metadata);var weak=WorldBuilderAssetResolver.Resolve(noStrong,world,loaded,new[]{WorldBuilderAssetResolver.Id(targets[1])});Require(weak.Imported.Contains(targets[1].Name),"Weak spawn seed not copied.");
        // Reborn: package repair runs through the production entry point with a schema-typed CreateObject seed and an unchanged authored-count receipt.
        var files=new Dictionary<string,byte[]>();foreach(var suffix in new[]{"","_l","_m"}){foreach(var extension in new[]{".manifest",".bin",".relo",".imp"})files.Add("data\\mod"+suffix+extension,initial["diagnostic"+extension]);files.Add("data\\mod"+suffix+".version",Encoding.ASCII.GetBytes("\r\n"));}string package=Path.Combine(output,"RebornConscriptProbe.big");File.WriteAllBytes(package,WorldBuilderAssetResolver.Pack(files));File.WriteAllText(Path.Combine(output,"trial-plan.json"),"{\"AssetCount\":1}");string prepared=Path.Combine(output,"game.xml");File.WriteAllText(prepared,"<!-- Reborn: schema-typed weak spawn fixture. --><ObjectCreationList xmlns='uri:ea.com:eala:asset' id='ResolverFixture'><CreateObject><CreateObject>VL_RussianPortBuilding_Collapse</CreateObject></CreateObject></ObjectCreationList>");Require(WorldBuilderAssetResolver.WeakSeeds(prepared).Contains(WorldBuilderAssetResolver.Id(targets[1])),"Schema weak source seed missing.");WorldBuilderAssetResolver.Run(package,catalogs,Path.Combine(catalogs,"static.manifest"),Path.Combine(catalogs,"global.manifest"),prepared,null);using(var big=BigArchive.Open(package))foreach(string suffix in new[]{"","_l","_m"})foreach(string extension in new[]{".manifest",".bin",".relo",".imp"})Require(big.ReadEntry(big.Entries.Single(e=>e.Name=="data\\mod"+suffix+extension)).SequenceEqual(first.Streams["diagnostic"+extension]),"Packaged stream variant differs.");
        // Reborn: a synthetic cyclic dependency proves complete identity-based selection without a native authoring restriction on copied stock graphs.
        string fixture=Path.Combine(output,"fixture");Directory.CreateDirectory(fixture);var a=owner with{TypeId=11,InstanceId=12,Name="Fixture:A",References=new[]{new AssetId(11,13)}};var b=a with{InstanceId=13,Name="Fixture:B",References=new[]{new AssetId(11,12)}};var cyclic=WorldBuilderAssetResolver.Serialize(new[]{new WorldBuilderAssetResolver.Native(a,new byte[4],Array.Empty<byte>(),Array.Empty<byte>()),new WorldBuilderAssetResolver.Native(b,new byte[4],Array.Empty<byte>(),Array.Empty<byte>())},stock,metadata);foreach(var file in cyclic)File.WriteAllBytes(Path.Combine(fixture,file.Key.Replace("diagnostic","worldbuilder")),file.Value);var seed=WorldBuilderAssetResolver.Serialize(new[]{new WorldBuilderAssetResolver.Native(owner with{References=new[]{new AssetId(11,12)}},new byte[4],Array.Empty<byte>(),Array.Empty<byte>())},stock,metadata);Require(WorldBuilderAssetResolver.Resolve(seed,Path.Combine(fixture,"worldbuilder.manifest"),Array.Empty<ManifestAsset>(),Array.Empty<AssetId>()).Imported.Length==2,"Cyclic stock closure failed.");byte[] corrupt=cyclic["diagnostic.bin"].ToArray();corrupt[0]^=1;File.WriteAllBytes(Path.Combine(fixture,"worldbuilder.bin"),corrupt);Refuse(()=>WorldBuilderAssetResolver.Resolve(seed,Path.Combine(fixture,"worldbuilder.manifest"),Array.Empty<ManifestAsset>(),Array.Empty<AssetId>()));
        // Reborn: the final archive check must suppress a successfully copied target and report only the deliberately unresolved use site.
        var pending=targets.Select(a=>new SdkMissingDependencyAudit.Finding("WORLDBUILDERASSET","Info",a.Name,"Fixture:Owner",prepared,7,3,"Model","pending")).Append(new("WORLDBUILDERASSET","Info","W3DContainer:MissingAfterResolve","Fixture:Owner",prepared,9,5,"Model","pending")).ToArray();string pendingReport=Path.Combine(output,"pending-dependencies.json");File.WriteAllText(pendingReport,System.Text.Json.JsonSerializer.Serialize(new SdkMissingDependencyAudit.Report(pending,1,0,3,0,true)));var previous=Console.Out;using var warningText=new StringWriter();try{Console.SetOut(warningText);WorldBuilderAssetResolver.CheckPending(package,pendingReport,catalogs);}finally{Console.SetOut(previous);}string warnings=warningText.ToString();Require(warnings.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries).Length==1&&warnings.StartsWith("Warning: WORLDBUILDERASSET ' W3DContainer:MissingAfterResolve '",StringComparison.Ordinal)&&warnings.Contains(prepared+"(9,5)")&&!warnings.Contains(targets[0].Name)&&!warnings.Contains(targets[1].Name),"Final package warning audit lost source evidence or warned about resolved assets.");
        CustomResources(output,stock,metadata,owner);
        PackRegression(output);
        SerializeRegression(stock,metadata,owner);
        RewriteRegression(output);
        // Reborn: import selectors are 1-based within their unchanged owner table; corrupt selectors and relocation targets must fail before packaging.
        Refuse(()=>WorldBuilderAssetResolver.ValidateNative(owner with{References=new[]{new AssetId(1,1)}},new byte[]{2,0,0,0},Array.Empty<byte>(),new byte[]{0,0,0,0,255,255,255,255}));Refuse(()=>WorldBuilderAssetResolver.ValidateNative(owner,new byte[]{5,0,0,0},new byte[]{0,0,0,0,255,255,255,255},Array.Empty<byte>()));
        // Reborn: one-past-end storage is valid for an empty final list; the corruption fixture must exceed it.
        WorldBuilderAssetResolver.ValidateNative(owner,new byte[]{4,0,0,0},new byte[]{0,0,0,0,255,255,255,255},Array.Empty<byte>());
        foreach(var file in first.Streams)File.WriteAllBytes(Path.Combine(output,file.Key),file.Value);File.WriteAllText(Path.Combine(output,"resolver-report.json"),System.Text.Json.JsonSerializer.Serialize(new{first.Imported,first.BytesRead,CustomResources=first.CustomData.Keys,GameExecuted=false,Published=false},new System.Text.Json.JsonSerializerOptions{WriteIndented=true}));Console.WriteLine($"Reborn: WorldBuilder resolver: PASS ({first.Imported.Length} retail assets, {first.BytesRead:N0} native bytes; closure, weak seeds, precedence, repeat bytes, all BIG variants, custom resources and corrupt header/pointer/import rejection).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: external texture resources must be present and copied byte-for-byte; a missing file cannot produce a silently invisible asset. */
    //-------------------------------------------------------------------------------------------------
    private static void CustomResources(string output,ManifestDocument template,byte[] metadata,ManifestAsset owner)
    {
        string folder=Path.Combine(output,"custom");Directory.CreateDirectory(folder);var texture=owner with{TypeId=Ep1TextureCompiler.TypeId,InstanceId=20,Name="Texture:ExternalFixture",References=Array.Empty<AssetId>(),AssetReferenceCount=0};var streams=WorldBuilderAssetResolver.Serialize(new[]{new WorldBuilderAssetResolver.Native(texture,new byte[12],Array.Empty<byte>(),Array.Empty<byte>())},template,metadata);foreach(var file in streams)File.WriteAllBytes(Path.Combine(folder,file.Key.Replace("diagnostic","worldbuilder")),file.Value);var input=WorldBuilderAssetResolver.Serialize(new[]{new WorldBuilderAssetResolver.Native(owner with{References=new[]{WorldBuilderAssetResolver.Id(texture)}},new byte[4],Array.Empty<byte>(),Array.Empty<byte>())},template,metadata);string path=Path.Combine(folder,"worldbuilder.manifest");bool missing=false;try{WorldBuilderAssetResolver.Resolve(input,path,Array.Empty<ManifestAsset>(),Array.Empty<AssetId>());}catch(FileNotFoundException){missing=true;}Require(missing,"Missing custom data was ignored.");Directory.CreateDirectory(Path.Combine(folder,"cdata"));string name=$"{texture.TypeId:x8}.{texture.TypeHash:x8}.{texture.InstanceId:x8}.{texture.InstanceHash:x8}.cdata";byte[] payload=Encoding.ASCII.GetBytes("Reborn: preserved custom fixture");File.WriteAllBytes(Path.Combine(folder,"cdata",name),payload);var result=WorldBuilderAssetResolver.Resolve(input,path,Array.Empty<ManifestAsset>(),Array.Empty<AssetId>());Require(result.CustomData.Single().Key=="data\\mod\\cdata\\"+name&&result.CustomData.Single().Value.SequenceEqual(payload),"External custom resource changed.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: streaming must preserve legacy BIG bytes, avoid payload-sized writer allocations and reject late corruption. */
    //-------------------------------------------------------------------------------------------------
    private static void PackRegression(string output)
    {
        var small=new Dictionary<string,byte[]>{{"data\\empty",Array.Empty<byte>()},{"data\\one",new byte[]{1,2,3}},{"data\\two",new byte[]{4,5}}};
        using(var memory=new MemoryStream()){
            WorldBuilderAssetResolver.WritePack(memory,small);
            Require(memory.ToArray().SequenceEqual(WorldBuilderAssetResolver.Pack(small)),"Streaming BIG differs from legacy bytes.");
        }
        // Reborn: many LOD entries alias one resource; the archive is 64 MiB but the writer needs only its small directory.
        byte[] payload=new byte[2*1024*1024];new Random(471).NextBytes(payload);
        var files=Enumerable.Range(0,32).ToDictionary(i=>"data\\lod_"+i,p=>payload);
        string path=Path.Combine(output,"streaming-pack.big");
        using(var file=new FileStream(path,FileMode.CreateNew,FileAccess.Write,FileShare.None)){
            long before=GC.GetAllocatedBytesForCurrentThread();WorldBuilderAssetResolver.WritePack(file,files);
            Require(GC.GetAllocatedBytesForCurrentThread()-before<1024*1024,"Streaming BIG writer allocated payload-sized storage.");
        }
        WorldBuilderAssetResolver.VerifyPack(path,files);
        using(var file=File.Open(path,FileMode.Open,FileAccess.ReadWrite,FileShare.None)){
            file.Position=file.Length-1;int value=file.ReadByte();file.Position--;file.WriteByte((byte)(value^1));
        }
        bool rejected=false;try{WorldBuilderAssetResolver.VerifyPack(path,files);}catch(InvalidDataException){rejected=true;}
        Require(rejected,"Streaming BIG readback accepted corrupt final payload.");
        Console.WriteLine("Reborn: streaming BIG: PASS (legacy bytes, 64 MiB shared LOD payloads, writer allocation below 1 MiB, late corruption rejected).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: a large native sidecar allocates only its final buffer, preserving every asset boundary, fingerprint and checksum. */
    //-------------------------------------------------------------------------------------------------
    private static void SerializeRegression(ManifestDocument stock,byte[] metadata,ManifestAsset owner)
    {
        var payload=new byte[2*1024*1024];new Random(497).NextBytes(payload);
        var assets=Enumerable.Range(0,33).Select(i=>new WorldBuilderAssetResolver.Native(owner with{
            TypeId=11,InstanceId=(uint)(100+i),Name="Fixture:Large_"+i,InstanceDataSize=payload.Length,
            AssetReferenceCount=0,References=Array.Empty<AssetId>()},payload,Array.Empty<byte>(),Array.Empty<byte>())).ToArray();
        long before=GC.GetAllocatedBytesForCurrentThread();
        var serialized=WorldBuilderAssetResolver.Serialize(assets,stock,metadata);
        long allocated=GC.GetAllocatedBytesForCurrentThread()-before;
        var bin=serialized["diagnostic.bin"];
        Require(allocated<bin.LongLength+1024*1024,"Native serialization allocated an extra sidecar-sized buffer.");
        var readback=ManifestReader.Read(serialized["diagnostic.manifest"]);
        Require(readback.Validate().Count==0&&readback.Assets.Count==assets.Length&&bin.Length==8+33*payload.Length,"Large native manifest/sidecar lengths differ.");
        for(int i=0;i<assets.Length;i++){
            Require(readback.Assets[i].InstanceId==assets[i].Asset.InstanceId&&readback.Assets[i].InstanceHash==assets[i].Asset.InstanceHash,"Large native asset identity/fingerprint differs.");
            Require(bin.AsSpan(8+i*payload.Length,payload.Length).SequenceEqual(payload),"Large native asset payload differs.");
        }
        Console.WriteLine($"Reborn: native sidecar memory: PASS (66 MiB payload, {allocated:N0} bytes allocated; one final buffer, exact boundaries and fingerprints).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: replacing a small native member must not retain unrelated audio/LOD data or overwrite a pre-existing temporary file. */
    //-------------------------------------------------------------------------------------------------
    private static void RewriteRegression(string output)
    {
        var payload=new byte[2*1024*1024];new Random(499).NextBytes(payload);
        var files=Enumerable.Range(0,32).ToDictionary(i=>"data\\mod\\cdata\\audio_"+i,_=>payload,StringComparer.OrdinalIgnoreCase);
        files.Add("data\\mod.bin",new byte[]{1,2,3,4});
        files.Add("data\\mod_l.bin",new byte[]{5,6});
        files.Add("data\\empty",Array.Empty<byte>());
        string package=Path.Combine(output,"streaming-rewrite.big");
        using(var stream=new FileStream(package,FileMode.CreateNew,FileAccess.Write,FileShare.None))WorldBuilderAssetResolver.WritePack(stream,files);
        var updates=new Dictionary<string,byte[]>{{"DATA\\MOD.BIN",new byte[]{7,8,9}},{"data\\new-resource",new byte[]{10,11}}};
        long before=GC.GetAllocatedBytesForCurrentThread();
        var result=WorldBuilderAssetResolver.RewritePack(package,updates);
        long allocated=GC.GetAllocatedBytesForCurrentThread()-before;
        Require(allocated<1024*1024,"Rewriting a BIG retained unrelated payload-sized storage.");
        foreach(var item in updates)files[item.Key]=item.Value;
        WorldBuilderAssetResolver.VerifyPack(package,files);
        Require(result.Entries==files.Count,"Rewritten BIG entry count differs.");
        using(var input=File.OpenRead(package))Require(result.Sha256==Convert.ToHexString(SHA256.HashData(input)),"Rewritten BIG receipt hash differs.");
        string expected=Path.Combine(output,"streaming-rewrite-expected.big");
        using(var stream=new FileStream(expected,FileMode.CreateNew,FileAccess.Write,FileShare.None))WorldBuilderAssetResolver.WritePack(stream,files);
        using(var input=File.OpenRead(expected))Require(result.Sha256==Convert.ToHexString(SHA256.HashData(input)),"Streamed rewrite differs from the complete expected archive.");
        string temporary=package+".worldbuilder.tmp";
        byte[] sentinel=Encoding.ASCII.GetBytes("Reborn: existing temporary file belongs to its creator.");
        File.WriteAllBytes(temporary,sentinel);
        bool refused=false;
        try{WorldBuilderAssetResolver.RewritePack(package,updates);}catch(IOException){refused=true;}
        Require(refused&&File.ReadAllBytes(temporary).SequenceEqual(sentinel),"Rewriter changed an unrelated temporary file.");
        using(var input=File.OpenRead(package))Require(result.Sha256==Convert.ToHexString(SHA256.HashData(input)),"Failed rewrite changed the original archive.");
        File.Delete(temporary);
        Console.WriteLine($"Reborn: bounded BIG rewrite: PASS (64 MiB passthrough, {allocated:N0} bytes allocated; case-insensitive replacement, exact archive, failed-write preservation).");
    }

    // Reborn: test failures identify the violated resolver contract without treating an unexpected exception as success.
    private static void Require(bool condition,string message){if(!condition)throw new InvalidDataException(message);}
    private static void Refuse(Action action){try{action();}catch(InvalidDataException){return;}throw new InvalidDataException("Corrupt WorldBuilder sidecar accepted.");}
}
