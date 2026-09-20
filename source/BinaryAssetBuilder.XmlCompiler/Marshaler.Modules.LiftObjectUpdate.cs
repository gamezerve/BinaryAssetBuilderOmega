using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, LiftedUnitModelStateMap* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LiftedUnitModelStateMap.ModelState), null), &objT->ModelState, state);
        Marshal(node.GetChildNode(nameof(LiftedUnitModelStateMap.ObjectFilter), null), &objT->ObjectFilter, state);
    }

    public static unsafe void Marshal(Node node, LiftedObjectModelStateMapList* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetChildNodes(nameof(LiftedObjectModelStateMapList.LiftedUnitModelState)), &objT->LiftedUnitModelState, state);
    }

    public static unsafe void Marshal(Node node, LiftObjectUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.LiftObjectLinkID), "0"), &objT->LiftObjectLinkID, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.LiftVelocity), null), &objT->LiftVelocity, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.MaxElevationFromGround), null), &objT->MaxElevationFromGround, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.TimeIncrement), null), &objT->TimeIncrement, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.MaxTimeLifted), null), &objT->MaxTimeLifted, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.MaxStructureShakeVelocity), "0"), &objT->MaxStructureShakeVelocity, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.AirplaneCrashWeapon), null), &objT->AirplaneCrashWeapon, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.SoftLandingWeapon), null), &objT->SoftLandingWeapon, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.RotationSpeed), "1.0"), &objT->RotationSpeed, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.Shader), null), &objT->Shader, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.ShakeIntensity), "0.0"), &objT->ShakeIntensity, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.ShakeRadius), "0.0"), &objT->ShakeRadius, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.ShakeFade), "1.0"), &objT->ShakeFade, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.DisabledTypesToProcess), "HELD PARALYZED EMP"), &objT->DisabledTypesToProcess, state);
        Marshal(node.GetChildNode(nameof(LiftObjectUpdateModuleData.ModelStateObjectFilters), null), &objT->ModelStateObjectFilters, state);
        Marshal(node.GetAttributeValue(nameof(LiftObjectUpdateModuleData.CrusherModifiesVelocity), "false"), &objT->CrusherModifiesVelocity, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }
}
