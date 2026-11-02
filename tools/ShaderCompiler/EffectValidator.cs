using System;
using System.IO;
using System.Runtime.InteropServices;

// Reborn: exercise the original Uprising DirectX effect-loader contract without opening or modifying the game.
internal static class EffectValidator
{
    internal const string Skip="AmbientLightColor;DirectionalLight;PointLight;Cloud;NoCloudMultiplier;RecolorColor;ShadowMapWorldToShadow;OpacityOverride;TintColor;EyePosition;ViewProjection;WorldBones;";
    [StructLayout(LayoutKind.Sequential)] private struct Present { public uint Width,Height,Format,Count,Multi,Quality,Swap; public IntPtr Window; public int Windowed,Depth; public uint DepthFormat,Flags,Rate,Interval; }
    [DllImport("d3d9.dll")] private static extern IntPtr Direct3DCreate9(uint version);
    [DllImport("user32.dll",CharSet=CharSet.Unicode)] private static extern IntPtr CreateWindowEx(uint ex,string cls,string name,uint style,int x,int y,int w,int h,IntPtr parent,IntPtr menu,IntPtr instance,IntPtr param);
    [DllImport("user32.dll")] private static extern bool DestroyWindow(IntPtr window);
    [DllImport("d3dx9_35.dll",CharSet=CharSet.Ansi)] private static extern int D3DXCreateEffectEx(IntPtr device,byte[] data,uint length,IntPtr defines,IntPtr include,string skip,uint flags,IntPtr pool,out IntPtr effect,out IntPtr errors);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate int CreateDevice(IntPtr self,uint adapter,uint type,IntPtr focus,uint flags,ref Present present,out IntPtr device);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate uint Release(IntPtr self);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] private delegate IntPtr BufferPointer(IntPtr self);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: obtain a method only from a locally created DirectX COM object. */
    //-------------------------------------------------------------------------------------------------
    private static T Method<T>(IntPtr obj,int slot) { return (T)(object)Marshal.GetDelegateForFunctionPointer(Marshal.ReadIntPtr(Marshal.ReadIntPtr(obj),slot*IntPtr.Size),typeof(T)); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require the same runtime DLL and skip-constant list as ra3ep1_1.1; device failures are not shader failures. */
    //-------------------------------------------------------------------------------------------------
    internal static void Validate(string file,Action<string> log)
    {
        IntPtr window=CreateWindowEx(0,"STATIC","Reborn shader validation",0,0,0,1,1,IntPtr.Zero,IntPtr.Zero,IntPtr.Zero,IntPtr.Zero);
        IntPtr d3d=IntPtr.Zero,device=IntPtr.Zero,effect=IntPtr.Zero,errors=IntPtr.Zero;
        try {
            d3d=Direct3DCreate9(32);if(d3d==IntPtr.Zero||window==IntPtr.Zero)throw new InvalidOperationException("DirectX validation is unavailable: a local hidden device could not be initialized.");
            var pp=new Present { Width=1,Height=1,Count=1,Swap=1,Window=window,Windowed=1 };
            int hr=Method<CreateDevice>(d3d,16)(d3d,0,1,window,0x22,ref pp,out device);
            if(hr<0)throw new InvalidOperationException("DirectX validation device unavailable (0x"+hr.ToString("X8")+"). Run on the normal Windows desktop; this result does not prove the shader is invalid.");
            byte[] data=File.ReadAllBytes(file);
            hr=D3DXCreateEffectEx(device,data,(uint)data.Length,IntPtr.Zero,IntPtr.Zero,Skip,0,IntPtr.Zero,out effect,out errors);
            string detail=errors==IntPtr.Zero?"[NULL]":Marshal.PtrToStringAnsi(Method<BufferPointer>(errors,3)(errors));
            if(hr<0)throw new InvalidDataException("Uprising effect loading failed: HRESULT=0x"+hr.ToString("X8")+", errors="+detail);
            log("Uprising effect loading: PASS (d3dx9_35, original application-managed constants).");
        }finally {
            if(errors!=IntPtr.Zero)Method<Release>(errors,2)(errors);if(effect!=IntPtr.Zero)Method<Release>(effect,2)(effect);
            if(device!=IntPtr.Zero)Method<Release>(device,2)(device);if(d3d!=IntPtr.Zero)Method<Release>(d3d,2)(d3d);if(window!=IntPtr.Zero)DestroyWindow(window);
        }
    }
}
