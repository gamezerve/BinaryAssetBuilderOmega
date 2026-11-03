namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: the three standard stock Includes name already compiled runtime providers; their XML bodies are not source dependencies.
internal static class SdkStockStreamReference
{
    // Reborn: recognize only the conventional root stream aliases; arbitrary reference Includes still require explicit provider support.
    internal static bool Known(string logical)=>logical.Replace('\\','/').ToLowerInvariant() is "data:static.xml" or "data:global.xml" or "data:audio.xml";
}
