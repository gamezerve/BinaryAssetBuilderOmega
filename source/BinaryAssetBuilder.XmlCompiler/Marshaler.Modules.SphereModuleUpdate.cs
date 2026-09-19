using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    public static unsafe void Marshal(string text, ShieldSphereUpdateOptionFlag* objT, Tracker state)
    {
        foreach (string rawToken in text.Split(WhiteSpaces, System.StringSplitOptions.RemoveEmptyEntries))
        {
            bool include = rawToken[0] != '-';
            string token = rawToken[0] is '+' or '-' ? rawToken[1..] : rawToken;
            if (!System.Enum.TryParse(token, false, out ShieldSphereUpdateOption value))
            {
                continue;
            }
            uint bit = 1u << (int)value;
            objT->Value[0] = include ? objT->Value[0] | bit : objT->Value[0] & ~bit;
        }
        state.InplaceEndianToPlatform(&objT->Value[0]);
    }

    public static unsafe void Marshal(Value value, ShieldSphereUpdateOptionFlag* objT, Tracker state)
    {
        if (value is not null)
        {
            Marshal(value.GetText(), objT, state);
        }
    }

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

    public static unsafe void Marshal(Node node, ShieldSphereUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ShieldSphereUpdateModuleData.MaxDamage), "0"), &objT->MaxDamage, state);
        Marshal(node.GetAttributeValue(nameof(ShieldSphereUpdateModuleData.ObjectStatus), ""), &objT->ObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(ShieldSphereUpdateModuleData.ModelCondition), ""), &objT->ModelCondition, state);
        Marshal(node.GetAttributeValue(nameof(ShieldSphereUpdateModuleData.AttributeModifierName), null), &objT->AttributeModifierName, state);
        Marshal(node.GetAttributeValue(nameof(ShieldSphereUpdateModuleData.ShieldedObjectStatus), ""), &objT->ShieldedObjectStatus, state);
        Marshal(node.GetAttributeValue(nameof(ShieldSphereUpdateModuleData.Options), null), &objT->Options, state);
        Marshal(node, (SphereModuleUpdateModuleData*)objT, state);
    }

    public static unsafe void Marshal(Node node, YurikoShieldSphereUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(YurikoShieldSphereUpdateModuleData.MajorShieldHitFX), null), &objT->MajorShieldHitFX, state);
        Marshal(node.GetAttributeValue(nameof(YurikoShieldSphereUpdateModuleData.MinorShieldHitFX), null), &objT->MinorShieldHitFX, state);
        Marshal(node.GetAttributeValue(nameof(YurikoShieldSphereUpdateModuleData.MinorShieldDamageTypes), null), &objT->MinorShieldDamageTypes, state);
        Marshal(node, (ShieldSphereUpdateModuleData*)objT, state);
    }
}
