using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;
namespace SageBinaryData;

// Reborn: these dormant original multiplayer types use the independently observed PC factory and field-loader ABI.
[StructLayout(LayoutKind.Sequential)]
public struct ConquerCardDefinitionBase { public BaseAssetType Base; public int Cost; public int Maximum; }
// Reborn: upgrade and veterancy choices preserve eight-byte records and concrete strong imports.
[StructLayout(LayoutKind.Sequential)]
public struct ConquerCardUpgrade { public AssetReference<UpgradeTemplate> Upgrade; public int Cost; }
[StructLayout(LayoutKind.Sequential)]
public struct ConquerCardVeterancy { public VeterancyLevel Veterancy; public int Cost; }
// Reborn: list destinations and strides are read from the original Unit field loader, not inferred from a later game.
[StructLayout(LayoutKind.Sequential)]
public struct ConquerCardDefinitionUnit { public ConquerCardDefinitionBase Base; public AssetReference<GameObject> Unit; public List<ConquerCardUpgrade> Upgrade; public List<ConquerCardVeterancy> Veterancy; }
[StructLayout(LayoutKind.Sequential)]
public struct ConquerCardDefinitionStructure { public ConquerCardDefinitionBase Base; public AssetReference<GameObject> Structure; }
[StructLayout(LayoutKind.Sequential)]
public struct ConquerCardDefinitionPower { public ConquerCardDefinitionBase Base; public AssetReference<SpecialPowerTemplate> Power; public AnsiString Side; }
// Reborn: the native Siege loader defaults ALL to two and keeps Seconds/EvaEvent/EventType at offsets four/eight/twelve.
public enum SiegeEvaEventType { SIEGE, GENERIC, ALL }
[StructLayout(LayoutKind.Sequential)]
public struct SiegeEvaEvent { public BaseInheritableAsset Base; public Time Seconds; public AssetReference<BaseAssetType> EvaEvent; public SiegeEvaEventType EventType; }
