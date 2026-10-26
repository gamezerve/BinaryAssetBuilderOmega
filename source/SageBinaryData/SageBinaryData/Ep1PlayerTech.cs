using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;
// Reborn: EP1 PlayerTech stores cost, weak grant FX and a strong dependency list after its base asset word.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerTech { public BaseInheritableAsset Base; public uint TechPointsRequired; public TypedAssetId<FXList> GrantFX; public List<AssetReference<BaseAssetType>> TechDependency; }
