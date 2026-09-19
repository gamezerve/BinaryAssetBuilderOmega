using System.Runtime.InteropServices;
using Relo;

namespace SageBinaryData;

[StructLayout(LayoutKind.Sequential)]
public struct LureObjectsUpdateModuleData
{
    public UpdateModuleData Base;
    public int LureObjectLinkID;
    public float GuardRadiusMaxSqr;
    public float GuardAttackRange;
    public ObjectStatusBitFlags GuardStatus;
    public int UpdateRate;
    public TypedAssetId<GameObject> GuardMarkerObjectRef;
    public DisabledBitFlags DisabledTypesToProcess;
    public List<Vector3> GuardOffset;
}
