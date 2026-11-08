using System.Runtime.InteropServices;
using Relo;
using SageBinaryData;

public static partial class Marshaler
{
    // Reborn: original streamed music/dialog/ambient records share the 128-byte EP1 sound base and one nullable AudioFile import.
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Ep1SingleFileSound
    {
        public Ep1BaseSingleSound Base;
        public AssetReference<AudioFile>* Filename;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: preserve optional filename ownership and the existing EP1 common sound writer without repacking stock samples. */
    //-------------------------------------------------------------------------------------------------
    public static unsafe void Marshal(Node node,Ep1SingleFileSound* target,Tracker state)
    {
        if(node is null)return;
        if(node.GetChildNode("LimitGroup",null)!=null)throw new System.NotSupportedException("Single-file audio LimitGroup references require an independent native witness.");
        Marshal(node.GetChildNode("Filename",null),&target->Filename,state);
        Marshal(node,&target->Base,state);
    }
}
