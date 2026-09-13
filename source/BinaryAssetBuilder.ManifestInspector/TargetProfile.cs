namespace BinaryAssetBuilder.ManifestInspector;

internal sealed record TargetProfile(
    string Name,
    ushort ManifestVersion,
    uint AllTypesHash)
{
    public static readonly TargetProfile TiberiumWars = new("Tiberium Wars 1.09", 5, 0xEB19D975);
    public static readonly TargetProfile KanesWrath = new("Kane's Wrath 1.02", 5, 0x12B3E763);
    public static readonly TargetProfile RedAlert3 = new("Red Alert 3", 6, 0x54EEE764);
    public static readonly TargetProfile Uprising = new("Red Alert 3 Uprising", 7, 0x5454A8E9);

    public static IReadOnlyList<TargetProfile> Known { get; } =
    [
        TiberiumWars,
        KanesWrath,
        RedAlert3,
        Uprising
    ];

    public static TargetProfile? Identify(ushort version, uint allTypesHash) =>
        Known.FirstOrDefault(profile =>
            profile.ManifestVersion == version && profile.AllTypesHash == allTypesHash);
}
