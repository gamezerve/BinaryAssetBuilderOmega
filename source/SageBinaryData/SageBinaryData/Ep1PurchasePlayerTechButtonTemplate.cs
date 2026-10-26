using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;
// Reborn: EP1 purchase buttons store technology and state imports after the base asset word.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PurchasePlayerTechButtonTemplate { public BaseAssetType Base; public AssetReference<BaseAssetType> PlayerTech; public AssetReference<BaseAssetType> StateData; }
