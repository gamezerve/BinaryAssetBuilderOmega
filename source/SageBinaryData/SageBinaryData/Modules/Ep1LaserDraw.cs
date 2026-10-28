using System.Runtime.InteropServices;
namespace SageBinaryData
{
    // Reborn: Uprising adds FadeInTime/FadeOutTime after LaserWidth; the inline shader, optional status validation pointer and distortion flag follow (112 bytes).
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1W3DLaserDrawModuleData
    {
        public DrawModuleData Base;
        public float Texture1_UTile, Texture1_VTile, Texture2_UTile, Texture2_VTile;
        public float Texture1_UScrollRate, Texture1_VScrollRate, Texture2_UScrollRate, Texture2_VScrollRate;
        public int Texture1_NumFrames;
        public float Texture1_FrameRate;
        public int Texture2_NumFrames;
        public float Texture2_FrameRate;
        public float LaserWidth;
        public Time FadeInTime, FadeOutTime;
        public int WeaponSlotID, LaserStateID;
        public FXShaderMaterial FXShader;
        public ObjectStatusValidationDataType* ObjectStatusValidation;
        public SageBool UseDistortionShader;
    }
}
