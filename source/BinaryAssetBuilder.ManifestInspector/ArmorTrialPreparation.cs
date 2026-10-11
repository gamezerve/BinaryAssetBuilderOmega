using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.Hashing;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prepare an isolated candidate config/package, never run a game or infer runtime acceptance from offline checks.
internal static class ArmorTrialPreparation
{
    private const int Limit = 1024 * 1024;
    private const string Config = "add-big RebornArmorProbe.big\nadd-manifest data\\reborn_armor_probe.manifest\n";
    // Reborn: mount only for the default-reader experiment; do not also queue the same mod reader.
    private const string DefaultConfig = "add-big RebornArmorProbe.big\n";
    private static readonly string[] Extensions = { ".manifest", ".bin", ".relo", ".imp" };

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently reproduce the fixed ConYard fixture or separately admitted authored payload before creating a candidate directory. */
    //-------------------------------------------------------------------------------------------------
    private static byte[][] ReadSource(string path, string? authoredXml = null)
    {
        CheckPath(path);
        if (new FileInfo(path).Length > Limit) throw new InvalidDataException("Trial source exceeds bound.");
        byte[][] payloads;
        using (var archive = BigArchive.Open(path, 4, 4096)) payloads = archive.Entries.Select(entry => archive.ReadEntry(entry, Limit)).ToArray();
        ArmorDiagnosticPackage.Verify(path, payloads);
        var manifest = ManifestReader.Read(payloads[0]);
        var entry = manifest.Assets.Single();
        var identity = new InstanceHandle("ArmorTemplate", "AlliedConYardArmor");
        if (manifest.Validate().Count != 0 || manifest.Header.Version != 7 || !manifest.Header.IsLinked
            || manifest.Header.AllTypesHash != 0x5454A8E9u || manifest.ReferencedManifests.Count != 0
            || entry.TypeId != identity.TypeId || entry.TypeHash != 0xA0E237D8u || entry.InstanceId != identity.InstanceId
            || entry.InstanceHash == 0 || entry.Name != identity.Name || entry.Tokenized != 1 || entry.References.Count != 0
            || entry.InstanceDataSize != 856 || entry.RelocationDataSize != 8 || entry.ImportsDataSize != 0)
            throw new InvalidDataException("Trial source armor identity differs.");
        // Reborn: reproduce actual current authored compiler bytes, not just a claimed GUN number in JSON.
        string fixtures = Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!;
        XmlDocument xml = new() { XmlResolver = null }; xml.Load(Path.Combine(fixtures, "AlliedConYardArmorProbe.xml"));
        // Reborn: variable builds must reproduce the separately admitted authored snapshot; legacy commands retain their fixed fixture.
        if (authoredXml != null) xml.LoadXml(ArmorAuthoringAdmission.Admit(Encoding.UTF8.GetBytes(authoredXml)).Xml);
        if (xml.DocumentElement!.GetAttribute("id") != "AlliedConYardArmor"
            || (authoredXml == null && xml.DocumentElement.ChildNodes.OfType<XmlElement>().Single(item => item.GetAttribute("Damage") == "GUN").GetAttribute("Percent") != "5000"))
            throw new InvalidDataException("Trial authored fixture differs.");
        var chunk = ArmorTokenSmokeTest.Compile(ArmorTokenSmokeTest.LoadSchemas(fixtures), xml.DocumentElement.OuterXml);
        byte[][] chunks = { chunk.InstanceBuffer, chunk.RelocationBuffer, chunk.ImportsBuffer };
        uint[] magics = { 0xBABB0000u, 0xBABE0000u, 0xBAB10000u };
        byte[] padded = new byte[256];
        uint[] row = { entry.TypeId, entry.TypeHash, entry.InstanceId, entry.InstanceHash, 0 };
        for (int index = 0; index < row.Length; index++) BinaryPrimitives.WriteUInt32LittleEndian(padded.AsSpan(index * 4), row[index]);
        if (FastHash.GetHashCode(padded) != manifest.Header.StreamChecksum) throw new InvalidDataException("Trial source checksum differs.");
        for (int index = 0; index < 3; index++)
            if (payloads[index + 1].Length != 8 + chunks[index].Length
                || BinaryPrimitives.ReadUInt32LittleEndian(payloads[index + 1]) != magics[index]
                || BinaryPrimitives.ReadUInt32LittleEndian(payloads[index + 1].AsSpan(4)) != manifest.Header.StreamChecksum
                || !payloads[index + 1].AsSpan(8).SequenceEqual(chunks[index]))
                throw new InvalidDataException("Trial source compiler payload differs.");
        return payloads;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: publish one fresh candidate with a short mod identity, observed stream variants and a non-executable launch plan. */
    //-------------------------------------------------------------------------------------------------
    internal static void Prepare(string source, string directory, bool defaultRoute = false, string? authoredXml = null)
    {
        source = Path.GetFullPath(source); directory = Path.GetFullPath(directory);
        CheckPath(Path.GetDirectoryName(directory)!);
        if (Directory.Exists(directory) || File.Exists(directory)) throw new InvalidDataException("Trial output already exists.");
        string configPath = Path.Combine(directory, "reborn_1.0.cfg");
        if (configPath.Length > 240 || configPath.Any(character => character > 126 || character < 32) || configPath.Contains('"') || configPath.Contains(';'))
            throw new InvalidDataException("Trial config path exceeds conservative ASCII/path policy.");
        string sourceHash = Convert.ToHexString(SHA256.HashData(ReadBounded(source)));
        byte[][] payloads = ReadSource(source, authoredXml);
        if (Convert.ToHexString(SHA256.HashData(ReadBounded(source))) != sourceHash) throw new InvalidDataException("Trial source changed during admission.");
        byte[] archive = ArmorDiagnosticPackage.Serialize(payloads, true, defaultRoute);
        string config = defaultRoute ? DefaultConfig : Config;
        Directory.CreateDirectory(directory);
        File.WriteAllBytes(Path.Combine(directory, "RebornArmorProbe.big"), archive);
        File.WriteAllText(configPath, config, new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(directory, "reborn_1.0.skudef"), config, new UTF8Encoding(false));
        File.WriteAllText(Path.Combine(directory, "DO_NOT_LAUNCH_YET.txt"),
            "Reborn: candidate only. Mod SKU is the same mod fragment, NOT a replacement stock SKU. No launch or effective precedence proved. Separate game-test authorization and runtime preflight are required.\n");
        Verify(directory, payloads, defaultRoute);
        var report = new { CandidatePrepared = true, OfflinePackageValidated = true, ConfigContentsValidated = true,
            OutputDirectory = directory, ConfigPath = configPath, ModSkuPath = Path.Combine(directory, "reborn_1.0.skudef"),
            ArchivePath = Path.Combine(directory, "RebornArmorProbe.big"), ArchiveBytes = archive.Length,
            ArchiveSha256 = Convert.ToHexString(SHA256.HashData(archive)), SourcePackageSha256 = sourceHash,
            // Reborn: identify the route hypothesis explicitly, keeping all runtime acceptance flags false.
            Route = defaultRoute ? "default-mod-reader" : "config-manifest-queue",
            ManifestQueueArgument = defaultRoute ? null : "data\\reborn_armor_probe.manifest",
            StreamStem = defaultRoute ? "data\\mod" : "data\\reborn_armor_probe",
            ArchiveEntryCount = defaultRoute ? 15 : 12, VersionMarkerHex = defaultRoute ? "0D0A" : null,
            StreamBytesUnchanged = true, StreamVariants = new[] { "", "_l", "_m" },
            RequestedArguments = new[] { "-modconfig", configPath }, WorkingDirectoryMustBeInstallation = true,
            ConfigConsumptionProven = false, ManifestLoadProven = false, OverrideAppliedInGame = false, GameExecuted = false,
            RuntimeReady = false, NextGate = "Separately authorized single controlled launch with effective config/package observation and GUN behavioral comparison." };
        string json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(directory, "trial-plan.json"), json);
        Console.WriteLine(json);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exact config text and all twelve independently indexed variant entries must recover the same four admitted stream payloads. */
    //-------------------------------------------------------------------------------------------------
    private static void Verify(string directory, byte[][] expected, bool defaultRoute = false)
    {
        CheckPath(directory);
        foreach (string name in new[] { "reborn_1.0.cfg", "reborn_1.0.skudef" })
        {
            string path = Path.Combine(directory, name); CheckPath(path);
            if (!File.ReadAllBytes(path).SequenceEqual(Encoding.ASCII.GetBytes(defaultRoute ? DefaultConfig : Config))) throw new InvalidDataException("Trial config differs.");
        }
        string archivePath = Path.Combine(directory, "RebornArmorProbe.big"); CheckPath(archivePath);
        if (new FileInfo(archivePath).Length > Limit) throw new InvalidDataException("Trial archive exceeds bound.");
        byte[] bytes = File.ReadAllBytes(archivePath);
        if (bytes.Length < 16 || Encoding.ASCII.GetString(bytes, 0, 4) != "BIG4"
            || BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) != bytes.Length
            || BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(8)) != (defaultRoute ? 15 : 12)) throw new InvalidDataException("Trial BIG header differs.");
        using var archive = BigArchive.Open(archivePath, defaultRoute ? 15 : 12, 4096);
        // Reborn: independently reconstruct the expected directory and per-family marker bytes for readback.
        string[] extensions = defaultRoute ? Extensions.Concat(new[] { ".version" }).ToArray() : Extensions;
        string stem = defaultRoute ? "data\\mod" : "data\\reborn_armor_probe";
        string[] names = new[] { "", "_l", "_m" }.SelectMany(variant => extensions.Select(extension => stem + variant + extension)).ToArray();
        long offset = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan(12));
        if (archive.Entries.Count != names.Length) throw new InvalidDataException("Trial variant count differs.");
        for (int index = 0; index < names.Length; index++)
        {
            var entry = archive.Entries[index];
            int slot = index % extensions.Length;
            byte[] payload = slot == 4 ? Encoding.ASCII.GetBytes("\r\n") : expected[slot];
            if (entry.Name != names[index] || entry.Offset != offset || entry.StoredSize != payload.Length
                || !archive.ReadEntry(entry, Limit).SequenceEqual(payload)) throw new InvalidDataException("Trial variant bytes differ.");
            offset += entry.StoredSize;
        }
        if (offset != bytes.Length) throw new InvalidDataException("Trial trailing bytes differ.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise generated-file corruption and occupied-directory refusals without changing source or leaving damaged candidate bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Controls(string source, string directory, bool defaultRoute = false, string? authoredXml = null)
    {
        byte[][] expected = ReadSource(source, authoredXml); Verify(directory, expected, defaultRoute);
        bool refused = false;
        try { Prepare(source, directory, defaultRoute, authoredXml); } catch (InvalidDataException) { refused = true; }
        if (!refused) throw new InvalidDataException("Trial occupied output accepted.");
        foreach (string name in new[] { "reborn_1.0.cfg", "reborn_1.0.skudef", "RebornArmorProbe.big" })
        {
            string path = Path.Combine(directory, name); byte[] original = File.ReadAllBytes(path);
            byte[] damaged = (byte[])original.Clone(); damaged[^1] ^= 1;
            try
            {
                File.WriteAllBytes(path, damaged); refused = false;
                try { Verify(directory, expected, defaultRoute); } catch (InvalidDataException) { refused = true; }
                if (!refused) throw new InvalidDataException("Trial corruption accepted.");
            }
            finally { File.WriteAllBytes(path, original); }
        }
        Verify(directory, expected, defaultRoute);
        Console.WriteLine($"Armor trial controls: OK ({(defaultRoute ? 15 : 12)} exact entries, two exact config fragments, occupied directory and three corruptions refused; no game).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse redirected paths before bounded reads/writes; hostile concurrent filesystem replacement is outside this trusted offline scope. */
    //-------------------------------------------------------------------------------------------------
    private static void CheckPath(string path)
    {
        if (!Path.IsPathFullyQualified(path)) throw new InvalidDataException("Trial paths must be absolute.");
        for (FileSystemInfo? item = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path); item != null;
            item = item is FileInfo file ? file.Directory : ((DirectoryInfo)item).Parent)
            if ((item.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Trial redirected path refused.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound source hashing before allocation rather than trust a preceding FileInfo length across a later read. */
    //-------------------------------------------------------------------------------------------------
    private static byte[] ReadBounded(string path)
    {
        CheckPath(path);
        using var stream = File.OpenRead(path);
        if (stream.Length < 16 || stream.Length > Limit) throw new InvalidDataException("Trial source length unsupported.");
        byte[] bytes = new byte[(int)stream.Length]; stream.ReadExactly(bytes); return bytes;
    }
}
