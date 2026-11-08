using Relo;
using SageBinaryData;
using BinaryAssetBuilder.Core.Hashing;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compile the common source priority while concrete component allocation supplies the original wire ID. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CoverageUIComponentBase* target,Tracker state)
    {if(node is null)return;Marshal(node.GetAttributeValue("Priority",null),&target->Priority,state);}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown CareerStatType values instead of silently storing zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockUIConfigCareerStatType* target,Tracker state)
    {if(value is null)return;if(!System.Enum.TryParse<Ep1StockUIConfigCareerStatType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Unknown CareerStatType: "+value);*target=parsed;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown CareerStatOrder values instead of silently storing zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockUIConfigCareerStatOrder* target,Tracker state)
    {if(value is null)return;if(!System.Enum.TryParse<Ep1StockUIConfigCareerStatOrder>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Unknown CareerStatOrder: "+value);*target=parsed;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown JoyPadButtonType values instead of silently storing zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockUIConfigJoyPadButtonType* target,Tracker state)
    {if(value is null)return;if(!System.Enum.TryParse<Ep1StockUIConfigJoyPadButtonType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Unknown JoyPadButtonType: "+value);*target=parsed;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown JoyPadActionType values instead of silently storing zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockUIConfigJoyPadActionType* target,Tracker state)
    {if(value is null)return;if(!System.Enum.TryParse<Ep1StockUIConfigJoyPadActionType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Unknown JoyPadActionType: "+value);*target=parsed;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject unknown PostGameStatType values instead of silently storing zero. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(string value,Ep1StockUIConfigPostGameStatType* target,Tracker state)
    {if(value is null)return;if(!System.Enum.TryParse<Ep1StockUIConfigPostGameStatType>(value,false,out var parsed)||!System.Enum.IsDefined(parsed))throw new System.IO.InvalidDataException("Unknown PostGameStatType: "+value);*target=parsed;}

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original AudioSettings defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigAudioSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.StreamBufferSizePerChannel),"65536"),&target->StreamBufferSizePerChannel,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.MaxRequestsPerStream),"4"),&target->MaxRequestsPerStream,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.ForceResetTime),"0s"),&target->ForceResetTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.EmergencyResetTime),"0s"),&target->EmergencyResetTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.MinOcclusion),"0"),&target->MinOcclusion,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DelayPriorToPlayingToReadSoundFile),"1s"),&target->DelayPriorToPlayingToReadSoundFile,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.ReadAheadTime),"3s"),&target->ReadAheadTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.QueueAheadTime),".5s"),&target->QueueAheadTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.LongTime),"60s"),&target->LongTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.AudioFootprintInBytes),"4194304"),&target->AudioFootprintInBytes,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DontLoadThirdVariationWhenFileReadQueueIsLongerThan),"50"),&target->DontLoadThirdVariationWhenFileReadQueueIsLongerThan,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DontLoadSecondVariationWhenFileReadQueueIsLongerThan),"100"),&target->DontLoadSecondVariationWhenFileReadQueueIsLongerThan,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan),"100"),&target->DontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.WhatIsLowPriorityForDontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan),"LOW"),&target->WhatIsLowPriorityForDontLoadForLowPrioritySoundsWhenFileReadQueueIsLongerThan,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.VolumeToGainConversionFactor),"1.66667"),&target->VolumeToGainConversionFactor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DefaultSoundVolume),"100"),&target->DefaultSoundVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DefaultVoiceVolume),"100"),&target->DefaultVoiceVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DefaultMusicVolume),"100"),&target->DefaultMusicVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DefaultMovieVolume),"100"),&target->DefaultMovieVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DefaultAmbientVolume),"100"),&target->DefaultAmbientVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.AutomaticSubtitleDuration),"8s"),&target->AutomaticSubtitleDuration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.AutomaticSubtitleLines),"8"),&target->AutomaticSubtitleLines,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PanRadiusScaleValue),"1.0"),&target->PanRadiusScaleValue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PanSize),"1.0"),&target->PanSize,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.MinSampleVolume),"0.01"),&target->MinSampleVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PositionDeltaForReverbCheck),"10.0"),&target->PositionDeltaForReverbCheck,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.TimeToFadeAudio),"1s"),&target->TimeToFadeAudio,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.AmbientStreamHysteresisVolume),"5"),&target->AmbientStreamHysteresisVolume,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.VolumeMultiplierFor2DSounds),"100%"),&target->VolumeMultiplierFor2DSounds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.VolumeMultiplierFor3DSounds),"100%"),&target->VolumeMultiplierFor3DSounds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.VoiceMoveToCampMaxCampnessAtStartPoint),"0"),&target->VoiceMoveToCampMaxCampnessAtStartPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.VoiceMoveToCampMinCampnessAtEndPoint),"5"),&target->VoiceMoveToCampMinCampnessAtEndPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.MinDelayBetweenEnterStateVoice),"5s"),&target->MinDelayBetweenEnterStateVoice,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.TimeSinceLastAttackForVoiceRetreatToCastle),"3s"),&target->TimeSinceLastAttackForVoiceRetreatToCastle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.DistanceToLookForEnemiesForVoiceRetreatToCastle),"500"),&target->DistanceToLookForEnemiesForVoiceRetreatToCastle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PostPostGameMusicWait),"10s"),&target->PostPostGameMusicWait,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.CapturedUnitsVoicePriorityOffset),"0"),&target->CapturedUnitsVoicePriorityOffset,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.UISelectionVoicePriorityOffset),"0"),&target->UISelectionVoicePriorityOffset,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.AddToSelectionVoiceTimeout),"0s"),&target->AddToSelectionVoiceTimeout,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.MinVolumeBeforeFade),"10"),&target->MinVolumeBeforeFade,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.MaxVolumeBeforeFade),"20"),&target->MaxVolumeBeforeFade,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PathMusicBufferTime),"1.5s"),&target->PathMusicBufferTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PathMusicLatencyMilisec),"500"),&target->PathMusicLatencyMilisec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigAudioSettings.AutomaticSubtitleWindowColor),null),&target->AutomaticSubtitleWindowColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigAudioSettings.AutomaticSubtitleTextColor),null),&target->AutomaticSubtitleTextColor,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigAudioSettings.VolumeSliderMultiplierForMovie)),&target->VolumeSliderMultiplierForMovie,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigAudioSettings.TacticalMicSettings),null),&target->TacticalMicSettings,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigAudioSettings.LivingWorldMicSettings),null),&target->LivingWorldMicSettings,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigAudioSettings.ShellMicSettings),null),&target->ShellMicSettings,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigAudioSettings.VolumeCompressorSettings),null),&target->VolumeCompressorSettings,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.SuppressOcclusion),"false"),&target->SuppressOcclusion,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioSettings.PlayEnemySightedEventsOnlyFromNearbyUnits),"false"),&target->PlayEnemySightedEventsOnlyFromNearbyUnits,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original AudioVolumeSliderMultiplier defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigAudioVolumeSliderMultiplier* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioVolumeSliderMultiplier.Slider),null),&target->Slider,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigAudioVolumeSliderMultiplier.Multiplier),null),&target->Multiplier,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original MicrophoneSettings defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigMicrophoneSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.MicrophonePreferredFractionCameraToGround),"86"),&target->MicrophonePreferredFractionCameraToGround,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.VolumeMicrophonePullTowardsTerrainLookAtPoint),"60"),&target->VolumeMicrophonePullTowardsTerrainLookAtPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.PanningMicrophonePullTowardsTerrainLookAtPoint),"60"),&target->PanningMicrophonePullTowardsTerrainLookAtPoint,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.MicrophoneMinDistanceToCamera),"100"),&target->MicrophoneMinDistanceToCamera,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.MicrophoneMaxDistanceToCamera),"300"),&target->MicrophoneMaxDistanceToCamera,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.ZoomMinDistance),"130"),&target->ZoomMinDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.ZoomMaxDistance),"425"),&target->ZoomMaxDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.ZoomSoundVolumePercentageAmount),"20"),&target->ZoomSoundVolumePercentageAmount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.ZoomFadeDistanceForMaxEffect),"200"),&target->ZoomFadeDistanceForMaxEffect,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.ZoomFadeZeroEffectEdgeLength),"260"),&target->ZoomFadeZeroEffectEdgeLength,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigMicrophoneSettings.ZoomFadeFullEffectEdgeLength),"135"),&target->ZoomFadeFullEffectEdgeLength,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original VolumeCompressionSettings defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigVolumeCompressionSettings* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigVolumeCompressionSettings.Threshold),"20"),&target->Threshold,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigVolumeCompressionSettings.Ratio),"10"),&target->Ratio,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigVolumeCompressionSettings.AttackTime),"100ms"),&target->AttackTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigVolumeCompressionSettings.ReleaseTime),"500ms"),&target->ReleaseTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigVolumeCompressionSettings.AppliesEquallyToAllChannels),"true"),&target->AppliesEquallyToAllChannels,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIConfigList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIConfigList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIConfigList.UIDynamicConfig)),&target->UIDynamicConfig,state);
        Marshal(node,(BaseAssetType*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIDynamicConfig defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIDynamicConfig* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIDynamicConfig.Name),null),&target->Name,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIDynamicConfig.Type),null),&target->Type,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIDynamicConfig.ComponentList),null),&target->ComponentList,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentAptJoypad defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentAptJoypad* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptJoypad.RelayLeftStickPosition),"false"),&target->RelayLeftStickPosition,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptJoypad.RelayRightStickPosition),"false"),&target->RelayRightStickPosition,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentAptMediator defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentAptMediator* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.AptGlobalLoadVarName),null),&target->AptGlobalLoadVarName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.AptEventHandlerName),null),&target->AptEventHandlerName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.AptFSCommandHandlerName),null),&target->AptFSCommandHandlerName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.AptTokenDelimiters),null),&target->AptTokenDelimiters,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.NewSkoolIndicator),null),&target->NewSkoolIndicator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.FunctionSeparator),null),&target->FunctionSeparator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.ValueSeparator),null),&target->ValueSeparator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.InputParameterSeparator),null),&target->InputParameterSeparator,state);
        // Reborn: preserve the original escaped leading-equals literal without passing its escape to APT.
        var separator=node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.ParameterNameSeparator),null)?.GetText();
        Marshal(separator!=null&&separator.StartsWith("\\=",System.StringComparison.Ordinal)?separator.Substring(1):separator,&target->ParameterNameSeparator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.ParameterStartIndicator),null),&target->ParameterStartIndicator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.SubObjectSeparator),null),&target->SubObjectSeparator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentAptMediator.OutputParameterSeparator),null),&target->OutputParameterSeparator,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentAutoMatch defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentAutoMatch* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentBootup defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentBootup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentBootup.RenderImageName),null),&target->RenderImageName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentBootup.LoadingTextImageName),null),&target->LoadingTextImageName,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCampaign defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCampaign* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCareerStats defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCareerStats* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCareerStats.DefaultRankImage),null),&target->DefaultRankImage,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentCareerStats.CareerStat)),&target->CareerStat,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original CareerStatData defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigCareerStatData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigCareerStatData.StatType),null),&target->StatType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigCareerStatData.StatLabel),null),&target->StatLabel,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigCareerStatData.RatioNumerator),null),&target->RatioNumerator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigCareerStatData.RatioDenominator),null),&target->RatioDenominator,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigCareerStatData.IsRatio),"false"),&target->IsRatio,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigCareerStatData.NoSummary),"false"),&target->NoSummary,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentChat defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentChat* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentChat.ChatTextLocalLabelPrefix),null),&target->ChatTextLocalLabelPrefix,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentChat.ChatTextGlobalLabelPrefix),null),&target->ChatTextGlobalLabelPrefix,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCheats defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCheats* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentComlinkNotification defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentComlinkNotification* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentComlinkNotification.MessageTokenSpec),null),&target->MessageTokenSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCommandBar defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCommandBar* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.HomeButtonImageName),"CbarHome"),&target->HomeButtonImageName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.ButtonSlotImageBaseName),"CbarSlot"),&target->ButtonSlotImageBaseName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.GeneralButtonImageBaseName),"CbarGeneral"),&target->GeneralButtonImageBaseName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.RenderClockName),null),&target->RenderClockName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.ClockScale),null),&target->ClockScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.ClockImage),null),&target->ClockImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.UnitCapFlashSeconds),null),&target->UnitCapFlashSeconds,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.UnitCapFinalWarningAmount),null),&target->UnitCapFinalWarningAmount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.UnitCapInitialWarningAmount),null),&target->UnitCapInitialWarningAmount,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.UnitCapNoMoreCP),null),&target->UnitCapNoMoreCP,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.UnitCapAlmostNoMoreCP),null),&target->UnitCapAlmostNoMoreCP,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.ScrollPowersSoundName),null),&target->ScrollPowersSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.OpenUISoundName),null),&target->OpenUISoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.CloseUISoundName),null),&target->CloseUISoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.HighlightButtonSoundName),null),&target->HighlightButtonSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.HighlightButtonForceFeedback),null),&target->HighlightButtonForceFeedback,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.TransitionForceFeedback),null),&target->TransitionForceFeedback,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.SelectButtonSoundName),null),&target->SelectButtonSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.ZoomInSoundName),null),&target->ZoomInSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.ZoomOutSoundName),null),&target->ZoomOutSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.CycleSoundName),null),&target->CycleSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.AptTokenTitle),null),&target->AptTokenTitle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.AptTokenCost),null),&target->AptTokenCost,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.AptTokenTime),null),&target->AptTokenTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandBar.AptTokenEnergy),null),&target->AptTokenEnergy,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentCommandBar.ClockColor),null),&target->ClockColor,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCommandList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCommandList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandList.AptTokenLeftTrigger),null),&target->AptTokenLeftTrigger,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandList.AptTokenRightTrigger),null),&target->AptTokenRightTrigger,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandList.AptTokenAButton),null),&target->AptTokenAButton,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandList.AptTokenBButton),null),&target->AptTokenBButton,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandList.AptTokenXButton),null),&target->AptTokenXButton,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCommandList.AptTokenYButton),null),&target->AptTokenYButton,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCoopCommander defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCoopCommander* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.AssaultButtonName),null),&target->AssaultButtonName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.AssaultButtonDesc),null),&target->AssaultButtonDesc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.AssaultButtonCommandBarButtonTemplate),null),&target->AssaultButtonCommandBarButtonTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.NormalButtonName),null),&target->NormalButtonName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.NormalButtonDesc),null),&target->NormalButtonDesc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.NormalButtonCommandBarButtonTemplate),null),&target->NormalButtonCommandBarButtonTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.HoldPosButtonName),null),&target->HoldPosButtonName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.HoldPosButtonDesc),null),&target->HoldPosButtonDesc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.HoldPosButtonCommandBarButtonTemplate),null),&target->HoldPosButtonCommandBarButtonTemplate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.HoldObjButtonName),null),&target->HoldObjButtonName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.HoldObjButtonDesc),null),&target->HoldObjButtonDesc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCoopCommander.HoldObjButtonCommandBarButtonTemplate),null),&target->HoldObjButtonCommandBarButtonTemplate,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCredits defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCredits* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentCursor defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentCursor* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCursor.AptEventCursorChanged),null),&target->AptEventCursorChanged,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentCursor.HoverSelectForceFeedback),null),&target->HoverSelectForceFeedback,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentDisconnect defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentDisconnect* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentFormation defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentFormation* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentFormation.MaxDragLength),null),&target->MaxDragLength,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentGameInfo defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentGameInfo* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.GameScoreDisplayToken),null),&target->GameScoreDisplayToken,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.ScorePlayerToken),null),&target->ScorePlayerToken,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.FactionLabelToken),null),&target->FactionLabelToken,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.GameScoreXLocWS),null),&target->GameScoreXLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.GameScoreXLoc),null),&target->GameScoreXLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.GameScoreYLocWS),null),&target->GameScoreYLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.GameScoreYLoc),null),&target->GameScoreYLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player1ScoreXLocWS),null),&target->Player1ScoreXLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player1ScoreXLoc),null),&target->Player1ScoreXLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player1ScoreYLocWS),null),&target->Player1ScoreYLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player1ScoreYLoc),null),&target->Player1ScoreYLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player2ScoreXLocWS),null),&target->Player2ScoreXLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player2ScoreXLoc),null),&target->Player2ScoreXLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player2ScoreYLocWS),null),&target->Player2ScoreYLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player2ScoreYLoc),null),&target->Player2ScoreYLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player3ScoreXLocWS),null),&target->Player3ScoreXLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player3ScoreXLoc),null),&target->Player3ScoreXLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player3ScoreYLocWS),null),&target->Player3ScoreYLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player3ScoreYLoc),null),&target->Player3ScoreYLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player4ScoreXLocWS),null),&target->Player4ScoreXLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player4ScoreXLoc),null),&target->Player4ScoreXLoc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player4ScoreYLocWS),null),&target->Player4ScoreYLocWS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameInfo.Player4ScoreYLoc),null),&target->Player4ScoreYLoc,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentGameInfo.FactionValue)),&target->FactionValue,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentGameSetup defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentGameSetup* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentGameViewJoypad defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentGameViewJoypad* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.LeftStickMultiplier),"100.0"),&target->LeftStickMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.RightStickMultiplier),"0.25"),&target->RightStickMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.ObjectAttractivityForce),"20.0"),&target->ObjectAttractivityForce,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MinAttractDistance),"40.0"),&target->MinAttractDistance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MinAttractDistanceZoomed),"40.0"),&target->MinAttractDistanceZoomed,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MaxAttractiveForce),"0.6"),&target->MaxAttractiveForce,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.KindOfExemptFromAttraction),null),&target->KindOfExemptFromAttraction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.ScrollSpeedMin),null),&target->ScrollSpeedMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.ScrollSpeedMax),null),&target->ScrollSpeedMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.ScrollSpeedMaxCutoff),null),&target->ScrollSpeedMaxCutoff,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.ScrollSpeedMinCutoff),null),&target->ScrollSpeedMinCutoff,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.InternalZoomInValue),null),&target->InternalZoomInValue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.InternalZoomOutValue),null),&target->InternalZoomOutValue,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.RapidScrollMultiplier),"2.0"),&target->RapidScrollMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MagnetismTuning_LOW_Strength),"1.1"),&target->MagnetismTuning_LOW_Strength,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MagnetismTuning_LOW_Distance),"0.6"),&target->MagnetismTuning_LOW_Distance,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MagnetismTuning_HIGH_Strength),"1.5"),&target->MagnetismTuning_HIGH_Strength,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MagnetismTuning_HIGH_Distance),"1.25"),&target->MagnetismTuning_HIGH_Distance,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentGameViewJoypad.MagnetismObjectFilter),null),&target->MagnetismObjectFilter,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentInGameShowHide defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentInGameShowHide* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentInGameText defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentInGameText* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AdjustFactor),null),&target->AdjustFactor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.TextIndent),null),&target->TextIndent,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.TooltipAppearDelayMS),"3000"),&target->TooltipAppearDelayMS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.TooltipDisappearDelayMS),"3000"),&target->TooltipDisappearDelayMS,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenTitle),null),&target->AptTokenTitle,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenPrereq),null),&target->AptTokenPrereq,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenCost),null),&target->AptTokenCost,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenTime),null),&target->AptTokenTime,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenEnergy),null),&target->AptTokenEnergy,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenShortDesc),null),&target->AptTokenShortDesc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.AptTokenLongDesc),null),&target->AptTokenLongDesc,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.ProductionTextConstruction),null),&target->ProductionTextConstruction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.ProductionTextUpgrade),null),&target->ProductionTextUpgrade,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.ProductionTextRecruit),null),&target->ProductionTextRecruit,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.ProductionTextScale),null),&target->ProductionTextScale,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.SubtitleStringLabelPrefix),null),&target->SubtitleStringLabelPrefix,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.SubtitleStringLabelSuffix),null),&target->SubtitleStringLabelSuffix,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentInGameText.SubtitleStringExclusionChar),null),&target->SubtitleStringExclusionChar,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentInGameText.StatusTextPosition),null),&target->StatusTextPosition,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentInGameText.ProductionTextColor),null),&target->ProductionTextColor,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentInGameText.ProductionTextFont),null),&target->ProductionTextFont,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentInputBridge defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentInputBridge* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentJoypadTactical defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentJoypadTactical* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentKeyboard defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentKeyboard* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentKeyboard.EditableTextLabelPrefix),null),&target->EditableTextLabelPrefix,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentLanLobby defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentLanLobby* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLanLobby.PlayerlistMinUpdateFreq),null)?.GetValue(),&target->PlayerlistMinUpdateFreq,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLanLobby.GamelistMinUpdateFreq),null)?.GetValue(),&target->GamelistMinUpdateFreq,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLanLobby.PlayerNameAptTokenSpec),null),&target->PlayerNameAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLanLobby.GameNameAptTokenSpec),null),&target->GameNameAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLanLobby.LocalPlayerNameAptTokenSpec),null),&target->LocalPlayerNameAptTokenSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentLoadScreen defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentLoadScreen* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentLobby defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentLobby* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.RandomStringLabel),null),&target->RandomStringLabel,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.FactionLabelSpec),null),&target->FactionLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.AIPersonalityLabelSpec),null),&target->AIPersonalityLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.AIDifficultyLabelSpec),null),&target->AIDifficultyLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.TeamLabelSpec),null),&target->TeamLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.MapListSpec),null),&target->MapListSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.GameplayTypeSpec),null),&target->GameplayTypeSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.GameTimeLimitSpec),null),&target->GameTimeLimitSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.GameResourcesSpec),null),&target->GameResourcesSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.HillTimeSpec),null),&target->HillTimeSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.CapturePointsSpec),null),&target->CapturePointsSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.BarrierTimeSpec),null),&target->BarrierTimeSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.CaptureFlagsSpec),null),&target->CaptureFlagsSpec,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.Faction)),&target->Faction,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.AIDifficulty)),&target->AIDifficulty,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.Team)),&target->Team,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.GameType)),&target->GameType,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.GameTime)),&target->GameTime,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.GameResources)),&target->GameResources,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.HillTime)),&target->HillTime,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.BarrierTime)),&target->BarrierTime,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.CapturePoints)),&target->CapturePoints,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentLobby.CaptureFlags)),&target->CaptureFlags,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.MultiplayerLobbySettings),null),&target->MultiplayerLobbySettings,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentLobby.SkirmishLobbySettings),null),&target->SkirmishLobbySettings,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMapList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMapList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentMapList.MapListLabelPrefix),null),&target->MapListLabelPrefix,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMessage defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMessage* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMode defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMode* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMouse defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMouse* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMouseInGame defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMouseInGame* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMouseTactical defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMouseTactical* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMovie defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMovie* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentMovie.MovieComponentName),null),&target->MovieComponentName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentMovie.MaxNumberOfStream),"3"),&target->MaxNumberOfStream,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentMovieArchive defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentMovieArchive* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentMovieArchive.GeneralMovie)),&target->GeneralMovie,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentMovieArchive.ScenarioMovie)),&target->ScenarioMovie,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentMovieArchive.CampaignMovie)),&target->CampaignMovie,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentObjectAction defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentObjectAction* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentObjectSelector defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentObjectSelector* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.PortraitName),"true"),&target->PortraitName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.UnitAbility),"true"),&target->UnitAbility,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.StatusTextNothingSelected),null),&target->StatusTextNothingSelected,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.StatusTextSelectedAcrossMap),null),&target->StatusTextSelectedAcrossMap,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.StatusTextSelectedAcrossScreen),null),&target->StatusTextSelectedAcrossScreen,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.AllArmyImageGDI),null),&target->AllArmyImageGDI,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.AllArmyImageNOD),null),&target->AllArmyImageNOD,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.AllArmyImageAlien),null),&target->AllArmyImageAlien,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.ClockImage),null),&target->ClockImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.AssaultTargetForceFeedback),null),&target->AssaultTargetForceFeedback,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentObjectSelector.AlienHighlightDecal),null),&target->AlienHighlightDecal,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentObjectSelector.GDIHighlightDecal),null),&target->GDIHighlightDecal,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentObjectSelector.NODHighlightDecal),null),&target->NODHighlightDecal,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentObjectSelector.ShortcutObjects)),&target->ShortcutObjects,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentObjectSelector.SupportObjects)),&target->SupportObjects,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentObjectSelector.ClockColor),null),&target->ClockColor,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectSelector.ShowAllHealthBars),"true"),&target->ShowAllHealthBars,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentObjectives defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentObjectives* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectives.AptStringNewBonusObjective),""),&target->AptStringNewBonusObjective,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentObjectives.AptStringObjectiveCompleted),""),&target->AptStringObjectiveCompleted,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOnlineBuddyList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOnlineBuddyList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineBuddyList.BuddyNameAptTokenSpec),null),&target->BuddyNameAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineBuddyList.BuddyLocAptTokenSpec),null),&target->BuddyLocAptTokenSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOnlineGameList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOnlineGameList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineGameList.GameNameAptTokenSpec),null),&target->GameNameAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineGameList.GameMapNameAptTokenSpec),null),&target->GameMapNameAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineGameList.GameListRefreshInterval),null),&target->GameListRefreshInterval,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOnlineInvite defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOnlineInvite* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineInvite.MaxNumberLoggedInvites),null)?.GetValue(),&target->MaxNumberLoggedInvites,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineInvite.InviteNameAptTokenSpec),null),&target->InviteNameAptTokenSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOnlineLobbyList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOnlineLobbyList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlineLobbyList.LobbyNameAptTokenSpec),null),&target->LobbyNameAptTokenSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOnlineLogin defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOnlineLogin* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOnlinePlayerList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOnlinePlayerList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOnlinePlayerList.PlayerNameAptTokenSpec),null),&target->PlayerNameAptTokenSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOptions defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOptions* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentOptions.ControlsToken),null),&target->ControlsToken,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentOptions.SaveFileName),null),&target->SaveFileName,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentOptions.ControlsText)),&target->ControlsText,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptions.DefaultValues),null),&target->DefaultValues,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentOptions.CommandList)),&target->CommandList,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original DefaultValues defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigDefaultValues* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.Difficulty),"1"),&target->Difficulty,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.Gamma),"1.30"),&target->Gamma,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.Brightness),"0.50"),&target->Brightness,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.Contrast),"1.30"),&target->Contrast,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VolumeMusic),"0.45"),&target->VolumeMusic,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VolumeFX),"0.70"),&target->VolumeFX,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VolumeVoice),"0.70"),&target->VolumeVoice,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VolumeAmbient),"0.50"),&target->VolumeAmbient,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VolumeMovie),"0.70"),&target->VolumeMovie,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ScrollSpeed),"1.00"),&target->ScrollSpeed,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ScrollMagnetism),"2"),&target->ScrollMagnetism,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.DefaultFaction),"-1"),&target->DefaultFaction,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VisionCamZoom),"1"),&target->VisionCamZoom,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ScrollSpeedMin),"0.2"),&target->ScrollSpeedMin,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ScrollSpeedMax),"2.0"),&target->ScrollSpeedMax,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ButtonIcon),"true"),&target->ButtonIcon,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ShowCommandList),"true"),&target->ShowCommandList,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.HealthBars),"true"),&target->HealthBars,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.HighlightPads),"true"),&target->HighlightPads,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.ToolTips),"true"),&target->ToolTips,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.Subtitles),"false"),&target->Subtitles,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.InvertRotate),"true"),&target->InvertRotate,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.Telemetry),"true"),&target->Telemetry,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigDefaultValues.VisionCamVisible),"true"),&target->VisionCamVisible,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original SingleCommandList defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigSingleCommandList* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigSingleCommandList.CategoryName),null),&target->CategoryName,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigSingleCommandList.CommandLine)),&target->CommandLine,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original SingleCommandLine defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigSingleCommandLine* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigSingleCommandLine.Purpose),null),&target->Purpose,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigSingleCommandLine.Description),null),&target->Description,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigSingleCommandLine.Input)),&target->Input,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original InputButton defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigInputButton* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigInputButton.ButtonName),null),&target->ButtonName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigInputButton.ButtonAction),null),&target->ButtonAction,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOptionsSettingsAudio defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOptionsSettingsAudio* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsAudio.LoadVarQueryReturnToken),null),&target->LoadVarQueryReturnToken,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsAudio.DefaultVoipEnable),null),&target->DefaultVoipEnable,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsAudio.DefaultVoipBoostMicGain),null),&target->DefaultVoipBoostMicGain,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOptionsSettingsGeneral defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOptionsSettingsGeneral* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.LoadVarQueryReturnToken),null),&target->LoadVarQueryReturnToken,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.MinTooltipDelayMS),null),&target->MinTooltipDelayMS,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.MaxTooltipDelayMS),null),&target->MaxTooltipDelayMS,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.DefaultTooltipDelayMS),null),&target->DefaultTooltipDelayMS,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.DefaultMouseSensitivity),null),&target->DefaultMouseSensitivity,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.DefaultAlternateMouse),null),&target->DefaultAlternateMouse,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.DefaultShowNews),null),&target->DefaultShowNews,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGeneral.DefaultShowAds),null),&target->DefaultShowAds,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOptionsSettingsGraphics defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOptionsSettingsGraphics* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.LoadVarQueryReturnToken),null),&target->LoadVarQueryReturnToken,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.DefaultBrightness),null),&target->DefaultBrightness,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.PresetsAptTokenSpec),null),&target->PresetsAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.PresetsStrLookupSpec),null),&target->PresetsStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ResolutionAptTokenSpec),null),&target->ResolutionAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.AntialiasingAptTokenSpec),null),&target->AntialiasingAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.AntialiasingOffStrLookupSpec),null),&target->AntialiasingOffStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.AntialiasingOnStrLookupSpec),null),&target->AntialiasingOnStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.TerrainDetailAptTokenSpec),null),&target->TerrainDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.TerrainDetailStrLookupSpec),null),&target->TerrainDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.WaterDetailAptTokenSpec),null),&target->WaterDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.WaterDetailStrLookupSpec),null),&target->WaterDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ModelDetailAptTokenSpec),null),&target->ModelDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ModelDetailStrLookupSpec),null),&target->ModelDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.TextureQualityAptTokenSpec),null),&target->TextureQualityAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.TextureQualityStrLookupSpec),null),&target->TextureQualityStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ShaderDetailAptTokenSpec),null),&target->ShaderDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ShaderDetailStrLookupSpec),null),&target->ShaderDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.VFXDetailAptTokenSpec),null),&target->VFXDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.VFXDetailStrLookupSpec),null),&target->VFXDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ShadowDetailAptTokenSpec),null),&target->ShadowDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.ShadowDetailStrLookupSpec),null),&target->ShadowDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.AnimationDetailAptTokenSpec),null),&target->AnimationDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.AnimationDetailStrLookupSpec),null),&target->AnimationDetailStrLookupSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.DecalDetailAptTokenSpec),null),&target->DecalDetailAptTokenSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsGraphics.DecalDetailStrLookupSpec),null),&target->DecalDetailStrLookupSpec,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOptionsSettingsHotkeys defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOptionsSettingsHotkeys* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsHotkeys.LoadVarQueryReturnToken),null),&target->LoadVarQueryReturnToken,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentOptionsSettingsNetwork defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentOptionsSettingsNetwork* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentOptionsSettingsNetwork.LoadVarQueryReturnToken),null),&target->LoadVarQueryReturnToken,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentPause defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentPause* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentPcSaveLoad defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentPcSaveLoad* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentPcSaveLoad.MapPrefix),null),&target->MapPrefix,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentPcSaveLoad.DescriptionPrefix),null),&target->DescriptionPrefix,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentPcSaveLoad.TimePrefix),null),&target->TimePrefix,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentPcSaveLoad.DatePrefix),null),&target->DatePrefix,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentPlasmaInterface defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentPlasmaInterface* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentPlayerPowers defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentPlayerPowers* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPlayerPowers.ClockImage),null),&target->ClockImage,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentPlayerTechStore defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentPlayerTechStore* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPlayerTechStore.HighlightButtonSoundName),null),&target->HighlightButtonSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPlayerTechStore.SelectButtonSoundName),null),&target->SelectButtonSoundName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPlayerTechStore.CancelButtonSoundName),null),&target->CancelButtonSoundName,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentPostGame defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentPostGame* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPostGame.AlliesFactionImage),null),&target->AlliesFactionImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPostGame.SovietFactionImage),null),&target->SovietFactionImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPostGame.JapanFactionImage),null),&target->JapanFactionImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPostGame.YurikoFactionImage),null),&target->YurikoFactionImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentPostGame.DefaultImage),null),&target->DefaultImage,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentPostGame.PostGameStat)),&target->PostGameStat,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original PostGameStatData defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigPostGameStatData* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigPostGameStatData.StatType),null),&target->StatType,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigPostGameStatData.StatLabel),null),&target->StatLabel,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentProfileManager defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentProfileManager* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentProfileManager.ProfileListLabelPrefix),null),&target->ProfileListLabelPrefix,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentRadar defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentRadar* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.RadarWidth),"128"),&target->RadarWidth,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.RadarHeight),"128"),&target->RadarHeight,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.OrientationArrowSize),null),&target->OrientationArrowSize,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.OrientationArrowImage),null),&target->OrientationArrowImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.StatusTextInfiltration),null),&target->StatusTextInfiltration,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.LeftStickMultiplier),"0.035"),&target->LeftStickMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.RightStickMultiplier),"350.0"),&target->RightStickMultiplier,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentRadar.LeftTriggerTolerance),"900"),&target->LeftTriggerTolerance,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentSUDs defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentSUDs* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentSUDs.TechPointAwardedSound),null),&target->TechPointAwardedSound,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentSaveLoad defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentSaveLoad* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentSaveLoad.FileToken),null),&target->FileToken,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentScenario defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentScenario* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentShell defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentShell* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentShell.LoadMusic),null),&target->LoadMusic,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentShell.LogoRenderImage),null),&target->LogoRenderImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentShell.LogoTexture),null),&target->LogoTexture,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentShellMultiplayer defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentShellMultiplayer* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.OfficialMapList),null),&target->OfficialMapList,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.MatchOptionsRankedLabelSpec),null),&target->MatchOptionsRankedLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.MatchOptionsMatchTypeLabelSpec),null),&target->MatchOptionsMatchTypeLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.MatchOptionsPlayersLabelSpec),null),&target->MatchOptionsPlayersLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.AnyOptionLabelSpec),null),&target->AnyOptionLabelSpec,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.TACLineLabelSpec),null),&target->TACLineLabelSpec,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.GameType)),&target->GameType,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentShellMultiplayer.RankedOption)),&target->RankedOption,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original MapListType defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigMapListType* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigMapListType.Map)),&target->Map,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentShellVisibility defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentShellVisibility* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentShellVisibility.MainMenuPersonalityGroup),null),&target->MainMenuPersonalityGroup,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentSound defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentSound* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentSpecialPowers defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentSpecialPowers* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentSpecialPowers.PowerTimerImage),null),&target->PowerTimerImage,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentSpecialPowers.PowerTimerFlashImageName),null),&target->PowerTimerFlashImageName,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentStats defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentStats* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentStats.PlayerToken),null),&target->PlayerToken,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentStats.TitleToken),null),&target->TitleToken,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentStats.DataToken),null),&target->DataToken,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentTicker defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentTicker* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentTicker.CustomRenderName),null),&target->CustomRenderName,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentTicker.NumberOfTickerDisplays),null),&target->NumberOfTickerDisplays,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentTicker.ScrollSpeedPixelPerSec),null),&target->ScrollSpeedPixelPerSec,state);
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentTicker.NumberOfPixelsBetweenHeadline),null),&target->NumberOfPixelsBetweenHeadline,state);
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentTicker.TickerColor),null),&target->TickerColor,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentTicker.TickerGroup)),&target->TickerGroup,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentTutorial defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentTutorial* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentVictoryDefeat defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentVictoryDefeat* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetChildNode(nameof(Ep1StockUIConfigUIComponentVictoryDefeat.PostEndGameMusic),null),&target->PostEndGameMusic,state);
        Marshal(node.GetChildNodes(nameof(Ep1StockUIConfigUIComponentVictoryDefeat.EndGameMusicAlias)),&target->EndGameMusicAlias,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: marshal original UIComponentVoiceChat defaults and tracked child/reference storage. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1StockUIConfigUIComponentVoiceChat* target,Tracker state)
    {
        if(node is null)return;
        Marshal(node.GetAttributeValue(nameof(Ep1StockUIConfigUIComponentVoiceChat.TalkHotKeySlot),null),&target->TalkHotKeySlot,state);
        Marshal(node,(Ep1CoverageUIComponentBase*)target,state);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: only actual original UI-config component classes have typed allocation authority. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1CoverageUIComponentBase** target,Tracker state)
    {
        if(node is null)return;uint wire=0;Marshal(node.GetAttributeValue("TypeId","0"),&wire,state);
        if(wire==FastHash.GetHashCode("UIComponentAptJoypad"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentAptJoypad,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentAptMediator"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentAptMediator,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentAutoMatch"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentAutoMatch,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentBootup"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentBootup,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCampaign"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCampaign,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCareerStats"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCareerStats,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentChat"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentChat,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCheats"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCheats,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentComlinkNotification"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentComlinkNotification,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCommandBar"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCommandBar,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCommandList"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCommandList,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCoopCommander"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCoopCommander,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCredits"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCredits,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentCursor"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentCursor,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentDisconnect"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentDisconnect,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentFormation"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentFormation,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentGameInfo"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentGameInfo,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentGameSetup"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentGameSetup,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentGameViewJoypad"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentGameViewJoypad,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentInGameShowHide"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentInGameShowHide,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentInGameText"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentInGameText,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentInputBridge"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentInputBridge,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentJoypadTactical"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentJoypadTactical,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentKeyboard"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentKeyboard,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentLanLobby"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentLanLobby,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentLoadScreen"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentLoadScreen,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentLobby"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentLobby,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMapList"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMapList,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMessage"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMessage,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMode"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMode,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMouse"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMouse,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMouseInGame"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMouseInGame,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMouseTactical"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMouseTactical,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMovie"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMovie,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentMovieArchive"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentMovieArchive,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentObjectAction"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentObjectAction,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentObjectSelector"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentObjectSelector,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentObjectives"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentObjectives,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOnlineBuddyList"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOnlineBuddyList,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOnlineGameList"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOnlineGameList,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOnlineInvite"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOnlineInvite,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOnlineLobbyList"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOnlineLobbyList,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOnlineLogin"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOnlineLogin,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOnlinePlayerList"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOnlinePlayerList,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOptions"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOptions,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOptionsSettingsAudio"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOptionsSettingsAudio,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOptionsSettingsGeneral"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOptionsSettingsGeneral,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOptionsSettingsGraphics"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOptionsSettingsGraphics,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOptionsSettingsHotkeys"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOptionsSettingsHotkeys,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentOptionsSettingsNetwork"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentOptionsSettingsNetwork,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentPause"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentPause,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentPcSaveLoad"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentPcSaveLoad,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentPlasmaInterface"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentPlasmaInterface,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentPlayerPowers"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentPlayerPowers,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentPlayerTechStore"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentPlayerTechStore,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentPostGame"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentPostGame,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentProfileManager"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentProfileManager,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentRadar"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentRadar,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentSUDs"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentSUDs,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentSaveLoad"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentSaveLoad,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentScenario"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentScenario,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentShell"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentShell,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentShellMultiplayer"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentShellMultiplayer,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentShellVisibility"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentShellVisibility,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentSound"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentSound,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentSpecialPowers"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentSpecialPowers,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentStats"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentStats,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentTicker"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentTicker,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentTutorial"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentTutorial,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentVictoryDefeat"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentVictoryDefeat,Ep1CoverageUIComponentBase>(node,target,state);
        else if(wire==FastHash.GetHashCode("UIComponentVoiceChat"))MarshalPolymorphicType<Ep1StockUIConfigUIComponentVoiceChat,Ep1CoverageUIComponentBase>(node,target,state);
        else throw new System.IO.InvalidDataException("UI component has no original native witness: "+wire.ToString("X8"));
        state.InplaceEndianToPlatform(&wire);*(uint*)*target=wire;
    }
}
