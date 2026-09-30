namespace BinaryAssetBuilder.Core;

// Reborn: an experimental plugin's correct target hash must not silently authorize production stream output.
public interface IAssetBuilderOutputPolicy
{
    string ProfileName { get; }
    bool CanWriteProductionOutput { get; }
    // Reborn: experimental processor cache eligibility must not be overridden by a settings descriptor.
    bool CanUseBuildCache { get; }
    // Reborn: session/precompiled document reuse is a separate eligibility decision from network asset caching.
    bool CanReuseCompiledDocuments { get; }
}
