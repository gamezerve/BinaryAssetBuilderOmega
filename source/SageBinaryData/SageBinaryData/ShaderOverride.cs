using System.Runtime.InteropServices;
using Relo;
using AnsiString = Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: official RA3 compiler metadata and EP1 slices define a 16-byte rule with an optional POID pointer, inline replacement POID and length/pointer technique string.
[StructLayout(LayoutKind.Sequential)]
public struct ShaderOverrideRule
{
    public unsafe TypedAssetId<FXShaderMaterial>* IfOriginalShaderIs;
    public TypedAssetId<FXShaderMaterial> ReplaceShaderName;
    public AnsiString ReplaceTechniqueName;
}

// Reborn: replace the reference-only placeholder with the observed 16-byte native root; AssetReference<ShaderOverride> remains a four-byte import record.
[StructLayout(LayoutKind.Sequential)]
public struct ShaderOverride
{
    public BaseAssetType Base;
    public uint Priority;
    public List<ShaderOverrideRule> Rule;
}
