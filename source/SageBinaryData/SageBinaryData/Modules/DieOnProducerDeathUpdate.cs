using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

// Reborn: retail EP1 producer-death watchers are 16 bytes: update header, sleep interval, then one padded boolean.
[StructLayout(LayoutKind.Sequential)]
public struct DieOnProducerDeathUpdateModuleData
{
    public UpdateModuleData Base;
    public Time SleepInterval;
    public SageBool DieOnProducerDisabled;
}
