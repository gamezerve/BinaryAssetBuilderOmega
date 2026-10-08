using System.Buffers.Binary;
using System.Text;
using System.Xml;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: characterize stock music reconciliation using synthetic words, never a generated Pathfinder header or native processor.
internal static class PathMusicStockReviewSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact names/IDs and offsets, preserve unknown event words and reject metadata, source and weak-reference mismatches. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        var header = new ManifestHeader(7,false,true,42,0x5454A8E9,2,36,20,8,0,0,0,0,0,0);
        // Reborn: synthetic asset identities are independent of native processor registration.
        ManifestAsset Asset(string name) => new(0x9A651D89,InstanceHandle.GetInstanceId(name),0x599CDAF2,0,0,0,0,0,16,0,0,0,"PathMusicEvent:"+name,"owned.xml",Array.Empty<AssetId>());
        var manifest = new ManifestDocument(header,new[] { Asset("First") with { InstanceDataSize = 20,RelocationDataSize = 8 },Asset("Second") },Array.Empty<ReferencedManifest>(),TargetProfile.Uprising,0,false);
        byte[] source = Source("First","Second"),first = Words(0,123,16,1,InstanceHandle.GetInstanceId("Second")),second = Words(0,456,0,1),relo = Words(8,uint.MaxValue);
        // Reborn: fixture offsets must match linked prefix plus preceding selected chunks.
        byte[] Read(string extension,long offset,int count) => (extension,offset,count) switch { (".bin",4,20) => first,(".bin",24,16) => second,(".relo",4,8) => relo,_ => throw new InvalidDataException("Incorrect or unbounded selected read.") };
        var report = PathMusicStockReview.Review(source,manifest,Read);
        Require(report.Events == 2 && report.AlternateReferences == 1 && report.SelectedBinBytes == 36 && report.SelectedRelocationBytes == 8 && report.Rows[0].Word4 == 123
            && report.Rows[1].Word4 == 456 && !report.HeaderRecovered && !report.ProcessorRecovered && !report.ProductionBuildReady,"Music observation was misreported as recovered production input.");
        Refuses(() => PathMusicStockReview.Review(source,manifest with { Header = header with { Version = 6 } },Read));
        Refuses(() => PathMusicStockReview.Review(source,manifest with { Header = header with { IsLinked = false } },Read));
        Refuses(() => PathMusicStockReview.Review(source,manifest with { ReferencedManifests = new[] { new ReferencedManifest("patch",true) } },Read));
        Refuses(() => PathMusicStockReview.Review(source,manifest with { Assets = new[] { manifest.Assets[0],Asset("Other") } },Read));
        Refuses(() => PathMusicStockReview.Review(source,manifest with { Assets = new[] { manifest.Assets[0],manifest.Assets[0] } },Read));
        foreach (var wrong in new[] { manifest.Assets[0] with { TypeHash = 0 },manifest.Assets[0] with { TypeId = 0 },manifest.Assets[0] with { InstanceId = 0 },
            manifest.Assets[0] with { Tokenized = 1 },manifest.Assets[0] with { InstanceDataSize = 16 },manifest.Assets[0] with { ImportsDataSize = 4 },
            manifest.Assets[0] with { References = new[] { new AssetId(1,2) },AssetReferenceCount = 1 } })
            Refuses(() => PathMusicStockReview.Review(source,manifest with { Assets = new[] { wrong,manifest.Assets[1] } },Read));
        Refuses(() => PathMusicStockReview.Review(Source("First","First"),manifest,Read));
        Refuses(() => PathMusicStockReview.Review(Source("First","Second","Outside"),manifest,Read));
        Refuses(() => PathMusicStockReview.Review(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(source).Replace("PathMusicEvent:BasePathMusicEvent","OtherBase")),manifest,Read));
        Refuses(() => PathMusicStockReview.Review(Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(source).Replace(" id="," IsCacheable='false' id=")),manifest,Read));
        Refuses(() => PathMusicStockReview.Review(Encoding.UTF8.GetBytes("<!DOCTYPE x [<!ENTITY x 'bad'>]><AssetDeclaration xmlns='uri:ea.com:eala:asset'>&x;</AssetDeclaration>"),manifest,Read));
        foreach (byte[] wrong in new[] { Words(1,123,16,1,InstanceHandle.GetInstanceId("Second")),Words(0,123,0,1,InstanceHandle.GetInstanceId("Second")),Words(0,123,16,0,InstanceHandle.GetInstanceId("Second")),Words(0,123,16,1,0),new byte[19] })
            Refuses(() => PathMusicStockReview.Review(source,manifest,(extension,offset,count) => extension == ".bin" && offset == 4 ? wrong : Read(extension,offset,count)));
        Refuses(() => PathMusicStockReview.Review(source,manifest,(extension,offset,count) => extension == ".relo" ? Words(4,uint.MaxValue) : Read(extension,offset,count)));
        Refuses(() => PathMusicStockReview.Review(source,manifest,(extension,offset,count) => extension == ".relo" ? Words(8,0) : Read(extension,offset,count)));
        Console.WriteLine("PathMusic stock review self-test: OK (exact name sets/identities, bounded offsets/words, weak alternate closure, metadata/source/DTD/native mismatch rejection; no header recovery or processor admission)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: generate only an in-memory literal stock-shaped owned source fixture. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Source(string first,string second,string? alternate = null) => Encoding.UTF8.GetBytes("<AssetDeclaration xmlns='uri:ea.com:eala:asset'><PathMusicEvent id='"+first+"' inheritFrom='PathMusicEvent:BasePathMusicEvent' RestartAlternateEvent='"+(alternate ?? second)+"'/><PathMusicEvent id='"+second+"' inheritFrom='PathMusicEvent:BasePathMusicEvent'/></AssetDeclaration>");

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write independent synthetic observed words without inferring the Pathfinder algorithm. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Words(params uint[] words)
    {
        byte[] bytes = new byte[words.Length*4]; for (int index = 0; index < words.Length; index++) BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(index*4,4),words[index]); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: incompatible stock evidence must refuse rather than silently synthesize source constants. */
    //-------------------------------------------------------------------------------------------------
    private static void Refuses(Action action)
    {
        try { action(); } catch (Exception error) when (error is InvalidDataException or XmlException) { return; }
        throw new InvalidDataException("Incompatible music observation accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on misleading header/processor completeness or lost raw word evidence. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
