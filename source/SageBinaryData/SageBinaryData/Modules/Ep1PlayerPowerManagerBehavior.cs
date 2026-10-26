using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: Uprising extends the KW manager with a strong optional EVA event reference; the x86 native header is sixteen bytes.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1PlayerPowerManagerBehaviorModuleData
{
    public UpdateModuleData Base;
    public AssetReference<SpecialPowerTemplate> SpecialPowerTemplate;
    public AssetReference<BaseAssetType> OnAvailableEvaEvent;
}
