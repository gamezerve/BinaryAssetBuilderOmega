using System.Buffers.Binary;
using System.Text;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;
using BinaryAssetBuilder.Utility;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: package two owned audio records as a fixed diagnostic proof, never as a production or general SDK build entry.
internal static class AudioFilePackageProbe
{
    // Reborn: retain detached native/custom snapshots; callers cannot mutate approved package evidence through getters.
    internal sealed class Entry
    {
        private readonly byte[] _bin,_relo,_imp,_custom;
        internal string Name { get; }
        internal string Source { get; }
        internal uint Id { get; }
        internal uint Hash { get; }
        internal string CustomName => $"166b084d.53c81e47.{Id:x8}.{Hash:x8}.cdata";

        //-------------------------------------------------------------------------------------------------
        /** Reborn: freeze one checked native/custom record with a diagnostic content hash, not an asserted EA compiler InstanceHash. */
        //-------------------------------------------------------------------------------------------------
        internal Entry(string name,string source,AssetBuffer native,byte[] custom)
        {
            // Reborn: names are caller-owned; only the two explicit source slots determine play location.
            AudioFileDiagnosticIdentity.Validate(name);
            if (source is not ("ram.xml" or "streamed.xml"))
                throw new InvalidDataException("Only the two owned audio source slots are admitted.");
            if (native.InstanceData.Length > 2048 || native.RelocationData.Length > 12 || native.ImportsData.Length != 0 || custom.Length > 1048576)
                throw new InvalidDataException("Audio package snapshot exceeds bounds.");
            _bin = (byte[])native.InstanceData.Clone(); _relo = (byte[])native.RelocationData.Clone(); _imp = (byte[])native.ImportsData.Clone(); _custom = (byte[])custom.Clone();
            Name = name; Source = source; Id = InstanceHandle.GetInstanceId(name);
            CheckNative(source,CopyNative(),_custom);
            // Reborn: payload-derived fixture identity prevents stale custom copies; this is explicitly not recovered production hashing.
            Hash = FastHash.GetHashCode(_bin.Concat(_custom).ToArray());
        }

        //-------------------------------------------------------------------------------------------------
        /** Reborn: return new buffers rather than leaking approved native arrays. */
        //-------------------------------------------------------------------------------------------------
        internal AssetBuffer CopyNative() => new() { InstanceData = (byte[])_bin.Clone(),RelocationData = (byte[])_relo.Clone(),ImportsData = (byte[])_imp.Clone() };

        //-------------------------------------------------------------------------------------------------
        /** Reborn: return a detached custom payload for staging or independent tests. */
        //-------------------------------------------------------------------------------------------------
        internal byte[] CopyCustom() => (byte[])_custom.Clone();
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact serializer output and tag-04 custom framing for the fixed mono XAS RAM/streamed identities. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckNative(string source,AssetBuffer native,byte[] custom)
    {
        AudioFileRuntimeProbe.Header parsed = AudioFileRuntimeProbe.Parse(native.InstanceData,native.InstanceData.Length);
        bool streamed = source == "streamed.xml";
        if (parsed.Samples != 12000 || parsed.Rate != 48000 || parsed.Channels != 1 || (parsed.HeaderSize != 0) != streamed)
            throw new InvalidDataException("Audio package runtime fields/play location differ.");
        byte[] inline = streamed ? native.InstanceData.AsSpan(checked((int)parsed.HeaderPointer),8).ToArray() : Array.Empty<byte>();
        string subtitle = Encoding.ASCII.GetString(native.InstanceData,32,checked((int)parsed.SubtitleLength));
        AssetBuffer expected = Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,subtitle,12000,48000,1,inline);
        if (!native.InstanceData.SequenceEqual(expected.InstanceData) || !native.RelocationData.SequenceEqual(expected.RelocationData) || native.ImportsData.Length != 0)
            throw new InvalidDataException("Audio package native envelope/relocations/imports differ.");
        using MemoryStream stream = new(custom,false);
        if (AudioCustomDataProbe.Inspect(stream,parsed,inline).CodecTag != 4) throw new InvalidDataException("Audio package has an unproven codec tag.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize fixed identities with the existing v7 utility writer and recovered linked headers into owned memory. */
    //-------------------------------------------------------------------------------------------------
    internal static Dictionary<string,byte[]> Serialize(Entry[] entries,AudioFileLocalEventProbe.Entry? localEvent = null)
    {
        if (entries.Length != 2 || entries[0].Source != "ram.xml" || entries[1].Source != "streamed.xml" || entries[0].Id == entries[1].Id)
            throw new InvalidDataException("Package requires unique RAM then streamed identities.");
        AssetBuffer[] native = entries.Select(entry => entry.CopyNative()).ToArray();
        // Reborn: the optional parent comes after both local AudioFiles and must retain their complete captured fingerprints.
        localEvent?.ValidateDependencies(entries);
        if (localEvent != null) native = native.Append(localEvent.CopyNative()).ToArray();
        var rows = entries.Select(entry => (Type:0x166B084Du,TypeHash:0x53C81E47u,Id:entry.Id,Hash:entry.Hash,Name:"AudioFile:"+entry.Name,Source:entry.Source,Refs:Array.Empty<AssetId>())).ToList();
        if (localEvent != null) rows.Add((0x844D7B9Fu,0x560C2E45u,localEvent.Id,localEvent.Hash,"AudioEvent:"+localEvent.Name,"event.xml",entries.Select(entry => new AssetId(0x166B084Du,entry.Id)).ToArray()));
        using MemoryStream identities = new(); using (BinaryWriter writer = new(identities,Encoding.UTF8,true))
            foreach (var row in rows) { writer.Write(row.Type); writer.Write(row.TypeHash); writer.Write(row.Id); writer.Write(row.Hash); writer.Write(row.Refs.Length); }
        uint checksum = FastHash.GetHashCode(identities.GetBuffer());
        using MemoryStream names = new(),sources = new(),table = new(),manifest = new(),references = new();
        using BinaryWriter refWriter = new(references,Encoding.UTF8,true);
        for (int index = 0; index < rows.Count; index++)
        {
            var row = rows[index];
            using AssetEntry entry = new() { TypeId = row.Type,TypeHash = row.TypeHash,InstanceId = row.Id,InstanceHash = row.Hash,
                Tokenized = false,NameOffset = (int)names.Length,SourceFileNameOffset = (int)sources.Length,
                AssetReferenceOffset = (int)references.Length,AssetReferenceCount = row.Refs.Length,
                InstanceDataSize = native[index].InstanceData.Length,RelocationDataSize = native[index].RelocationData.Length,ImportsDataSize = native[index].ImportsData.Length };
            entry.SaveToStream(table,false); names.Write(Encoding.UTF8.GetBytes(row.Name+'\0')); sources.Write(Encoding.UTF8.GetBytes(row.Source+'\0'));
            foreach (AssetId reference in row.Refs) { refWriter.Write(reference.TypeId); refWriter.Write(reference.InstanceId); }
        }
        refWriter.Flush();
        using (BinaryAssetBuilder.Utility.ManifestHeader header = new() { IsLinked = true,AllTypesHash = 0x5454A8E9u,StreamChecksum = checksum,AssetCount = (uint)rows.Count,
            TotalInstanceDataSize = (uint)native.Sum(value => value.InstanceData.Length),MaxInstanceChunkSize = (uint)native.Max(value => value.InstanceData.Length),
            MaxRelocationChunkSize = (uint)native.Max(value => value.RelocationData.Length),MaxImportsChunkSize = (uint)native.Max(value => value.ImportsData.Length),
            AssetReferenceBufferSize = (uint)references.Length,AssetNameBufferSize = (uint)names.Length,SourceFileNameBufferSize = (uint)sources.Length }) header.SaveToStream(manifest,false);
        table.WriteTo(manifest); references.WriteTo(manifest); names.WriteTo(manifest); sources.WriteTo(manifest);
        Dictionary<string,byte[]> result = new() { ["diagnostic.manifest"] = manifest.ToArray(),
            ["DIAGNOSTIC_ONLY.txt"] = Encoding.UTF8.GetBytes("Fixed owned AudioFile package proof. NOT a playable Uprising mod.\nDiagnostic content InstanceHash; no production hash/cache/SDK/plugin/game-load claim.\n") };
        foreach (var data in new[] { (Name:"diagnostic.bin",Magic:0xBABB0000u,Parts:native.Select(value => value.InstanceData)),
            (Name:"diagnostic.relo",Magic:0xBABE0000u,Parts:native.Select(value => value.RelocationData)),(Name:"diagnostic.imp",Magic:0xBAB10000u,Parts:native.Select(value => value.ImportsData)) })
        {
            using MemoryStream stream = new(); using BinaryWriter writer = new(stream,Encoding.UTF8,true);
            writer.Write(data.Magic); writer.Write(checksum); foreach (byte[] part in data.Parts) writer.Write(part);
            writer.Flush(); result.Add(data.Name,stream.ToArray());
        }
        foreach (Entry entry in entries) result.Add(Path.Combine("diagnostic","cdata",entry.CustomName),entry.CopyCustom());
        if (localEvent != null) result["DIAGNOSTIC_ONLY.txt"] = Encoding.UTF8.GetBytes("Fixed local AudioEvent -> two AudioFile diagnostic package. NOT a playable Uprising mod.\nExplicit prepared local metadata; diagnostic dependency/content hashes, no general graph/production/SDK/game-load claim.\n");
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require exact frozen payloads, two independent readers, linked offsets and custom identity/framing before publication. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(string directory,Entry[] entries,AudioFileLocalEventProbe.Entry? localEvent = null)
    {
        Dictionary<string,byte[]> expected = Serialize(entries,localEvent);
        foreach (var file in expected)
            if (!ReadBounded(Path.Combine(directory,file.Key)).SequenceEqual(file.Value)) throw new InvalidDataException("Frozen audio package bytes differ.");
        string customDirectory = Path.Combine(directory,"diagnostic","cdata");
        if (!Directory.EnumerateFileSystemEntries(customDirectory).Select(Path.GetFileName).Order().SequenceEqual(entries.Select(entry => entry.CustomName).Order()))
            throw new InvalidDataException("Audio package custom identities are missing or orphaned.");
        string path = Path.Combine(directory,"diagnostic.manifest"); ManifestDocument parsed = ManifestReader.Read(ReadBounded(path));
        AssetBuffer[] approved = entries.Select(entry => entry.CopyNative()).ToArray();
        if (localEvent != null) approved = approved.Append(localEvent.CopyNative()).ToArray();
        if (parsed.Validate().Count != 0 || parsed.Header.Version != 7 || parsed.Header.ContainerPrefixSize != 4 || !parsed.Header.IsLinked || parsed.Header.IsBigEndian
            || parsed.Header.AllTypesHash != 0x5454A8E9u || parsed.Assets.Count != approved.Length || parsed.ReferencedManifests.Count != 0
            || parsed.Header.TotalInstanceDataSize != approved.Sum(value => value.InstanceData.Length)
            || parsed.Header.MaxInstanceChunkSize != approved.Max(value => value.InstanceData.Length)
            || parsed.Header.MaxRelocationChunkSize != approved.Max(value => value.RelocationData.Length)
            || parsed.Header.MaxImportsChunkSize != approved.Max(value => value.ImportsData.Length) || parsed.Header.AssetReferenceBufferSize != (localEvent == null ? 0 : 16) || parsed.Header.ReferenceManifestNameBufferSize != 0)
            throw new InvalidDataException("Audio package manifest shape differs.");
        using BinaryAssetBuilder.Utility.Manifest utility = new();
        if (!utility.Load(path,false) || utility.AssetCount != approved.Length || utility.StreamChecksum != parsed.Header.StreamChecksum || utility.AllTypesHash != 0x5454A8E9u)
            throw new InvalidDataException("Audio package utility readback differs.");
        int bin = 8,relo = 8;
        for (int index = 0; index < 2; index++)
        {
            ManifestAsset asset = parsed.Assets[index]; var other = utility.Assets[index]; Entry entry = entries[index]; AssetBuffer native = entry.CopyNative();
            if (asset.TypeId != 0x166B084Du || asset.TypeHash != 0x53C81E47u || asset.InstanceId != entry.Id || asset.InstanceHash != entry.Hash
                || asset.Name != "AudioFile:"+entry.Name || asset.SourceFile != entry.Source || asset.Tokenized != 0 || asset.References.Count != 0
                || asset.InstanceDataSize != native.InstanceData.Length || asset.RelocationDataSize != native.RelocationData.Length || asset.ImportsDataSize != 0
                || other.QualifiedName != asset.Name || other.TypeHash != asset.TypeHash || other.InstanceHash != asset.InstanceHash || other.Tokenized || other.ExternalReferences.Any()
                || other.LinkedInstanceOffset != bin || other.LinkedRelocationOffset != relo || other.LinkedImportsOffset != 8)
                throw new InvalidDataException("Audio package entry/offset/identity readback differs.");
            CheckNative(entry.Source,new AssetBuffer { InstanceData = AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".bin"),null,bin,asset.InstanceDataSize),
                RelocationData = AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".relo"),null,relo,asset.RelocationDataSize),ImportsData = Array.Empty<byte>() },
                ReadBounded(Path.Combine(customDirectory,$"{asset.TypeId:x8}.{asset.TypeHash:x8}.{asset.InstanceId:x8}.{asset.InstanceHash:x8}.cdata")));
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize;
        }
        // Reborn: independently decode the optional parent's one-biased imports into prior local manifest entries, not external names.
        if (localEvent != null)
        {
            ManifestAsset asset = parsed.Assets[2]; var other = utility.Assets[2];
            AssetId[] refs = entries.Select(entry => new AssetId(0x166B084Du,entry.Id)).ToArray();
            if (asset.TypeId != 0x844D7B9Fu || asset.TypeHash != 0x560C2E45u || asset.InstanceId != localEvent.Id || asset.InstanceHash != localEvent.Hash
                || asset.Name != "AudioEvent:"+localEvent.Name || asset.SourceFile != "event.xml" || asset.Tokenized != 0
                || asset.InstanceDataSize != 176 || asset.RelocationDataSize != 8 || asset.ImportsDataSize != 12 || !asset.References.SequenceEqual(refs)
                || other.QualifiedName != asset.Name || other.TypeHash != asset.TypeHash || other.InstanceHash != asset.InstanceHash || other.Tokenized
                || !other.ExternalReferences.Select(handle => new AssetId(handle.TypeId,handle.InstanceId)).SequenceEqual(refs)
                || other.LinkedInstanceOffset != bin || other.LinkedRelocationOffset != relo || other.LinkedImportsOffset != 8)
                throw new InvalidDataException("Local AudioEvent manifest/native dependency identity differs.");
            AssetBuffer read = new() { InstanceData = AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".bin"),null,bin,176),
                RelocationData = AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".relo"),null,relo,8),ImportsData = AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".imp"),null,8,12) };
            AudioFileLocalEventProbe.CheckNative(read,localEvent.Settings);
            foreach (int slot in new[] { 152,164 })
            {
                uint selector = BinaryPrimitives.ReadUInt32LittleEndian(read.InstanceData.AsSpan(slot)); AssetId target = asset.References[checked((int)selector)-1];
                if (parsed.Assets.Take(2).Count(candidate => candidate.TypeId == target.TypeId && candidate.InstanceId == target.InstanceId) != 1)
                    throw new InvalidDataException("Local AudioEvent selector does not resolve uniquely before its parent.");
            }
            bin += 176; relo += 8;
        }
        foreach (var stream in new[] { (Name:"diagnostic.bin",Magic:0xBABB0000u,Size:bin),(Name:"diagnostic.relo",Magic:0xBABE0000u,Size:relo),(Name:"diagnostic.imp",Magic:0xBAB10000u,Size:localEvent == null ? 8 : 20) })
        {
            byte[] bytes = ReadBounded(Path.Combine(directory,stream.Name));
            if (bytes.Length != stream.Size || BinaryPrimitives.ReadUInt32LittleEndian(bytes) != stream.Magic || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) != parsed.Header.StreamChecksum)
                throw new InvalidDataException("Audio package linked header/checksum/length differs.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stage exclusive files below a checked parent, verify them, then rename without replacing existing output; retain failed evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static void Publish(string output,Entry[] entries,AudioFileLocalEventProbe.Entry? localEvent = null)
    {
        output = Path.GetFullPath(output); string parent = Path.GetDirectoryName(output)!;
        if (!Directory.Exists(parent) || Directory.Exists(output) || File.Exists(output)) throw new InvalidDataException("Audio package needs an existing parent and absent output directory.");
        for (DirectoryInfo? check = new(parent); check != null; check = check.Parent)
            if ((check.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Audio package parent cannot contain reparse points.");
        Dictionary<string,byte[]> payloads = Serialize(entries,localEvent); string staging = Path.Combine(parent,"Reborn-AudioPackage-Staging-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(staging);
        try
        {
            foreach (var file in payloads)
            { string target = Path.Combine(staging,file.Key); Directory.CreateDirectory(Path.GetDirectoryName(target)!); using FileStream writer = new(target,FileMode.CreateNew,FileAccess.Write); writer.Write(file.Value); }
            Verify(staging,entries,localEvent); Directory.Move(staging,output);
        }
        catch { Console.Error.WriteLine("Owned audio staging retained after failed publication: "+staging); throw; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: cap package reads using one handle; no whole game binary or unbounded custom payload read is allowed. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadBounded(string path)
    {
        using FileStream stream = File.OpenRead(path);
        if (stream.Length > 1048576) throw new InvalidDataException("Audio package file exceeds 1 MiB proof bound.");
        byte[] bytes = new byte[checked((int)stream.Length)]; stream.ReadExactly(bytes);
        if (stream.ReadByte() != -1) throw new InvalidDataException("Audio package file grew during read."); return bytes;
    }
}
