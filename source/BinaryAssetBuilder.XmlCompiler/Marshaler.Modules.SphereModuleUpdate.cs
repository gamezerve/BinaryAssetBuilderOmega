using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(Node node, SphereModuleUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.RadiusMax), "40"), &objT->RadiusMax, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.RadiusMin), "10"), &objT->RadiusMin, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.DamageTypesNotToAbsorb), null), &objT->DamageTypesNotToAbsorb, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.ScanFrequency), "1s"), &objT->ScanFrequency, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.Duration), "10s"), &objT->Duration, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.SphereBoneName), null), &objT->SphereBoneName, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.SphereSizeMultiplier), "1.0"), &objT->SphereSizeMultiplier, state);
        Marshal(node.GetChildNode(nameof(SphereModuleUpdateModuleData.PositionOffset), null), &objT->PositionOffset, state);
        Marshal(node.GetChildNode(nameof(SphereModuleUpdateModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node.GetChildNode(nameof(SphereModuleUpdateModuleData.IgnoreInsideToInsideCheck), null), &objT->IgnoreInsideToInsideCheck, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.InitiallyActive), "false"), &objT->InitiallyActive, state);
        Marshal(node.GetAttributeValue(nameof(SphereModuleUpdateModuleData.DrawDebugCircle), "true"), &objT->DrawDebugCircle, state);
        Marshal(node, (UpdateModuleData*)objT, state);
    }

    public static unsafe void Marshal(Node node, DamageSphereUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.UnpackTime), "0s"), &objT->UnpackTime, state);
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.Weapon), null), &objT->Weapon, state);
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.ExpansionPerSecond), "0"), &objT->ExpansionPerSecond, state);
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.UnpackModelConditions), ""), &objT->UnpackModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.UnpackObjectStatus), ""), &objT->UnpackObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.ModelConditions), ""), &objT->ModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(DamageSphereUpdateModuleData.ObjectStatus), ""), &objT->ObjectStatus, state);
        Marshal(node, (SphereModuleUpdateModuleData*)objT, state);
    }
}
