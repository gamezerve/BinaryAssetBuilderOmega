using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prepare only a stock-shaped AK47 default-reader candidate; offline package validity never implies runtime loading.
internal static class WeaponDiagnosticPackage
{
    private const int Limit = 1024 * 1024;
    private static readonly string[] Extensions = { ".manifest", ".bin", ".relo", ".imp" };
    private const string Fragment = "add-big RebornWeaponProbe.big\n";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze exact Core streams, prepare/read back the candidate and prove corruption/no-overwrite refusal on owned files. */
    //-------------------------------------------------------------------------------------------------
    internal static string Exercise(string prefix, InstanceDeclaration instance, AssetBuffer buffer, uint checksum)
    {
        byte[][] payloads = ReadSources(prefix, instance, buffer, checksum);
        string directory = Path.GetDirectoryName(PathFor(prefix + ".manifest"))!;
        Prepare(directory, payloads); Verify(directory, payloads);
        string archive = Path.Combine(directory, "RebornWeaponProbe.big");
        byte[] original = ReadBounded(archive);
        int controls = 0;
        // Reborn: keep initial/final package verification in fast mode; reserve intentional mutations for developer builds.
        if(SdkBuildTests.Enabled){
        Refuse(() => Prepare(directory, payloads));
        Require(ReadBounded(archive).SequenceEqual(original), "Occupied weapon package was overwritten.");
        // Reborn: pre-publication stream validation must reject equal-length corruption without accepting it as compiler output.
        string binPath = prefix + ".bin"; byte[] bin = payloads[1];
        try { var changed = bin.ToArray(); changed[^1] ^= 1; File.WriteAllBytes(binPath, changed); Refuse(() => ReadSources(prefix, instance, buffer, checksum)); }
        finally { File.WriteAllBytes(binPath, bin); }
        // Reborn: independently refuse changed runtime stream paths and concrete reference IDs, not merely damaged native bytes.
        byte[] firstReference = new byte[8];
        BinaryPrimitives.WriteUInt32LittleEndian(firstReference, instance.ValidatedReferencedInstances[0].TypeId);
        BinaryPrimitives.WriteUInt32LittleEndian(firstReference.AsSpan(4), instance.ValidatedReferencedInstances[0].InstanceId);
        int referenceOffset = payloads[0].AsSpan().IndexOf(firstReference);
        int globalOffset = payloads[0].AsSpan().IndexOf(Encoding.ASCII.GetBytes("global.manifest"));
        Require(referenceOffset >= 0 && globalOffset >= 0, "Weapon reference corruption fixture unavailable.");
        foreach (int offset in new[] { referenceOffset, globalOffset })
        {
            try { var changed = payloads[0].ToArray(); changed[offset] ^= 1; File.WriteAllBytes(prefix + ".manifest", changed); Refuse(() => ReadSources(prefix, instance, buffer, checksum)); }
            finally { File.WriteAllBytes(prefix + ".manifest", payloads[0]); }
        }
        foreach (int offset in new[] { 4, 8, 16, original.Length - 1 })
        {
            try { var changed = original.ToArray(); changed[offset] ^= 1; File.WriteAllBytes(archive, changed); Refuse(() => Verify(directory, payloads)); }
            finally { File.WriteAllBytes(archive, original); }
        }
        foreach (string file in new[] { "weaponprobe_1.0.cfg", "weaponprobe_1.0.skudef" })
        {
            string path = Path.Combine(directory, file); byte[] originalText = ReadBounded(path);
            try { File.WriteAllText(path, "add-big Wrong.big\n"); Refuse(() => Verify(directory, payloads)); }
            finally { File.WriteAllBytes(path, originalText); }
        }
        }
        Verify(directory, payloads);
        var receipt = new { Asset = instance.Handle.Name, Damage = BinaryPrimitives.ReadSingleLittleEndian(buffer.InstanceData.AsSpan(2392, 4)),
            PackagePath = archive, PackageSha256 = Convert.ToHexString(SHA256.HashData(original)), PackageBytes = original.Length,
            ConfigPath = Path.Combine(directory, "weaponprobe_1.0.cfg"), ModSkuPath = Path.Combine(directory, "weaponprobe_1.0.skudef"),
            RequestedArguments = new[] { "-modconfig", Path.Combine(directory, "weaponprobe_1.0.cfg") },
            ConservativeConfigPathChecked = true, WorkingDirectoryMustBeInstallation = true,
            ArchiveEntries = 15, VersionMarkerHex = "0D0A", Route = "default-mod-reader-candidate",
            CoreInstanceHash = instance.Handle.InstanceHash, StreamChecksum = checksum,
            OrderedReferences = instance.ValidatedReferencedInstances.Select(item => item.Name).ToArray(),
            ExternalManifests = new[] { "static.manifest", "global.manifest" }, NegativeControlsPassed = controls,
            PackageValidatedOffline = true, RuntimeReady = false, ConfigConsumptionProven = false, OverrideAppliedInGame = false,
            RecursiveAudioClosureProven = false, InstalledFilesModified = false, GameExecuted = false };
        File.WriteAllText(Path.Combine(directory, "weapon-package.json"), JsonSerializer.Serialize(receipt, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Weapon diagnostic package: PASS (15 entries, two exact fragments, two-reader manifest, {controls} refusals; no game): {archive}");
        return archive;

        //-------------------------------------------------------------------------------------------------
        /** Reborn: count only deliberate invalid-data refusals, never unrelated I/O failures. */
        //-------------------------------------------------------------------------------------------------
        void Refuse(Action action)
        {
            try { action(); }
            catch (InvalidDataException) { controls++; return; }
            throw new InvalidDataException("Weapon package negative control was accepted.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: place candidates beside scope directories to avoid deeply nested launch paths; occupied destinations fail closed. */
    //-------------------------------------------------------------------------------------------------
    internal static string PathFor(string streamManifest)
    {
        string scope = Path.GetDirectoryName(streamManifest)!;
        return Path.Combine(Path.GetDirectoryName(scope)!, "weapon-" + Path.GetFileName(scope)[^8..], "RebornWeaponProbe.big");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently bind incoming stream headers, payloads and direct reference IDs to the current Core result. */
    //-------------------------------------------------------------------------------------------------
    private static byte[][] ReadSources(string prefix, InstanceDeclaration instance, AssetBuffer buffer, uint checksum)
    {
        byte[][] payloads = Extensions.Select(extension => ReadBounded(prefix + extension)).ToArray();
        var manifest = ManifestReader.Read(payloads[0]);
        Require(manifest.Validate().Count == 0 && manifest.Header.Version == 7 && manifest.Header.IsLinked
            && manifest.Header.AllTypesHash == 0x5454A8E9u && manifest.Header.StreamChecksum == checksum && manifest.Assets.Count == 1,
            "Weapon package manifest header differs.");
        var entry = manifest.Assets.Single();
        string[] names = { "Multisound:SOV_Conscript_GunFireMS", "FXList:FX_MuzzleSoviet", "FXList:FX_MuzzleSoviet" };
        Require(instance.Handle.Name == "WeaponTemplate:SovietAntiInfantryInfantryAK47" && entry.Name == instance.Handle.Name
            && entry.TypeId == 0x94D4D96Eu && entry.TypeHash == 0x27E8D984u && entry.Tokenized == 1
            && entry.InstanceId == instance.Handle.InstanceId && entry.InstanceHash == instance.Handle.InstanceHash && entry.InstanceHash != 0
            && entry.SourceFile == instance.Document.LogicalSourcePath
            && entry.InstanceDataSize == 2508 && entry.RelocationDataSize == 28 && entry.ImportsDataSize == 0
            && instance.ValidatedReferencedInstances.Select(item => item.Name).SequenceEqual(names)
            && entry.References.SequenceEqual(instance.ValidatedReferencedInstances.Select(item => new AssetId(item.TypeId, item.InstanceId)))
            && manifest.ReferencedManifests.Select(item => item.Path).SequenceEqual(new[] { "static.manifest", "global.manifest" })
            && manifest.ReferencedManifests.All(item => !item.IsPatch), "Weapon package identity/reference mapping differs.");
        byte[][] chunks = { buffer.InstanceData, buffer.RelocationData, buffer.ImportsData };
        uint[] magics = { 0xBABB0000u, 0xBABE0000u, 0xBAB10000u };
        Require(chunks[0].Length == 2508 && chunks[1].Length == 28 && chunks[2].Length == 0, "Weapon compiler chunks differ.");
        for (int index = 0; index < chunks.Length; index++)
            Require(payloads[index + 1].Length == 8 + chunks[index].Length
                && BinaryPrimitives.ReadUInt32LittleEndian(payloads[index + 1]) == magics[index]
                && BinaryPrimitives.ReadUInt32LittleEndian(payloads[index + 1].AsSpan(4)) == checksum
                && payloads[index + 1].AsSpan(8).SequenceEqual(chunks[index]), "Weapon package linked bytes differ.");
        var baseline = buffer.InstanceData.ToArray();
        float damage = BinaryPrimitives.ReadSingleLittleEndian(baseline.AsSpan(2392, 4));
        Require(float.IsFinite(damage) && damage >= 0 && damage <= 10000, "Weapon package damage exceeds proven range.");
        BinaryPrimitives.WriteSingleLittleEndian(baseline.AsSpan(2392, 4), 5f);
        Require(Convert.ToHexString(SHA256.HashData(baseline)) == "2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B", "Weapon package stock-shaped fingerprint differs.");
        uint[] relocations = { 2068, 2224, 2228, 2236, 2248, 2348, uint.MaxValue };
        for (int index = 0; index < relocations.Length; index++)
            Require(BinaryPrimitives.ReadUInt32LittleEndian(buffer.RelocationData.AsSpan(index * 4, 4)) == relocations[index], "Weapon package relocation differs.");
        return payloads;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reuse only proven BIG4 framing, with fresh-directory admission and no install or launch operation. */
    //-------------------------------------------------------------------------------------------------
    private static void Prepare(string directory, byte[][] payloads)
    {
        CheckPath(Path.GetDirectoryName(directory)!);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Weapon package directory already exists.");
        string config = Path.Combine(directory, "weaponprobe_1.0.cfg");
        if (config.Length > 240 || config.Any(character => character < 32 || character > 126) || config.Contains('"') || config.Contains(';'))
            throw new InvalidDataException("Weapon candidate config exceeds conservative ASCII/path policy.");
        byte[] archive = ArmorDiagnosticPackage.Serialize(payloads, true, true);
        Directory.CreateDirectory(directory);
        using (var stream = new FileStream(Path.Combine(directory, "RebornWeaponProbe.big"), FileMode.CreateNew, FileAccess.Write, FileShare.None)) stream.Write(archive);
        foreach (string name in new[] { "weaponprobe_1.0.cfg", "weaponprobe_1.0.skudef" })
            File.WriteAllBytes(Path.Combine(directory, name), Encoding.ASCII.GetBytes(Fragment));
        File.WriteAllText(Path.Combine(directory, "TEST_CANDIDATE_ONLY.txt"),
            "Reborn: fixed AK47 candidate. The SKU is a mod fragment, not a replacement stock SKU. External-reference runtime loading and gameplay remain unproven. No installation or launch performed.\n");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: read every entry/range/marker back and require both independent manifest readers to preserve concrete reference order. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(string directory, byte[][] expected)
    {
        string archivePath = Path.Combine(directory, "RebornWeaponProbe.big"); byte[] bytes = ReadBounded(archivePath);
        Require(bytes.Length >= 16 && Encoding.ASCII.GetString(bytes, 0, 4) == "BIG4"
            && BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) == bytes.Length
            && BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(8)) == 15, "Weapon package BIG header differs.");
        long position = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(12));
        Require(position >= 16 && position <= bytes.Length && position % 8 == 0, "Weapon package header range differs.");
        using var archive = BigArchive.Open(archivePath, 15, 4096);
        Require(archive.Entries.Count == 15 && expected.Length == 4, "Weapon package entry count differs.");
        string[] extensions = Extensions.Concat(new[] { ".version" }).ToArray();
        int index = 0;
        foreach (string variant in new[] { "", "_l", "_m" })
        {
            for (int field = 0; field < extensions.Length; field++)
            {
                var entry = archive.Entries[index++]; byte[] payload = field == 4 ? new byte[] { 13, 10 } : expected[field];
                Require(entry.Name == "data\\mod" + variant + extensions[field] && entry.Offset == position
                    && entry.StoredSize == payload.Length && archive.ReadEntry(entry, Limit).SequenceEqual(payload), "Weapon package entry/marker/payload differs.");
                position += entry.StoredSize;
                if (field == 0)
                {
                    var parsed = ManifestReader.Read(archive.ReadEntry(entry, Limit));
                    Require(parsed.Validate().Count == 0, "Weapon embedded manifest invalid.");
                    // Reborn: the pointer-based Utility reader uses a separate owned copy; it never resolves game-install files here.
                    string readerPath = Path.Combine(directory, "reader-" + (variant.Length == 0 ? "base" : variant[1..]) + ".manifest");
                    CheckPath(readerPath);
                    if (!File.Exists(readerPath))
                    {
                        using var stream = new FileStream(readerPath, FileMode.CreateNew, FileAccess.Write, FileShare.None); stream.Write(payload);
                    }
                    else Require(ReadBounded(readerPath).SequenceEqual(payload), "Weapon owned reader copy changed.");
                    using var utility = new BinaryAssetBuilder.Utility.Manifest();
                    Require(utility.Load(readerPath, false) && utility.AssetCount == 1 && utility.Assets[0].Tokenized
                        && utility.Assets[0].TypeId == parsed.Assets.Single().TypeId && utility.Assets[0].TypeHash == parsed.Assets.Single().TypeHash
                        && utility.Assets[0].InstanceId == parsed.Assets.Single().InstanceId && utility.Assets[0].InstanceHash == parsed.Assets.Single().InstanceHash
                        && utility.AllTypesHash == parsed.Header.AllTypesHash && utility.Version == 7 && utility.IsLinked
                        && utility.StreamChecksum == parsed.Header.StreamChecksum
                        && utility.Assets[0].ExternalReferences.Select(item => new AssetId(item.TypeId, item.InstanceId)).SequenceEqual(parsed.Assets.Single().References)
                        && utility.ReferencedManifests.Select(item => item.Path).SequenceEqual(parsed.ReferencedManifests.Select(item => item.Path)), "Weapon embedded second reader differs.");
                }
            }
        }
        Require(position == bytes.Length, "Weapon package has extra payload bytes.");
        foreach (string name in new[] { "weaponprobe_1.0.cfg", "weaponprobe_1.0.skudef" })
            Require(ReadBounded(Path.Combine(directory, name)).SequenceEqual(Encoding.ASCII.GetBytes(Fragment)), "Weapon package config fragment differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound reads and reject redirected ancestry; this is not a hostile concurrent filesystem security boundary. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadBounded(string path)
    {
        CheckPath(path); using var stream = File.OpenRead(path);
        if (stream.Length < 1 || stream.Length > Limit) throw new InvalidDataException("Weapon package read size unsupported.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes); return bytes;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: all diagnostic input/output paths must be ordinary absolute owned paths. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckPath(string path)
    {
        if (!Path.IsPathFullyQualified(path)) throw new InvalidDataException("Weapon package requires absolute paths.");
        for (FileSystemInfo? item = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path); item != null;
            item = item is FileInfo file ? file.Directory : ((DirectoryInfo)item).Parent)
            // Reborn: a not-yet-created leaf has sentinel attributes; validate its existing ancestors before CreateNew instead.
            if (item.Exists && (item.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Weapon package redirected path refused.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: no package receipt is emitted unless every identity, framing and byte comparison passes. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    { if (!condition) throw new InvalidDataException(message); }
}
