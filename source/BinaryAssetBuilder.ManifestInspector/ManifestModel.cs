namespace BinaryAssetBuilder.ManifestInspector;

internal sealed record ManifestHeader(
    ushort Version,
    bool IsBigEndian,
    bool IsLinked,
    uint StreamChecksum,
    uint AllTypesHash,
    uint AssetCount,
    uint TotalInstanceDataSize,
    uint MaxInstanceChunkSize,
    uint MaxRelocationChunkSize,
    uint MaxImportsChunkSize,
    uint AssetReferenceBufferSize,
    uint ReferenceManifestNameBufferSize,
    uint AssetNameBufferSize,
    uint SourceFileNameBufferSize,
    int ContainerPrefixSize);

internal sealed record AssetId(uint TypeId, uint InstanceId);

internal sealed record ReferencedManifest(string Path, bool IsPatch);

internal sealed record ManifestAsset(
    uint TypeId,
    uint InstanceId,
    uint TypeHash,
    uint InstanceHash,
    int AssetReferenceOffset,
    int AssetReferenceCount,
    int NameOffset,
    int SourceFileNameOffset,
    int InstanceDataSize,
    int RelocationDataSize,
    int ImportsDataSize,
    uint? Tokenized,
    string Name,
    string SourceFile,
    IReadOnlyList<AssetId> References)
{
    public string TypeName
    {
        get
        {
            var separator = Name.IndexOf(':');
            return separator < 0 ? Name : Name[..separator];
        }
    }
}

internal sealed record ManifestDocument(
    ManifestHeader Header,
    IReadOnlyList<ManifestAsset> Assets,
    IReadOnlyList<ReferencedManifest> ReferencedManifests,
    TargetProfile? Profile,
    int PayloadSize,
    bool WasRefPackCompressed)
{
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        if (Header.AssetCount != Assets.Count)
        {
            errors.Add($"Header asset count {Header.AssetCount} differs from parsed count {Assets.Count}.");
        }

        var duplicate = Assets
            .GroupBy(asset => (asset.TypeId, asset.InstanceId))
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicate is not null)
        {
            errors.Add(
                $"Duplicate asset id Type=0x{duplicate.Key.TypeId:X8}, Instance=0x{duplicate.Key.InstanceId:X8}.");
        }

        if (Assets.Any(asset => asset.InstanceDataSize < 0 || asset.RelocationDataSize < 0 || asset.ImportsDataSize < 0))
        {
            errors.Add("One or more assets have a negative stream chunk size.");
        }

        if (Assets.Any(asset => asset.AssetReferenceOffset < 0 || (asset.AssetReferenceOffset & 3) != 0))
        {
            errors.Add("One or more assets have an invalid asset-reference buffer offset.");
        }

        if (Assets.Any(asset => asset.AssetReferenceCount != asset.References.Count))
        {
            errors.Add("One or more parsed asset-reference counts differ from their manifest entries.");
        }

        var instanceTotal = Assets.Aggregate(0L, (total, asset) => total + asset.InstanceDataSize);
        // LOD manifests such as static_l/static_m are overlays on a referenced
        // base manifest. Their header describes the merged logical stream,
        // while their BIN contains only locally replaced chunks.
        if (!ReferencedManifests.Any(reference => reference.IsPatch)
            && instanceTotal != Header.TotalInstanceDataSize)
        {
            errors.Add(
                $"Instance chunk total {instanceTotal} differs from header total {Header.TotalInstanceDataSize}.");
        }

        return errors;
    }
}
