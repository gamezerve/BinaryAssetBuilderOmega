using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Utility;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: frame bounded local music chunks with explicitly experimental identity fields, never production SDK output or stock type hashes.
internal static class PathMusicPackageProbe
{
    // Reborn: retain only the observed type ID; zero type/catalog hashes deliberately prevent a stock identity claim.
    private const uint TypeId = 0x9A651D89u;
    private const string Notice = "Reborn: LOCAL PATHMUSIC DIAGNOSTIC ONLY. NOT a playable Uprising mod.\nAllTypesHash=0; TypeHash=0; InstanceHash/checksum are diagnostic SHA-256 prefixes, not Core/EA processing identities.\nNo official AUDIO resolution, processor/cache registration, production linking or game-load proof.\n";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize frozen authored order and weak-ID chunks into owned memory with v7 utility framing and explicit non-stock identities. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,byte[]> Serialize(PathMusicAuthoredSnapshot snapshot)
    {
        var report = snapshot.Preflight(); Chunk[] chunks = snapshot.Compile();
        uint checksum = DiagnosticWord(report.DiagnosticFingerprint);
        using MemoryStream names = new(),sources = new(),table = new(),manifest = new();
        for (int index = 0; index < report.Rows.Length; index++)
        {
            var row = report.Rows[index]; Chunk chunk = chunks[index];
            using AssetEntry entry = new() { TypeId = TypeId,TypeHash = 0,InstanceId = row.InstanceId,
                InstanceHash = DiagnosticWord(report.DiagnosticFingerprint+":"+row.Name),Tokenized = false,
                NameOffset = (int)names.Length,SourceFileNameOffset = (int)sources.Length,
                AssetReferenceOffset = 0,AssetReferenceCount = 0,InstanceDataSize = chunk.InstanceBuffer.Length,
                RelocationDataSize = chunk.RelocationBuffer.Length,ImportsDataSize = 0 };
            entry.SaveToStream(table,false);
            names.Write(Encoding.UTF8.GetBytes("PathMusicEvent:"+row.Name+'\0')); sources.Write(Encoding.UTF8.GetBytes("events.xml\0"));
        }
        using (BinaryAssetBuilder.Utility.ManifestHeader header = new() { IsLinked = true,AllTypesHash = 0,StreamChecksum = checksum,
            AssetCount = (uint)chunks.Length,TotalInstanceDataSize = (uint)chunks.Sum(chunk => chunk.InstanceBuffer.Length),
            MaxInstanceChunkSize = (uint)chunks.Max(chunk => chunk.InstanceBuffer.Length),
            MaxRelocationChunkSize = (uint)chunks.Max(chunk => chunk.RelocationBuffer.Length),MaxImportsChunkSize = 0,
            AssetNameBufferSize = (uint)names.Length,SourceFileNameBufferSize = (uint)sources.Length }) header.SaveToStream(manifest,false);
        table.WriteTo(manifest); names.WriteTo(manifest); sources.WriteTo(manifest);
        Dictionary<string,byte[]> files = new() { ["diagnostic.manifest"] = manifest.ToArray(),["DIAGNOSTIC_ONLY.txt"] = Encoding.UTF8.GetBytes(Notice) };
        foreach (var stream in new[] { (Name:"diagnostic.bin",Magic:0xBABB0000u,Parts:chunks.Select(chunk => chunk.InstanceBuffer)),
            (Name:"diagnostic.relo",Magic:0xBABE0000u,Parts:chunks.Select(chunk => chunk.RelocationBuffer)),
            (Name:"diagnostic.imp",Magic:0xBAB10000u,Parts:chunks.Select(chunk => chunk.ImportsBuffer)) })
        {
            using MemoryStream data = new(); using BinaryWriter writer = new(data,Encoding.UTF8,true);
            writer.Write(stream.Magic); writer.Write(checksum); foreach (byte[] part in stream.Parts) writer.Write(part);
            writer.Flush(); files.Add(stream.Name,data.ToArray());
        }
        snapshot.VerifyCurrent(); return files;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare all bounded bytes, independent metadata/utility readers and manually decoded native weak IDs and relocation offsets. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(string directory,PathMusicAuthoredSnapshot snapshot)
    {
        Dictionary<string,byte[]> expected = Serialize(snapshot);
        directory = Path.GetFullPath(directory);
        CheckParents(directory);
        if (!Directory.EnumerateFileSystemEntries(directory).Select(Path.GetFileName).Order(StringComparer.Ordinal)
            .SequenceEqual(expected.Keys.Order(StringComparer.Ordinal))) throw new InvalidDataException("Music package file set differs.");
        Dictionary<string,byte[]> actual = expected.Keys.ToDictionary(name => name,name => SdkEnvironmentPreflight.Read(Path.Combine(directory,name),16384));
        foreach (var file in expected)
            if (!actual[file.Key].AsSpan().SequenceEqual(file.Value)) throw new InvalidDataException("Frozen music package bytes differ.");
        var report = snapshot.Preflight(); ManifestDocument parsed = ManifestReader.Read(actual["diagnostic.manifest"]);
        if (parsed.Validate().Count != 0 || parsed.Header.Version != 7 || !parsed.Header.IsLinked || parsed.Header.IsBigEndian
            || parsed.Header.ContainerPrefixSize != 4 || parsed.Header.AllTypesHash != 0 || parsed.Assets.Count != report.Rows.Length
            || parsed.ReferencedManifests.Count != 0 || parsed.Header.AssetReferenceBufferSize != 0 || parsed.Header.ReferenceManifestNameBufferSize != 0
            || parsed.Header.TotalInstanceDataSize != report.Rows.Sum(row => row.BinBytes)
            || parsed.Header.MaxInstanceChunkSize != report.Rows.Max(row => row.BinBytes)
            || parsed.Header.MaxRelocationChunkSize != report.Rows.Max(row => row.RelocationBytes) || parsed.Header.MaxImportsChunkSize != 0
            || parsed.Header.StreamChecksum != DiagnosticWord(report.DiagnosticFingerprint)) throw new InvalidDataException("Music manifest metadata differs.");
        using BinaryAssetBuilder.Utility.Manifest utility = new();
        if (!utility.Load(Path.Combine(directory,"diagnostic.manifest"),false) || utility.AssetCount != report.Rows.Length
            || utility.AllTypesHash != 0 || utility.StreamChecksum != parsed.Header.StreamChecksum) throw new InvalidDataException("Music utility readback differs.");
        int bin = 8,relo = 8;
        for (int index = 0; index < report.Rows.Length; index++)
        {
            var row = report.Rows[index]; var asset = parsed.Assets[index]; var other = utility.Assets[index];
            if (asset.TypeId != TypeId || asset.TypeHash != 0 || asset.InstanceId != row.InstanceId
                || asset.InstanceHash != DiagnosticWord(report.DiagnosticFingerprint+":"+row.Name) || asset.Tokenized != 0
                || asset.Name != "PathMusicEvent:"+row.Name || asset.SourceFile != "events.xml" || asset.References.Count != 0
                || asset.InstanceDataSize != row.BinBytes || asset.RelocationDataSize != row.RelocationBytes || asset.ImportsDataSize != 0
                || other.QualifiedName != asset.Name || other.TypeHash != 0 || other.InstanceHash != asset.InstanceHash || other.Tokenized
                || other.ExternalReferences.Any() || other.LinkedInstanceOffset != bin || other.LinkedRelocationOffset != relo || other.LinkedImportsOffset != 8)
                throw new InvalidDataException("Music entry/identity/offset readback differs.");
            ReadOnlySpan<byte> native = actual["diagnostic.bin"].AsSpan(bin,row.BinBytes);
            // Reborn: decode weak identity before closure counting; spans cannot escape into deferred queries.
            uint alternateId = row.Alternate == null ? 0 : Word(native,16);
            if (Word(native,0) != 0 || Word(native,4) != row.EventValue || Word(native,8) != (row.Alternate == null ? 0u : 16u)
                || native[12] != (row.Cacheable ? 1 : 0) || native[13] != 0 || native[14] != 0 || native[15] != 0
                || (row.Alternate != null && (Word(native,16) != InstanceHandle.GetInstanceId(row.Alternate)
                    || report.Rows.Count(candidate => candidate.Name == row.Alternate && candidate.InstanceId == alternateId) != 1)))
                throw new InvalidDataException("Music native fields/local weak target differ.");
            if (row.Alternate != null && (Word(actual["diagnostic.relo"],relo) != 8 || Word(actual["diagnostic.relo"],relo+4) != uint.MaxValue))
                throw new InvalidDataException("Music weak relocation differs.");
            bin += row.BinBytes; relo += row.RelocationBytes;
        }
        foreach (var stream in new[] { (Name:"diagnostic.bin",Magic:0xBABB0000u,Size:bin),
            (Name:"diagnostic.relo",Magic:0xBABE0000u,Size:relo),(Name:"diagnostic.imp",Magic:0xBAB10000u,Size:8) })
            if (actual[stream.Name].Length != stream.Size || Word(actual[stream.Name],0) != stream.Magic
                || Word(actual[stream.Name],4) != parsed.Header.StreamChecksum) throw new InvalidDataException("Music linked stream framing differs.");
        snapshot.VerifyCurrent();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: recheck raw inputs before staging and immediately before a no-overwrite rename; retain failed owned staging for inspection. */
    //-------------------------------------------------------------------------------------------------
    internal static void Publish(string output,PathMusicAuthoredSnapshot snapshot)
    {
        output = Path.GetFullPath(output); string parent = Path.GetDirectoryName(output)!;
        if (!Directory.Exists(parent) || Directory.Exists(output) || File.Exists(output)) throw new InvalidDataException("Music package requires existing parent and absent output.");
        CheckParents(parent); Dictionary<string,byte[]> files = Serialize(snapshot);
        string staging = Path.Combine(parent,"Reborn-MusicPackage-Staging-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(staging);
        try
        {
            foreach (var file in files) { using FileStream writer = new(Path.Combine(staging,file.Key),FileMode.CreateNew,FileAccess.Write); writer.Write(file.Value); }
            Verify(staging,snapshot); snapshot.VerifyCurrent(); Directory.Move(staging,output);
        }
        catch { Console.Error.WriteLine("Owned music staging retained: "+staging); throw; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: prohibit redirected package ancestors before bounded reads or owned staging. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckParents(string directory)
    {
        for (DirectoryInfo? check = new(directory); check != null; check = check.Parent)
            if ((check.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Music package path contains a reparse point.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly domain-separate truncated diagnostic digests from all recovered stock/Core identity algorithms. */
    //-------------------------------------------------------------------------------------------------
    private static uint DiagnosticWord(string text) => Word(SHA256.HashData(Encoding.UTF8.GetBytes("Reborn-MusicPackage-v1:"+text)),0);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently decode little-endian words from bounded native/framing slices. */
    //-------------------------------------------------------------------------------------------------
    private static uint Word(ReadOnlySpan<byte> bytes,int offset) => BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(offset,4));
}
