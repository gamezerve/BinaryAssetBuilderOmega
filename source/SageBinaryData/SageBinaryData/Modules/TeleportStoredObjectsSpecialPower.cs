using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: teleport storage retains the SpecialPower header, paired storage ID, OCL import and second-click selection flag.
[StructLayout(LayoutKind.Sequential)]
public struct TeleportStoredObjectsSpecialPowerModuleData
{
    public SpecialPowerModuleData Base;
    public int StoreObjectsLinkID;
    public AssetReference<ObjectCreationList> OCL;
    public SageBool GetObjectsCurrentlyAtStorePosition;
}
