using System.Runtime.InteropServices;
using Relo;
namespace SageBinaryData;

// Reborn: each retail protocol binding contains two strong asset imports; the root owns their ordered list.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerTechUpgradeBindingInfo
{
    public AssetReference<BaseAssetType> TechDependency;
    public AssetReference<UpgradeTemplate> UpgradeToGrant;
}

// Reborn: retain the ordinary inheritable asset header before the binding list.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerTechUpgradeBinding
{
    public BaseInheritableAsset Base;
    public List<Ep1PlayerTechUpgradeBindingInfo> Binding;
}
