using System.Runtime.InteropServices;
using Relo;
using AnsiString=Relo.String<sbyte>;

namespace SageBinaryData;

// Reborn: EP1 uses a 44-byte upgrade root containing a source-sized nugget list, unlike the legacy KW root-level fields.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1SubObjectsUpgradeModuleData
{
    public UpgradeModuleData Base;
    public List<Ep1SubObjectsUpgradeNugget> Nugget;
}

// Reborn: retail 44-byte nugget layout; per-upgrade visibility, textures and animation remain independently authored.
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Ep1SubObjectsUpgradeNugget
{
    public List<AnsiString> ShowSubObjects;
    public List<AnsiString> HideSubObjects;
    public Time FadeTime;
    public Time WaitBeforeFadeTime;
    public TypedAssetId<UpgradeTemplate> Upgrade;
    public List<ReplaceTexture> UpgradeTexture;
    public AnimAndDuration* CustomAnimAndDuration;
    public SageBool SkipFadeOnCreate;
    public SageBool HideSubObjectsOnRemove;
    public SageBool UnHideSubObjectsOnRemove;
}
