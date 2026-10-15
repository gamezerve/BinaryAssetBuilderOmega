using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 inserts ShroudClearingHeight at +8; preserve the legacy 36-byte VisionInfo for unrelated consumers.
[StructLayout(LayoutKind.Sequential)]
public struct Ep1VisionInfo
{
    public float VisionRange;
    public float ShroudClearingRange;
    public float ShroudClearingHeight;
    public Percentage VisionSide;
    public Percentage VisionRear;
    public float VisionBonusTestRadius;
    public uint VisionBonusTestSegments;
    public Percentage VisionBonusPercentPerFoot;
    public Percentage MaxVisionBonusPercent;
    public Percentage MinVisionBonusPercent;
}
