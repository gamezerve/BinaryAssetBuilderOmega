using BinaryAssetBuilder.Core;
using BinaryAssetBuilder.XmlCompiler;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: synthetic compressed framing validates variable linked packaging without executing any native codec.
internal static class AudioPoolPackageSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: test bounded cardinality/order, variable source/play-location binding, immutable snapshots and package corruption/no-overwrite gates. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        string root = Path.Combine(Path.GetTempPath(),"Reborn-PoolPackageTest-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(root);
        foreach (int count in new[] { 1,3,8 })
        {
            var entries = Enumerable.Range(0,count).Select(index => Entry(index,index%2 != 0)).Reverse().ToArray();
            string output = Path.Combine(root,"pool"+count); AudioFilePackageProbe.Publish(output,entries,variable:true);
            var payloads = AudioFilePackageProbe.Serialize(entries,variable:true); var manifest = ManifestReader.Read(payloads["diagnostic.manifest"]);
            Require(manifest.Assets.Select(asset => asset.SourceFile).SequenceEqual(entries.Select(entry => entry.Source)),"Pool manifest order/source differs.");
            Require(payloads["diagnostic.bin"].Length == 8+entries.Sum(entry => entry.CopyNative().InstanceData.Length) && payloads["diagnostic.relo"].Length == 8+entries.Sum(entry => entry.CopyNative().RelocationData.Length) && payloads["diagnostic.imp"].Length == 8,"Pool stream sizes differ.");
            var detached = entries[0].CopyNative(); detached.InstanceData[0] ^= 1; entries[0].CopyCustom()[0] ^= 1;
            Reject(() => AudioFilePackageProbe.Publish(output,entries,variable:true)); AudioFilePackageProbe.Verify(output,entries,variable:true);
            if (count > 1) Reject(() => AudioFilePackageProbe.Verify(output,entries.Reverse().ToArray(),variable:true));
            foreach (string name in new[] { "diagnostic.manifest","diagnostic.bin","diagnostic.relo","diagnostic.imp" }.Concat(entries.Select(entry => Path.Combine("diagnostic","cdata",entry.CustomName))))
            {
                string path = Path.Combine(output,name); byte[] bytes = File.ReadAllBytes(path); byte[] changed = (byte[])bytes.Clone(); changed[0] ^= 1;
                File.WriteAllBytes(path,changed); Reject(() => AudioFilePackageProbe.Verify(output,entries,variable:true)); File.WriteAllBytes(path,bytes);
                File.Move(path,path+".missing"); Reject(() => AudioFilePackageProbe.Verify(output,entries,variable:true)); File.Move(path+".missing",path);
            }
            AudioFilePackageProbe.Verify(output,entries,variable:true);
        }
        Reject(() => AudioFilePackageProbe.Serialize(Array.Empty<AudioFilePackageProbe.Entry>(),variable:true));
        Reject(() => AudioFilePackageProbe.Serialize(Enumerable.Range(0,9).Select(index => Entry(index,false)).ToArray(),variable:true));
        Reject(() => AudioFilePackageProbe.Serialize(new[] { Entry(0,false),Entry(0,false) },variable:true));
        Reject(() => AudioFilePackageProbe.Serialize(new[] { Entry(0,false),new AudioFilePackageProbe.Entry("Different","tone0.xml",Entry(1,false).CopyNative(),Entry(1,false).CopyCustom(),false) },variable:true));
        // Reborn: different literal names cannot evade case-insensitive source and SAGE identity collision rejection.
        Reject(() => AudioFilePackageProbe.Serialize(new[] { Entry(0,false),new AudioFilePackageProbe.Entry("Different","TONE0.xml",Entry(1,false).CopyNative(),Entry(1,false).CopyCustom(),false) },variable:true));
        Reject(() => AudioFilePackageProbe.Serialize(new[] { Entry(0,false),new AudioFilePackageProbe.Entry("variable_0","other.xml",Entry(1,false).CopyNative(),Entry(1,false).CopyCustom(),false) },variable:true));
        Reject(() => AudioFilePackageProbe.Serialize(new[] { new AudioFilePackageProbe.Entry("Fixed","ram.xml",Entry(0,false).CopyNative(),Entry(0,false).CopyCustom()) },variable:true));
        Reject(() => AudioFilePackageProbe.Serialize(new[] { Entry(0,false) }));
        Reject(() => new AudioFilePackageProbe.Entry("Valid","CON.xml",Entry(0,false).CopyNative(),Entry(0,false).CopyCustom(),false));
        Reject(() => new AudioFilePackageProbe.Entry("Valid","tone.xml",Entry(0,false).CopyNative(),Entry(0,false).CopyCustom(),true));
        Console.WriteLine("Pool package self-test: OK (1/3/8 reordered leaves, dynamic linked offsets, two readers, immutable/native/custom evidence, bounds/duplicates/play-location/corruption/missing/no-overwrite rejection; synthetic framing only).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct only synthetic tag-04 bodies with explicit source names and play location; no codec correctness is claimed. */
    //-------------------------------------------------------------------------------------------------
    private static AudioFilePackageProbe.Entry Entry(int index,bool streamed)
    {
        byte[] header = streamed ? Convert.FromHexString("0400BB8040002EE0") : Array.Empty<byte>();
        AssetBuffer native = Ra3Ep1AudioFileRuntimeSerializer.Serialize(TargetPlatform.Win32,"PoolSubtitle_"+index,12000,48000,1,header);
        byte[] custom = Convert.FromHexString(streamed ? "8000000C00002EE0DEADBEEF" : "0400BB8000002EE00000000C00002EE0DEADBEEF");
        return new("Variable_"+index,"tone"+index+".xml",native,custom,streamed);
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: treat invalid bounded package evidence as rejection, never as successful publication. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action)
    { try { action(); } catch (Exception error) when (error is InvalidDataException or IOException or NotSupportedException) { return; } throw new InvalidDataException("Invalid variable package succeeded."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: enforce independent order/size expectations for each synthetic fixture. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
