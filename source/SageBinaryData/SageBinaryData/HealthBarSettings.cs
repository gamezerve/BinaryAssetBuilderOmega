
using Relo;
using System.Runtime.InteropServices;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct HealthBarObjectMapping
{
    // Reborn: HealthBarTemplateRef is a strong import selector, not a weak instance hash.
    public AssetReference<HealthBarTemplate> HealthBar;
    public ObjectFilter ObjectFilter;
}

[StructLayout(LayoutKind.Sequential)]
public struct HealthBarSettings
{
    public BaseAssetType Base;
    // Reborn: retail EP1 stores the default strong reference at +4, before the mapping list at +8.
    public AssetReference<HealthBarTemplate> DefaultHealthBar;
    public List<HealthBarObjectMapping> ObjectMapping;
}
