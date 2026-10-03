using System.Runtime.InteropServices;
using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove independent stock native goldens while retaining the 28-byte legacy ABI and closed production audio gates.
internal static class AudioFileSerializationSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test exact RAM/streamed bytes, offsets, bounds and ownership; optional stock comparison reads only selected slices. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(params string[] manifests)
    {
        Require(Marshal.SizeOf<Ra3Ep1AudioFileRuntimeSerializer.NativePrefix>() == 32 && Marshal.SizeOf<SageBinaryData.AudioFileRuntime>() == 28,"EP1/legacy AudioFile ABI differs.");
        string[] fields = { "SubtitleLength","SubtitlePointer","Samples","Rate","HeaderPointer","HeaderSize","Channels" };
        for (int index = 0; index < fields.Length; index++) Require(Marshal.OffsetOf<Ra3Ep1AudioFileRuntimeSerializer.NativePrefix>(fields[index]).ToInt32() == (index+1)*4,"AudioFile field offset differs.");
        byte[] sound = Convert.FromHexString("0604BB804005B396");
        AssetBuffer ram = Serialize("DIALOGEVENT:wimpact_debrisvsgroundaSubTitle",48000,48000,1,Array.Empty<byte>());
        AssetBuffer voice = Serialize("DIALOGEVENT:aeva_keepcommanddirectiveSubTitle",373654,48000,2,sound);
        Require(ram.InstanceData.SequenceEqual(Convert.FromHexString("000000002B0000002000000080BB000080BB00000000000000000000010000004449414C4F474556454E543A77696D706163745F646562726973767367726F756E64615375625469746C6500"))
            && ram.RelocationData.SequenceEqual(Convert.FromHexString("08000000FFFFFFFF")),"Independent RAM golden differs.");
        Require(voice.InstanceData.SequenceEqual(Convert.FromHexString("000000002D0000002000000096B3050080BB00005000000008000000020000004449414C4F474556454E543A616576615F6B656570636F6D6D616E646469726563746976655375625469746C650000000604BB804005B396"))
            && voice.RelocationData.SequenceEqual(Convert.FromHexString("0800000014000000FFFFFFFF")),"Independent streamed golden differs.");
        Require(ram.ImportsData.Length == 0 && voice.ImportsData.Length == 0,"Runtime unexpectedly emitted imports.");
        AssetBuffer repeat = Serialize("DIALOGEVENT:aeva_keepcommanddirectiveSubTitle",373654,48000,2,sound);
        sound[0] ^= 1; repeat.InstanceData[0] = 1;
        Require(voice.InstanceData[0] == 0 && voice.InstanceData[80] == 6,"Serializer retained mutable input/output aliases.");
        foreach (int length in new[] { 1,2,3,4,1024 })
        {
            AssetBuffer aligned = Serialize(new string('x',length),1,400,1,Array.Empty<byte>());
            Require(aligned.InstanceData.Length == 32+((length+4)&~3),"Subtitle alignment differs.");
            AudioFileRuntimeProbe.Parse(aligned.InstanceData,aligned.InstanceData.Length);
        }
        foreach (TargetPlatform platform in new[] { TargetPlatform.Xbox360,TargetPlatform.PlayStation3,(TargetPlatform)999 }) Reject(() => Ra3Ep1AudioFileRuntimeSerializer.Serialize(platform,"x",1,400,1,Array.Empty<byte>()));
        foreach (string text in new[] { "","x\0","é",new string('x',1025),"x\n" }) Reject(() => Serialize(text,1,400,1,Array.Empty<byte>()));
        Reject(() => Serialize(null!,1,400,1,Array.Empty<byte>()));
        foreach (int samples in new[] { -1,0,0x40000000 }) Reject(() => Serialize("x",samples,400,1,Array.Empty<byte>()));
        foreach (int rate in new[] { 0,399,96001 }) Reject(() => Serialize("x",1,rate,1,Array.Empty<byte>()));
        foreach (byte channels in new byte[] { 0,3,5,255 }) Reject(() => Serialize("x",1,400,channels,Array.Empty<byte>()));
        byte[] originalHeader = Convert.FromHexString("0604BB804005B396");
        foreach (int length in new[] { 1,7,9 }) Reject(() => Serialize("x",373654,48000,2,new byte[length]));
        foreach (int slot in new[] { 1,2,4,7 })
        { byte[] bad = (byte[])originalHeader.Clone(); bad[slot] ^= slot == 1 ? (byte)4 : (byte)1; Reject(() => Serialize("x",373654,48000,2,bad)); }
        foreach (string path in manifests) Compare(path,ram,voice);
        Console.WriteLine("AudioFile serializer self-test: OK (32-byte EP1/28-byte legacy, two stock goldens, exact relocations/no imports, alignment/caps/current-header/ownership rejection; no processor registration)");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare the two exact serialized records with bounded real native/RELO/IMP slices and stock identity metadata. */
    //-------------------------------------------------------------------------------------------------
    private static void Compare(string path,AssetBuffer ram,AssetBuffer voice)
    {
        AudioFileRuntimeProbe.Run(path); ManifestDocument manifest = ManifestReader.Read(File.ReadAllBytes(path));
        long bin = 8,relo = 8,imp = 8; int found = 0;
        foreach (ManifestAsset asset in manifest.Assets)
        {
            AssetBuffer? expected = asset.Name == "AudioFile:WImpact_DebrisVsGrounda" ? ram : asset.Name == "AudioFile:Aeva_KeepCommandDirective" ? voice : null;
            if (expected != null)
            {
                Require(asset.TypeId == 0x166B084Du && asset.TypeHash == 0x53C81E47u && asset.Tokenized == 0 && asset.References.Count == 0,"Stock runtime identity mismatch.");
                Require(expected.InstanceData.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".bin"),null,bin,asset.InstanceDataSize))
                    && expected.RelocationData.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".relo"),null,relo,asset.RelocationDataSize))
                    && expected.ImportsData.SequenceEqual(AssetStreamProbe.ReadRange(Path.ChangeExtension(path,".imp"),null,imp,asset.ImportsDataSize)),"Actual stock serialized runtime differs."); found++;
            }
            bin += asset.InstanceDataSize; relo += asset.RelocationDataSize; imp += asset.ImportsDataSize;
        }
        Require(found == 2,"Two selected stock runtime records required."); Console.WriteLine("  Actual EP1 AudioFile native serialization: two records exactly matched.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: serialize only Win32 values through the isolated runtime helper, not the legacy audio processor. */
    //-------------------------------------------------------------------------------------------------
    private static AssetBuffer Serialize(string subtitle,int samples,int rate,byte channels,byte[] header) => Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,subtitle,samples,rate,channels,header);

    //-------------------------------------------------------------------------------------------------
    /** Reborn: require unsupported serialization requests to fail before returning native output. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    {
        try { action(); } catch (Exception error) when (error is ArgumentException or NotSupportedException) { return; }
        throw new InvalidDataException("Unsupported AudioFile runtime serialization accepted.");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on the first native serialization contract violation. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
