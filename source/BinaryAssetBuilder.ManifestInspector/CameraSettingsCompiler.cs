using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Xml;
using SageBinaryData;
using Relo;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: emit the original untokenized singleton camera record with no synthetic references or stock payload copying.
internal static class CameraSettingsCompiler
{
    private static readonly string[] Fields={"DefaultMinHeight","DefaultMaxHeight","DefaultPitchAngle","DefaultYawAngle","DefaultScrollSpeedScalar","DefaultHeightMapSmoothnessScalar","ScrollAmountCutoff","ScrollPitchMultiplier","CameraAdjustSpeed","ShakeSubtleIntensity","ShakeNormalIntensity","ShakeStrongIntensity","ShakeSevereIntensity","ShakeCineExtremeIntensity","ShakeCineInsaneIntensity","MaxShakeIntensity","MaxShakeRange","CameraLockHeightDelta","CameraTerrainSampleRadiusForHeight","CameraEaseFactor"};
    //-------------------------------------------------------------------------------------------------
    /** Reborn: retain XML omission as zero-initialized native storage; serialize every supplied camera field in EA's witnessed order. */
    //-------------------------------------------------------------------------------------------------
    internal static Chunk Compile(XmlNode node)
    {
        if(Marshal.SizeOf<CameraSettings>()!=84)throw new InvalidDataException("EP1 CameraSettings size differs.");
        var source=(XmlElement)node;var bytes=new byte[84];
        for(int i=0;i<Fields.Length;i++){
            string value=source.GetAttribute(Fields[i]);float number=value.Length==0?0:XmlConvert.ToSingle(value);
            BinaryPrimitives.WriteInt32LittleEndian(bytes.AsSpan(4+i*4,4),BitConverter.SingleToInt32Bits(number));
        }
        return new Chunk{InstanceBuffer=bytes,RelocationBuffer=Array.Empty<byte>(),ImportsBuffer=Array.Empty<byte>()};
    }
    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare the original EA source to its linked EP1 global stream, including the precise type fingerprint and field bytes. */
    //-------------------------------------------------------------------------------------------------
    internal static void Qualify(string global)
    {
        var catalog=ManifestReader.Read(SdkEnvironmentPreflight.Read(global,64*1048576));
        var asset=catalog.Assets.Single(a=>a.Name=="CameraSettings:Settings_Camera");
        if(asset.TypeId!=0xE862FF86u||asset.TypeHash!=0xFDCC69DEu||asset.Tokenized!=0||asset.InstanceDataSize!=84||asset.RelocationDataSize!=0||asset.ImportsDataSize!=0||asset.References.Count!=0)
            throw new InvalidDataException("EP1 CameraSettings stock fingerprint/layout differs.");
        var original=GameObjectExtendedRootContract.Original("System/CameraSettings.xml","CameraSettings").Single();
        var expected=W3DContainerCompiler.ReadStock(global,catalog,asset);
        if(!Compile(original).InstanceBuffer.SequenceEqual(expected))throw new InvalidDataException("CameraSettings original source/native field order differs.");
    }
}
