using System;
using System.IO;
using System.Linq;
using System.Buffers.Binary;
using System.Security.Cryptography;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.Core;

// Reborn: freeze only the reviewed AK47 document, references and stock-shaped payload for diagnostic stream writes.
internal sealed class ExperimentalWeaponOutputScope : IExperimentalOutputScope
{
    private readonly DocumentProcessor processor;
    private readonly AssetDeclarationDocument document;
    private readonly InstanceDeclaration instance;
    private readonly IAssetBuilderPlugin plugin;
    private readonly Settings settings;
    private readonly string xml, source, logicalSource, references;
    private readonly byte[] sourceBytes;
    private readonly string[] manifests, manifestDigests;
    private readonly uint instanceId, instanceHash, processingHash;
    private readonly AssetBuffer expected;
    public int InstanceCount => 1;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: perform exact admission and independently fingerprint normalized weapon bytes before any output directory exists. */
    //-------------------------------------------------------------------------------------------------
    internal ExperimentalWeaponOutputScope(DocumentProcessor processor, AssetDeclarationDocument document)
    {
        if (document == null || document.SelfInstances.Count != 1 || document.OutputInstances.Count != 0)
            throw new InvalidDataException("Experimental weapon requires one fresh document instance.");
        this.processor = processor; this.document = document; settings = Settings.Current;
        instance = document.SelfInstances.Single(); plugin = processor.Plugins.GetPlugin(instance.Handle.TypeId);
        xml = instance.XmlNode.OuterXml; source = document.SourcePath; logicalSource = document.LogicalSourcePath;
        ExperimentalArmorOutputScope.CheckSource(source); sourceBytes = File.ReadAllBytes(source);
        instanceId = instance.Handle.InstanceId; instanceHash = instance.Handle.InstanceHash; processingHash = instance.ProcessingHash;
        references = ReferenceSignature();
        manifests = (settings.ProcessedExternalManifests ?? Array.Empty<string>()).ToArray();
        if (manifests.Length != 2) throw new InvalidDataException("Experimental weapon requires two frozen external manifests.");
        manifestDigests = manifests.Select(DigestManifest).ToArray();
        Validate();
        var compiled = plugin.ProcessInstance(instance);
        if (compiled.InstanceData.Length != 2508 || compiled.RelocationData.Length != 28 || compiled.ImportsData.Length != 0)
            throw new InvalidDataException("Experimental weapon payload shape differs.");
        var baseline = compiled.InstanceData.ToArray();
        float damage = BinaryPrimitives.ReadSingleLittleEndian(baseline.AsSpan(2392, 4));
        if (!float.IsFinite(damage) || damage < 0 || damage > 10000)
            throw new InvalidDataException("Experimental weapon damage exceeds the proven interval.");
        BinaryPrimitives.WriteSingleLittleEndian(baseline.AsSpan(2392, 4), 5f);
        if (Convert.ToHexString(SHA256.HashData(baseline)) != "2B0C3AABB5521C7C8315A1A5EA22DFCF8D681C6546E9AC58DBF0D0495B2A8C6B")
            throw new InvalidDataException("Experimental weapon differs outside the reviewed damage slot.");
        uint[] locations = { 2068, 2224, 2228, 2236, 2248, 2348, uint.MaxValue };
        for (int index = 0; index < locations.Length; index++)
            if (BinaryPrimitives.ReadUInt32LittleEndian(compiled.RelocationData.AsSpan(index * 4, 4)) != locations[index])
                throw new InvalidDataException("Experimental weapon relocation differs.");
        expected = new AssetBuffer { InstanceData = compiled.InstanceData.ToArray(),
            RelocationData = compiled.RelocationData.ToArray(), ImportsData = compiled.ImportsData.ToArray() };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: every write rechecks source, mapped plugin, schema identity, ordered dependencies and immutable external snapshots. */
    //-------------------------------------------------------------------------------------------------
    public void Validate(AssetDeclarationDocument selected = null)
    {
        var policy = plugin as IAssetBuilderOutputPolicy;
        var metadata = plugin.GetExtendedTypeInformation(0x94D4D96Eu);
        string[] names = { "Multisound:SOV_Conscript_GunFireMS", "FXList:FX_MuzzleSoviet", "FXList:FX_MuzzleSoviet" };
        if ((selected != null && !ReferenceEquals(selected, document)) || !ReferenceEquals(Settings.Current, settings)
            || !ReferenceEquals(processor.Plugins.GetPlugin(0x94D4D96Eu), plugin)
            || !ReferenceEquals(processor.Plugins.DefaultPlugin, plugin) || processor.Plugins.AllPlugins.Count != 1
            || policy == null || policy.ProfileName != "RA3EP1-AK47-Document-Diagnostic-v1" || policy.CanWriteProductionOutput
            || policy.CanUseBuildCache || policy.CanReuseCompiledDocuments || plugin.AllTypesHash != 0x5454A8E9u
            || settings.TargetPlatform != TargetPlatform.Win32 || !settings.LinkedStreams || settings.BigEndian || settings.BuildCache
            || !string.IsNullOrEmpty(settings.CustomPostfix) || !string.IsNullOrEmpty(settings.StreamPostfix)
            || settings.ExternalManifestReferences != "static.manifest;global.manifest"
            || !(settings.ProcessedExternalManifests ?? Array.Empty<string>()).SequenceEqual(manifests)
            || document.InclusionItems.Count != 0 || document.Instances.Count != 1 || document.SelfInstances.Count != 1
            || !ReferenceEquals(document.SelfInstances.Single(), instance) || instance.InheritFromHandle != null || instance.HasCustomData
            || instance.WeakReferencedInstances.Count != 0 || instance.ReferencedFiles.Count != 0
            || instance.ValidatedReferencedInstances == null || !instance.ValidatedReferencedInstances.Select(item => item.Name).SequenceEqual(names)
            || instance.ReferencedInstances.Count != 3 || instance.AllDependentInstances == null || instance.AllDependentInstances.Count != 0
            || ReferenceSignature() != references || instance.Handle.Name != "WeaponTemplate:SovietAntiInfantryInfantryAK47"
            || instance.Handle.TypeId != 0x94D4D96Eu || instance.Handle.TypeHash != 0x27E8D984u
            || !metadata.Tokenized || metadata.HasCustomData || metadata.UseBuildCache || metadata.TypeHash != instance.Handle.TypeHash
            || metadata.ProcessingHash != processingHash || instance.ProcessingHash != processingHash
            || instance.Handle.InstanceId != instanceId || instance.Handle.InstanceHash != instanceHash || instanceHash == 0
            || instance.XmlNode.OuterXml != xml || document.SourcePath != source || document.LogicalSourcePath != logicalSource
            || document.OutputInstances.Count > 1 || document.OutputInstances.Any(item => !ReferenceEquals(item, instance)))
            throw new InvalidDataException("Experimental weapon scope changed or is unsupported.");
        ExperimentalArmorOutputScope.CheckSource(source);
        if (!File.ReadAllBytes(source).SequenceEqual(sourceBytes)
            || !manifests.Select(DigestManifest).SequenceEqual(manifestDigests))
            throw new InvalidDataException("Experimental weapon source or external manifest changed.");
        if (document.OutputInstances.Count == 1 && document.OutputChecksum != AssetDeclarationDocument.ComputeOutputChecksum(document.OutputInstances))
            throw new InvalidDataException("Experimental weapon checksum differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only the bound instance and exact frozen payload may enter the intermediate writer. */
    //-------------------------------------------------------------------------------------------------
    public void ValidateAsset(BinaryAsset asset)
    {
        Validate();
        if (!ReferenceEquals(asset.Instance, instance) || asset.Buffer == null
            || !asset.Buffer.InstanceData.SequenceEqual(expected.InstanceData)
            || !asset.Buffer.RelocationData.SequenceEqual(expected.RelocationData)
            || !asset.Buffer.ImportsData.SequenceEqual(expected.ImportsData))
            throw new InvalidDataException("Experimental weapon commit payload differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: independently reload intermediate headers and chunks before manifest or link output. */
    //-------------------------------------------------------------------------------------------------
    public void ValidateCommittedAsset(BinaryAsset asset)
    {
        Validate();
        if (!ReferenceEquals(asset.Instance, instance) || !asset.IsOutputAsset || asset.Buffer != null)
            throw new InvalidDataException("Experimental weapon has not been committed.");
        string path = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
        ExperimentalArmorOutputScope.CheckAncestors(path);
        using var stream = File.OpenRead(path); using var header = new AssetHeader(); header.LoadFromStream(stream, false);
        if (!header.IsValidFileLength(stream.Length) || header.TypeId != instance.Handle.TypeId || header.TypeHash != instance.Handle.TypeHash
            || header.InstanceId != instanceId || header.InstanceHash != instanceHash || header.InstanceDataSize != 2508
            || header.RelocationDataSize != 28 || header.ImportsDataSize != 0)
            throw new InvalidDataException("Experimental weapon intermediate header differs.");
        using var reader = new BinaryReader(stream);
        foreach (byte[] chunk in new[] { expected.InstanceData, expected.RelocationData, expected.ImportsData })
            if (!reader.ReadBytes(chunk.Length).SequenceEqual(chunk))
                throw new InvalidDataException("Experimental weapon intermediate payload differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze concrete dependency identity/hash order separately from nominal source references. */
    //-------------------------------------------------------------------------------------------------
    private string ReferenceSignature() => string.Join(";", instance.ReferencedInstances.Concat(instance.ValidatedReferencedInstances ?? new System.Collections.Generic.List<InstanceHandle>())
        .Select(item => $"{item.Name}:{item.TypeId:X8}:{item.InstanceId:X8}:{item.TypeHash:X8}:{item.InstanceHash:X8}"));

    //-------------------------------------------------------------------------------------------------
    /** Reborn: bound ordinary external snapshot reads; this is not protection from hostile concurrent filesystem replacement. */
    //-------------------------------------------------------------------------------------------------
    private static string DigestManifest(string path)
    {
        ExperimentalArmorOutputScope.CheckAncestors(path);
        if (!File.Exists(path) || new FileInfo(path).Length > 64 * 1024 * 1024)
            throw new InvalidDataException("Experimental weapon external snapshot unavailable or oversized.");
        using var stream = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(stream));
    }
}
