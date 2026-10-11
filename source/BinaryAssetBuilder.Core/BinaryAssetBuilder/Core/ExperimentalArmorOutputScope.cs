using System;
using System.IO;
using System.Linq;
using BinaryAssetBuilder.Core.SageXml;
using BinaryAssetBuilder.Utility;

namespace BinaryAssetBuilder.Core;

// Reborn: private per-manager diagnostic capability, not production eligibility or a mutable global output switch.
internal sealed class ExperimentalArmorOutputScope
{
    private readonly DocumentProcessor _processor;
    private readonly AssetDeclarationDocument _document;
    private readonly InstanceDeclaration _instance;
    private readonly Settings _settings;
    private readonly IAssetBuilderPlugin _plugin;
    private readonly uint _instanceHash, _instanceId, _processingHash;
    private readonly string _xml, _source, _name, _logicalSource;
    private readonly byte[] _sourceBytes;
    private readonly AssetBuffer _expected;

    //-------------------------------------------------------------------------------------------------
    /** Reborn: freeze one normalized armor declaration and its compiler bytes before any output directory is created. */
    //-------------------------------------------------------------------------------------------------
    internal ExperimentalArmorOutputScope(DocumentProcessor processor, AssetDeclarationDocument document)
    {
        _processor = processor; _document = document; _settings = Settings.Current;
        _instance = document.SelfInstances.Single(); _plugin = processor.Plugins.DefaultPlugin;
        _instanceHash = _instance.Handle.InstanceHash; _instanceId = _instance.Handle.InstanceId;
        _processingHash = _instance.ProcessingHash; _xml = _instance.XmlNode.OuterXml; _source = document.SourcePath;
        _name = _instance.Handle.Name; _logicalSource = document.LogicalSourcePath;
        CheckSource(_source); _sourceBytes = File.ReadAllBytes(_source);
        Validate();
        var compiled = _plugin.ProcessInstance(_instance);
        if (compiled.InstanceData.Length > 1024 * 1024 || compiled.ImportsData.Length != 0)
            throw new InvalidDataException("Experimental armor payload exceeds scope.");
        _expected = new AssetBuffer { InstanceData = (byte[])compiled.InstanceData.Clone(),
            RelocationData = (byte[])compiled.RelocationData.Clone(), ImportsData = (byte[])compiled.ImportsData.Clone() };
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject changed profile, settings, source, identity, dependencies or selected document before every scoped write. */
    //-------------------------------------------------------------------------------------------------
    internal void Validate(AssetDeclarationDocument document = null)
    {
        var policy = _plugin as IAssetBuilderOutputPolicy;
        var metadata = _plugin.GetExtendedTypeInformation(0x3A6C5E8Eu);
        if ((document != null && !ReferenceEquals(document, _document)) || !ReferenceEquals(Settings.Current, _settings)
            || !ReferenceEquals(_processor.Plugins.DefaultPlugin, _plugin) || _processor.Plugins.AllPlugins.Count != 0
            || policy == null || policy.ProfileName != "RA3EP1-Armor-Experimental-v1" || policy.CanWriteProductionOutput
            || policy.CanUseBuildCache || policy.CanReuseCompiledDocuments || _plugin.AllTypesHash != 0x5454A8E9u
            || _settings.TargetPlatform != TargetPlatform.Win32 || !_settings.LinkedStreams || _settings.BigEndian || _settings.BuildCache
            || !string.IsNullOrEmpty(_settings.CustomPostfix) || !string.IsNullOrEmpty(_settings.StreamPostfix)
            || (_settings.ProcessedExternalManifests?.Length ?? 0) != 0 || !string.IsNullOrEmpty(_settings.ExternalManifestReferences)
            || _document.InclusionItems.Count != 0 || _document.Instances.Count != 1 || _document.SelfInstances.Count != 1
            || !ReferenceEquals(_document.SelfInstances.Single(), _instance) || _instance.InheritFromHandle != null || _instance.HasCustomData
            || _instance.ReferencedInstances.Count != 0 || _instance.WeakReferencedInstances.Count != 0
            || _instance.ValidatedReferencedInstances == null || _instance.ValidatedReferencedInstances.Count != 0
            || _instance.Handle.TypeId != 0x3A6C5E8Eu || _instance.Handle.TypeHash != 0xA0E237D8u
            || !metadata.Tokenized || metadata.TypeHash != _instance.Handle.TypeHash || metadata.ProcessingHash != _processingHash
            || _instance.ProcessingHash != _processingHash || _instanceHash == 0 || _instance.Handle.InstanceHash != _instanceHash
            || _instance.Handle.InstanceId != _instanceId || _instance.XmlNode.OuterXml != _xml || _document.SourcePath != _source
            || _instance.Handle.Name != _name || _document.LogicalSourcePath != _logicalSource
            || _document.OutputInstances.Count > 1 || _document.OutputInstances.Any(item => !ReferenceEquals(item, _instance)))
            throw new InvalidDataException("Experimental armor output scope changed or is unsupported.");
        CheckSource(_source);
        if (new FileInfo(_source).Length != _sourceBytes.Length || !File.ReadAllBytes(_source).SequenceEqual(_sourceBytes))
            throw new InvalidDataException("Experimental armor source changed.");
        if (_document.OutputInstances.Count == 1 && _document.OutputChecksum != AssetDeclarationDocument.ComputeOutputChecksum(_document.OutputInstances))
            throw new InvalidDataException("Experimental armor document checksum differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: intermediate commits must contain exactly the bound compiler result, never cache/local data or a foreign declaration. */
    //-------------------------------------------------------------------------------------------------
    internal void ValidateAsset(BinaryAsset asset)
    {
        Validate();
        if (!ReferenceEquals(asset.Instance, _instance) || asset.Buffer == null
            || !asset.Buffer.InstanceData.SequenceEqual(_expected.InstanceData)
            || !asset.Buffer.RelocationData.SequenceEqual(_expected.RelocationData)
            || !asset.Buffer.ImportsData.SequenceEqual(_expected.ImportsData))
            throw new InvalidDataException("Experimental armor commit payload differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: refuse intermediate tampering before manifest/link writes, including equal-length payload changes. */
    //-------------------------------------------------------------------------------------------------
    internal void ValidateCommittedAsset(BinaryAsset asset)
    {
        if (!ReferenceEquals(asset.Instance, _instance) || !asset.IsOutputAsset || asset.Buffer != null)
            throw new InvalidDataException("Experimental armor asset has not been committed.");
        string path = Path.Combine(asset.AssetOutputDirectory, asset.AssetFileName);
        CheckAncestors(path);
        using var stream = File.OpenRead(path);
        using var header = new AssetHeader();
        header.LoadFromStream(stream, false);
        if (!header.IsValidFileLength(stream.Length) || header.TypeId != _instance.Handle.TypeId
            || header.TypeHash != _instance.Handle.TypeHash || header.InstanceId != _instanceId || header.InstanceHash != _instanceHash
            || header.InstanceDataSize != _expected.InstanceData.Length || header.RelocationDataSize != _expected.RelocationData.Length
            || header.ImportsDataSize != _expected.ImportsData.Length)
            throw new InvalidDataException("Experimental armor intermediate header differs.");
        using var reader = new BinaryReader(stream);
        foreach (byte[] chunk in new[] { _expected.InstanceData, _expected.RelocationData, _expected.ImportsData })
            if (!reader.ReadBytes(chunk.Length).SequenceEqual(chunk)) throw new InvalidDataException("Experimental armor intermediate payload differs.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject redirected source/output ancestry and bound source reads to one MiB. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckSource(string path)
    {
        CheckAncestors(path);
        if (!File.Exists(path) || new FileInfo(path).Length > 1024 * 1024) throw new InvalidDataException("Experimental armor source unavailable or oversized.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require ordinary explicit paths; this does not protect against malicious concurrent filesystem replacement. */
    //-------------------------------------------------------------------------------------------------
    internal static void CheckAncestors(string path)
    {
        if (!Path.IsPathFullyQualified(path)) throw new InvalidDataException("Experimental output paths must be absolute.");
        for (FileSystemInfo item = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path); item != null;
             item = item is FileInfo file ? file.Directory : ((DirectoryInfo)item).Parent)
            if ((item.Attributes & FileAttributes.ReparsePoint) != 0) throw new InvalidDataException("Redirected experimental output path refused.");
    }
}
