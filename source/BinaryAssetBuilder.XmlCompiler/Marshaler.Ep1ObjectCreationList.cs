using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: EP1 ObjectCreationList with all six official nugget lists, in the official RA3/EP1 field order and XSD defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1ObjectCreationList* value,Tracker state)
    {
        if(node is null)return;
        Ep1Flags<Ep1OCLType>(node.GetAttributeValue(nameof(Ep1ObjectCreationList.Flags),"NONE"),value->Flags.Value,Ep1OCLBitFlags.Count,Ep1OCLBitFlags.NumSpans,state);
        Marshal(node.GetChildNodes(nameof(Ep1ObjectCreationList.CreateObject)),&value->CreateObject,state);
        Marshal(node.GetChildNodes(nameof(Ep1ObjectCreationList.Attack)),&value->Attack,state);
        Marshal(node.GetChildNodes(nameof(Ep1ObjectCreationList.FireWeapon)),&value->FireWeapon,state);
        Marshal(node.GetChildNodes(nameof(Ep1ObjectCreationList.FlingObject)),&value->FlingObject,state);
        Marshal(node.GetChildNodes(nameof(Ep1ObjectCreationList.StrafeAreaSquad)),&value->StrafeAreaSquad,state);
        Marshal(node.GetChildNodes(nameof(Ep1ObjectCreationList.ParabolicCurve)),&value->ParabolicCurve,state);
        Marshal(node,(BaseAssetType*)value,state);
    }

    public static unsafe void Marshal(Node node,Ep1CreateObjectNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.PutInContainer),null),&value->PutInContainer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.CreateFX),null),&value->CreateFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.Count),"1"),&value->Count,state);
        Ep1Flags<Ep1OCNuggetOption>(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.Options),null),value->Options.Value,Ep1OCNuggetOptionFlag.Count,Ep1OCNuggetOptionFlag.NumSpans,state);
        Ep1Flags<Ep1DispositionType>(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.Disposition),null),value->Disposition.Value,Ep1DispositionTypeFlag.Count,Ep1DispositionTypeFlag.NumSpans,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.VelocityScale),"1.0"),&value->VelocityScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinForceMagnitude),null),&value->MinForceMagnitude,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxForceMagnitude),null),&value->MaxForceMagnitude,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinLifetime),null),&value->MinLifetime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxLifetime),null),&value->MaxLifetime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinDistanceAFormation),null),&value->MinDistanceAFormation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinDistanceBFormation),null),&value->MinDistanceBFormation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxDistanceFormation),null),&value->MaxDistanceFormation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.FadeTime),null),&value->FadeTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.StartingModelConditions),"INVALID"),&value->StartingModelConditions,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.TempModelCondition),"INVALID"),&value->TempModelCondition,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.TempModelConditionTime),"0s"),&value->TempModelConditionTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.DispositionIntensity),null),&value->DispositionIntensity,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.DestinationPlayer),null),&value->DestinationPlayer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.WaypointSpawn),null),&value->WaypointSpawn,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.DispositionAngle),null),&value->DispositionAngle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinForcePitch),null),&value->MinForcePitch,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxForcePitch),null),&value->MaxForcePitch,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinForceYaw),"0d"),&value->MinForceYaw,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxForceYaw),"359d"),&value->MaxForceYaw,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.OrientationOffset),null),&value->OrientationOffset,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.FadeSound),null),&value->FadeSound,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinHordeDensity),"100"),&value->MinHordeDensity,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxHordeDensity),"100"),&value->MaxHordeDensity,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MinHealth),"1.0"),&value->MinHealth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.MaxHealth),"1.0"),&value->MaxHealth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.InvulnerableTime),null),&value->InvulnerableTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.StartingBusyTime),null),&value->StartingBusyTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.JustBuiltFrames),null),&value->JustBuiltFrames,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.VeterancyLevel),null),&value->VeterancyLevel,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.FireSpecialPowerTemplate),null),&value->FireSpecialPowerTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.StatusToCopyFromParent),null),&value->StatusToCopyFromParent,state);
        Marshal(node.GetChildNode(nameof(Ep1CreateObjectNugget.Offset),null),&value->Offset,state);
        Marshal(node.GetChildNodes(nameof(Ep1CreateObjectNugget.RequiredUpgrade)),&value->RequiredUpgrade,state);
        Marshal(node.GetChildNodes(nameof(Ep1CreateObjectNugget.ForbiddenUpgrade)),&value->ForbiddenUpgrade,state);
        Marshal(node.GetChildNodes(nameof(Ep1CreateObjectNugget.CreateObject)),&value->CreateObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.UseUpgradedLocomotor),"false"),&value->UseUpgradedLocomotor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1CreateObjectNugget.DisabledWhileBusy),"false"),&value->DisabledWhileBusy,state);
    }

    public static unsafe void Marshal(Node node,Ep1AttackNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1AttackNugget.NumberOfShots),"1"),&value->NumberOfShots,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttackNugget.WeaponSlot),"PRIMARY_WEAPON"),&value->WeaponSlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1AttackNugget.DeliveryDecalRadius),null),&value->DeliveryDecalRadius,state);
    }

    public static unsafe void Marshal(Node node,Ep1FireWeaponNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FireWeaponNugget.Weapon),null),&value->Weapon,state);
    }

    public static unsafe void Marshal(Node node,Ep1FlingObjectNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1FlingObjectNugget.CreateObject),null),&value->CreateObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FlingObjectNugget.Speed),null),&value->Speed,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FlingObjectNugget.Height),"0.0"),&value->Height,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FlingObjectNugget.Count),"1"),&value->Count,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FlingObjectNugget.InitialDelay),"0s"),&value->InitialDelay,state);
        Marshal(node.GetChildNode(nameof(Ep1FlingObjectNugget.Offset),null),&value->Offset,state);
        Marshal(node.GetChildNode(nameof(Ep1FlingObjectNugget.DistanceFromTarget),null),&value->DistanceFromTarget,state);
        Marshal(node.GetChildNode(nameof(Ep1FlingObjectNugget.AngleAroundTarget),null),&value->AngleAroundTarget,state);
        Marshal(node.GetChildNode(nameof(Ep1FlingObjectNugget.DistanceAtTarget),null),&value->DistanceAtTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1FlingObjectNugget.UsePrimaryObjectForOffsets),"false"),&value->UsePrimaryObjectForOffsets,state);
    }

    public static unsafe void Marshal(Node node,Ep1StrafeAreaNugget* value,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StrafeAreaNugget.CreateObject),null),&value->CreateObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StrafeAreaNugget.Count),"1"),&value->Count,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StrafeAreaNugget.Height),"0.0"),&value->Height,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StrafeAreaNugget.DistanceFromTarget),"0.0"),&value->DistanceFromTarget,state);
        Marshal(node.GetChildNode(nameof(Ep1StrafeAreaNugget.InterOffset),null),&value->InterOffset,state);
        Marshal(node.GetChildNode(nameof(Ep1StrafeAreaNugget.IntraOffset),null),&value->IntraOffset,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StrafeAreaNugget.MaintainOffestToTarget),"true"),&value->MaintainOffestToTarget,state);
    }

    public static unsafe void Marshal(Node node,Ep1ParabolicCurveNugget* value,Tracker state)
    {
        if(node is null)return;
        Ep1Flags<Ep1ParabolicOption>(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.Options),null),value->Options.Value,Ep1ParabolicOptionFlag.Count,Ep1ParabolicOptionFlag.NumSpans,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.CreateObject),null),&value->CreateObject,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.InitialDelay),"0s"),&value->InitialDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.ApproachTime),"0s"),&value->ApproachTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.StartDistanceFromTarget),"0.0"),&value->StartDistanceFromTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.EndDistanceFromTarget),"0.0"),&value->EndDistanceFromTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.CruiseHeight),"0.0"),&value->CruiseHeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.AttackHeight),"0.0"),&value->AttackHeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.ApproachRoll),"0d"),&value->ApproachRoll,state);
        Marshal(node.GetAttributeValue(nameof(Ep1ParabolicCurveNugget.LeaveRoll),"0d"),&value->LeaveRoll,state);
        Marshal(node.GetChildNode(nameof(Ep1ParabolicCurveNugget.Offset),null),&value->Offset,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: shared EP1 flag-list parser; bit positions are the EP1 enum ordinals (= XSD positions), +/- tokens set/clear bits. */
    //-------------------------------------------------------------------------------------------------
    private static unsafe void Ep1Flags<T>(Value value,uint* words,int count,int spans,Tracker state) where T:struct,System.Enum
    {
        if(value is null)return;
        foreach(string raw in value.GetText().Split(WhiteSpaces,System.StringSplitOptions.RemoveEmptyEntries))
        {
            if(string.Equals(raw,"ALL",System.StringComparison.Ordinal)){for(int i=0;i<spans;i++)words[i]=uint.MaxValue;continue;}
            bool include=raw[0]!='-';string token=raw[0] is '+' or '-'?raw.Substring(1):raw;
            if(!System.Enum.TryParse(token,false,out T parsed))continue;
            int bit=System.Convert.ToInt32(parsed);if(bit<0||bit>=count)continue;
            if(include)words[bit/32]|=1u<<(bit%32);else words[bit/32]&=~(1u<<(bit%32));
        }
        for(int i=0;i<spans;i++)state.InplaceEndianToPlatform(&words[i]);
    }
}
