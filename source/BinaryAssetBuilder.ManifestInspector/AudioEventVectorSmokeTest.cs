using System.Buffers.Binary;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: vector selection admission and actual core/mixed publication remain bounded to eight unique local leaves.
internal static class AudioEventVectorSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: prove cloned vector ownership, content equality, bounds/defaults and full vector native/import/manifest order with synthetic framing. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run()
    {
        uint[] weights = { 0,125,1000000 }; int[] slots = { 7,2,0 };
        var settings = new AuthoredAudioEventSource.Settings(37.5f,weights,slots,8,9); settings.Validate();
        var equal = new AuthoredAudioEventSource.Settings(37.5f,(uint[])weights.Clone(),(int[])slots.Clone(),8,9);
        Require(settings == equal && settings.Equals((object)equal) && settings.GetHashCode() == equal.GetHashCode(),"Vector equality differs.");
        weights[1] = 999; slots[0] = 1; settings.Slots()[0] = 1;
        Require(settings.WeightAt(1) == 125 && settings.Slots()[0] == 7,"Vector array alias escaped.");
        Require(settings != new AuthoredAudioEventSource.Settings(37.5f,new uint[] { 0,126,1000000 },new[] { 7,2,0 },8,9),"Changed vector weight compares equal.");
        foreach (var invalid in new[] {
            new AuthoredAudioEventSource.Settings(60,Array.Empty<uint>(),Array.Empty<int>(),8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[9],Enumerable.Range(0,9).ToArray(),8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[] { 1,1,1 },new[] { 0,1 },8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[] { 1,1 },new[] { 0,0 },8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[] { 1 },new[] { 8 },8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[] { 1 },new[] { -1 },8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[] { 1000001 },new[] { 0 },8,8),
            new AuthoredAudioEventSource.Settings(60,new uint[] { 0,0,0 },new[] { 0,1,2 },8,8),
            new AuthoredAudioEventSource.Settings(float.NaN,new uint[] { 1 },new[] { 0 },8,8) }) Reject(invalid.Validate);
        string[] names = Enumerable.Range(0,8).Select(index => "Vector_"+index).ToArray();
        var parsed = AuthoredAudioEventSource.Read(Encoding.UTF8.GetBytes(Source(8,new[] { 7,2,0 },true)),names);
        Require(parsed.Settings.WeightAt(0) == 1000 && parsed.Settings.WeightAt(1) == 0 && parsed.Settings.WeightAt(2) == 1000000,"Vector weight defaults/bounds differ.");
        Reject(() => AuthoredAudioEventSource.Read(Encoding.UTF8.GetBytes(Source(8,Enumerable.Range(0,9).ToArray(),false)),names));
        foreach (int count in new[] { 3,8 }) Check(count,false,null);
        Console.WriteLine("AudioEvent vector self-test: OK (private arrays/content equality, 3/8 selected real-core synthetic mixed packages, independent scalar/selectors/imports/order, defaults/bounds/collision/corruption rejection; no codec/game-load proof).");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: explicitly requested native cases exercise all-three/all-eight selection without changing fixed pair behavior. */
    //-------------------------------------------------------------------------------------------------
    internal static void NativeProof(string library)
    { foreach (int count in new[] { 3,8 }) Check(count,true,library); Console.WriteLine("AudioEvent vector native proof: OK (3/8 XAS leaves + all selected event, parent source/recompile and two-reader linked verification; no playback/production proof)."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: compare every selected wire word and reference against source order before testing forged matching inventories. */
    //-------------------------------------------------------------------------------------------------
    private static void Check(int count,bool native,string? library)
    {
        string source = Path.Combine(Path.GetTempPath(),"Reborn-EventVector-"+Guid.NewGuid().ToString("N")); Directory.CreateDirectory(source);
        int[] order = Enumerable.Range(0,count).Reverse().ToArray();
        var entries = Enumerable.Range(0,count).Select(index => AudioPoolPackageSmokeTest.Entry(index,index%2 != 0)).ToArray();
        string xml = Source(count,order,false); string eventPath = Path.Combine(source,"event.xml"); File.WriteAllBytes(eventPath,Encoding.UTF8.GetBytes(xml));
        string? work = null; AuthoredAudioPool? pool = null;
        if (native)
        {
            AuthoredAudioPoolSmokeTest.Fixture(source,count,false);
            xml = xml.Replace("Vector_","PoolAsset_"); File.WriteAllBytes(eventPath,Encoding.UTF8.GetBytes(xml));
            pool = AuthoredAudioPool.Read(source,true); string job = AudioEncoderSupervisor.Run(library!,"encode-pool-event",pool:pool); work = Path.Combine(job,"worker");
            entries = AudioPoolResultGate.Verify(work,pool,true,true);
        }
        else xml = xml.Replace("Vector_","Variable_");
        File.WriteAllBytes(eventPath,Encoding.UTF8.GetBytes(xml));
        var parsed = AuthoredAudioEventSource.Read(Encoding.UTF8.GetBytes(xml),entries.Select(entry => entry.Name).ToArray());
        var compiled = AudioFileLocalEventProbe.Build(source,entries,authoredName:parsed.Name,variable:true); var bytes = compiled.CopyNative();
        Require(compiled.Settings == parsed.Settings && bytes.InstanceData.Length == 152+12*count && bytes.ImportsData.Length == 4*(count+1),"Vector source/native dimensions differ.");
        for (int ordinal = 0; ordinal < count; ordinal++)
        {
            Require(BinaryPrimitives.ReadUInt32LittleEndian(bytes.InstanceData.AsSpan(152+12*ordinal)) == ordinal+1 && BinaryPrimitives.ReadUInt32LittleEndian(bytes.InstanceData.AsSpan(156+12*ordinal)) == (uint)(100+ordinal) && BinaryPrimitives.ReadUInt32LittleEndian(bytes.InstanceData.AsSpan(160+12*ordinal)) == 0x3F800000u,"Vector wire selector/weight/child volume differs.");
            Require(BinaryPrimitives.ReadUInt32LittleEndian(bytes.ImportsData.AsSpan(4*ordinal)) == 152+12*ordinal,"Vector import offset differs.");
        }
        Require(BinaryPrimitives.ReadUInt32LittleEndian(bytes.ImportsData.AsSpan(4*count)) == uint.MaxValue && compiled.References(entries).Select(reference => reference.InstanceId).SequenceEqual(order.Select(index => entries[index].Id)),"Vector terminator/reference order differs.");
        if (!native) { work = Path.Combine(source,"package"); AudioFilePackageProbe.Publish(work,entries,compiled,variable:true); AudioFilePackageProbe.Verify(work,entries,compiled,variable:true); }
        else
        {
            int eventStart = 8+entries.Sum(entry => entry.CopyNative().InstanceData.Length);
            AudioPoolWorkerSmokeTest.RejectForgedResult(work!,pool!,true,"package/diagnostic.bin",true,eventStart+152+12*(count-1));
            AudioPoolWorkerSmokeTest.RejectForgedResult(work!,pool!,true,"package/diagnostic.imp",true,8+4*(count-1));
            AudioPoolResultGate.Verify(work!,pool!,true,true);
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: construct literal ordered Sound weights, including independent omitted/zero/maximum boundary cases. */
    //-------------------------------------------------------------------------------------------------
    private static string Source(int poolCount,int[] order,bool boundaries) => "<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"VectorEvent\" Volume=\"37.5\" Control=\"LOOP INTERRUPT\">"+string.Concat(order.Select((slot,index) => "<Sound"+(boundaries && index == 0 ? "" : " Weight=\""+(boundaries ? index == 1 ? 0 : 1000000 : 100+index)+"\"")+">AudioFile:Vector_"+slot+"</Sound>"))+"</AudioEvent></AssetDeclaration>";

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject invalid vectors rather than allowing shape/default coercion to expand admission. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action) { try { action(); } catch (InvalidDataException) { return; } throw new InvalidDataException("Invalid event vector succeeded."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop on independent vector/wire/table disagreements. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
