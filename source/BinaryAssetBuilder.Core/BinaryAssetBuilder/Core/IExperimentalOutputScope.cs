namespace BinaryAssetBuilder.Core;

// Reborn: internal per-manager contract keeps each diagnostic asset family responsible for its own exact admission.
internal interface IExperimentalOutputScope
{
    int InstanceCount { get; }
    void Validate(SageXml.AssetDeclarationDocument document = null);
    void ValidateAsset(BinaryAsset asset);
    void ValidateCommittedAsset(BinaryAsset asset);
}
