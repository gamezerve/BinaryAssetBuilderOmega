using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate WaterTextureType tokens before storing the original enum ordinal. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockWaterTextureType* target,Tracker state)
    {
        if(value is null)return;
        if(!System.Enum.TryParse<Ep1StockWaterTextureType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid WaterTextureType: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate FontUse tokens before storing the original enum ordinal. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockFontUse* target,Tracker state)
    {
        if(value is null)return;
        if(!System.Enum.TryParse<Ep1StockFontUse>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid FontUse: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate AIBeaconType tokens before storing the original enum ordinal. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockAIBeaconType* target,Tracker state)
    {
        if(value is null)return;
        if(!System.Enum.TryParse<Ep1StockAIBeaconType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid AIBeaconType: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate ShroudStatusType tokens before storing the original enum ordinal. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockShroudStatusType* target,Tracker state)
    {
        if(value is null)return;
        if(!System.Enum.TryParse<Ep1StockShroudStatusType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid ShroudStatusType: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate UIMouseTacticalQuickChatDisplayChatDestination tokens before storing the original enum ordinal. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockUIMouseTacticalQuickChatDisplayChatDestination* target,Tracker state)
    {
        if(value is null)return;
        if(!System.Enum.TryParse<Ep1StockUIMouseTacticalQuickChatDisplayChatDestination>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid UIMouseTacticalQuickChatDisplayChatDestination: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: validate MouseHUDFixedElementId tokens before storing the original enum ordinal. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockMouseHUDFixedElementId* target,Tracker state)
    {
        if(value is null)return;
        if(!System.Enum.TryParse<Ep1StockMouseHUDFixedElementId>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Invalid MouseHUDFixedElementId: "+value);
        *target=parsed;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal JoypadCommandBarTemplate with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockJoypadCommandBarTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockJoypadCommandBarTemplate.HomeButton),null),&target->HomeButton,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockJoypadCommandBarTemplate.Slot)),&target->Slot,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockJoypadCommandBarTemplate.GeneralButton)),&target->GeneralButton,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal JoypadCommandBarTemplateButtonSlot with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockJoypadCommandBarTemplateButtonSlot* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockJoypadCommandBarTemplateButtonSlot.Ordinal),null),&target->Ordinal,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockJoypadCommandBarTemplateButtonSlot.Button),null),&target->Button,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DamageFX with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDamageFX* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockDamageFX.FXGroup)),&target->FXGroup,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DamageFXGroup with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDamageFXGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.Type),"DEFAULT"),&target->Type,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.Amount),"0.0"),&target->Amount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.ThrottleTime),"0s"),&target->ThrottleTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.MajorFX),null),&target->MajorFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.MinorFX),null),&target->MinorFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.RearFX),null),&target->RearFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.SideFX),null),&target->SideFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.TopFX),null),&target->TopFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.VeteranMajorFX),null),&target->VeteranMajorFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.VeteranMinorFX),null),&target->VeteranMinorFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.VeteranRearFX),null),&target->VeteranRearFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.VeteranSideFX),null),&target->VeteranSideFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXGroup.VeteranTopFX),null),&target->VeteranTopFX,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal WaterTextureList with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockWaterTextureList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockWaterTextureList.Type),null),&target->Type,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockWaterTextureList.Texture)),&target->Texture,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal StaticGameLOD with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockStaticGameLOD* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.Level),"MEDIUM"),&target->Level,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.ModelLOD),"HIGH"),&target->ModelLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.EffectsLOD),"MEDIUM"),&target->EffectsLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.MaxParticleCount),"1500"),&target->MaxParticleCount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.ShadowLOD),"MEDIUM"),&target->ShadowLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.TerrainLOD),"MEDIUM"),&target->TerrainLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.WaterLOD),"MEDIUM"),&target->WaterLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.MaxTankTrackEdges),"100"),&target->MaxTankTrackEdges,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.MaxTankTrackOpaqueEdges),"25"),&target->MaxTankTrackOpaqueEdges,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.MaxTankTrackFadeDelay),"30000"),&target->MaxTankTrackFadeDelay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.TextureReductionFactor),"0"),&target->TextureReductionFactor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.AnimationLOD),"HIGH"),&target->AnimationLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.ShaderLOD),"MEDIUM"),&target->ShaderLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.DecalLOD),"HIGH"),&target->DecalLOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.MinParticlePriority),"MEDIUM_OR_ABOVE"),&target->MinParticlePriority,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.MinParticleSkipPriority),"ALWAYS_RENDER"),&target->MinParticleSkipPriority,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.AntiAliasingQuality),"0"),&target->AntiAliasingQuality,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.UseShadowDecals),"true"),&target->UseShadowDecals,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.UseShadowMapping),"false"),&target->UseShadowMapping,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.UseTerrainNormalMap),"false"),&target->UseTerrainNormalMap,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.ShowProps),"true"),&target->ShowProps,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockStaticGameLOD.EnableVSync),"true"),&target->EnableVSync,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal WaterSetting with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockWaterSetting* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockWaterSetting.TimeOfDay),null),&target->TimeOfDay,state);
        Marshal(node.GetChildNode(nameof(Ep1StockWaterSetting.DiffuseColor),null),&target->DiffuseColor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal GameLODPreset with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockGameLODPreset* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockGameLODPreset.Level),"MEDIUM"),&target->Level,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGameLODPreset.SystemMemory),"500"),&target->SystemMemory,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGameLODPreset.Xres),"1024"),&target->Xres,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGameLODPreset.YRes),"768"),&target->YRes,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockGameLODPreset.Processor)),&target->Processor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal TerrainParticleMapping with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockTerrainParticleMapping* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainParticleMapping.AccelerationMultiplier),"100%"),&target->AccelerationMultiplier,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockTerrainParticleMapping.Mapping)),&target->Mapping,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal TerrainParticleMap with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockTerrainParticleMap* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainParticleMap.TerrainClass),null),&target->TerrainClass,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainParticleMap.ParticleSystem),null),&target->ParticleSystem,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainParticleMap.RequiredModelConditions),null),&target->RequiredModelConditions,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainParticleMap.ForbiddenModelConditions),null),&target->ForbiddenModelConditions,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal FontDefaultSetting with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockFontDefaultSetting* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockFontDefaultSetting.Name),null),&target->Name,state);
        // Reborn: explicitly authored optional values own tracked relocated storage, including scalar zero.
        { var value=node.GetChildNode(nameof(Ep1StockFontDefaultSetting.Size),null); if(value is not null){using var cell=state.Push((void**)&target->Size,(uint)sizeof(uint),1);Marshal(value.GetValue(),target->Size,state);} }
        Marshal(node.GetChildNode(nameof(Ep1StockFontDefaultSetting.Settings),null),&target->Settings,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal FontSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockFontSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockFontSettings.AntiAliased),null),&target->AntiAliased,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ArmyDefinition with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockArmyDefinition* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.Side),null),&target->Side,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.MCVTemplate),null),&target->MCVTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.GathererTemplate),null),&target->GathererTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.ExpanderTemplate),null),&target->ExpanderTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.EnergyProducer),null),&target->EnergyProducer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.PrimaryResourceNodeCollector),null),&target->PrimaryResourceNodeCollector,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.PrimaryResourceThingToBuild),null),&target->PrimaryResourceThingToBuild,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.SecondaryResourceNodeCollector),null),&target->SecondaryResourceNodeCollector,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.VehicleRepairFacility),null),&target->VehicleRepairFacility,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.NavalRepairFacility),null),&target->NavalRepairFacility,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.AircraftRepairFacility),null),&target->AircraftRepairFacility,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockArmyDefinition.WallPiece),null),&target->WallPiece,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockArmyDefinition.TechStructure)),&target->TechStructure,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DebugHotKeys with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDebugHotKeys* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockDebugHotKeys.Map),null),&target->Map,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DrawGroupInfo with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDrawGroupInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.FontName),"Arial"),&target->FontName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.FontSize),"12"),&target->FontSize,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.DropShadowOffsetX),"-1"),&target->DropShadowOffsetX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.DropShadowOffsetY),"-1"),&target->DropShadowOffsetY,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.PixelOffetXInteger),null),&target->PixelOffetXInteger,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.PixelOffetYInteger),null),&target->PixelOffetYInteger,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.PixelOffetXReal),null),&target->PixelOffetXReal,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.PixelOffetYReal),null),&target->PixelOffetYReal,state);
        Marshal(node.GetChildNode(nameof(Ep1StockDrawGroupInfo.TextColor),null),&target->TextColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockDrawGroupInfo.TextDropShadowColor),null),&target->TextDropShadowColor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.IsBold),null),&target->IsBold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.UsePlayerColor),"true"),&target->UsePlayerColor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDrawGroupInfo.PixelOffsetAsPercentage),"false"),&target->PixelOffsetAsPercentage,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal Language with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockLanguage* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockLanguage.Separators),null),&target->Separators,state);
        Marshal(node.GetChildNode(nameof(Ep1StockLanguage.UnicodeFontName),null),&target->UnicodeFontName,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockLanguage.FontInfo)),&target->FontInfo,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal SeparatorList with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockSeparatorList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockSeparatorList.Thousand),","),&target->Thousand,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockSeparatorList.Decimal),"."),&target->Decimal,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockSeparatorList.TimeMinuteToSecond),":"),&target->TimeMinuteToSecond,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal FontDefine with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockFontDefine* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockFontDefine.FontTitle),null),&target->FontTitle,state);
        Marshal(node.GetChildNode(nameof(Ep1StockFontDefine.Font),null),&target->Font,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BounceKickTerrainMapping with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBounceKickTerrainMapping* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockBounceKickTerrainMapping.Mapping)),&target->Mapping,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BounceKickTerrainData with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBounceKickTerrainData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockBounceKickTerrainData.TerrainClass),null),&target->TerrainClass,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBounceKickTerrainData.BounceMultiplier),null),&target->BounceMultiplier,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal MeshInstanceTemplate with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockMeshInstanceTemplate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockMeshInstanceTemplate.Rule)),&target->Rule,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal MatchRule with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockMatchRule* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockMatchRule.MeshID),null),&target->MeshID,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMatchRule.Instances),null),&target->Instances,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UnitOverlayIconSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUnitOverlayIconSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.AmmoPipImage),null),&target->AmmoPipImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.AmmoPipFrameImage),null),&target->AmmoPipFrameImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.ContainPipImage),null),&target->ContainPipImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.ContainPipFrameImage),null),&target->ContainPipFrameImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.GarrisonPipImage),null),&target->GarrisonPipImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.UnitTypeIconBackgroundImage),null),&target->UnitTypeIconBackgroundImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.TargetIndicatorImage),null),&target->TargetIndicatorImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.TargetIndicatorGlowImage),null),&target->TargetIndicatorGlowImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.PowerIconImageSequence),null),&target->PowerIconImageSequence,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.RepairIconImageSequence),null),&target->RepairIconImageSequence,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.InfiltratedIconImageSequence),null),&target->InfiltratedIconImageSequence,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.HealUnitIconImageSequence),null),&target->HealUnitIconImageSequence,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.RepairVehicleIconImageSequence),null),&target->RepairVehicleIconImageSequence,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.AmmoPipScale),"100%"),&target->AmmoPipScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.AmmoPipRange),null),&target->AmmoPipRange,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.ContainPipScale),"100%"),&target->ContainPipScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.GarrisonPipScale),"100%"),&target->GarrisonPipScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.UnitTypeIconScale),"100%"),&target->UnitTypeIconScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.StatusIconScale),"100%"),&target->StatusIconScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.VeterancyIconScale),"100%"),&target->VeterancyIconScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.TargetIndicatorScale),"100%"),&target->TargetIndicatorScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.HealingIconScale),"100%"),&target->HealingIconScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUnitOverlayIconSettings.HealingIconDisplayTime),"3s"),&target->HealingIconDisplayTime,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUnitOverlayIconSettings.FactionVeterancy)),&target->FactionVeterancy,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal WaterTransparency with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockWaterTransparency* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockWaterTransparency.TransparentWaterDepth),"3.0"),&target->TransparentWaterDepth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockWaterTransparency.TransparentWaterMinOpacity),"1.0"),&target->TransparentWaterMinOpacity,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockWaterTransparency.StandingWaterTexture),null),&target->StandingWaterTexture,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal NetworkSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockNetworkSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockNetworkSettings.RunAheadSlack),"30"),&target->RunAheadSlack,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BaseSingletonSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBaseSingletonSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal MiscEvaEvents with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockMiscEvaEvents* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.LowPower),null),&target->LowPower,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.SilosNeeded),null),&target->SilosNeeded,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.BuildingStolen),null),&target->BuildingStolen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.IncomingRadarTransmission),null),&target->IncomingRadarTransmission,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.AllyDefeated),null),&target->AllyDefeated,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.EnemyDefeated),null),&target->EnemyDefeated,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.LocalPlayerDefeated),null),&target->LocalPlayerDefeated,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.LocalPlayerVictorious),null),&target->LocalPlayerVictorious,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CannotBuildDueToCPLimit),null),&target->CannotBuildDueToCPLimit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.BuildQueuePausedDueToCPLimit),null),&target->BuildQueuePausedDueToCPLimit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.BuildQueuePausedDueToFunds),null),&target->BuildQueuePausedDueToFunds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CannotBuildDueToFunds),null),&target->CannotBuildDueToFunds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CannotRepairDueToFunds),null),&target->CannotRepairDueToFunds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CannotUsePowerDueToFunds),null),&target->CannotUsePowerDueToFunds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.TiberiumFieldDepleted),null),&target->TiberiumFieldDepleted,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.NewMissionObjective),null),&target->NewMissionObjective,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CompletedMissionObjective),null),&target->CompletedMissionObjective,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.NewBonusObjective),null),&target->NewBonusObjective,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CompletedBonusMissionObjective),null),&target->CompletedBonusMissionObjective,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.IntelDatabaseEntryReceived),null),&target->IntelDatabaseEntryReceived,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.UpgradeComplete),null),&target->UpgradeComplete,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.BuildingBeingStolen),null),&target->BuildingBeingStolen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.BeaconPlacedPlayer),null),&target->BeaconPlacedPlayer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.BeaconPlacedAlly),null),&target->BeaconPlacedAlly,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.ObservedPlayerDefeated),null),&target->ObservedPlayerDefeated,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.WallsBeingClimbed),null),&target->WallsBeingClimbed,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.StructureInfiltrated),null),&target->StructureInfiltrated,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.ControlPointCaptured),null),&target->ControlPointCaptured,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.ControlPointLost),null),&target->ControlPointLost,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.ControlPointHasBeenCaptured),null),&target->ControlPointHasBeenCaptured,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CTFFlagCaptured),null),&target->CTFFlagCaptured,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CTFEnemyCapturedFlag),null),&target->CTFEnemyCapturedFlag,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CTFFlagDropped),null),&target->CTFFlagDropped,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CTFEnemyDroppedFlag),null),&target->CTFEnemyDroppedFlag,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CTFYouHaveFlag),null),&target->CTFYouHaveFlag,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.CTFEnemyHasFlag),null),&target->CTFEnemyHasFlag,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.SiegeBarrierDisengaged),null),&target->SiegeBarrierDisengaged,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEvents.SiegeBarrierEngaged),null),&target->SiegeBarrierEngaged,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockMiscEvaEvents.AIBeaconTypeSettings)),&target->AIBeaconTypeSettings,state);
        Marshal(node,(BaseInheritableAsset*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal MiscEvaEventsAIBeaconTypeSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockMiscEvaEventsAIBeaconTypeSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEventsAIBeaconTypeSettings.BeaconType),null),&target->BeaconType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockMiscEvaEventsAIBeaconTypeSettings.PlacedEvent),null),&target->PlacedEvent,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal AutoParticleSystemSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockAutoParticleSystemSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockAutoParticleSystemSettings.ParticleScale),null),&target->ParticleScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockAutoParticleSystemSettings.MaxParticleCount),"2500"),&target->MaxParticleCount,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.FireSmall),null),&target->FireSmall,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.FireMedium),null),&target->FireMedium,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.FireLarge),null),&target->FireLarge,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.SmokeSmall),null),&target->SmokeSmall,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.SmokeMedium),null),&target->SmokeMedium,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.SmokeLarge),null),&target->SmokeLarge,state);
        Marshal(node.GetChildNode(nameof(Ep1StockAutoParticleSystemSettings.Aflame),null),&target->Aflame,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal AutoParticleSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockAutoParticleSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockAutoParticleSettings.ParticlePrefix),null),&target->ParticlePrefix,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockAutoParticleSettings.ParticleSystemId),null),&target->ParticleSystemId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockAutoParticleSettings.ParticleMax),null),&target->ParticleMax,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BreezeInfo with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBreezeInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockBreezeInfo.Direction),"1.0472"),&target->Direction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBreezeInfo.Intensity),"0.027488125"),&target->Intensity,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBreezeInfo.Lean),"0.0157075"),&target->Lean,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBreezeInfo.Randomness),"0.2"),&target->Randomness,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBreezeInfo.BreezePeriod),"10s"),&target->BreezePeriod,state);
        Marshal(node,(Ep1StockBaseSubsystemSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BaseSubsystemSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBaseSubsystemSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockBaseSubsystemSettings.LoadAfter),null),&target->LoadAfter,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BuildSystemSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBuildSystemSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.AllowedHeightVariationForBuildingPlacement),"3.0"),&target->AllowedHeightVariationForBuildingPlacement,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.RotationalSnapIterations),"4.0"),&target->RotationalSnapIterations,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.BuildabilityDisplayRadius),"90.0"),&target->BuildabilityDisplayRadius,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.BuildabilityDisplayShape),"CYLINDER"),&target->BuildabilityDisplayShape,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.BuildabilityDecalMargin),"0.142857"),&target->BuildabilityDecalMargin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.BuildabilityDecalSize),"30.0"),&target->BuildabilityDecalSize,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.BuildabilityDecalRadiusPercent),"18.5185"),&target->BuildabilityDecalRadiusPercent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSystemSettings.BuildabilityUsesStaticDecalSize),"true"),&target->BuildabilityUsesStaticDecalSize,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DamageFXSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDamageFXSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockDamageFXSettings.GroundDamageFX),null),&target->GroundDamageFX,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DynamicsContactTagMapping with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDynamicsContactTagMapping* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockDynamicsContactTagMapping.Mapping)),&target->Mapping,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ContactTagMapping with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockContactTagMapping* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockContactTagMapping.ContactA),null),&target->ContactA,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockContactTagMapping.ContactB),null),&target->ContactB,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockContactTagMapping.MinForce),"1.2"),&target->MinForce,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockContactTagMapping.FX),null),&target->FX,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal GraphicsHardware with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockGraphicsHardware* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockGraphicsHardware.Device)),&target->Device,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal GraphicsDevice with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockGraphicsDevice* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockGraphicsDevice.VendorId),null),&target->VendorId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGraphicsDevice.DeviceId),null),&target->DeviceId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGraphicsDevice.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGraphicsDevice.Level),null),&target->Level,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockGraphicsDevice.MacLevel),null),&target->MacLevel,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal InvisibilityManagerSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockInvisibilityManagerSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockInvisibilityManagerSettings.CamouflageDetectorObjectFilter),null),&target->CamouflageDetectorObjectFilter,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal LightingSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockLightingSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockLightingSettings.TimeOfDay),null),&target->TimeOfDay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockLightingSettings.OccludedLuminanceScale),"0.5"),&target->OccludedLuminanceScale,state);
        Marshal(node.GetChildNode(nameof(Ep1StockLightingSettings.LightingMultiplierIfNoClouds),null),&target->LightingMultiplierIfNoClouds,state);
        Marshal(node.GetChildNode(nameof(Ep1StockLightingSettings.CurrentLightingSettings),null),&target->CurrentLightingSettings,state);
        Marshal(node.GetChildNode(nameof(Ep1StockLightingSettings.TerrainLighting),null),&target->TerrainLighting,state);
        Marshal(node,(Ep1StockBaseSubsystemSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal LightingTime with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockLightingTime* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockLightingTime.TimeOfDay),null),&target->TimeOfDay,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockLightingTime.Lighting)),&target->Lighting,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal LightingInfo with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockLightingInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockLightingInfo.Ambient),null),&target->Ambient,state);
        Marshal(node.GetChildNode(nameof(Ep1StockLightingInfo.Diffuse),null),&target->Diffuse,state);
        Marshal(node.GetChildNode(nameof(Ep1StockLightingInfo.LightPos),null),&target->LightPos,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal LightingGroup with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockLightingGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockLightingGroup.LightingTimeGroup)),&target->LightingTimeGroup,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal MissionHotSpotSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockMissionHotSpotSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockMissionHotSpotSettings.WaypointMarker),null),&target->WaypointMarker,state);
        Marshal(node.GetChildNode(nameof(Ep1StockMissionHotSpotSettings.DecalTemplate),null),&target->DecalTemplate,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ModelSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockModelSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockModelSettings.AnimationSharingCap),null),&target->AnimationSharingCap,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockModelSettings.AnimationSharingFrameTolerance),null),&target->AnimationSharingFrameTolerance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockModelSettings.AnimationSharingDrasticThreshold),null),&target->AnimationSharingDrasticThreshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockModelSettings.ForceModelsToFollowTimeOfDay),null),&target->ForceModelsToFollowTimeOfDay,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockModelSettings.ForceModelsToFollowWeather),null),&target->ForceModelsToFollowWeather,state);
        Marshal(node,(Ep1StockBaseSubsystemSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal OrdersManagerSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockOrdersManagerSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockOrdersManagerSettings.LineFadeTime),"0.25s"),&target->LineFadeTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockOrdersManagerSettings.FormationPreviewShader),null),&target->FormationPreviewShader,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockOrdersManagerSettings.BuildPlacementShader),null),&target->BuildPlacementShader,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.ExecutingLine),null),&target->ExecutingLine,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PendingLine),null),&target->PendingLine,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PatrolLine),null),&target->PatrolLine,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PatrolPendingLine),null),&target->PatrolPendingLine,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.TargetMoveLine),null),&target->TargetMoveLine,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.TargetAttackLine),null),&target->TargetAttackLine,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PatrolNode),null),&target->PatrolNode,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PatrolStart),null),&target->PatrolStart,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PendingNode),null),&target->PendingNode,state);
        Marshal(node.GetChildNode(nameof(Ep1StockOrdersManagerSettings.PendingStart),null),&target->PendingStart,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal LineSetting with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockLineSetting* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockLineSetting.Width),"1.5"),&target->Width,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockLineSetting.VOffsetRate),"0"),&target->VOffsetRate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockLineSetting.Texture),null),&target->Texture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockLineSetting.TextureTileSize),"30.0"),&target->TextureTileSize,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal PathfindSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockPathfindSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxPathfindCellsPerFrame),null),&target->MaxPathfindCellsPerFrame,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxPathfindCellsPerPhase),null),&target->MaxPathfindCellsPerPhase,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsFindMeleeEngagementLocation),null),&target->MaxCellsFindMeleeEngagementLocation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustDestination),null),&target->MaxCellsAdjustDestination,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustHordeMeleeDestination),null),&target->MaxCellsAdjustHordeMeleeDestination,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustTargetDestination),null),&target->MaxCellsAdjustTargetDestination,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustToPossibleDestination),null),&target->MaxCellsAdjustToPossibleDestination,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustToMeleeDestination),null),&target->MaxCellsAdjustToMeleeDestination,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustToNearestGroundCell),null),&target->MaxCellsAdjustToNearestGroundCell,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsAdjustToNearestValidCell),null),&target->MaxCellsAdjustToNearestValidCell,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsPatchPath),null),&target->MaxCellsPatchPath,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxCellsToExamineTowardsGoal),null),&target->MaxCellsToExamineTowardsGoal,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxExtraCollisionResponsePathsPerFrame),null),&target->MaxExtraCollisionResponsePathsPerFrame,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.MaxExtraMoveAwayFromPathPathsPerFrame),null),&target->MaxExtraMoveAwayFromPathPathsPerFrame,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockPathfindSettings.LayerZCloseEnough),null),&target->LayerZCloseEnough,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ScorchMarkSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockScorchMarkSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockScorchMarkSettings.ScorchMarkSetting)),&target->ScorchMarkSetting,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal ScorchMarkSetting with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockScorchMarkSetting* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockScorchMarkSetting.TerrainClass),null),&target->TerrainClass,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockScorchMarkSetting.Scorch)),&target->Scorch,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal SkirmishSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockSkirmishSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockSkirmishSettings.RandomCrateSpawner),null),&target->RandomCrateSpawner,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockSkirmishSettings.HandicapBuildSpeed)),&target->HandicapBuildSpeed,state);
        Marshal(node,(Ep1StockBaseSubsystemSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal BuildSpeed with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockBuildSpeed* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockBuildSpeed.Speed),null),&target->Speed,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal TerrainVisualSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockTerrainVisualSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockTerrainVisualSettings.WaterPosition),null),&target->WaterPosition,state);
        Marshal(node.GetChildNode(nameof(Ep1StockTerrainVisualSettings.WaterExtents),null),&target->WaterExtents,state);
        Marshal(node,(Ep1StockBaseSubsystemSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal W3DDeviceSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockW3DDeviceSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.MaxTerrainTracks),"100"),&target->MaxTerrainTracks,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.OpacityOfSimpleMergeDecals),"35%"),&target->OpacityOfSimpleMergeDecals,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.OccludedColorLuminanceScale),"0.5"),&target->OccludedColorLuminanceScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.MaxRoadSegments),"4000"),&target->MaxRoadSegments,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.MaxRoadVertex),"5000"),&target->MaxRoadVertex,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.MaxRoadIndex),"10000"),&target->MaxRoadIndex,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.FogAlpha),"63"),&target->FogAlpha,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.ShroudAlpha),"0"),&target->ShroudAlpha,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.ClearAlpha),"255"),&target->ClearAlpha,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.DefaultShroudStatus),"FOGGED"),&target->DefaultShroudStatus,state);
        Marshal(node.GetChildNode(nameof(Ep1StockW3DDeviceSettings.ShroudColor),null),&target->ShroudColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockW3DDeviceSettings.TreeFadeObjectFilter),null),&target->TreeFadeObjectFilter,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockW3DDeviceSettings.StandardPublicBone)),&target->StandardPublicBone,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockW3DDeviceSettings.UseSimpleHordeDecals),"false"),&target->UseSimpleHordeDecals,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal TerrainAssetCollection with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockTerrainAssetCollection* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockTerrainAssetCollection.TerrainAsset)),&target->TerrainAsset,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal TerrainAsset with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockTerrainAsset* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainAsset.id),null),&target->id,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainAsset.Texture),null),&target->Texture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainAsset.BumpTexture),null),&target->BumpTexture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainAsset.Class),null),&target->Class,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockTerrainAsset.WBFolder),null),&target->WBFolder,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarButtonBuild with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarButtonBuild* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIJoypadCommandBarButtonBuild.FactionSettings)),&target->FactionSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarButtonBuildFactionSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarButtonBuildFactionSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarButtonBuildFactionSettings.Faction),null),&target->Faction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarButtonBuildFactionSettings.LockedButtonStateData),null),&target->LockedButtonStateData,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarControlGroups with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarControlGroups* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarControlGroups.Template),null),&target->Template,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarHomogenousGroup with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarHomogenousGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarHomogenousGroup.Template),null),&target->Template,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarMixedGroup with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarMixedGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarMixedGroup.Template),null),&target->Template,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarSingleUnit with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarSingleUnit* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarSingleUnit.UnitTemplate),null),&target->UnitTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarSingleUnit.ProductionStructureTemplate),null),&target->ProductionStructureTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarSingleUnit.NonProductionStructureTemplate),null),&target->NonProductionStructureTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarSingleUnit.HotSpotTemplate),null),&target->HotSpotTemplate,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarStances with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarStances* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarStances.Template),null),&target->Template,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarCoopCommunicator with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarCoopCommunicator* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarCoopCommunicator.AICoopTemplate),null),&target->AICoopTemplate,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarPlayerPowers with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarPlayerPowers* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarPlayerPowers.Template),null),&target->Template,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIJoypadCommandBarTopMenu with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIJoypadCommandBarTopMenu* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIJoypadCommandBarTopMenu.Template),null),&target->Template,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalQuickChatDisplay with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalQuickChatDisplay* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplay.CloseKey),null),&target->CloseKey,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplay.DelayDuration),"0s"),&target->DelayDuration,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalQuickChatDisplay.MenuItem)),&target->MenuItem,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalQuickChatDisplay.CloseMenuItem),null),&target->CloseMenuItem,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalQuickChatDisplayMenuItem with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalQuickChatDisplayMenuItem* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItem.Title),null),&target->Title,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItem.ChatDestination),null),&target->ChatDestination,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItem.ChatMessageGroup)),&target->ChatMessageGroup,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItem.MenuItem)),&target->MenuItem,state);
        // Reborn: explicitly authored optional values own tracked relocated storage, including scalar zero.
        { var value=node.GetChildNode(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItem.CloseMenuItem),null); if(value is not null){using var cell=state.Push((void**)&target->CloseMenuItem,(uint)sizeof(Ep1StockUIMouseTacticalQuickChatDisplayCloseItem),1);Marshal(value,target->CloseMenuItem,state);} }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup.Faction),"Neutral"),&target->Faction,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessageGroup.ChatMessage)),&target->ChatMessage,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalQuickChatDisplayMenuItemChatMessage with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessage* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplayMenuItemChatMessage.MessageRef),null),&target->MessageRef,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalQuickChatDisplayCloseItem with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalQuickChatDisplayCloseItem* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalQuickChatDisplayCloseItem.Title),null),&target->Title,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseDragSelectionFeedback with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseDragSelectionFeedback* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseDragSelectionFeedback.BoxEdgeWidth),"2"),&target->BoxEdgeWidth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseDragSelectionFeedback.LassoWidth),"2"),&target->LassoWidth,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseDragSelectionFeedback.BoxColor),null),&target->BoxColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseDragSelectionFeedback.LassoColor),null),&target->LassoColor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseHUDElementHotKeyGate with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseHUDElementHotKeyGate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseHUDElementHotKeyGate.ElementHotKeyMapping)),&target->ElementHotKeyMapping,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseHUDElementHotKeyMapping with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseHUDElementHotKeyMapping* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseHUDElementHotKeyMapping.ElementId),null),&target->ElementId,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseHUDElementHotKeyMapping.HotKeySlot),null),&target->HotKeySlot,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalAttackMoveButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalAttackMoveButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalBuildQueueTab with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalBuildQueueTab* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalBuildQueueTab.QueueTypeSettings)),&target->QueueTypeSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalBuildQueueTabQueueTypeSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalBuildQueueTabQueueTypeSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalBuildQueueTabQueueTypeSettings.QueueType),null),&target->QueueType,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalBuildQueueTabQueueTypeSettings.MouseOverHelp),null),&target->MouseOverHelp,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalBuildQueueTypeTab with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalBuildQueueTypeTab* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalBuildQueueTypeTab.QueueTypeSettings)),&target->QueueTypeSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalBuildQueueTypeTabQueueTypeSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalBuildQueueTypeTabQueueTypeSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalBuildQueueTypeTabQueueTypeSettings.QueueType),null),&target->QueueType,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalBuildQueueTypeTabQueueTypeSettings.MouseOverHelp),null),&target->MouseOverHelp,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandAttack with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandAttack* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.AttackObjectCursor),null),&target->AttackObjectCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.ForceAttackObjectCursor),null),&target->ForceAttackObjectCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.ForceAttackGroundCursor),null),&target->ForceAttackGroundCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.ExplosiveChargeCursor),null),&target->ExplosiveChargeCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.ClearGarrisonCursor),null),&target->ClearGarrisonCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.DisguiseCursor),null),&target->DisguiseCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttack.CallAirstrikeCursor),null),&target->CallAirstrikeCursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandAttackMove with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandAttackMove* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttackMove.NormalCursor),null),&target->NormalCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandAttackMove.ForcedCursor),null),&target->ForcedCursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandCommunicateWithAllies with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandCommunicateWithAllies* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandCommunicateWithAllies.PlaceUnattachedBeaconCursor),null),&target->PlaceUnattachedBeaconCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandCommunicateWithAllies.PlaceAttachedBeaconCursor),null),&target->PlaceAttachedBeaconCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandCommunicateWithAllies.PingRadarCursor),null),&target->PingRadarCursor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalCommandCommunicateWithAllies.BeaconAttachableObjectFilter),null),&target->BeaconAttachableObjectFilter,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandDock with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandDock* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandDock.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandEnter with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandEnter* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandEnter.DefaultCursor),null),&target->DefaultCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandEnter.ContestCursor),null),&target->ContestCursor,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalCommandEnter.InfiltratorEffectTypeSettings)),&target->InfiltratorEffectTypeSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandEnterInfiltratorEffectTypeSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandEnterInfiltratorEffectTypeSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandEnterInfiltratorEffectTypeSettings.Effect),null),&target->Effect,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandEnterInfiltratorEffectTypeSettings.Cursor),null),&target->Cursor,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandForceAttack with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandForceAttack* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandForceAttack.ObjectCursor),null),&target->ObjectCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandForceAttack.GroundCursor),null),&target->GroundCursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandForceMove with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandForceMove* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandForceMove.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandGetCrate with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandGetCrate* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandGetCrate.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandMineOre with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandMineOre* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandMineOre.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandMove with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandMove* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandMove.ForwardCursor),null),&target->ForwardCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandMove.ReverseCursor),null),&target->ReverseCursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandPlaceAIBeacon with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandPlaceAIBeacon* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeacon.BeaconTypeSettings)),&target->BeaconTypeSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandPlaceAIBeaconTypeSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings.Type),null),&target->Type,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings.PlaceUnattachedCursor),null),&target->PlaceUnattachedCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings.PlaceAttachedCursor),null),&target->PlaceAttachedCursor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings.AttachableObjectFilter),null),&target->AttachableObjectFilter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings.CanTargetObjects),"true"),&target->CanTargetObjects,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandPlaceAIBeaconTypeSettings.CanTargetPosition),"true"),&target->CanTargetPosition,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandPlaceRallyPoint with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandPlaceRallyPoint* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandPlaceRallyPoint.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandReverseMove with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandReverseMove* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandReverseMove.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandSelect with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandSelect* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSelect.Cursor),null),&target->Cursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSelect.TakeUnitsFromAllyHotKeySlot),null),&target->TakeUnitsFromAllyHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSelect.GiveUnitsToAllyHotKeySlot),null),&target->GiveUnitsToAllyHotKeySlot,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandSell with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandSell* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalCommandSell.DefaultCursors),null),&target->DefaultCursors,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalCommandSell.FactionCursors)),&target->FactionCursors,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandSellCursorSet with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandSellCursorSet* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSellCursorSet.ValidTarget),null),&target->ValidTarget,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSellCursorSet.InvalidTarget),null),&target->InvalidTarget,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandSellFactionCursorSet with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandSellFactionCursorSet* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSellFactionCursorSet.Faction),null),&target->Faction,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalCommandSellFactionCursorSet.Cursors),null),&target->Cursors,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandSetRallyPoint with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandSetRallyPoint* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandSetRallyPoint.Cursor),null),&target->Cursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCommandToggleSelfRepair with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCommandToggleSelfRepair* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandToggleSelfRepair.ValidTargetCursor),null),&target->ValidTargetCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalCommandToggleSelfRepair.InvalidTargetCursor),null),&target->InvalidTargetCursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCoOpAIButtonsAssaultButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCoOpAIButtonsAssaultButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCoOpAIButtonsHelpButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCoOpAIButtonsHelpButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCoOpAIButtonsHoldObjectButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCoOpAIButtonsHoldObjectButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCoOpAIButtonsHoldPositionButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCoOpAIButtonsHoldPositionButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalCoOpAIButtonsNormalButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalCoOpAIButtonsNormalButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalDeactivateMissionHotSpotsButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalDeactivateMissionHotSpotsButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalExitContainerButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalExitContainerButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalExitContainerButton.HelpTitleFormat),null),&target->HelpTitleFormat,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalExitContainerButton.HelpDescription),null),&target->HelpDescription,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalFastForwardButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalFastForwardButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalForceAttackButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalForceAttackButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalForceAttackMoveButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalForceAttackMoveButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalForceMoveButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalForceMoveButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalGroupSelection with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalGroupSelection* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalGroupSelection.Group)),&target->Group,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalGroupSelectionGroup with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalGroupSelectionGroup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalGroupSelectionGroup.CreateHotKeySlot),null),&target->CreateHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalGroupSelectionGroup.AddToHotKeySlot),null),&target->AddToHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalGroupSelectionGroup.SelectHotKeySlot),null),&target->SelectHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalGroupSelectionGroup.ViewHotKeySlot),null),&target->ViewHotKeySlot,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalLookAt with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalLookAt* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ScrollCursor),null),&target->ScrollCursor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ScrollLeftHotKeySlot),null),&target->ScrollLeftHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ScrollRightHotKeySlot),null),&target->ScrollRightHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ScrollUpHotKeySlot),null),&target->ScrollUpHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ScrollDownHotKeySlot),null),&target->ScrollDownHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.RotateLeftHotKeySlot),null),&target->RotateLeftHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.RotateRightHotKeySlot),null),&target->RotateRightHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ZoomInHotKeySlot),null),&target->ZoomInHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.ZoomOutHotKeySlot),null),&target->ZoomOutHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.KeyScrollFactor),"100%"),&target->KeyScrollFactor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.MouseDragScrollFactor),"100%"),&target->MouseDragScrollFactor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAt.MouseEdgeScrollFactor),"100%"),&target->MouseEdgeScrollFactor,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalLookAt.ViewBookmark)),&target->ViewBookmark,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalLookAtViewBookmark with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalLookAtViewBookmark* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAtViewBookmark.GoToHotKeySlot),null),&target->GoToHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalLookAtViewBookmark.SaveHotKeySlot),null),&target->SaveHotKeySlot,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalMainCommandBarHotKeys with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalMainCommandBarHotKeys* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMainCommandBarHotKeys.SelectionRefinementPageSlot),null),&target->SelectionRefinementPageSlot,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalMainCommandBarHotKeys.BuildQueuePage)),&target->BuildQueuePage,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalMainCommandBarHotKeys.ButtonSlot)),&target->ButtonSlot,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalMainCommandBarHotKeysBuildQueue with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalMainCommandBarHotKeysBuildQueue* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMainCommandBarHotKeysBuildQueue.QueueType),null),&target->QueueType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMainCommandBarHotKeysBuildQueue.Slot),null),&target->Slot,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalMiscHotKeys with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalMiscHotKeys* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.AllCheer),null),&target->AllCheer,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.AttackMove),null),&target->AttackMove,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.CameraReset),null),&target->CameraReset,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ChatWithAllies),null),&target->ChatWithAllies,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ChatWithBuddies),null),&target->ChatWithBuddies,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ChatWithEveryone),null),&target->ChatWithEveryone,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.CommunicateWithAlliesMode),null),&target->CommunicateWithAlliesMode,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.CreateFormation),null),&target->CreateFormation,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.CycleHarvesters),null),&target->CycleHarvesters,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.DeleteBeacon),null),&target->DeleteBeacon,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ForceAttack),null),&target->ForceAttack,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ForceAttackMove),null),&target->ForceAttackMove,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ForceMove),null),&target->ForceMove,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.GoToNextBuildQueue),null),&target->GoToNextBuildQueue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.GoToPriorBuildQueue),null),&target->GoToPriorBuildQueue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.GoToNextSubGroup),null),&target->GoToNextSubGroup,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.GoToPriorSubGroup),null),&target->GoToPriorSubGroup,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenAdvancedCommands),null),&target->OpenAdvancedCommands,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenLoadGameScreen),null),&target->OpenLoadGameScreen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenMessenger),null),&target->OpenMessenger,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenPauseScreen),null),&target->OpenPauseScreen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenPlayerTechStore),null),&target->OpenPlayerTechStore,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenSaveGameScreen),null),&target->OpenSaveGameScreen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.OpenStatusScreen),null),&target->OpenStatusScreen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.PlaceRallyPoint),null),&target->PlaceRallyPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.PlanningMode),null),&target->PlanningMode,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.PreferSelection),null),&target->PreferSelection,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.QuickChatMenu),null),&target->QuickChatMenu,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ReverseMove),null),&target->ReverseMove,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.Scatter),null),&target->Scatter,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.SelectAll),null),&target->SelectAll,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.SelectMatchingUnits),null),&target->SelectMatchingUnits,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.SellMode),null),&target->SellMode,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ShowAllHealthBars),null),&target->ShowAllHealthBars,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.Stop),null),&target->Stop,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.TelestratorErase),null),&target->TelestratorErase,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.TelestratorNextColor),null),&target->TelestratorNextColor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.TelestratorNextLineWidth),null),&target->TelestratorNextLineWidth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.TelestratorPriorColor),null),&target->TelestratorPriorColor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.TelestratorToggle),null),&target->TelestratorToggle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ToggleFastForwardMode),null),&target->ToggleFastForwardMode,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ToggleHUD),null),&target->ToggleHUD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.TogglePowerMode),null),&target->TogglePowerMode,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ToggleRepairMode),null),&target->ToggleRepairMode,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ViewHomeBase),null),&target->ViewHomeBase,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.ViewLastEvaEvent),null),&target->ViewLastEvaEvent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeys.WaypointMode),null),&target->WaypointMode,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalMiscHotKeys.StanceSettings)),&target->StanceSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalMiscHotKeysStanceSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalMiscHotKeysStanceSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeysStanceSettings.Stance),null),&target->Stance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalMiscHotKeysStanceSettings.Slot),null),&target->Slot,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalOpenAdvancedCommandsButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalOpenAdvancedCommandsButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalPauseButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalPauseButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalPlanningModeButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalPlanningModeButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalPlayerPowerHotKeys with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalPlayerPowerHotKeys* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalPlayerPowerHotKeys.Slot)),&target->Slot,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalPlayerTechStoreButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalPlayerTechStoreButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalPlayerTechStoreButton.TechPointAwardedSound),null),&target->TechPointAwardedSound,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalPlayerTechStoreButton.YurikoMouseOverHelp),null),&target->YurikoMouseOverHelp,state);
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalRadarTelestrator with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalRadarTelestrator* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalRadarTelestrator.StrokeSustainDuration),"10s"),&target->StrokeSustainDuration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalRadarTelestrator.StrokeFadeDuration),"2s"),&target->StrokeFadeDuration,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalReverseMoveButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalReverseMoveButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalSelectionDetailsButtonHotKeys with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalSelectionDetailsButtonHotKeys* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalSelectionDetailsButtonHotKeys.UnitAbilitySlot)),&target->UnitAbilitySlot,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalSelectionRefinementTab with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalSelectionRefinementTab* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalSellModeButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalSellModeButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalSetStanceButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalSetStanceButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIMouseTacticalSetStanceButton.StanceSettings)),&target->StanceSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalSetStanceButtonStanceSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalSetStanceButtonStanceSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalSetStanceButtonStanceSettings.Stance),null),&target->Stance,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseTacticalSetStanceButtonStanceSettings.MouseOverHelp),null),&target->MouseOverHelp,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalStopButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalStopButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalTelestrator with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalTelestrator* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseTacticalTelestrator.DrawCursor),null),&target->DrawCursor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalThreatMeter with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalThreatMeter* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalToggleRepairModeButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalToggleRepairModeButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalUnitIntroButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalUnitIntroButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseTacticalWaypointModeButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseTacticalWaypointModeButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(UIMouseSimpleFixedButton*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMouseVoiceChat with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMouseVoiceChat* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseVoiceChat.ToggleModeHotKeySlot),null),&target->ToggleModeHotKeySlot,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMouseVoiceChat.TalkHotKeySlot),null),&target->TalkHotKeySlot,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseVoiceChat.ModeButtonHelp),null),&target->ModeButtonHelp,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMouseVoiceChat.TalkButtonHelp),null),&target->TalkButtonHelp,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UITimerDisplay with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUITimerDisplay* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUITimerDisplay.FlashDuration),"1s"),&target->FlashDuration,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUITimerDisplay.Position),null),&target->Position,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUITimerDisplay.FlashColor),null),&target->FlashColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUITimerDisplay.NormalFont),null),&target->NormalFont,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUITimerDisplay.NormalColor),null),&target->NormalColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUITimerDisplay.ReadyFont),null),&target->ReadyFont,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUITimerDisplay.ReadyColor),null),&target->ReadyColor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUITimerDisplay.Centered),"false"),&target->Centered,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIWorldBuilderDecalGrid with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIWorldBuilderDecalGrid* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIWorldBuilderDecalGrid.Texture),null),&target->Texture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIWorldBuilderDecalGrid.Opacity),"1.0"),&target->Opacity,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIWorldBuilderDecalGrid.Color),null),&target->Color,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UICommandButton with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUICommandButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUICommandButton.FactionSettings)),&target->FactionSettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UICommandButtonFactionSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUICommandButtonFactionSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICommandButtonFactionSettings.Faction),null),&target->Faction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICommandButtonFactionSettings.LockedImage),null),&target->LockedImage,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUICommandButtonFactionSettings.ImageTintColor),null),&target->ImageTintColor,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UICoopTinting with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUICoopTinting* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICoopTinting.FadeInFrames),"3"),&target->FadeInFrames,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICoopTinting.FadeOutFrames),"3"),&target->FadeOutFrames,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICoopTinting.SustainFrames),"10"),&target->SustainFrames,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICoopTinting.TintFrequency),"1.0"),&target->TintFrequency,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUICoopTinting.TintAmplitude),"1.0"),&target->TintAmplitude,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUICoopTinting.TintColor),null),&target->TintColor,state);
        Marshal(node,(Ep1StockBaseSingletonSettings*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIDecalCloud with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIDecalCloud* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIDecalCloud.BuildTexture),null),&target->BuildTexture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIDecalCloud.DefenseTexture),null),&target->DefenseTexture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIDecalCloud.SpecialPowerRestrictionTexture),null),&target->SpecialPowerRestrictionTexture,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIDecalGrid with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIDecalGrid* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIDecalGrid.WallTexture),null),&target->WallTexture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIDecalGrid.WallOpacity),"1.0"),&target->WallOpacity,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIDecalGrid.WallDecalColor),null),&target->WallDecalColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIDecalGrid.PrimarySettings),null),&target->PrimarySettings,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIDecalGrid.SecondarySettings),null),&target->SecondarySettings,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal DecalSettings with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockDecalSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockDecalSettings.ValidTexture),null),&target->ValidTexture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDecalSettings.InvalidTexture),null),&target->InvalidTexture,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockDecalSettings.Opacity),"1.0"),&target->Opacity,state);
        Marshal(node.GetChildNode(nameof(Ep1StockDecalSettings.LegalColor),null),&target->LegalColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockDecalSettings.InvalidColor),null),&target->InvalidColor,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIFloatingTextDisplay with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIFloatingTextDisplay* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIFloatingTextDisplay.TimeOut),"333ms"),&target->TimeOut,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIFloatingTextDisplay.MoveUpSpeed),"1.0"),&target->MoveUpSpeed,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIFloatingTextDisplay.MoveVanishRate),"0.1"),&target->MoveVanishRate,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIFloatingTextDisplay.Font),null),&target->Font,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIMessageDisplay with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIMessageDisplay* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIMessageDisplay.Delay),"5s"),&target->Delay,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMessageDisplay.Color1),null),&target->Color1,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMessageDisplay.Color2),null),&target->Color2,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIMessageDisplay.Font),null),&target->Font,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIRadarObjectsLayer with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIRadarObjectsLayer* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIRadarObjectsLayer.MinUnitDotSize),"3"),&target->MinUnitDotSize,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIRadarObjectsLayer.MinStructureDotSize),"5"),&target->MinStructureDotSize,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIRadarObjectsLayer.NeutralColor),null),&target->NeutralColor,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIRadarObjectsLayer.SpecialIcon)),&target->SpecialIcon,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UIRadarObjectsLayerSpecialIcon with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIRadarObjectsLayerSpecialIcon* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIRadarObjectsLayerSpecialIcon.Image),null),&target->Image,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIRadarObjectsLayerSpecialIcon.ImageOffset),null),&target->ImageOffset,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIRadarObjectsLayerSpecialIcon.ObjectFilter),null),&target->ObjectFilter,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal UISubtitleDisplay with the original defaults and tracked list/reference ownership. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUISubtitleDisplay* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUISubtitleDisplay.HoldTime),"20s"),&target->HoldTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUISubtitleDisplay.LineCount),"8"),&target->LineCount,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUISubtitleDisplay.Font),null),&target->Font,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUISubtitleDisplay.PanelColor),null),&target->PanelColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUISubtitleDisplay.TextColor),null),&target->TextColor,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

}
