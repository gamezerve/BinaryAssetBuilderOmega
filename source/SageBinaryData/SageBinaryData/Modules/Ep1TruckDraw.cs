using Relo;
using System.Runtime.InteropServices;
using AnsiString=Relo.String<sbyte>;
namespace SageBinaryData
{
    // Reborn: tread scrolling owns two bone names and the three official real/percentage controls in twenty-eight bytes.
    [StructLayout(LayoutKind.Sequential)]
    public struct Ep1TreadScrollingMapping
    {
        public AnsiString LeftTrack,RightTrack;
        public float SpeedMultiplier;
        public Percentage TreadPivotSpeedFraction,TreadDriveSpeedFraction;
    }
    // Reborn: the 396-byte EP1 truck draw extends the reviewed 216-byte scripted draw; KW-only dust references are absent.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1W3DTruckDrawModuleData
    {
        public W3DScriptedModelDrawModuleData Base;
        public AnsiString LeftFrontTireBone;
        public AnsiString RightFrontTireBone;
        public AnsiString LeftRearTireBone;
        public AnsiString RightRearTireBone;
        public AnsiString MidLeftFrontTireBone;
        public AnsiString MidRightFrontTireBone;
        public AnsiString MidLeftRearTireBone;
        public AnsiString MidRightRearTireBone;
        public AnsiString MidLeftMidTireBone;
        public AnsiString MidRightMidTireBone;
        public AnsiString LeftFrontTireBone2;
        public AnsiString RightFrontTireBone2;
        public AnsiString LeftRearTireBone2;
        public AnsiString RightRearTireBone2;
        public AnsiString MidLeftMidTireBone2;
        public AnsiString MidRightMidTireBone2;
        public float TireRotationMultiplier,TireRotationMultiplierFront,PowerslideRotationAddition;
        public AnsiString CabBone,TrailerBone;
        public float CabRotationMultiplier,TrailerRotationMultiplier,RotationDamping;
        public AssetReference<BaseAssetType>* TreadTerrainParticles;
        public List<Ep1TreadScrollingMapping> TreadScrollingMap;
    }
}
