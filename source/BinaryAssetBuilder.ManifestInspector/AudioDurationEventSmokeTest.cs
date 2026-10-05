using System.Buffers.Binary;
using System.Text;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: prove duration AudioEvent closure with synthetic framing by default and supervised actual codec work only by request.
internal static class AudioDurationEventSmokeTest
{
    //-------------------------------------------------------------------------------------------------
    /** Reborn: exercise singleton/subset/reversed full selection and parent source/hash reconstruction in bounded mixed packages. */
    //-------------------------------------------------------------------------------------------------
    internal static void Run(string? library = null)
    {
        foreach (int count in new[] { 1,4,8 })
        {
            int[] samples = count == 1 ? new[] { 96000 } : count == 4 ? new[] { 12001,24000,47999,96000 } : Enumerable.Repeat(96000,8).ToArray();
            int[] slots = count == 1 ? new[] { 0 } : count == 4 ? new[] { 3,0 } : Enumerable.Range(0,8).Reverse().ToArray();
            string source = AudioDurationPoolSmokeTest.Fixture(samples,false);
            if (count == 8) foreach (string path in Directory.EnumerateFiles(source,"*.xml")) File.WriteAllText(path,File.ReadAllText(path).Replace("IsStreamedOnPC=\"false\"","IsStreamedOnPC=\"true\""),new UTF8Encoding(false));
            string eventPath = Path.Combine(source,"event.xml");
            byte[] authored = Encoding.UTF8.GetBytes("<AssetDeclaration xmlns=\"uri:ea.com:eala:asset\"><AudioEvent id=\"DurationEvent\" Volume=\"37.5\" Control=\"LOOP INTERRUPT\">"+string.Concat(slots.Select((slot,index) => "<Sound Weight=\""+(100+index)+"\">AudioFile:PoolAsset_"+slot+"</Sound>"))+"</AudioEvent></AssetDeclaration>");
            File.WriteAllBytes(eventPath,authored); var pool = AuthoredAudioPool.Read(source,true,true);
            Reject(() => AudioEncoderSupervisor.Run(Path.Combine(source,"unused.dll"),"encode-pool-duration-package",pool:pool));
            Reject(() => AudioEncoderSupervisor.Run(Path.Combine(source,"unused.dll"),"encode-pool-event",pool:pool));
            string work;
            if (library != null) work = Path.Combine(AudioEncoderSupervisor.Run(library,"encode-pool-duration-event",pool:pool),"worker");
            else
            {
                // Reborn: prepare synthetic leaf metadata through the leaf-only mode, then install exact frozen event bytes before mixed publication.
                work = Path.Combine(Path.GetTempPath(),"Reborn-DurationMixed-"+Guid.NewGuid().ToString("N")); AudioEncoderPoc.RunPool(Path.Combine(source,"unused.dll"),work,AuthoredAudioPool.Read(source,durationCandidate:true),false);
                File.WriteAllBytes(Path.Combine(work,"event.xml"),pool.Copy("event.xml"));
                Directory.CreateDirectory(Path.Combine(work,"encoded")); string schema = Path.Combine(Path.GetDirectoryName(ReferencePipelineSmokeTest.FindFixture())!,"AudioFileIdentityPipeline.xsd");
                for (int index = 0; index < count; index++)
                {
                    var row = pool.Rows[index]; var core = AudioFileIdentitySmokeTest.Build(work,schema,AudioFileIdentitySmokeTest.Processing,row.Source); var prepared = AudioFileCorePreparation.Prepare(core,true);
                    var entry = AudioDurationPoolSmokeTest.Synthetic(row,samples[index],prepared.Settings.Subtitle); var native = entry.CopyNative(); string prefix = Path.Combine(work,"encoded","leaf"+index);
                    File.WriteAllBytes(prefix+".input.wav",prepared.CopyWave()); File.WriteAllBytes(prefix+".runtime.bin",native.InstanceData); File.WriteAllBytes(prefix+".runtime.relo",native.RelocationData);
                    File.WriteAllBytes(prefix+".snr",row.Streamed ? native.InstanceData.AsSpan(checked((int)BinaryPrimitives.ReadUInt32LittleEndian(native.InstanceData.AsSpan(20,4))),8).ToArray() : entry.CopyCustom());
                    if (row.Streamed) File.WriteAllBytes(prefix+".sns",entry.CopyCustom());
                }
                AudioPoolResultGate.Publish(work,pool);
            }
            var entries = AudioPoolResultGate.Verify(work,pool,true,true); var local = AudioPoolResultGate.BuildEvent(work,pool,entries)!; var bytes = local.CopyNative();
            Require(local.Settings.Slots().SequenceEqual(slots) && entries.Select(entry => entry.Samples).SequenceEqual(samples),"Mixed duration source order/scalars differ.");
            for (int index = 0; index < slots.Length; index++)
                Require(BinaryPrimitives.ReadUInt32LittleEndian(bytes.InstanceData.AsSpan(152+12*index)) == index+1 && BinaryPrimitives.ReadUInt32LittleEndian(bytes.InstanceData.AsSpan(156+12*index)) == 100+index
                    && BinaryPrimitives.ReadUInt32LittleEndian(bytes.ImportsData.AsSpan(4*index)) == 152+12*index,"Mixed duration selector/weight/import ordinal differs.");
            Require(local.References(entries).Select(reference => reference.InstanceId).SequenceEqual(slots.Select(slot => entries[slot].Id)),"Mixed duration reference order differs.");
            foreach (int index in new[] { slots[0],slots[^1] }.Distinct())
            {
                var changed = (AudioFilePackageProbe.Entry[])entries.Clone(); byte[] custom = changed[index].CopyCustom(); custom[^1] ^= 1;
                changed[index] = new(changed[index].Name,changed[index].Source,changed[index].CopyNative(),custom,changed[index].Streamed,changed[index].Samples);
                Reject(() => local.ValidateDependencies(changed));
                var rebuilt = AudioPoolResultGate.BuildEvent(work,pool,changed)!; Require(rebuilt.Hash != local.Hash,"Selected duration payload did not invalidate event fingerprint.");
            }
            if (count > slots.Length)
            {
                int index = Enumerable.Range(0,count).First(slot => !slots.Contains(slot)); var changed = (AudioFilePackageProbe.Entry[])entries.Clone(); byte[] custom = changed[index].CopyCustom(); custom[^1] ^= 1;
                changed[index] = new(changed[index].Name,changed[index].Source,changed[index].CopyNative(),custom,changed[index].Streamed,changed[index].Samples); local.ValidateDependencies(changed);
                Require(AudioPoolResultGate.BuildEvent(work,pool,changed)!.Hash == local.Hash,"Unselected duration payload changed event fingerprint.");
            }
            int start = 8+entries.Sum(entry => entry.CopyNative().InstanceData.Length);
            foreach (int offset in new[] { start+4,start+152+12*(slots.Length-1),start+156+12*(slots.Length-1) }) AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,"package/diagnostic.bin",true,offset);
            AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,"package/diagnostic.imp",true,8+4*(slots.Length-1));
            AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,"package/diagnostic/cdata/"+entries[slots[0]].CustomName,true,4);
            AudioPoolWorkerSmokeTest.RejectForgedResult(work,pool,true,"event.xml",true);
            DateTime stamp = File.GetLastWriteTimeUtc(eventPath); File.WriteAllBytes(eventPath,Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(authored).Replace("37.5","37.6"))); File.SetLastWriteTimeUtc(eventPath,stamp); Reject(pool.VerifyCurrent); File.WriteAllBytes(eventPath,authored);
            pool.VerifyCurrent(); AudioPoolResultGate.Verify(work,pool,true,true);
            if (library != null) Console.WriteLine($"Duration mixed native evidence: leaves={count} selected={slots.Length} files={AudioEncoderSupervisor.Inventory(work,80).Length} bytes={AudioEncoderSupervisor.Inventory(work,80).Sum(item => item.Length)} work={work}");
        }
        Console.WriteLine("Audio duration event proof: OK (1/4/8 pools, late/reversed Sound selection, selected/unselected fingerprint isolation, parent recompilation, two-reader packages and matching-inventory corruption rejection; "+(library == null ? "synthetic framing only" : "real XAS, no playback/game-load proof")+")");
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: invalid mixed-duration evidence must reject instead of widening a leaf-only or canonical mode. */
    //-------------------------------------------------------------------------------------------------
    private static void Reject(Action action) { try { action(); } catch (Exception error) when (error is InvalidDataException or NotSupportedException) { return; } throw new InvalidDataException("Invalid mixed duration evidence accepted."); }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: stop at the first independent mixed duration/source/dependency contract mismatch. */
    //-------------------------------------------------------------------------------------------------
    private static void Require(bool condition,string message) { if (!condition) throw new InvalidDataException(message); }
}
