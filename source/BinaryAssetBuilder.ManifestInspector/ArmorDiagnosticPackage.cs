using System.Buffers.Binary;
using System.Text;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: package only the actual single-armor linked family; authored stock identity is allowed, but runtime override precedence is not inferred.
internal static class ArmorDiagnosticPackage
{
    private const int Limit = 1024 * 1024;
    private static readonly string[] Extensions = { ".manifest", ".bin", ".relo", ".imp" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise package readback, no-overwrite and rejected-input behavior on this test's owned files, restoring injected corruption. */
    //-------------------------------------------------------------------------------------------------
    internal static string Exercise(string prefix, InstanceDeclaration instance, AssetBuffer buffer, uint checksum)
    {
        string archivePath = Path.Combine(Path.GetDirectoryName(prefix)!, "RebornArmorProbe.big");
        string result = Write(prefix, archivePath, instance, buffer, checksum);
        byte[] original = ReadBounded(result);
        byte[][] expected = Extensions.Select(extension => ReadBounded(prefix + extension)).ToArray();
        ExpectRefusal(() => Write(prefix, archivePath, instance, buffer, checksum));
        if (!ReadBounded(result).SequenceEqual(original)) throw new InvalidDataException("Armor package existing output was changed.");
        string rejected = Path.Combine(Path.GetDirectoryName(prefix)!, "RejectedArmorProbe.big");
        byte[] damagedBin = (byte[])expected[1].Clone(); damagedBin[^1] ^= 1;
        try
        {
            File.WriteAllBytes(prefix + ".bin", damagedBin);
            ExpectRefusal(() => Write(prefix, rejected, instance, buffer, checksum));
            if (File.Exists(rejected)) throw new InvalidDataException("Armor package rejected input produced output.");
        }
        finally { File.WriteAllBytes(prefix + ".bin", expected[1]); }
        foreach (int byteOffset in new[] { 4, 8, 16, original.Length - 1 })
        {
            byte[] damaged = (byte[])original.Clone(); damaged[byteOffset] ^= 1;
            try { File.WriteAllBytes(result, damaged); ExpectRefusal(() => Verify(result, expected)); }
            finally { File.WriteAllBytes(result, original); }
        }
        Verify(result, expected);
        Console.WriteLine($"  Armor diagnostic BIG4: four exact linked entries, corruption/no-overwrite/input refusals; archive={result}");
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: negative package controls must report a deliberate policy/readback refusal rather than succeed silently. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectRefusal(Action action)
    {
        try { action(); }
        catch (InvalidDataException) { return; }
        throw new InvalidDataException("Armor package negative control was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate bounded current linked bytes before creating a new diagnostic BIG4, then independently read its complete directory and payloads. */
    //-------------------------------------------------------------------------------------------------
    internal static string Write(string prefix, string archivePath, InstanceDeclaration instance, AssetBuffer buffer, uint checksum)
    {
        CheckAncestors(Path.GetDirectoryName(archivePath)!);
        if (File.Exists(archivePath) || Directory.Exists(archivePath)) throw new InvalidDataException("Armor package output already exists.");
        byte[][] payloads = Extensions.Select(extension => ReadBounded(prefix + extension)).ToArray();
        var manifest = ManifestReader.Read(payloads[0]);
        if (manifest.Validate().Count != 0 || manifest.Header.Version != 7 || !manifest.Header.IsLinked
            || manifest.Header.AllTypesHash != 0x5454A8E9u || manifest.Header.StreamChecksum != checksum
            || manifest.Assets.Count != 1 || manifest.ReferencedManifests.Count != 0)
            throw new InvalidDataException("Armor package manifest differs.");
        var entry = manifest.Assets.Single();
        if (entry.TypeId != 0x3A6C5E8Eu || entry.TypeHash != 0xA0E237D8u || entry.InstanceId != instance.Handle.InstanceId
            || entry.InstanceHash != instance.Handle.InstanceHash || entry.InstanceHash == 0 || entry.Tokenized != 1
            || entry.Name != instance.Handle.Name || entry.SourceFile != instance.Document.LogicalSourcePath || entry.References.Count != 0
            || entry.InstanceDataSize != buffer.InstanceData.Length || entry.RelocationDataSize != buffer.RelocationData.Length
            || entry.ImportsDataSize != 0 || buffer.ImportsData.Length != 0)
            throw new InvalidDataException("Armor package identity/chunks differ.");
        byte[][] chunks = { buffer.InstanceData, buffer.RelocationData, buffer.ImportsData };
        uint[] magics = { 0xBABB0000u, 0xBABE0000u, 0xBAB10000u };
        for (int index = 0; index < chunks.Length; index++)
            if (payloads[index + 1].Length != 8 + chunks[index].Length
                || BinaryPrimitives.ReadUInt32LittleEndian(payloads[index + 1]) != magics[index]
                || BinaryPrimitives.ReadUInt32LittleEndian(payloads[index + 1].AsSpan(4)) != checksum
                || !payloads[index + 1].AsSpan(8).SequenceEqual(chunks[index]))
                throw new InvalidDataException("Armor package linked bytes differ.");
        byte[] archive = Serialize(payloads);
        // Reborn: CreateNew preserves any competing existing file; no cleanup or stock-file replacement follows a failure.
        using (var output = new FileStream(archivePath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) output.Write(archive);
        Verify(archivePath, payloads);
        return archivePath;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve the observed BIG4 mixed-endian header and use explicit ASCII names/offsets for four uncompressed streams only. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] Serialize(byte[][] payloads)
    {
        byte[][] names = Extensions.Select(extension => Encoding.ASCII.GetBytes("data\\reborn_armor_probe" + extension + "\0")).ToArray();
        int directorySize = 16 + names.Sum(name => 8 + name.Length);
        int headerSize = (directorySize + 7) & ~7;
        int size = checked(headerSize + payloads.Sum(payload => payload.Length));
        if (size > Limit) throw new InvalidDataException("Armor package exceeds one MiB.");
        byte[] bytes = new byte[size];
        Encoding.ASCII.GetBytes("BIG4").CopyTo(bytes, 0);
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(4), (uint)size);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(8), 4);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(12), (uint)headerSize);
        int directoryOffset = 16, payloadOffset = headerSize;
        for (int index = 0; index < names.Length; index++)
        {
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(directoryOffset), (uint)payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(directoryOffset + 4), (uint)payloads[index].Length);
            names[index].CopyTo(bytes, directoryOffset + 8);
            payloads[index].CopyTo(bytes, payloadOffset);
            directoryOffset += 8 + names[index].Length; payloadOffset += payloads[index].Length;
        }
        return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: existing BIG reader plus separate header/range checks must recover the exact staged stream family, not intermediate .asset data. */
    //-------------------------------------------------------------------------------------------------
    internal static void Verify(string path, byte[][] expected)
    {
        byte[] bytes = ReadBounded(path);
        if (bytes.Length < 16 || Encoding.ASCII.GetString(bytes, 0, 4) != "BIG4"
            || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) != bytes.Length
            || BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(8)) != 4)
            throw new InvalidDataException("Armor package BIG header differs.");
        long offset = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(12));
        using var archive = BigArchive.Open(path, 4, 4096);
        if (archive.Entries.Count != 4 || expected.Length != 4) throw new InvalidDataException("Armor package entry count differs.");
        for (int index = 0; index < 4; index++)
        {
            var entry = archive.Entries[index];
            if (entry.Name != "data\\reborn_armor_probe" + Extensions[index] || entry.Offset != offset
                || entry.StoredSize != expected[index].Length || !archive.ReadEntry(entry, Limit).SequenceEqual(expected[index]))
                throw new InvalidDataException("Armor package entry/payload differs.");
            offset += entry.StoredSize;
        }
        if (offset != bytes.Length) throw new InvalidDataException("Armor package has extra payload bytes.");
        if (ManifestReader.Read(archive.ReadEntry(archive.Entries[0], Limit)).Validate().Count != 0)
            throw new InvalidDataException("Armor package embedded manifest invalid.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound every owned input/package read before allocation and reject ordinary filesystem redirection. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadBounded(string path)
    {
        CheckAncestors(path);
        using var stream = File.OpenRead(path);
        if (stream.Length < 8 || stream.Length > Limit) throw new InvalidDataException("Armor package input length unsupported.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: this trusted offline diagnostic path is not a security boundary against hostile concurrent filesystem replacement. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckAncestors(string path)
    {
        if (!Path.IsPathFullyQualified(path)) throw new InvalidDataException("Armor package requires absolute paths.");
        for (FileSystemInfo? item = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path); item != null;
            item = item is FileInfo file ? file.Directory : ((DirectoryInfo)item).Parent)
            if ((item.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Armor package redirected path refused.");
    }
}
