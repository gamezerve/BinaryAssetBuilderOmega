namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: Studio explicitly selects fast/developer builds; standalone regression commands retain their existing full-test default.
internal static class SdkBuildTests
{
    internal const string EnvironmentKey="REBORN_DEVELOPER_BUILD_TESTS";
    internal static bool Enabled=>Environment.GetEnvironmentVariable(EnvironmentKey) switch{null or "" or "1"=>true,"0"=>false,_=>throw new InvalidDataException("Developer build tests must be selected as 0 or 1.")};
}
