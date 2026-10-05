using BinaryAssetBuilder.Core;

namespace BinaryAssetBuilder.ManifestInspector;

// Reborn: freeze explicit event source and validated variable package evidence before actual core compilation; no mixed publication is enabled.
internal sealed class AudioPoolEventPreflight
{
    private readonly string _encoded,_source;
    private readonly byte[] _bytes;
    private readonly AuthoredAudioPool _pool;
    private readonly AudioEncoderSupervisor.Item[] _inventory;
    // Reborn: raw output checks become mandatory only after all exclusive event artifacts have been written.
    private bool _outputsWritten;
    internal string Directory { get; }
    internal AudioFileLocalEventProbe.Entry Event { get; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: own immutable source bytes and content inventories alongside detached compiled event evidence. */
    //-------------------------------------------------------------------------------------------------
    private AudioPoolEventPreflight(string encoded,string source,byte[] bytes,AuthoredAudioPool pool,AudioEncoderSupervisor.Item[] inventory,string directory,AudioFileLocalEventProbe.Entry entry)
    { _encoded = encoded; _source = source; _bytes = bytes; _pool = pool; _inventory = inventory; Directory = directory; Event = entry; }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: reject exact source/copy or encoded package changes, even if timestamps or regenerated package hashes still agree. */
    //-------------------------------------------------------------------------------------------------
    internal void VerifyCurrent()
    {
        if (!_bytes.SequenceEqual(AudioEncoderSupervisor.Read(_source,8192)) || !_bytes.SequenceEqual(AudioEncoderSupervisor.Read(Path.Combine(Directory,"event.xml"),8192))
            || !_inventory.SequenceEqual(AudioEncoderSupervisor.Inventory(_encoded,80))) throw new InvalidDataException("Pool event preflight evidence is stale.");
        Event.ValidateDependencies(AudioPoolResultGate.Verify(_encoded,_pool,true,true));
        if (_outputsWritten)
        {
            AssetBuffer native = Event.CopyNative();
            if (!native.InstanceData.SequenceEqual(AudioEncoderSupervisor.Read(Path.Combine(Directory,"event.bin"),248))
                || !native.RelocationData.SequenceEqual(AudioEncoderSupervisor.Read(Path.Combine(Directory,"event.relo"),8))
                || !native.ImportsData.SequenceEqual(AudioEncoderSupervisor.Read(Path.Combine(Directory,"event.imp"),36))) throw new InvalidDataException("Pool event raw evidence differs.");
        }
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: admit exact selected local targets from an independently verified leaf package, compile twice through isolated real core, and retain only raw event evidence. */
    //-------------------------------------------------------------------------------------------------
    internal static AudioPoolEventPreflight Run(string encodedDirectory,string eventSource)
    {
        encodedDirectory = Path.GetFullPath(encodedDirectory); eventSource = Path.GetFullPath(eventSource);
        var pool = AuthoredAudioPool.Read(encodedDirectory); var inventory = AudioEncoderSupervisor.Inventory(encodedDirectory,80);
        var entries = AudioPoolResultGate.Verify(encodedDirectory,pool,true,true);
        byte[] bytes = AudioEncoderSupervisor.Read(eventSource,8192);
        var authored = AuthoredAudioEventSource.Read(bytes,entries.Select(entry => entry.Name).ToArray());
        string directory = Path.Combine(Path.GetTempPath(),"Reborn-PoolEvent-"+Guid.NewGuid().ToString("N")); System.IO.Directory.CreateDirectory(directory);
        WriteNew(Path.Combine(directory,"event.xml"),bytes);
        var compiled = AudioFileLocalEventProbe.Build(directory,entries,authoredName:authored.Name,variable:true);
        if (compiled.Settings != authored.Settings || !compiled.References(entries).Select(reference => reference.InstanceId).SequenceEqual(authored.Settings.Slots().Select(slot => entries[slot].Id)))
            throw new InvalidDataException("Pool event source/core selection differs.");
        var replay = AudioFileLocalEventProbe.Build(directory,entries,authoredName:authored.Name,variable:true);
        AssetBuffer native = compiled.CopyNative(),other = replay.CopyNative();
        if (compiled.Hash != replay.Hash || !native.InstanceData.SequenceEqual(other.InstanceData) || !native.RelocationData.SequenceEqual(other.RelocationData) || !native.ImportsData.SequenceEqual(other.ImportsData))
            throw new InvalidDataException("Pool event independent recompilation differs.");
        var result = new AudioPoolEventPreflight(encodedDirectory,eventSource,bytes,pool,inventory,directory,compiled); result.VerifyCurrent();
        WriteNew(Path.Combine(directory,"event.bin"),native.InstanceData); WriteNew(Path.Combine(directory,"event.relo"),native.RelocationData); WriteNew(Path.Combine(directory,"event.imp"),native.ImportsData);
        result._outputsWritten = true;
        result.VerifyCurrent();
        Console.WriteLine("Owned pool event preflight: "+directory);
        Console.WriteLine($"Pool event preflight: OK ({entries.Length} leaves, selected slots {string.Join(",",authored.Settings.Slots())}, BIN/RELO/IMP={native.InstanceData.Length}/{native.RelocationData.Length}/{native.ImportsData.Length}; raw event only, no mixed package or game-load proof).");
        return result;
    }

    //-------------------------------------------------------------------------------------------------
    /** Reborn: never overwrite source/output artifacts when retaining owned raw event evidence. */
    //-------------------------------------------------------------------------------------------------
    private static void WriteNew(string path,byte[] bytes) { using FileStream writer = new(path,FileMode.CreateNew,FileAccess.Write); writer.Write(bytes); }
}
