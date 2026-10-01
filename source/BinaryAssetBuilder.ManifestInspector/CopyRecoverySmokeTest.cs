using System.Reflection;
using System.Xml;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove failed local/cache copies preserve existing asset/custom-data files using isolated synthetic candidates.
internal static class CopyRecoverySmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test validation, staging, publication rollback and chunk-boundary copying through the real private copy seam. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        Settings previous = Settings.Current;
        string root = Path.Combine(Path.GetTempPath(), "Reborn-Ep1CopyRecovery-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            Settings.Current = new Settings { BuildCache = false, TargetPlatform = TargetPlatform.Win32, CustomPostfix = "", BigEndian = false };
            PluginRegistry registry = new(Array.Empty<PluginDescriptor>(), TargetPlatform.Win32);
            DocumentProcessor processor = new(Settings.Current, registry, null!);
            MethodInfo copy = typeof(BinaryAsset).GetMethod("CopyAsset", BindingFlags.NonPublic | BindingFlags.Instance)!;
            foreach (string scenario in new[] { "type-id", "type-hash", "instance-id", "instance-hash", "missing-asset",
                "short-header", "short-payload", "extra-payload", "negative-instance", "negative-relo", "negative-import",
                "missing-custom", "locked-source", "locked-asset", "locked-custom-source", "locked-custom", "new-asset-rollback", "valid-pair", "valid-big-endian",
                "empty-chunks", "tail-one", "tail-fifteen" })
            {
                string directory = Path.Combine(root, scenario);
                Directory.CreateDirectory(directory);
                Settings.Current.BigEndian = scenario == "valid-big-endian";
                using OutputManager manager = new(processor, null, Path.Combine(directory, "output"), Path.Combine(directory, "intermediate"),
                    null!, null!, Array.Empty<string>());
                XmlDocument xml = new();
                xml.LoadXml("<ArmorTemplate xmlns=\"uri:ea.com:eala:asset\" id=\"CopyRecovery\" />");
                InstanceDeclaration declaration = new(new AssetDeclarationDocument()) { XmlNode = xml.DocumentElement! };
                declaration.Handle.TypeHash = 0xA0E237D8u;
                declaration.Handle.InstanceHash = 0x12345678u;
                declaration.HasCustomData = scenario is "missing-custom" or "locked-custom-source" or "locked-custom" or "new-asset-rollback" or "valid-pair";
                // Reborn: create prior output before constructing BinaryAsset, whose availability flags snapshot existing files.
                string destination = Path.Combine(manager.IntermediateOutputDirectory, "assets", declaration.Handle.FileBase + ".asset");
                string customDestination = Path.Combine(manager.OutputDirectory, "cdata", declaration.Handle.FileBase + ".cdata");
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                Directory.CreateDirectory(Path.GetDirectoryName(customDestination)!);
                byte[] oldPayload = { 1, 2, 3, 4 };
                byte[] oldCustom = { 5, 6, 7, 8 };
                if (scenario != "new-asset-rollback") WriteAsset(destination, declaration, oldPayload);
                File.WriteAllBytes(customDestination, oldCustom);
                BinaryAsset asset = manager.GetBinaryAsset(declaration, true);
                byte[]? oldAsset = File.Exists(destination) ? File.ReadAllBytes(destination) : null;
                if (oldAsset != null) Require(asset.InstanceFileSize == 4, "Existing header did not load.");
                string candidate = Path.Combine(directory, "candidate.asset");
                string candidateCustom = Path.Combine(directory, "candidate.cdata");
                int payloadSize = scenario switch { "empty-chunks" => 0, "tail-one" => 0x100000 - 32 + 1,
                    "tail-fifteen" => 0x100000 - 32 + 15, _ => 4 };
                byte[] payload = Enumerable.Repeat((byte)90, payloadSize).ToArray();
                WriteAsset(candidate, declaration, payload, scenario);
                File.WriteAllBytes(candidateCustom, new byte[] { 9, 10, 11 });
                if (scenario == "missing-asset") File.Delete(candidate);
                if (scenario == "missing-custom") File.Delete(candidateCustom);
                if (scenario == "short-header") File.WriteAllBytes(candidate, new byte[16]);
                if (scenario == "short-payload")
                {
                    using FileStream stream = File.OpenWrite(candidate);
                    stream.SetLength(35);
                }
                if (scenario == "extra-payload")
                {
                    using FileStream stream = File.OpenWrite(candidate);
                    stream.SetLength(37);
                }
                FileStream? held = scenario switch
                {
                    "locked-source" => new FileStream(candidate, FileMode.Open, FileAccess.Read, FileShare.None),
                    "locked-asset" => new FileStream(destination, FileMode.Open, FileAccess.Read, FileShare.Read),
                    "locked-custom-source" => new FileStream(candidateCustom, FileMode.Open, FileAccess.Read, FileShare.None),
                    "locked-custom" or "new-asset-rollback" => new FileStream(customDestination, FileMode.Open, FileAccess.Read, FileShare.Read),
                    _ => null
                };
                bool accepted;
                try { accepted = (bool)copy.Invoke(asset, new object[] { candidate, candidateCustom })!; }
                finally { held?.Dispose(); }
                bool expected = scenario is "valid-pair" or "valid-big-endian" or "empty-chunks" or "tail-one" or "tail-fifteen";
                Require(accepted == expected, $"Unexpected copy outcome: {scenario}.");
                if (expected)
                {
                    Require(File.ReadAllBytes(destination).SequenceEqual(File.ReadAllBytes(candidate)), $"Successful copy changed bytes: {scenario}.");
                    Require(asset.InstanceFileSize == payloadSize, "Published header metadata stayed stale.");
                    Require(File.ReadAllBytes(customDestination).SequenceEqual(declaration.HasCustomData
                        ? File.ReadAllBytes(candidateCustom) : oldCustom), "Custom-data replacement/preservation differs.");
                }
                else
                {
                    Require(oldAsset == null ? !File.Exists(destination) : File.ReadAllBytes(destination).SequenceEqual(oldAsset),
                        $"Failed copy destroyed existing asset or published a new asset: {scenario}.");
                    Require(File.ReadAllBytes(customDestination).SequenceEqual(oldCustom), $"Failed copy changed custom data: {scenario}.");
                    if (oldAsset != null) Require(asset.InstanceFileSize == 4, "Rejected candidate replaced cached header metadata.");
                }
                Require(!Directory.EnumerateFiles(directory, "*.copy-*", SearchOption.AllDirectories).Any(),
                    $"Copy left staging or backup files after completed recovery: {scenario}.");
            }
            using AssetHeader invalid = new() { InstanceDataSize = int.MaxValue, RelocationDataSize = int.MaxValue, ImportsDataSize = int.MaxValue };
            Require(!invalid.IsValidFileLength(29) && invalid.IsValidFileLength(32L + 3L * int.MaxValue), "Asset length validation overflowed 32 bits.");
            // Reborn: a blocked rollback must retain its recoverable backup rather than deleting the only old copy.
            string recoveryTarget = Path.Combine(root, "blocked-restore.asset");
            string recoveryBackup = Path.Combine(root, "blocked-restore.backup");
            File.WriteAllBytes(recoveryTarget, new byte[] { 90, 91 });
            File.WriteAllBytes(recoveryBackup, new byte[] { 1, 2 });
            MethodInfo restore = typeof(BinaryAsset).GetMethod("RestoreCopiedFile", BindingFlags.NonPublic | BindingFlags.Static)!;
            using (FileStream held = new(recoveryTarget, FileMode.Open, FileAccess.Read, FileShare.Read))
                restore.Invoke(null, new object[] { recoveryTarget, recoveryBackup, true });
            Require(File.ReadAllBytes(recoveryTarget).SequenceEqual(new byte[] { 90, 91 })
                && File.ReadAllBytes(recoveryBackup).SequenceEqual(new byte[] { 1, 2 }), "Blocked rollback discarded recovery data.");
            restore.Invoke(null, new object[] { recoveryTarget, recoveryBackup, true });
            Require(File.ReadAllBytes(recoveryTarget).SequenceEqual(new byte[] { 1, 2 }) && !File.Exists(recoveryBackup), "Unlocked backup recovery failed.");
            Console.WriteLine("Copy recovery self-test: OK (22 cases plus blocked recovery: existing-output preservation, strict identities/lengths, custom staging, rollback, endian, small tails, cleanup, 64-bit totals)");
        }
        finally { Settings.Current = previous; }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: write bounded synthetic intermediate candidates with optional single-field corruptions. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteAsset(string path, InstanceDeclaration declaration, byte[] payload, string scenario = "valid")
    {
        AssetHeader header = new() { TypeId = declaration.Handle.TypeId, TypeHash = declaration.Handle.TypeHash,
            InstanceId = declaration.Handle.InstanceId, InstanceHash = declaration.Handle.InstanceHash, InstanceDataSize = payload.Length };
        if (scenario == "type-id") header.TypeId ^= 1;
        if (scenario == "type-hash") header.TypeHash ^= 1;
        if (scenario == "instance-id") header.InstanceId ^= 1;
        if (scenario == "instance-hash") header.InstanceHash ^= 1;
        // Reborn: balance negative sizes against another chunk so total-length equality alone cannot reject these fixtures.
        if (scenario == "negative-instance") { header.InstanceDataSize = -4; header.RelocationDataSize = 8; }
        if (scenario == "negative-relo") { header.InstanceDataSize = 8; header.RelocationDataSize = -4; }
        if (scenario == "negative-import") { header.InstanceDataSize = 8; header.ImportsDataSize = -4; }
        using Stream stream = File.Create(path);
        header.SaveToStream(stream, Settings.Current.BigEndian);
        stream.Write(payload);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on the first destination-preservation or copy-publication mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }
}
