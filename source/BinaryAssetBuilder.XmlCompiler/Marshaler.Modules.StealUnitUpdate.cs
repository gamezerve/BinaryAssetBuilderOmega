using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, StealUnitUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        // Reborn: keep the derived class over the already qualified EP1 attachment record.
        Marshal(node, &objT->Base, state);
    }
}
