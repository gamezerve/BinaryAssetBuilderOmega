using System.Buffers.Binary;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.Core.SageXml;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: verify actual intermediate/manifest/link output through the separate bounded weapon capability, never package or launch it.
internal static class WeaponStreamProbe
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare actual Core streams with both readers and exercise capability refusals before handing back their diagnostic path. */
    //-------------------------------------------------------------------------------------------------
    internal static string Write(string parent, DocumentProcessor processor, AssetDeclarationDocument document,
        InstanceDeclaration instance, AssetBuffer buffer)
    {
        // Reborn: the ordinary writer must still deny this same plugin before touching any production path.
        string denied = Path.Combine(parent, "ProductionDeniedWeapon");
        try
        {
            using var rejected = new OutputManager(processor, null!, denied, denied + "-intermediate", null!, null!, Array.Empty<string>());
            throw new InvalidDataException("Weapon diagnostic unexpectedly gained ordinary output authority.");
        }
        catch (BinaryAssetBuilderException error) when (error.Message.Contains("experimental", StringComparison.OrdinalIgnoreCase)) { }
        Require(!Directory.Exists(denied) && !File.Exists(denied + ".manifest"), "Production denial wrote files.");
        using var manager = OutputManager.CreateExperimentalWeaponDiagnostic(processor, document, parent);
        var asset = manager.GetBinaryAsset(instance, true); asset.Buffer = buffer;
        byte original = buffer.InstanceData[0];
        try { buffer.InstanceData[0] ^= 1; ExpectScope(() => asset.Commit()); }
        finally { buffer.InstanceData[0] = original; }
        document.PrepareOutputInstances(manager);
        string intermediate = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
        byte[] committed = File.ReadAllBytes(intermediate); byte[] damaged = committed.ToArray(); damaged[^1] ^= 1;
        try { File.WriteAllBytes(intermediate, damaged); ExpectScope(() => manager.CommitManifest(document)); }
        finally { File.WriteAllBytes(intermediate, committed); }
        Require(!File.Exists(manager.OutputDirectory + ".manifest"), "Rejected intermediate emitted a manifest.");
        manager.CommitManifest(document); manager.LinkStream(document);
        string path = manager.OutputDirectory + ".manifest";
        var parsed = ManifestReader.Read(File.ReadAllBytes(path));
        Require(parsed.Validate().Count == 0 && parsed.Header.Version == 7 && parsed.Header.IsLinked
            && parsed.Header.AllTypesHash == 0x5454A8E9u && parsed.Header.StreamChecksum == document.OutputChecksum && parsed.Assets.Count == 1,
            "Weapon manifest header differs.");
        var entry = parsed.Assets.Single();
        Require(entry.Name == instance.Handle.Name && entry.TypeId == instance.Handle.TypeId && entry.TypeHash == instance.Handle.TypeHash
            && entry.InstanceId == instance.Handle.InstanceId && entry.InstanceHash == instance.Handle.InstanceHash && entry.Tokenized == 1
            && entry.SourceFile == document.LogicalSourcePath && entry.References.SequenceEqual(instance.ValidatedReferencedInstances
                .Select(item => new AssetId(item.TypeId, item.InstanceId)))
            && parsed.ReferencedManifests.Select(item => item.Path).SequenceEqual(new[] { "static.manifest", "global.manifest" })
            && parsed.ReferencedManifests.All(item => !item.IsPatch), "Weapon manifest identity/reference mapping differs.");
        using var utility = new BinaryAssetBuilder.Utility.Manifest();
        Require(utility.Load(path, false) && utility.AssetCount == 1 && utility.Assets[0].Tokenized
            && utility.Assets[0].InstanceHash == entry.InstanceHash && utility.Assets[0].ExternalReferences.Length == 3
            && utility.Assets[0].ExternalReferences.Select(item => new AssetId(item.TypeId, item.InstanceId)).SequenceEqual(entry.References)
            && utility.StreamChecksum == document.OutputChecksum
            && utility.ReferencedManifests.Select(item => item.Path).SequenceEqual(new[] { "static.manifest", "global.manifest" }),
            "Weapon second-reader manifest differs.");
        foreach (var stream in new[] { (".bin", 0xBABB0000u, buffer.InstanceData),
            (".relo", 0xBABE0000u, buffer.RelocationData), (".imp", 0xBAB10000u, buffer.ImportsData) })
        {
            byte[] bytes = File.ReadAllBytes(manager.OutputDirectory + stream.Item1);
            Require(bytes.Length == 8 + stream.Item3.Length && BinaryPrimitives.ReadUInt32LittleEndian(bytes) == stream.Item2
                && BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(4)) == document.OutputChecksum
                && bytes.AsSpan(8).SequenceEqual(stream.Item3), "Weapon linked payload differs.");
        }
        // Reborn: changed document/source/settings/snapshots must not reuse an already valid output capability.
        string xmlTypeId = instance.XmlNode.Attributes!["TypeId"]!.Value;
        try { instance.XmlNode.Attributes["TypeId"]!.Value = "0"; ExpectScope(() => manager.LinkStream(document)); }
        finally { instance.XmlNode.Attributes["TypeId"]!.Value = xmlTypeId; }
        byte[] source = File.ReadAllBytes(document.SourcePath);
        try { File.AppendAllText(document.SourcePath, " "); ExpectScope(() => manager.LinkStream(document)); }
        finally { File.WriteAllBytes(document.SourcePath, source); }
        try { Settings.Current.BuildCache = true; ExpectScope(() => manager.LinkStream(document)); }
        finally { Settings.Current.BuildCache = false; }
        string runtimePaths = Settings.Current.ExternalManifestReferences;
        try { Settings.Current.ExternalManifestReferences = "global.manifest;static.manifest"; ExpectScope(() => manager.LinkStream(document)); }
        finally { Settings.Current.ExternalManifestReferences = runtimePaths; }
        string external = Settings.Current.ProcessedExternalManifests[0]; byte[] manifest = File.ReadAllBytes(external);
        try { var corrupt = manifest.ToArray(); corrupt[^1] ^= 1; File.WriteAllBytes(external, corrupt); ExpectScope(() => manager.LinkStream(document)); }
        finally { File.WriteAllBytes(external, manifest); }
        ExpectScope(() => manager.LinkStream(null!)); ExpectScope(() => manager.CleanOutput());
        // Reborn: repeat after restored controls and require byte-identical output rather than only checking refusal messages.
        var before = new[] { ".manifest", ".bin", ".relo", ".imp" }.ToDictionary(extension => extension,
            extension => File.ReadAllBytes(manager.OutputDirectory + extension));
        manager.LinkStream(document);
        foreach (var item in before) Require(File.ReadAllBytes(manager.OutputDirectory + item.Key).SequenceEqual(item.Value), "Weapon refusal/recovery changed streams.");
        Console.WriteLine("Weapon scoped streams: PASS (two readers, ordered references, nine refusal controls, recovery; no package/game).");
        return path;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: controls must fail specifically through diagnostic scope admission, not arbitrary I/O errors. */
    //-------------------------------------------------------------------------------------------------
    private static void ExpectScope(Action action)
    {
        try { action(); }
        catch (InvalidDataException error) when (error.Message.Contains("Experimental", StringComparison.OrdinalIgnoreCase)) { return; }
        throw new InvalidDataException("Weapon scope negative control was accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reader and stream discrepancies stop the diagnostic rather than producing a successful receipt. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition, string message)
    { if (!condition) throw new InvalidDataException(message); }
}
