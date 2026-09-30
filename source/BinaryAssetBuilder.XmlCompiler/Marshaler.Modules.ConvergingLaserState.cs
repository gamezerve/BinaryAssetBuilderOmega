using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: emit converging laser fields in recovered native order with schema defaults. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node, ConvergingLaserStateModuleData* objT, Tracker state)
    {
        if (node is null)
        {
            return;
        }
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.ConvergingAngle), null), &objT->ConvergingAngle, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.AngleRotation), "0d"), &objT->AngleRotation, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.Radius), "10.0"), &objT->Radius, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.SweepFXList), null), &objT->SweepFXList, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.VeteranSweepFXList), null), &objT->VeteranSweepFXList, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.SweepFXTimeout), "0s"), &objT->SweepFXTimeout, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.SweepWeapon), null), &objT->SweepWeapon, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.ModelConditions), null), &objT->ModelConditions, state);
        Marshal(node.GetAttributeValue(nameof(ConvergingLaserStateModuleData.Lifetime), "1s"), &objT->Lifetime, state);
        Marshal(node, (LaserStateModuleData*)objT, state);
    }
}
