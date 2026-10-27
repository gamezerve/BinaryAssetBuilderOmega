using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize the complete Uprising detector, inherited upgrades and real typed references using official defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, Ep1StealthDetectorUpdateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.DetectionRate), "0.4s"), &objT->DetectionRate, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.DetectionRange), "0.0"), &objT->DetectionRange, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.InitiallyDisabled), "false"), &objT->InitiallyDisabled, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.PingSound), null), &objT->PingSound, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.LoudPingSound), null), &objT->LoudPingSound, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.IRBeaconParticleSys), null), &objT->IRBeaconParticleSys, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.IRParticleSys), null), &objT->IRParticleSys, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.IRBrightParticleSys), null), &objT->IRBrightParticleSys, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.IRGridParticleSys), null), &objT->IRGridParticleSys, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.IRParticleSysBone), null), &objT->IRParticleSysBone, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.ExtraRequiredKindOf), null), &objT->ExtraRequiredKindOf, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.ExtraForbiddenKindOf), null), &objT->ExtraForbiddenKindOf, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.CanDetectWhileGarrisoned), "false"), &objT->CanDetectWhileGarrisoned, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.CanDetectWhileContained), "false"), &objT->CanDetectWhileContained, state);
        Marshal(node.GetAttributeValue(nameof(Ep1StealthDetectorUpdateModuleData.RequiredUpgrade), null), &objT->RequiredUpgrade, state);

        Marshal(node, (UpgradeModuleData*)objT, state);
    }
}
