using System.Runtime.InteropServices;

namespace SageBinaryData;

// Reborn: EP1 PropBody adds no health or other payload fields to BodyModuleData; its native header is eight bytes.
[StructLayout(LayoutKind.Sequential)]
public struct PropBodyModuleData { public BodyModuleData Base; }
