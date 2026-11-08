using System.Runtime.InteropServices;

namespace SageBinaryData
{
    public enum ProductionQueueType
    {
        INVALID = -1,
        MAIN_STRUCTURE,
        OTHER_STRUCTURE,
        INFANTRY,
        VEHICLE,
        AIRCRAFT,
        UPGRADE,
        // Reborn: EP1 watercraft occupies the next original production queue ordinal.
        WATERCRAFT,
        SPECIALPOWERS,
        BOOKMARKS
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProductionQueueTypeBitFlags
    {
        public const int Count = 7;
        public const int BitsInSpan = 32;
        public const int NumSpans = (Count + (BitsInSpan - 1)) / BitsInSpan;

        public unsafe fixed uint Value[NumSpans];
    }
}
