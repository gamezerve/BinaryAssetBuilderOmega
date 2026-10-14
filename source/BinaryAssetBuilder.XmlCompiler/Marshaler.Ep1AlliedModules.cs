using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve destruction bone strings, typed FX imports and weak object references with official velocity defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, BuildingDestructionItem* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionItem.BonePrefix), null), &objT->BonePrefix, state);
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionItem.FX), null), &objT->FX, state);
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionItem.CreationObject), null), &objT->CreationObject, state);
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionItem.FlingVelocity), "1.0"), &objT->FlingVelocity, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile ordered destruction records and the existing die base; an empty required Item collection fails explicitly. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, BuildingDestructionModuleData* objT, Tracker state)
    {
        if (node is null) return;
        var items = node.GetChildNodes(nameof(BuildingDestructionModuleData.Item));
        if (items == null || items.GetCount() == 0) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "EP1 building destruction requires Item children.");
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionModuleData.AssociatedModule), null), &objT->AssociatedModule, state);
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionModuleData.MaxDebrisCloudsPerSecond), "2.0"), &objT->MaxDebrisCloudsPerSecond, state);
        Marshal(node.GetAttributeValue(nameof(BuildingDestructionModuleData.FillerMesh), "NEWSKIN_FILL"), &objT->FillerMesh, state);
        Marshal(items, &objT->Item, state);
        Marshal(node, (DieModuleData*)objT, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain the existing upgrade base and by-value filter while distinguishing absent RadiusY from an authored zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, TechUpgradeModuleData* objT, Tracker state)
    {
        if (node is null) return;
        Marshal(node.GetAttributeValue(nameof(TechUpgradeModuleData.Radius), null), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(TechUpgradeModuleData.RadiusY), null), &objT->RadiusY, state);
        Ep1ToggleEnum(node.GetAttributeValue(nameof(TechUpgradeModuleData.DistanceType), "CIRCULAR"), &objT->DistanceType, state, typeof(Ep1DistanceCheckType), false);
        Marshal(node.GetAttributeValue(nameof(TechUpgradeModuleData.FXLineBoneName), null), &objT->FXLineBoneName, state);
        Marshal(node.GetChildNode(nameof(TechUpgradeModuleData.ObjectFilter), null), &objT->ObjectFilter, state);
        Marshal(node, (UpgradeModuleData*)objT, state);
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the required event-list string without pretending the target script callback has been validated. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, LUAEventListBehaviorModuleData* objT, Tracker state)
    {
        if (node is null) return;
        var name = node.GetAttributeValue(nameof(LUAEventListBehaviorModuleData.EventListName), null);
        if (name == null) throw new BinaryAssetBuilderException(ErrorCode.InternalError, "EP1 LUAEventList requires EventListName.");
        Marshal(name, &objT->EventListName, state);
        Marshal(node, (BehaviorModuleData*)objT, state);
    }
}
